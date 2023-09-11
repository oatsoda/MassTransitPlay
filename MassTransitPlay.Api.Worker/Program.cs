﻿using MassTransit;
using MassTransitPlay.Api.Events;
using MassTransitPlay.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) => { 

    services.AddMassTransit(x =>
    {
        x.AddConsumer<IssueCreatedConsumer>();

        x.AddEntityFrameworkOutbox<IssueTrackerDbContext>(o =>
        {
            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);

            o.UseSqlServer();
        });

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h => {
                h.Username("guest");
                h.Password("guest");
            });
            cfg.ConfigureEndpoints(context);
        });
    });
    
    // SQL
    var sqlConnectionString = hostContext.Configuration.GetConnectionString("IssueTrackerContext") ?? throw new NotImplementedException($"SQL Connection string not found. Environment: '{hostContext.HostingEnvironment.EnvironmentName}'");
    services.AddDbContext<IssueTrackerDbContext>(opt => opt.UseSqlServer(sqlConnectionString));
});


await builder.RunConsoleAsync();


public class IssueCreatedConsumer : IConsumer<IssueCreated>
{
    public Task Consume(ConsumeContext<IssueCreated> context)
    {
        Console.WriteLine($"CONSUMED! Issue: {context.Message.Id}");
        return Task.CompletedTask;
    }
}