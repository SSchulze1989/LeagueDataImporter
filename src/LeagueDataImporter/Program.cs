﻿// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using iRLeagueDatabase.DataTransfer;
using LeagueDataImporter;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();
var configuration = builder.Build();

var username = configuration["Username"];
var password = configuration["Password"];
var leagueName = "SkippyCup";
var connectionString = configuration.GetConnectionString("ModelDb");
var exporter = new DataExporter(leagueName, username, password);

Console.Write("Loading old SeasonsData ...");
var seasonsData = await exporter.GetSeasons();
Console.Write("Done!\n");
using var importer = new DataImporter(connectionString);
var league = await importer.SetOrCreateLeague(leagueName);

Console.Write("Loading members data...");
var membersData = await exporter.GetMembers();
Console.Write("Done!\n");
Console.Write("Importing members...");
var memberMap = await importer.ImportMembers(membersData);
Console.Write("Done!\n");

Console.Write("Loading teams data...");
var teamsData = await exporter.GetTeams();
Console.Write("Done!\n");
Console.Write("Importing teams...");
await importer.ImportTeams(teamsData, membersData);
Console.Write("Done!\n");

Console.Write("Loading data for VoteCategories...");
var voteCategoriesData = await exporter.GetVoteCategories();
Console.Write("Done!\n");
Console.Write("Importing VoteCategories...");
var voteCategoryMap = await importer.ImportVoteCategories(voteCategoriesData);
Console.Write("Done!\n");

foreach (var seasonData in seasonsData)
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
    
    foreach ((var @event, var session) in eventMap)
    {
        try
        {
            Console.Write($"Loading results data for session {session.SessionId}...");
            var resultsData = await exporter.GetResultsFromSession(session);
            Console.Write("Done!\n");
            Console.Write($"Importing results for event {@event.EventId}...");
            await importer.ImportEventResults(@event, resultsData);
            Console.Write("Done!\n");
        }
        catch (Exception)
        {
        }
    }
    foreach ((var @event, var session) in eventMap)
    {
        try
        {
            Console.Write($"Loading reviews data for session {session.SessionId}...");
            var reviewsData = await exporter.GetReviewsFromSession(session);
            Console.Write("Done!\n");
            Console.Write($"Importing reviews for event {@event.EventId}...");
            await importer.ImportEventReviews(@event, reviewsData, memberMap, voteCategoryMap);
            Console.Write("Done!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

Console.WriteLine("Finished");