using MassTransit;
using MassTransitPlay.Stats.Domain.Persistence;
using MassTransitPlay.Stats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) => { 

    services.AddMassTransit(x =>
    {
        x.AddConsumer<IssueCreatedIntegrationEventConsumer>()
            .Endpoint(e => e.Name = nameof(IssueCreatedIntegrationEventConsumer));

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
            
            cfg.ReceiveEndpoint(nameof(IssueCreatedIntegrationEventConsumer), e =>
            {
                e.UseMessageRetry(r =>
                {
                    // Handling the concurrency retry like this is less efficient than handling it inside the consumer, where the exception class has the latest version of the Entity, as we could avoid the re-query to get the latest Entity.
                    r.Handle<DbUpdateConcurrencyException>();
                    r.Handle<DbUpdateException>(ex => ex.GetBaseException() is SqlException sx && (sx.Number == 2601 || sx.Number == 2627));
                    r.Immediate(10);
                });            
                
                e.ConfigureConsumer<IssueCreatedIntegrationEventConsumer>(context);
            });

            // cfg.ConfigureEndpoints(context);
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