using static MassTransitPlay.Api.Features.Issues.Post;

namespace MassTransitPlay.Api.Features.Issues;

public class IssuesEndpoints : IEndpointCollection
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/issues")
             .WithTags("Issues")
             .WithOpenApi(); 

        group.MapGet("", GetList.Execute)
             .WithName("ListIssues");

        group.MapGet("/{id}", Get.Execute)
             .WithName("GetIssue");

        group.MapPost("", Post.Execute)
             .AddEndpointFilter<ValidationFilter<PostCommand>>()
             .WithName("CreateIssue");
    }
}
