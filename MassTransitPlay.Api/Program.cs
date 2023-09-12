using MassTransit;
using MassTransitPlay.Api.Domain.Persistence;
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

// API Endpoints
builder.Services.Scan(scan => 
    scan.FromEntryAssembly()
        .AddClasses(f => f.AssignableTo<IEndpointCollection>())
        .AsImplementedInterfaces()
        .WithSingletonLifetime()
);

// ----------------------------------------------------------------------
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogWarning($"Starting up [Env: {app.Environment.EnvironmentName}]");

using var scope = app.Services.CreateScope();

// SQL Startup (Production: Move this to a pipeline task)
scope.ServiceProvider.GetRequiredService<IssueTrackerDbContext>().Database.Migrate();

// API
foreach (var endpointCollection in scope.ServiceProvider.GetServices<IEndpointCollection>())
{
    logger.LogDebug("Registering: {endpoints}", endpointCollection.GetType().Name);
    endpointCollection.RegisterEndpoints(app);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ----------------------------------------------------------------------
app.Run();


public interface IEndpointCollection
{
    void RegisterEndpoints(IEndpointRouteBuilder app);
}

