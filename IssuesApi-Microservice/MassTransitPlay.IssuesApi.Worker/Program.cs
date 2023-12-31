﻿using MassTransit;
using MassTransitPlay.Api.Worker;
using MassTransitPlay.Api.Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) => { 

    var rabbitMqConnectionString = hostContext.Configuration.GetConnectionString("RabbitMq") ?? throw new NotImplementedException($"RabbitMQ string not found. Environment: '{hostContext.HostingEnvironment.EnvironmentName}'");
    services.AddMassTransit(x =>
    {
        x.AddConsumer<IssueCreatedSendEmailConsumer>();
        x.AddConsumer<IssueCreatedSendNotificationConsumer>();

        x.AddEntityFrameworkOutbox<IssueTrackerDbContext>(o =>
        {
            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);

            o.UseSqlServer();
        });

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri(rabbitMqConnectionString), "/", h => {
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