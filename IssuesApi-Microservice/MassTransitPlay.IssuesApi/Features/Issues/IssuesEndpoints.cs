namespace MassTransitPlay.Api.Features.Issues;

public class IssuesEndpoints : IEndpointCollection
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {            
        app.MapGet("/issues", GetList.Execute)
            .WithName("ListIssues")
            .WithOpenApi(); 

        app.MapGet("/issues/{id}", Get.Execute)
            .WithName("GetIssue")
            .WithOpenApi();

        app.MapPost("/issues", Post.Execute)
            .WithName("CreateIssue")
            .WithOpenApi();
    }
}
