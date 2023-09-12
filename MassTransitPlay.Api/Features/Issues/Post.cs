using MassTransit;
using MassTransitPlay.Api.Domain.Models;
using MassTransitPlay.Api.Domain.Models.Events;
using MassTransitPlay.Api.Domain.Persistence;

namespace MassTransitPlay.Api.Features.Issues;

public static class Post
{
    public static async Task<IResult> Execute(CreateIssueCommand command, IssueTrackerDbContext dbContext, IBus publish)
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
        await publish.Publish(new IssueCreated(issue.Id)); // Because MassTransit AddEntityFrameworkOutbox + UseBusOutbox is enabled, this will use the Outbox instead of immediate handling, and be committed Tx as part of the SaveChangesAsync

        await dbContext.SaveChangesAsync();

        return Results.Created($"/todoitems/{issue.Id}", null);  
    }
}

public record CreateIssueCommand(Guid OriginatorId, string Title, string Description);