using MassTransitPlay.Api.Domain.Persistence;

namespace MassTransitPlay.Api.Features.Issues;

public static class Get
{
    public static async Task<IResult> Execute(Guid id, IssueTrackerDbContext dbContext)
    {
        var issue = await dbContext.Posts.FindAsync(id);
        if (issue == null)
            return Results.NotFound();

        return Results.Ok(new { Id = issue.Id, Title = issue.Title, Description = issue.Description });   
    }
}
