using System.Net.Sockets;
using Common;
using JasperFx.Core.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using QuestionService.Data;
using QuestionService.Interfaces;
using QuestionService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.AddServiceDefaults();
builder.Services.AddScoped<IQuestionService, QuestionService.Services.QuestionService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddKeyCloakAuthentication();

builder.AddNpgsqlDbContext<QuestionDbContext>("questionDb");

await builder.UseWolverineWithRabbitMqAsync(options =>
{
    options.PublishAllMessages().ToRabbitExchange("questions");
    options.ApplicationAssembly = typeof(Program).Assembly;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapDefaultEndpoints();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<QuestionDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occurred with seeding the database.");
}

app.Run();
