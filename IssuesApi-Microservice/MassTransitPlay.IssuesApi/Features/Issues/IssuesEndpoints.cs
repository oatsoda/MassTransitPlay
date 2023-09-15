using static MassTransitPlay.Api.Features.Issues.Post;
using static MassTransitPlay.Api.Post;

namespace MassTransitPlay.Api.Features.Issues;

public class IssuesEndpoints : IEndpointCollection
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {            
        app.MapGet("/issues", GetList.Execute)
            .WithName("ListIssues")
            .WithTags("Issues")
            .WithOpenApi(); 

        app.MapGet("/issues/{id}", Get.Execute)
            .WithName("GetIssue")
            .WithTags("Issues")
            .WithOpenApi();

        app.MapPost("/issues", Post.Execute)
            .AddEndpointFilter<ValidationFilter<PostCommand>>()
            .WithName("CreateIssue")
            .WithTags("Issues")
            .WithOpenApi();
    }
}
