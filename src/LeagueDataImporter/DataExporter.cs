using System.Net;
using System.Text;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueManager.Locations;

public class DataExporter
{
    private readonly string leagueName;
    private HttpClient client { get; }

    public DataExporter(string leagueName, string username, string password)
    {
        this.leagueName = leagueName;
        var credentials = new NetworkCredential(username, password);
        var handler = new HttpClientHandler()
        {
            Credentials = credentials,
        };
        client = new HttpClient(handler);
        client.BaseAddress = new Uri("https://irleaguemanager.ddns.net/irleaguerestservice/api/");
        client.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/xml");
    }

    public async Task<SeasonDataDTO[]> GetSeasons()
    {
        return await GetAsync<SeasonDataDTO>(Array.Empty<long>());
    }

    public async Task<LeagueMemberDataDTO[]> GetMembers()
    {
        return await GetAsync<LeagueMemberDataDTO>(Array.Empty<long>());
    }

    public async Task<TeamDataDTO[]> GetTeams()
    {
        return await GetAsync<TeamDataDTO>(Array.Empty<long>());
    }

    public async Task<VoteCategoryDTO[]> GetVoteCategories()
    {
        return await GetAsync<VoteCategoryDTO>(Array.Empty<long>());
    }

    public async Task<ScheduleDataDTO[]> GetSchedulesFromSeason(SeasonDataDTO season)
    {
        return await GetAsync<ScheduleDataDTO>(season.Schedules.Select(x => x.ScheduleId.GetValueOrDefault()));
    }

    public async Task<IncidentReviewDataDTO[]> GetReviewsFromSession(SessionDataDTO session)
    {
        return await GetAsync<IncidentReviewDataDTO>(session.ReviewIds);
    }

    public async Task<SessionResultsDTO> GetResultsFromSession(SessionDataDTO session)
    {
        var requestUri = $"Results?leagueName={leagueName}&sessionId={session.SessionId}";
        var result = await client.GetAsync(requestUri);
        var content = await result.Content.ReadAsAsync<SessionResultsDTO>();
        return content;
    }

    public async Task<RaceTrack[]> GetRaceTracks()
    {
        var requestUrl = "TrackInfo";
        var result = await client.GetAsync(requestUrl);
        var content = await result.Content.ReadAsAsync<RaceTrack[]>();
        return content;
    }

    public async Task<StandingsDataDTO[]> GetStandingsFromSeason(SeasonDataDTO season)
    {
        var sb = new StringBuilder();
        sb.Append("Standings");
        sb.Append($"?leagueName={leagueName}");
        sb.Append($"&seasonId={season.SeasonId}");
        var requestUrl = sb.ToString();
        var result = await client.GetAsync(requestUrl);
        var content = await result.Content.ReadAsAsync<SeasonStandingsDTO>();
        return content.Standings;
    }

    public async Task<StandingsDataDTO[]> GetStandingsFromSession(SeasonDataDTO season, SessionDataDTO session)
    {
        var sb = new StringBuilder();
        sb.Append("Standings");
        sb.Append($"?leagueName={leagueName}");
        sb.Append($"&seasonId={season.SeasonId}");
        sb.Append($"&sessionId={session.SessionId}");
        var requestUrl = sb.ToString();
        var result = await client.GetAsync(requestUrl);
        var content = await result.Content.ReadAsAsync<SeasonStandingsDTO>();
        return content.Standings;
    }

    public async Task<T[]> GetAsync<T>(IEnumerable<long> ids)
    {
        if (ids == null)
        {
            ids = Array.Empty<long>();
        }

        var sb = new StringBuilder();
        sb.Append("Model");
        sb.Append($"?leagueName={leagueName}");
        sb.Append($"&requestType={typeof(T).Name}");
        foreach((var id, var index) in ids.Select((x, i) => (x, i)))
        {
            sb.Append($"&requestIds[{index}]={id}");
        }
        var requestUrl = sb.ToString();
        var result = await client.GetAsync(requestUrl);
        var content = await result.Content.ReadAsAsync<MappableDTO[]>();
        return content.OfType<T>().ToArray();
    }
}