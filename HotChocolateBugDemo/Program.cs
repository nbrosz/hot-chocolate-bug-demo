var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddErrorFilter<ErrorFilter>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

public class Query
{
    public string TestError() => throw new System.Exception("This is a test error");
}

public class ErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        return error.WithCode("CUSTOM_ERROR_CODE");
    }
}