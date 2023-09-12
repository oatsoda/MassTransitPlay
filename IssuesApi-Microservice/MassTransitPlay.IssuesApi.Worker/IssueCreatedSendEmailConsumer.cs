using MassTransit;
using MassTransitPlay.Api.Domain.Models.Events;

namespace MassTransitPlay.Api.Worker;

public class IssueCreatedSendEmailConsumer : IConsumer<IssueCreated>
{
    public Task Consume(ConsumeContext<IssueCreated> context)
    {
        Console.WriteLine($"SEND EMAIL! Issue: {context.Message.Id}");
        return Task.CompletedTask;
    }
}
