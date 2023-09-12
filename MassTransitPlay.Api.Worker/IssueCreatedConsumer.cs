using MassTransit;
using MassTransitPlay.Api.Domain.Models.Events;

namespace MassTransitPlay.Api.Worker;

public class IssueCreatedConsumer : IConsumer<IssueCreated>
{
    public Task Consume(ConsumeContext<IssueCreated> context)
    {
        Console.WriteLine($"CONSUMED! Issue: {context.Message.Id}");
        // e.g. Send Email or Send SignalR Broadcast Notification or Send Push Notification
        return Task.CompletedTask;
    }
}

