﻿using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        }

        public async Task<LeagueEntity> SetOrCreateLeague(string leagueName)
        {
            LeagueName = leagueName;
            LeagueEntity league = await dbContext.Leagues
                .Include(x => x.Seasons)
                .FirstOrDefaultAsync(x => x.Name == leagueName);
            if (league == null)
            {
                league = new()
                {
                    Name = leagueName,
                };
                dbContext.Leagues.Add(league);
            }
            league.Seasons.Clear();
            await dbContext.SaveChangesAsync();
            LeagueId = league.Id;
            return League = league;
        }

        public async Task<SeasonEntity> ImportSeason(SeasonDataDTO seasonData)
        {
            // check if season with this name exists:
            SeasonEntity season  = await dbContext.Seasons
                .FirstOrDefaultAsync(x => x.SeasonName == seasonData.SeasonName);
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
            ScheduleEntity schedule = new();
            season.Schedules.Add(schedule);
            schedule = MapScheduleDataToEntity(scheduleData, schedule);
            await dbContext.SaveChangesAsync();
            foreach(var session in scheduleData.Sessions.OfType<RaceSessionDataDTO>())
            {
                var @event = ImportScheduleEvent(schedule, session);
            }
            await dbContext.SaveChangesAsync();
            return schedule;
        }

        public async Task ImportEventResults(EventEntity @event, SessionResultsDTO sessionResults)
        {
            var members = await dbContext.Members
                .ToArrayAsync();
            foreach(var resultsData in sessionResults.ScoredResults)
            {
                ScoredEventResultEntity eventResult = await dbContext.ScoredEventResults
                    .Include(x => x.ScoredSessionResults)
                    .Where(x => x.EventId == @event.EventId)
                    .Where(x => x.Name == resultsData.ScoringName)
                    .FirstOrDefaultAsync();
                if (eventResult == null)
                {
                    eventResult = new();
                    @event.ScoredEventResults.Add(eventResult);
                }
                MapSessionResultToEventResultEntity(resultsData, eventResult, members);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task ImportMembers(LeagueMemberDataDTO[] membersData)
        {
            foreach(var memberData in membersData)
            {
                MemberEntity member = await dbContext.Members
                    .FirstOrDefaultAsync(x => x.IRacingId == memberData.IRacingId);
                if (member == null)
                {
                    member = new();
                    dbContext.Members.Add(member);
                }
                member = MapMemberDataToEntity(memberData, member);
            }
            await dbContext.SaveChangesAsync();
            // create league members
            var membersIracingIds = membersData.Select(x => x.IRacingId).ToList();
            var members = await dbContext.Members
                .Where(x => membersIracingIds.Contains(x.IRacingId))
                .ToListAsync();
            foreach(var member in members)
            {
                var leagueMember = await dbContext.LeagueMembers
                    .FirstOrDefaultAsync(x => x.MemberId == member.Id);
                if (leagueMember == null)
                {
                    leagueMember = new()
                    {
                        Member = member,
                    };
                    League.LeagueMembers.Add(leagueMember);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task ImportTeams(TeamDataDTO[] teamsData, LeagueMemberDataDTO[] membersData)
        {
            foreach (var teamData in teamsData)
            {
                TeamEntity team = await dbContext.Teams
                    .FirstOrDefaultAsync(x => x.Name == teamData.Name);
                if (team == null)
                {
                    team = new();
                    League.Teams.Add(team);
                }
                team = MapTeamDataToEntity(teamData, team);

                // get members in team
                var teamMemberIracingIds = membersData
                    .Where(x => teamData.MemberIds.ToList().Contains(x.MemberId.Value))
                    .Select(x => x.IRacingId);
                var teamMemberIds = await dbContext.Members
                    .Where(x => teamMemberIracingIds.Contains(x.IRacingId))
                    .Select(x => x.Id)
                    .ToListAsync();
                var teamLeagueMembers = await dbContext.LeagueMembers
                    .Where(x => teamMemberIds.Contains(x.MemberId))
                    .ToListAsync();
                team.Members = teamLeagueMembers;
            }
            await dbContext.SaveChangesAsync();
        }

        public Task<EventEntity> ImportScheduleEvent(ScheduleEntity schedule, RaceSessionDataDTO sessionData)
        {
            EventEntity @event = new();
            schedule.Events.Add(@event);
            @event = MapSessionDataToEventEntity(sessionData, @event);
            return Task.FromResult(@event);
        }

        private static TeamEntity MapTeamDataToEntity(TeamDataDTO teamData, TeamEntity entity)
        {
            entity.Name = teamData.Name;
            entity.TeamColor = teamData.TeamColor;
            entity.TeamHomepage = teamData.TeamHomepage;
            entity.Profile = teamData.Profile;
            return entity;
        }

        private static MemberEntity MapMemberDataToEntity(LeagueMemberDataDTO data, MemberEntity entity)
        {
            entity.Firstname = data.Firstname;
            entity.Lastname = data.Lastname;
            entity.IRacingId = data.IRacingId;
            entity.DanLisaId = data.DanLisaId;
            entity.DiscordId = data.DiscordId;
            return entity;
        }

        private static SeasonEntity MapSeasonDataToEntity(SeasonDataDTO data, SeasonEntity entity)
        {
            entity.SeasonName = data.SeasonName;
            entity.SeasonStart = data.SeasonStart;
            entity.SeasonEnd = data.SeasonEnd;
            entity.Finished = data.Finished;
            entity.HideCommentsBeforeVoted = data.HideCommentsBeforeVoted;
            return entity;
        }

        private static ScheduleEntity MapScheduleDataToEntity(ScheduleDataDTO data, ScheduleEntity entity)
        {
            entity.Name = data.Name;
            return entity;
        }

        private static EventEntity MapSessionDataToEventEntity(RaceSessionDataDTO data, EventEntity entity)
        {
            entity.Name = data.Name;
            entity.Duration = data.Duration;
            entity.Date = data.Date;
            entity.EventType  = ConvertEventType(data.SessionType);

            int sessionNr = 1;
            if (data.PracticeAttached)
            {
                SessionEntity practice = new()
                {
                    Name = "Practice",
                    Duration = data.PracticeLength,
                    SessionType = iRLeagueApiCore.Common.Enums.SessionType.Practice,
                    SessionNr = sessionNr++,
                };
                entity.Sessions.Add(practice);
            }
            if (data.QualyAttached)
            {
                SessionEntity qualy = new()
                {
                    Name = "Qualifying",
                    Duration = data.QualyLength,
                    SessionType = iRLeagueApiCore.Common.Enums.SessionType.Qualifying,
                    SessionNr = sessionNr++,
                };
                entity.Sessions.Add(qualy);
            }
            if (data.SessionType == iRLeagueManager.Enums.SessionType.HeatEvent)
            {
                foreach(var subSession in data.SubSessions.OfType<SessionDataDTO>())
                {
                    SessionEntity session = MapSessionDataToEntity(subSession, new());
                    session.SessionNr = sessionNr++;
                    entity.Sessions.Add(session);
                }
            }
            else
            {
                SessionEntity session = MapSessionDataToEntity(data, new());
                entity.Sessions.Add(session);
            }
            return entity;
        }

        private static SessionEntity MapSessionDataToEntity(SessionDataDTO data, SessionEntity entity)
        {
            entity.Name = data.Name;
            entity.Duration = data.Duration;
            entity.Laps = default;
            entity.SessionType = ConvertSessionType(data.SessionType);
            return entity;
        }

        private static ScoredEventResultEntity MapSessionResultToEventResultEntity(ScoredResultDataDTO data, ScoredEventResultEntity entity, MemberEntity[] members)
        {
            entity.Name = data.ScoringName;
            entity.ScoredSessionResults.Clear();
            ScoredSessionResultEntity sessionResult = new() { Name = entity.Name };
            entity.ScoredSessionResults.Add(sessionResult);
            foreach(var row in data.FinalResults)
            {
                ScoredResultRowEntity rowEntity = MapScoredResultRowDataToEntity(row, new(), members);
                sessionResult.ScoredResultRows.Add(rowEntity);
            }
            return entity;
        }

        private static ScoredResultRowEntity MapScoredResultRowDataToEntity(ScoredResultRowDataDTO data, ScoredResultRowEntity entity, MemberEntity[] members)
        {
            entity.AvgLapTime = data.AvgLapTime.Ticks;
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
            entity.FastestLapTime = data.FastestLapTime.Ticks;
            entity.FastLapNr = data.FastLapNr;
            entity.FinalPosition = data.FinalPosition;
            entity.FinalPositionChange = data.FinalPositionChange;
            entity.FinishPosition = data.FinishPosition;
            entity.Incidents = data.Incidents;
            entity.Interval = data.Interval.Ticks;
            entity.MemberId = members.Single(x => x.Firstname == data.Firstname && x.Lastname == data.Lastname).Id;
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
            entity.QualifyingTime = data.QualifyingTime.Ticks;
            entity.RacePoints = data.RacePoints;
            entity.SeasonStartIRating = data.SeasonStartIRating;
            entity.SimSessionType = (int)data.SimSessionType;
            entity.StartPosition = data.StartPosition;
            entity.Status = (int)data.Status;
            entity.TotalPoints = data.TotalPoints;
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
