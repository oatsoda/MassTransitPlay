namespace MassTransitPlay.Api;

public interface IEndpointCollection
{
    void RegisterEndpoints(IEndpointRouteBuilder app);
}
