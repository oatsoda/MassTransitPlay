using MassTransit;
using MassTransitPlay.Api;
using MassTransitPlay.Api.Events;
using MassTransitPlay.Data;
using MassTransitPlay.Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<IssueTrackerDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);

        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// SQL
var sqlConnectionString = builder.Configuration.GetConnectionString("IssueTrackerContext") ?? throw new NotImplementedException($"SQL Connection string not found. Environment: '{builder.Environment.EnvironmentName}'");
builder.Services.AddDbContext<IssueTrackerDbContext>(opt => opt.UseSqlServer(sqlConnectionString));

// OPEN API / SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------------------------
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogWarning($"Starting up [Env: {app.Environment.EnvironmentName}]");

// SQL Startup

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IssueTrackerDbContext>().Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// API

app.MapGet("/issues", async (IssueTrackerDbContext dbContext) => {

    var issues = await dbContext.Posts.ToListAsync();

    return Results.Ok(issues.Select(issue => new { Id = issue.Id, Title = issue.Title, Description = issue.Description }).ToArray());        
})
.WithName("Get Issues")
.WithOpenApi(); 

app.MapGet("/issues/{id}", async (Guid id, IssueTrackerDbContext dbContext) => {

    var issue = await dbContext.Posts.FindAsync(id);
    if (issue == null)
        return Results.NotFound();

    return Results.Ok(new { Id = issue.Id, Title = issue.Title, Description = issue.Description });        
})
.WithName("Get Issue by Id")
.WithOpenApi();

app.MapPost("/issues", async (CreateIssue command, IssueTrackerDbContext dbContext, IBus publish) =>
{
    var issue = new Issue
    {
        Title = command.Title,
        Description = command.Description,
        IsOpen = true,
        Opened = DateTimeOffset.Now,
        OriginatorId = command.OriginatorId
    };

    dbContext.Posts.Add(issue);
    await publish.Publish(new IssueCreated(issue.Id));
    await dbContext.SaveChangesAsync();

    return Results.Created($"/todoitems/{issue.Id}", null);
})
.WithName("Create Issue")
.WithOpenApi();

// ----------------------------------------------------------------------
app.Run();


public record CreateIssue(Guid OriginatorId, string Title, string Description);

