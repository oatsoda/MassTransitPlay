using MassTransit;
using MassTransitPlay.Api.Domain.Models.Events;

namespace MassTransitPlay.Api.Worker;

public class IssueCreatedSendNotificationConsumer : IConsumer<IssueCreated>
{
    public Task Consume(ConsumeContext<IssueCreated> context)
    {
        Console.WriteLine($"SEND NOTIFICATION! Issue: {context.Message.Id}");
        // e.g. Send SignalR Broadcast Notification or Send Push Notification
        // (unless there is *really* some benefit to moving this into a separate Microservice - such as additional logic *before* sending - e.g. Notification requires looking up metadata from a database)
        return Task.CompletedTask;
    }
}