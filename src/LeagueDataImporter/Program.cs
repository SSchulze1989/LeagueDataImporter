// See https://aka.ms/new-console-template for more information
using System.Security;
using System.Net;
using System.Web.Http;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Sessions;
using System.Linq;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();
var configuration = builder.Build();

var username = configuration["Username"];
var password = configuration["Password"];
var leagueName = "SkippyCup";
var importer = new DataImporter(username, password);
var seasons = await importer.GetAsync<SeasonDataDTO>(leagueName, new long[] {1});

Console.WriteLine("Finished");