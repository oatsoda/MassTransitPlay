using MassTransit;
using MassTransitPlay.Stats.Domain.Persistence;
using MassTransitPlay.Stats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) => { 

    services.AddMassTransit(x =>
    {
        x.AddConsumer<IssueCreatedIntegrationEventConsumer>();

        x.AddEntityFrameworkOutbox<IssueStatsDbContext>(o =>
        {
            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);

            o.UseSqlServer();
        });

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri("rabbitmq://localhost"), "/", h => {
                h.Username("guest");
                h.Password("guest");
            });
            cfg.ConfigureEndpoints(context);
        });
    });
    
    // SQL
    var sqlConnectionString = hostContext.Configuration.GetConnectionString("IssueStatsContext") ?? throw new NotImplementedException($"SQL Connection string not found. Environment: '{hostContext.HostingEnvironment.EnvironmentName}'");
    services.AddDbContext<IssueStatsDbContext>(opt => opt.UseSqlServer(sqlConnectionString));
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

using var scope = app.Services.CreateScope();

// SQL Startup (Production: Move this to a pipeline task)
scope.ServiceProvider.GetRequiredService<IssueStatsDbContext>().Database.Migrate();

await app.RunAsync();