namespace MassTransitPlay.Api.Features.Issues;

public class IssuesEndpoints : IEndpointCollection
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {            
        app.MapGet("/issues", GetList.Execute)
            .WithName("List Issues")
            .WithOpenApi(); 

        app.MapGet("/issues/{id}", Get.Execute)
            .WithName("Get Issue")
            .WithOpenApi();

        app.MapPost("/issues", Post.Execute)
            .WithName("Create Issue")
            .WithOpenApi();
    }
}
