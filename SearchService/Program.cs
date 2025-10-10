using System.Text.RegularExpressions;
using Common;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SearchService.Data;
using SearchService.Models;
using Typesense;
using Typesense.Setup;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var typesenseUri = builder.Configuration["services:typesense:typesense:0"];


if (string.IsNullOrWhiteSpace(typesenseUri)) throw new InvalidOperationException("Uri not found in config");
var typesenseApiKey = builder.Configuration["typesense-api-key"];
if (string.IsNullOrWhiteSpace(typesenseApiKey)) throw new InvalidOperationException("No typesense key");

var uri = new Uri(typesenseUri);
builder.Services.AddTypesenseClient(config =>
{
    config.ApiKey = typesenseApiKey;
    config.Nodes = new List<Node>
    {
        new(uri.Host, uri.Port.ToString(), uri.Scheme)
    };
});

await builder.UseWolverineWithRabbitMqAsync(options =>
{
    options.ListenToRabbitQueue("questions.search", cfg =>
    {
        cfg.BindExchange("questions");
    });
    options.ApplicationAssembly = typeof(Program).Assembly;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapGet("/search", async (string query, ITypesenseClient client) =>
{
    string? tag = null;
    var tagMatch = Regex.Match(query, @"\[(.*?)\]");
    if (tagMatch.Success)
    {
        tag = tagMatch.Groups[1].Value;
        query = query.Replace(tagMatch.Value, "").Trim();
    }

    var searchParams = new SearchParameters(query, "title,content");
    if (!string.IsNullOrWhiteSpace(tag)) searchParams.FilterBy = $"tags:=[{tag}]";
    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParams);
        return Results.Ok(result.Hits.Select(h => h.Document));
    }
    catch (Exception e)
    {
        return Results.Problem("Search Failed", e.Message);
    }
});

app.MapGet("/search/similar-titles", async (string query, ITypesenseClient client) =>
{
    var searchParams = new SearchParameters(query, "title");

    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParams);
        return Results.Ok(result.Hits.Select(h => h.Document));
    }
    catch (Exception e)
    {
        return Results.Problem("Typesense Failed", e.Message);
    }
        
});




using var scope = app.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<ITypesenseClient>();
await SearchInitializer.EnsureIndexExists(client);
app.Run();
