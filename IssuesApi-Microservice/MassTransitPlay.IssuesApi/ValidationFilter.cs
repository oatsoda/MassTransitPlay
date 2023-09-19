using FluentValidation;

namespace MassTransitPlay.Api;

public class ValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> m_Validator;

    public ValidationFilter(IValidator<T> validator)
    {
        m_Validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var requestBody = context.GetArgument<T>(0);
        var validationResult = await m_Validator.ValidateAsync(requestBody);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());  

        return await next(context);
    }
}
