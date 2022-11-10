// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using iRLeagueDatabase.DataTransfer;
using LeagueDataImporter;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();
var configuration = builder.Build();

var username = configuration["Username"];
var password = configuration["Password"];
var leagueName = args.Length == 0 ? string.Empty : args[0];

Console.WriteLine($"--- Importing data for league \"{leagueName}\" ---");

var connectionString = configuration.GetConnectionString("ModelDb");
var exporter = new DataExporter(leagueName, username, password);
// check if league exists
var oldLeagues = await exporter.GetLeagueNames();
if (oldLeagues.Any(x => x.Equals(leagueName)) == false)
{
    Console.WriteLine("Error! LeagueName \"{0}\" does not exist on remote", leagueName);
    return -1;
}

using var importer = new DataImporter(connectionString);
var league = await importer.SetOrCreateLeague(leagueName);

Console.Write("Loading legacy track data...");
var legacyTracks = await exporter.GetRaceTracks();
Console.Write("Done!\n");
Console.Write("Importing legacy track ids...");
await importer.ImportLegacyTrackIds(legacyTracks);
Console.Write("Done!\n");

Console.Write("Loading members data...");
var membersData = await exporter.GetMembers();
Console.Write("Done!\n");
Console.Write("Importing members...");
var members = await importer.ImportMembers(membersData);
Console.Write("Done!\n");

Console.Write("Loading teams data...");
var teamsData = await exporter.GetTeams();
Console.Write("Done!\n");
Console.Write("Importing teams...");
var teams = await importer.ImportTeams(teamsData, membersData);
Console.Write("Done!\n");

Console.Write("Loading old SeasonsData ...");
var seasonsData = await exporter.GetSeasons();
Console.Write("Done!\n");

Console.Write("Loading data for VoteCategories...");
var voteCategoriesData = await exporter.GetVoteCategories();
Console.Write("Done!\n");
Console.Write("Importing VoteCategories...");
var voteCategories = await importer.ImportVoteCategories(voteCategoriesData);
Console.Write("Done!\n");

IEnumerable<SeasonDataDTO> importSeasons;
if (args.Contains("--import-all"))
{
    importSeasons = seasonsData;
}
else
{
    importSeasons = seasonsData.TakeLast(1);
}

foreach (var seasonData in importSeasons)
{
    Console.Write($"Importing data for season {seasonData.SeasonName}...");
    var season = await importer.ImportSeason(seasonData);
    Console.Write("Done!\n");
    Console.Write("Loading season schedule data...");
    var schedulesData = await exporter.GetSchedulesFromSeason(seasonData);
    Console.Write("Done!\n");
    foreach (var scheduleData in schedulesData)
    {
        Console.Write($"Importing data for schedule {season.SeasonName}->{scheduleData.Name}...");
        var schedule = await importer.ImportSeasonSchedule(season, scheduleData);
        Console.Write("Done!\n");
    }
    var scheduleMap = season.Schedules.Zip(schedulesData).ToList();
    var eventMap = season.Schedules.SelectMany(x => x.Events).Zip(schedulesData.SelectMany(x => x.Sessions)).ToList();

    if (args.Contains("--skip-results") == false)
    {
        foreach ((var @event, var session) in eventMap)
        {
            try
            {
                Console.Write($"Loading results data for session {session.SessionId}...");
                var resultsData = await exporter.GetResultsFromSession(session);
                Console.Write("Done!\n");
                Console.Write($"Importing results for event {@event.EventId}...");
                await importer.ImportEventResults(@event, resultsData, members, teams);
                Console.Write("Done!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    if (args.Contains("--skip-reviews") == false)
    {
        foreach ((var @event, var session) in eventMap)
        {
            try
            {
                Console.Write($"Loading reviews data for session {session.SessionId}...");
                var reviewsData = await exporter.GetReviewsFromSession(session);
                Console.Write("Done!\n");
                Console.Write($"Importing reviews for event {@event.EventId}...");
                await importer.ImportEventReviews(@event, reviewsData, members, voteCategories);
                Console.Write("Done!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    if (args.Contains("--skip-standings") == false)
    {
        foreach((var @event, var session) in eventMap)
        {
            try
            {
                Console.Write($"Loading standings data from session {session.SessionId}...");
                var standingsData = await exporter.GetStandingsFromSession(seasonData, session);
                Console.Write("Done!\n");
                Console.Write($"Importing standings for event {@event.EventId}...");
                await importer.ImportStandings(standingsData, season, @event, members, teams);
                Console.Write("Done!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

Console.WriteLine("--- Data import Finished ---");
return 0;