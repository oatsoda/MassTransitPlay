using MassTransitPlay.Api.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MassTransitPlay.Api.Features.Issues;

public static class GetList
{
    public static async Task<IResult> Execute(IssueTrackerDbContext dbContext)
    {
        var issues = await dbContext.Posts.ToListAsync();
        return Results.Ok(issues.Select(issue => new { Id = issue.Id, Title = issue.Title, Description = issue.Description }).ToArray()); 
    }
}
