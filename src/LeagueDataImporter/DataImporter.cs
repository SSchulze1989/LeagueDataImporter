using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabaseCore.Models;
using iRLeagueManager.Locations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDataImporter
{
    public class DataImporter : IDisposable
    {
        private readonly string connectionString;
        private readonly LeagueDbContext dbContext;
        private bool disposedValue;

        public LeagueEntity League { get; private set; }
        public string LeagueName { get; private set; }
        public long LeagueId { get; private set; }

        public DataImporter(string connectionString)
        {
            this.connectionString = connectionString;
            var optionsBuilder = new DbContextOptionsBuilder<LeagueDbContext>();
            optionsBuilder.UseMySQL(connectionString);
            dbContext = new LeagueDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();
        }

        public async Task<LeagueEntity> SetOrCreateLeague(string leagueName)
        {
            LeagueName = leagueName;
            LeagueEntity league = await dbContext.Leagues
                .Include(x => x.Seasons)
                .Include(x => x.LeagueMembers)
                .FirstOrDefaultAsync(x => x.Name == leagueName);
            if (league == null)
            {
                league = new()
                {
                    Name = leagueName,
                };
                dbContext.Leagues.Add(league);
            }
            await dbContext.SaveChangesAsync();
            LeagueId = league.Id;
            return League = league;
        }

        public async Task<VoteCategoryEntity[]> ImportVoteCategories(VoteCategoryDTO[] voteCategoriesData)
        {
            var voteCategories = new List<VoteCategoryEntity>();
            foreach (var voteCategoryData in voteCategoriesData)
            {
                VoteCategoryEntity voteCategory = await dbContext.VoteCategories
                    .Where(x => x.LeagueId == League.Id)
                    .FirstOrDefaultAsync(x => x.ImportId == voteCategoryData.CatId);
                if (voteCategory == null)
                {
                    voteCategory = new();
                    League.VoteCategories.Add(voteCategory);
                }
                voteCategory = MapVoteCategoryDataToEntity(voteCategoryData, voteCategory);
                voteCategories.Add(voteCategory);
            }
            await dbContext.SaveChangesAsync();
            return voteCategories.ToArray();
        }

        public async Task<SeasonEntity> ImportSeason(SeasonDataDTO seasonData)
        {
            // check if season with this name exists:
            SeasonEntity season = await dbContext.Seasons
                .Include(x => x.Schedules)
                .Where(x => x.LeagueId == LeagueId)
                .FirstOrDefaultAsync(x => x.ImportId == seasonData.SeasonId);
            if (season == null)
            {
                season = new();
                League.Seasons.Add(season);
            }
            season = MapSeasonDataToEntity(seasonData, season);
            await dbContext.SaveChangesAsync();
            return season;
        }

        public async Task<ScheduleEntity> ImportSeasonSchedule(SeasonEntity season, ScheduleDataDTO scheduleData)
        {
            ScheduleEntity schedule = await dbContext.Schedules
                .Include(x => x.Events)
                    .ThenInclude(x => x.Sessions)
                .Where(x => x.LeagueId == LeagueId)
                .Where(x => x.SeasonId == season.SeasonId)
                .FirstOrDefaultAsync(x => x.ImportId == scheduleData.ScheduleId);
            if (schedule == null)
            {
                schedule = new();
                season.Schedules.Add(schedule);
            }
            schedule = MapScheduleDataToEntity(scheduleData, schedule);
            await dbContext.SaveChangesAsync();
            foreach ((var sessionData, var index) in scheduleData.Sessions.OfType<RaceSessionDataDTO>().WithIndex())
            {
                EventEntity @event = schedule.Events
                    .SingleOrDefault(x => x.ImportId == sessionData.SessionId);
                if (@event == null)
                {
                    @event = new();
                    schedule.Events.Add(@event);
                }
                @event = MapSessionDataToEventEntity(sessionData, @event);
                var track = await dbContext.TrackConfigs
                    .FirstAsync(x => x.LegacyTrackId == sessionData.LocationId);
                @event.Track = track;
            }
            await dbContext.SaveChangesAsync();
            return schedule;
        }

        public async Task ImportEventResults(EventEntity @event, SessionResultsDTO sessionResults, LeagueMemberEntity[] members, TeamEntity[] teams)
        {
            foreach(var resultsData in sessionResults.ScoredResults)
            {
                ScoredEventResultEntity eventResult = await dbContext.ScoredEventResults
                    .Include(x => x.ScoredSessionResults)
                        .ThenInclude(x => x.ScoredResultRows)
                            .ThenInclude(x => x.TeamResultRows)
                    .Where(x => x.LeagueId == LeagueId)
                    .Where(x => x.EventId == @event.EventId)
                    .FirstOrDefaultAsync(x => x.ImportId == resultsData.ScoringId);
                if (eventResult == null)
                {
                    eventResult = new();
                    @event.ScoredEventResults.Add(eventResult);
                }
                eventResult = await MapSessionResultToEventResultEntity(resultsData, eventResult, members, teams);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task ImportEventReviews(EventEntity @event, IncidentReviewDataDTO[] reviewsData, 
            LeagueMemberEntity[] members, 
            VoteCategoryEntity[] voteCategories)
        {
            // load event reviews and sessions
            await dbContext.IncidentReviews
                .Include(x => x.Session)
                .Include(x => x.InvolvedMembers)
                .Include(x => x.AcceptedReviewVotes)
                    .ThenInclude(x => x.VoteCategory)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.ReviewCommentVotes)
                        .ThenInclude(x => x.VoteCategory)
                .Where(x => x.LeagueId == LeagueId)
                .Where(x => x.Session.EventId == @event.EventId)
                .LoadAsync();
            dbContext.ChangeTracker.DetectChanges();
            Debug.Assert(@event.Sessions != null && @event.Sessions.Count() > 0);
            var raceSession = @event.Sessions.First(x => x.SessionType == iRLeagueApiCore.Common.Enums.SessionType.Race);
            foreach (var reviewData in reviewsData)
            {
                IncidentReviewEntity review = await dbContext.IncidentReviews
                    .Where(x => x.LeagueId == LeagueId)
                    .SingleOrDefaultAsync(x => x.ImportId == reviewData.ReviewId);
                if (review == null)
                {
                    review = new();
                    raceSession.IncidentReviews.Add(review);
                }
                review = MapReviewDataToEntity(reviewData, review, members, voteCategories);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<LeagueMemberEntity[]> ImportMembers(LeagueMemberDataDTO[] membersData)
        {
            var members = new List<MemberEntity>();
            foreach(var memberData in membersData)
            {
                if (string.IsNullOrEmpty(memberData.IRacingId))
                {
                    continue;
                }

                MemberEntity member = await dbContext.Members
                    .FirstOrDefaultAsync(x => x.IRacingId == memberData.IRacingId);
                if (member == null)
                {
                    member = new();
                    dbContext.Members.Add(member);
                }
                member = MapMemberDataToEntity(memberData, member);
                members.Add(member);
            }
            await dbContext.SaveChangesAsync();
            var leagueMembers = new List<LeagueMemberEntity>();
            foreach(var member in members)
            {
                var leagueMember = await dbContext.LeagueMembers
                    .Include(x => x.Member)
                    .Where(x => x.LeagueId == LeagueId)
                    .FirstOrDefaultAsync(x => x.MemberId == member.Id);
                if (leagueMember == null)
                {
                    leagueMember = new()
                    {
                        Member = member,
                    };
                    League.LeagueMembers.Add(leagueMember);
                }
                leagueMembers.Add(leagueMember);
            }
            await dbContext.SaveChangesAsync();
            return leagueMembers.ToArray();
        }

        public async Task<TeamEntity[]> ImportTeams(TeamDataDTO[] teamsData, LeagueMemberDataDTO[] membersData)
        {
            var teams = new List<TeamEntity>();
            foreach (var teamData in teamsData)
            {
                TeamEntity team = await dbContext.Teams
                    .Where(x => x.LeagueId == LeagueId)
                    .FirstOrDefaultAsync(x => x.ImportId == teamData.TeamId);
                if (team == null)
                {
                    team = new();
                    League.Teams.Add(team);
                }
                team = MapTeamDataToEntity(teamData, team);

                // get members in team
                var teamLeagueMembers = await dbContext.LeagueMembers
                    .Where(x => teamData.MemberIds
                        .ToList()
                        .Contains(x.Member.ImportId.GetValueOrDefault()))
                    .Where(x => x.LeagueId == LeagueId)
                    .ToListAsync();
                team.Members = teamLeagueMembers;
                teams.Add(team);
            }
            await dbContext.SaveChangesAsync();
            return teams.ToArray();
        }

        public async Task ImportLegacyTrackIds(RaceTrack[] legacyTracks)
        {
            foreach(var legacyTrack in legacyTracks)
            {
                foreach (var legacyTrackConfig in legacyTrack.Configs)
                {
                    var trackConfig = await dbContext.TrackConfigs
                        .FirstAsync(x => x.TrackId == legacyTrackConfig.iracingTrkId);
                    trackConfig.LegacyTrackId = $"{legacyTrack.TrackId}-{legacyTrackConfig.ConfigId}";
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task ImportStandings(StandingsDataDTO[] standings, SeasonEntity season, EventEntity @event, LeagueMemberEntity[] members, TeamEntity[] teams)
        {
            foreach (var standing in standings)
            {
                var standingEntity = await dbContext.Standings
                    .Include(x => x.StandingRows)
                        .ThenInclude(x => x.ResultRows)
                    .Where(x => x.LeagueId == LeagueId)
                    .Where(x => x.SeasonId == season.SeasonId)
                    .Where(x => x.EventId == @event.EventId)
                    .Where(x => x.Name == standing.Name)
                    .FirstOrDefaultAsync();
                if (standingEntity == null)
                {
                    standingEntity = new()
                    {
                        Season = season,
                        Event = @event,
                    };
                    dbContext.Standings.Add(standingEntity);
                }
                standingEntity = await MapStandingDataToEntity(standing, standingEntity, members, teams);
            }
            await dbContext.SaveChangesAsync();
        }

        private static IncidentReviewEntity MapReviewDataToEntity(IncidentReviewDataDTO data, IncidentReviewEntity entity,
            LeagueMemberEntity[] members,
            VoteCategoryEntity[] voteCategories)
        {
            entity.ImportId = data.ReviewId;
            entity.AuthorName = data.AuthorName;
            entity.Corner = data.Corner;
            entity.OnLap = data.OnLap;
            entity.IncidentKind = data.IncidentKind;
            entity.IncidentNr = data.IncidentNr;
            entity.InvolvedMembers = members
                .Where(x => data.InvolvedMemberIds
                    .ToList()
                    .Contains(x.Member.ImportId.GetValueOrDefault()))
                    .Select(x => x.Member)
                .ToList();
            if (data.AcceptedReviewVotes != null)
            foreach(var voteData in data.AcceptedReviewVotes)
            {
                AcceptedReviewVoteEntity vote = entity.AcceptedReviewVotes
                    .Where(x => x.LeagueId == entity.LeagueId)
                    .SingleOrDefault(x => x.ImportId == voteData.ReviewVoteId);
                if (vote == null)
                {
                    vote = new();
                    vote.ImportId = voteData.ReviewVoteId;
                    entity.AcceptedReviewVotes.Add(vote);
                }
                vote.VoteCategory = voteCategories
                    .SingleOrDefault(x => x.ImportId == voteData.VoteCategoryId);
                vote.MemberAtFault = members
                    .SingleOrDefault(x => x.Member.ImportId == voteData.MemberAtFaultId)?.Member;
                vote.Description = voteData.Description;
            }
            if (data.Comments != null)
            foreach(var commentData in data.Comments)
            {
                ReviewCommentEntity comment = entity.Comments
                    .Where(x => x.LeagueId == entity.LeagueId)
                    .SingleOrDefault(x => x.ImportId == commentData.CommentId);
                if (comment == null)
                {
                    comment = new();
                    entity.Comments.Add(comment);
                }
                comment = MapCommentDataToEntity(commentData, comment, members, voteCategories);
            }
            entity.ResultLongText = data.ResultLongText;
            return entity;
        }

        private static ReviewCommentEntity MapCommentDataToEntity(ReviewCommentDataDTO data, ReviewCommentEntity entity,
            LeagueMemberEntity[] members,
            VoteCategoryEntity[] voteCategories)
        {
            entity.ImportId = data.CommentId;
            entity.Text = data.Text;
            foreach(var voteData in data.CommentReviewVotes)
            {
                ReviewCommentVoteEntity vote = entity.ReviewCommentVotes
                    .Where(x => x.LeagueId == entity.LeagueId)
                    .SingleOrDefault(x => x.ImportId == voteData.ReviewVoteId);
                if (vote == null)
                {
                    vote = new();
                    vote.ImportId = voteData.ReviewVoteId;
                    entity.ReviewCommentVotes.Add(vote);
                }
                vote.VoteCategory = voteCategories
                    .SingleOrDefault(x => x.ImportId == voteData.VoteCategoryId);
                vote.MemberAtFault = members
                    .SingleOrDefault(x => x.Member.ImportId == voteData.MemberAtFaultId)?.Member;
                vote.Description = voteData.Description;
            }
            entity.AuthorName = data.AuthorName ?? data.CreatedByUserName;
            entity.Date = data.Date;
            return entity;
        }

        private static VoteCategoryEntity MapVoteCategoryDataToEntity(VoteCategoryDTO data, VoteCategoryEntity entity)
        {
            entity.ImportId = data.CatId;
            entity.Index = data.Index;
            entity.Text = data.Text;
            entity.DefaultPenalty = data.DefaultPenalty;
            return entity;
        }

        private static TeamEntity MapTeamDataToEntity(TeamDataDTO teamData, TeamEntity entity)
        {
            entity.ImportId = teamData.TeamId;
            entity.Name = teamData.Name;
            entity.TeamColor = teamData.TeamColor;
            entity.TeamHomepage = teamData.TeamHomepage;
            entity.Profile = teamData.Profile;
            return entity;
        }

        private static MemberEntity MapMemberDataToEntity(LeagueMemberDataDTO data, MemberEntity entity)
        {
            entity.ImportId = data.MemberId;
            entity.Firstname = data.Firstname;
            entity.Lastname = data.Lastname;
            entity.IRacingId = data.IRacingId;
            entity.DanLisaId = data.DanLisaId;
            entity.DiscordId = data.DiscordId;
            return entity;
        }

        private static SeasonEntity MapSeasonDataToEntity(SeasonDataDTO data, SeasonEntity entity)
        {
            entity.ImportId = data.SeasonId;
            entity.SeasonName = data.SeasonName;
            entity.SeasonStart = data.SeasonStart;
            entity.SeasonEnd = data.SeasonEnd;
            entity.Finished = data.Finished;
            entity.HideCommentsBeforeVoted = data.HideCommentsBeforeVoted;
            return entity;
        }

        private static ScheduleEntity MapScheduleDataToEntity(ScheduleDataDTO data, ScheduleEntity entity)
        {
            entity.ImportId = data.ScheduleId;
            entity.Name = data.Name;
            return entity;
        }

        private static EventEntity MapSessionDataToEventEntity(RaceSessionDataDTO data, EventEntity entity)
        {
            entity.ImportId = data.SessionId;
            entity.Name = data.Name;
            entity.Duration = data.Duration;
            entity.Date = data.Date;
            entity.EventType  = ConvertEventType(data.SessionType);

            int sessionNr = 1;
            if (data.PracticeAttached)
            {
                SessionEntity practice = entity.Sessions
                    .FirstOrDefault(x => x.SessionType == iRLeagueApiCore.Common.Enums.SessionType.Practice);
                if (practice == null)
                {
                    practice = new()
                    {
                        Name = "Practice",
                        Duration = data.PracticeLength,
                        SessionType = iRLeagueApiCore.Common.Enums.SessionType.Practice,
                    };
                    entity.Sessions.Add(practice);
                }
                practice.SessionNr = sessionNr++;
            }
            if (data.QualyAttached)
            {
                SessionEntity qualy = entity.Sessions
                    .FirstOrDefault(x => x.SessionType == iRLeagueApiCore.Common.Enums.SessionType.Qualifying);
                if (qualy == null)
                {
                    qualy = new()
                    {
                        Name = "Qualifying",
                        Duration = data.QualyLength,
                        SessionType = iRLeagueApiCore.Common.Enums.SessionType.Qualifying,
                    };
                    entity.Sessions.Add(qualy);
                }
                qualy.SessionNr = sessionNr++;
            }
            if (data.SessionType == iRLeagueManager.Enums.SessionType.HeatEvent)
            {
                foreach(var subSession in data.SubSessions.OfType<SessionDataDTO>())
                {
                    SessionEntity session = entity.Sessions
                        .SingleOrDefault(x => x.ImportId == subSession.SessionId);
                    if (session == null)
                    {
                        session = new();
                        entity.Sessions.Add(session);
                    }
                    session.SessionNr = sessionNr++;
                    MapSessionDataToEntity(subSession, session);
                }
            }
            else
            {
                SessionEntity session = entity.Sessions
                    .SingleOrDefault(x => x.ImportId == data.SessionId);
                if (session == null)
                {
                    session = new();
                    entity.Sessions.Add(session);
                }
                session.SessionNr = sessionNr++;
                MapSessionDataToEntity(data, session);
            }
            return entity;
        }

        private static SessionEntity MapSessionDataToEntity(SessionDataDTO data, SessionEntity entity)
        {
            entity.ImportId = data.SessionId;
            entity.Name = data.Name;
            entity.Duration = data.Duration;
            entity.Laps = default;
            entity.SessionType = ConvertSessionType(data.SessionType);
            return entity;
        }

        private async Task<ScoredEventResultEntity> MapSessionResultToEventResultEntity(ScoredResultDataDTO data, ScoredEventResultEntity entity, 
            LeagueMemberEntity[] members, TeamEntity[] teams)
        {
            entity.ImportId = data.ScoringId;
            entity.Name = data.ScoringName;
            ScoredSessionResultEntity sessionResult = entity.ScoredSessionResults
                .SingleOrDefault(x => x.ImportId == data.ScoringId);
            if (sessionResult == null)
            {
                sessionResult = new() { Name = entity.Name };
                entity.ScoredSessionResults.Add(sessionResult);
            }
            sessionResult.ImportId = data.ScoringId;
            foreach(var rowData in data.FinalResults)
            {
                ScoredResultRowEntity rowEntity = sessionResult.ScoredResultRows
                    .SingleOrDefault(x => x.ImportId == rowData.ScoredResultRowId);
                if (rowEntity == null)
                {
                    rowEntity = new();
                    sessionResult.ScoredResultRows.Add(rowEntity);
                }
                rowEntity = MapScoredResultRowDataToEntity(rowData, rowEntity, members, teams);
            }
            if (data is ScoredTeamResultDataDTO teamResultData)
            {
                foreach (var rowData in teamResultData.TeamResults)
                {
                    ScoredResultRowEntity rowEntity = sessionResult.ScoredResultRows
                        .SingleOrDefault(x => x.ImportId == rowData.ScoredResultRowId);
                    if (rowEntity == null)
                    {
                        rowEntity = new();
                        sessionResult.ScoredResultRows.Add(rowEntity);
                        rowEntity.ScoredSessionResult = sessionResult;
                    }
                    rowEntity = await MapScoredTeamResultRowDataToEntity(rowData, rowEntity, members, teams);
                }
            }
            return entity;
        }

        private async Task<ScoredResultRowEntity> MapScoredTeamResultRowDataToEntity(ScoredTeamResultRowDataDTO data, ScoredResultRowEntity entity,
            LeagueMemberEntity[] members, TeamEntity[] teams)
        {
            entity.ImportId = data.ScoredResultRowId;
            entity.FastestLapTime = data.FastestLapTime;
            entity.AvgLapTime = data.AvgLapTime;
            entity.BonusPoints = data.BonusPoints;
            entity.CarClass = data.CarClass;
            entity.ClassId = data.ClassId;
            entity.FinalPosition = data.FinalPosition;
            entity.FinalPositionChange = data.FinalPositionChange;
            entity.LeadLaps = data.LeadLaps;
            entity.PenaltyPoints = data.PenaltyPoints;
            entity.RacePoints = data.RacePoints;
            entity.Team = teams.Single(x => x.ImportId == data.TeamId);
            entity.TotalPoints = data.TotalPoints;
            entity.TeamResultRows.Clear();
            foreach(var rowData in data.ScoredResultRows)
            {
                ScoredResultRowEntity rowEntity = await dbContext.ScoredResultRows
                    .Where(x => x.LeagueId == LeagueId)
                    .SingleOrDefaultAsync(x => x.ImportId == rowData.ScoredResultRowId);
                if (rowEntity == null)
                {
                    rowEntity = entity.ScoredSessionResult.ScoredResultRows
                        .SingleOrDefault(x => x.ImportId == rowData.ScoredResultRowId)
                        ?? throw new InvalidOperationException($"ScoredResultRow ImportId:{rowData.ScoredResultRowId} in TeamResultRow ImportId:{entity.ImportId} not found!");
                }
                entity.TeamResultRows.Add(rowEntity);
            }
            return entity;
        }

        private async Task<StandingEntity> MapStandingDataToEntity(StandingsDataDTO data, StandingEntity entity, 
            LeagueMemberEntity[] members, TeamEntity[] teams)
        {
            entity.Name = data.Name;
            entity.IsTeamStanding = data is TeamStandingsDataDTO;
            foreach((var rowData, var index) in data.StandingsRows.Select((x, i) => (x, i)))
            {
                var rowEntity = entity.StandingRows
                    .ElementAtOrDefault(index);
                if (rowEntity == null)
                {
                    rowEntity = new();
                    entity.StandingRows.Add(rowEntity);
                }
                var resultRowIds = data.StandingsRows
                    .SelectMany(x => x.DriverResults)
                    .Select(x => x.ScoredResultRowId);
                var resultRows = await dbContext.ScoredResultRows
                    .Where(x => x.LeagueId == LeagueId)
                    .Where(x => resultRowIds.Contains(x.ImportId))
                    .ToListAsync();
                rowEntity = MapStandingRowDataToEntity(rowData, rowEntity, members, teams, resultRows);
            }
            return entity;
        }

        private static StandingRowEntity MapStandingRowDataToEntity(StandingsRowDataDTO data, StandingRowEntity entity,
            LeagueMemberEntity[] members, TeamEntity[] teams, IEnumerable<ScoredResultRowEntity> resultRows)
        {
            entity.CarClass = data.CarClass;
            entity.ClassId = data.ClassId;
            entity.CompletedLaps = data.CompletedLaps;
            entity.CompletedLapsChange = data.CompletedLapsChange;
            entity.DroppedResultCount = data.DroppedResultCount;
            entity.FastestLaps = data.FastestLaps;
            entity.FastestLapsChange = data.FastestLapsChange;
            entity.Incidents = data.Incidents;
            entity.IncidentsChange = data.IncidentsChange;
            entity.LastPosition = data.LastPosition;
            entity.LeadLaps = data.LeadLaps;
            entity.LeadLapsChange = data.LeadLapsChange;
            entity.Member = members.FirstOrDefault(x => x.Member.ImportId == data.MemberId).Member;
            entity.PenaltyPoints = data.PenaltyPoints;
            entity.PenaltyPointsChange = data.PenaltyPointsChange;
            entity.PolePositions = data.PolePositions;
            entity.PolePositionsChange = data.PolePositionsChange;
            entity.Position = data.Position;
            entity.PositionChange = data.PositionChange;
            entity.RacePoints = data.RacePoints;
            entity.RacePointsChange = data.RacePointsChange;
            entity.Races = data.Races;
            entity.RacesCounted = data.RacesCounted;
            entity.Team = teams.FirstOrDefault(x => x.ImportId == data.TeamId);
            entity.Top10 = data.Top10;
            entity.Top3 = data.Top3;
            entity.Top5 = data.Top5;
            entity.TotalPoints = data.TotalPoints;
            entity.TotalPointsChange = data.TotalPointsChange;
            entity.Wins = data.Wins;
            entity.WinsChange = data.WinsChange;
            entity.ResultRows = MapStandingResultRowsList(data.DriverResults, entity.ResultRows, resultRows);
            return entity;
        }

        private static ICollection<StandingRows_ScoredResultRows> MapStandingResultRowsList(ScoredResultRowDataDTO[] standingResultRowData,
            ICollection<StandingRows_ScoredResultRows> standingResultRows, IEnumerable<ScoredResultRowEntity> resultRows)
        {
            foreach(var rowData in standingResultRowData)
            {
                var resultRow = standingResultRows.FirstOrDefault(x => x.ScoredResultRow.ImportId == rowData.ScoredResultRowId);
                if (resultRow is null)
                {
                    var scoredResultRow = resultRows.First(x => x.ImportId == rowData.ScoredResultRowId);
                    resultRow = new()
                    {
                        ScoredResultRow = scoredResultRow,
                    };
                    standingResultRows.Add(resultRow);
                }
                resultRow.IsScored = rowData.IsDroppedResult == false;
            }
            return standingResultRows;
        }

        private ScoredResultRowEntity MapScoredResultRowDataToEntity(ScoredResultRowDataDTO data, ScoredResultRowEntity entity,
            LeagueMemberEntity[] members, TeamEntity[] teams)
        {
            entity.ImportId = data.ScoredResultRowId;
            entity.AvgLapTime = data.AvgLapTime;
            entity.BonusPoints = data.BonusPoints;
            entity.Car = data.Car;
            entity.CarClass = data.CarClass;
            entity.CarId  = data.CarId;
            entity.CarNumber = data.CarNumber;
            entity.ClassId = data.ClassId;
            entity.ClubId = data.ClubId;
            entity.ClubName = data.ClubName;
            entity.CompletedLaps = data.CompletedLaps;
            entity.CompletedPct = data.CompletedPct;
            entity.Division = data.Division;
            entity.FastestLapTime = data.FastestLapTime;
            entity.FastLapNr = data.FastLapNr;
            entity.FinalPosition = data.FinalPosition;
            entity.FinalPositionChange = data.FinalPositionChange;
            entity.FinishPosition = data.FinishPosition;
            entity.Incidents = data.Incidents;
            entity.Interval = data.Interval;
            entity.LeadLaps = data.LeadLaps;
            entity.Member = members.Single(x => x.Member.ImportId == data.MemberId).Member;
            entity.NewCpi = data.NewCpi;
            entity.NewIRating = data.NewIRating;
            entity.NewLicenseLevel = data.NewLicenseLevel;
            entity.NewSafetyRating = data.NewSafetyRating;
            entity.OldCpi = data.OldCpi;
            entity.OldIRating = data.OldIRating;
            entity.OldLicenseLevel = data.OldLicenseLevel;
            entity.OldSafetyRating = data.OldSafetyRating;
            entity.PenaltyPoints = data.PenaltyPoints;
            entity.PositionChange = data.PositionChange;
            entity.QualifyingTime = data.QualifyingTime;
            entity.RacePoints = data.RacePoints;
            entity.SeasonStartIRating = data.SeasonStartIRating;
            entity.SimSessionType = (int)data.SimSessionType;
            entity.StartPosition = data.StartPosition;
            entity.Status = (int)data.Status;
            entity.TotalPoints = data.TotalPoints;
            entity.Team = teams.SingleOrDefault(x => x.ImportId == data.TeamId);
            return entity;
        }

        private static iRLeagueApiCore.Common.Enums.EventType ConvertEventType(iRLeagueManager.Enums.SessionType sessionType)
        {
            switch (sessionType)
            {
                case iRLeagueManager.Enums.SessionType.Race:
                    return iRLeagueApiCore.Common.Enums.EventType.SingleRace;
                case iRLeagueManager.Enums.SessionType.HeatEvent:
                    return iRLeagueApiCore.Common.Enums.EventType.MultiRace;
                case iRLeagueManager.Enums.SessionType.Qualifying:
                    return iRLeagueApiCore.Common.Enums.EventType.Qualifying;
                case iRLeagueManager.Enums.SessionType.Practice:
                    return iRLeagueApiCore.Common.Enums.EventType.Practice;
                case iRLeagueManager.Enums.SessionType.Heat:
                    throw new InvalidOperationException("Cannot convert session of type Heat to Event");
                default:
                    return default;
            }
        }

        private static iRLeagueApiCore.Common.Enums.SessionType ConvertSessionType(iRLeagueManager.Enums.SessionType sessionType)
        {
            switch(sessionType)
            {
                case iRLeagueManager.Enums.SessionType.Race:
                    return iRLeagueApiCore.Common.Enums.SessionType.Race;
                case iRLeagueManager.Enums.SessionType.Practice:
                    return iRLeagueApiCore.Common.Enums.SessionType.Practice;
                case iRLeagueManager.Enums.SessionType.HeatEvent:
                    throw new InvalidOperationException("Cannot convert session of type HeatEvent to Session");
                case iRLeagueManager.Enums.SessionType.Qualifying:
                    return iRLeagueApiCore.Common.Enums.SessionType.Qualifying;
                case iRLeagueManager.Enums.SessionType.Heat:
                    return iRLeagueApiCore.Common.Enums.SessionType.Race;
                default:
                    return default;
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
