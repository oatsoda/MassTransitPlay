using FluentValidation;
using MassTransit;
using MassTransitPlay.Api.Domain.Models;
using MassTransitPlay.Api.Domain.Models.Events;
using MassTransitPlay.Api.Domain.Persistence;
using MassTransitPlay.SharedContracts;

namespace MassTransitPlay.Api.Features.Issues;

public static partial class Post
{
    public record PostCommand(Guid OriginatorId, string Title, string Description);

    public class PostCommandValidator: AbstractValidator<PostCommand>
    {
        public PostCommandValidator()
        {
            RuleFor(m => m.OriginatorId).NotEmpty();
            RuleFor(m => m.Title).NotEmpty().MaximumLength(20);
            RuleFor(m => m.Description).NotEmpty().MaximumLength(50);
        }
    }

    public static async Task<IResult> Execute(PostCommand command, IssueTrackerDbContext dbContext, IPublishEndpoint publish, LinkGenerator linker, PostCommandValidator validator)
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
        await publish.Publish(new IssueCreatedIntegrationEvent(issue.Id, issue.Title));

        // TODO: Handle errors - especially conflicts
        await dbContext.SaveChangesAsync();

        return Results.Created(linker.GetPathByName("GetIssue", values: new { id = issue.Id })!, null);  
    }
}
