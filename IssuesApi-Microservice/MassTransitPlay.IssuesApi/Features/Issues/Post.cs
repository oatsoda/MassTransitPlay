using MassTransit;
using MassTransitPlay.Api.Domain.Models;
using MassTransitPlay.Api.Domain.Models.Events;
using MassTransitPlay.Api.Domain.Persistence;

namespace MassTransitPlay.Api.Features.Issues;

public static class Post
{
    public record CreateIssueCommand(Guid OriginatorId, string Title, string Description);

    public static async Task<IResult> Execute(CreateIssueCommand command, IssueTrackerDbContext dbContext, IPublishEndpoint publish, LinkGenerator linker)
    {
        var issue = new Issue
        {
            Title = command.Title,
            Description = command.Description,
            IsOpen = true,
            Opened = DateTimeOffset.Now,
            OriginatorId = command.OriginatorId
        };
        issue.Tasks.Add(new IssueTask() { Title = "Review", Description = "Read and Apply Tags" });

        dbContext.Posts.Add(issue);
        await publish.Publish(new IssueCreated(issue.Id)); // Because MassTransit AddEntityFrameworkOutbox + UseBusOutbox is enabled, this will use the Outbox instead of immediate handling, and be committed Tx as part of the SaveChangesAsync

        // TODO: Handle errors - especially conflicts
        await dbContext.SaveChangesAsync();

        return Results.Created(linker.GetPathByName("GetIssue", values: new { id = issue.Id })!, null);  
    }
}
