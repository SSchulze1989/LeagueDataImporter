using System.Net;
using iRLeagueDatabase.DataTransfer;

public class DataImporter
{
    private HttpClient client { get; }

    public DataImporter(string username, string password)
    {
        var credentials = new NetworkCredential(username, password);
        var handler = new HttpClientHandler()
        {
            Credentials = credentials,
        };
        client = new HttpClient();
        client.BaseAddress = new Uri("https://irleaguemanager.ddns.net/irleaguerestservice/api/");
        client.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/xml");
    }

    public void ImportSeason(string leagueName, long seasonId)
    {

    }

    public async Task<T[]> GetAsync<T>(string leagueName, long[] ids)
    {
        var requestIdString = string.Join("&", ids.Select((x, i) => $"requestIds[{i}]={x}"));
        var requestUri = $"Model?leagueName={leagueName}&requestType={typeof(T).Name}&{requestIdString}";
        var result = await client.GetAsync(requestUri);
        var content = await result.Content.ReadAsAsync<MappableDTO[]>();
        return content.OfType<T>().ToArray();
    }
}