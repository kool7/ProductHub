using ProductHub.Api;
using ProductHub.Api.Middlewares;
using ProductHub.Application;
using ProductHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseMiddleware<ProductHubExceptionHandlingMiddleware>();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}

public partial class Program { }
