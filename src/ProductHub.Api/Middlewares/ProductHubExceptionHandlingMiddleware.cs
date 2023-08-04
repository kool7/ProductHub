using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using ProductHub.Domain.Exceptions;
using System.Net;

namespace ProductHub.Api.Middlewares
{
    public class ProductHubExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ProductHubExceptionHandlingMiddleware> _logger;

        public ProductHubExceptionHandlingMiddleware(ILogger<ProductHubExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                var validationException = ex as ValidationException;
                if (validationException != null)
                {
                    var response = CreateValidationErrorResponse(validationException.Errors);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsJsonAsync(response);
                }
                else
                {

                    var problemDetails = ex switch
                    {
                        ArgumentException argEx => new ProblemDetails
                        {
                            Title = "Bad Request",
                            Detail = argEx.Message,
                            Status = (int)HttpStatusCode.BadRequest,
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                        },
                        ProductNotFoundException productNotFoundEx => new ProblemDetails
                        {
                            Title = "Not Found",
                            Detail = productNotFoundEx.Message,
                            Status = (int)HttpStatusCode.NotFound,
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                        },
                        ValidationException validationEx => new ValidationProblemDetails()
                        {
                            Title = "Validation Error",
                            Detail = validationEx.Message,
                            Status = (int)HttpStatusCode.BadRequest,
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        },
                        _ => new ProblemDetails
                        {
                            Title = "Internal Server Error",
                            Detail = "An internal server error occurred.",
                            Status = (int)HttpStatusCode.InternalServerError,
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                        },
                    };

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = problemDetails.Status.Value;

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            }
        }

        private ValidationErrorResponse CreateValidationErrorResponse(IEnumerable<ValidationFailure> validationErrors)
        {
            var response = new ValidationErrorResponse
            {
                Title = "Validation Failure",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string[]>()
            };

            foreach (var error in validationErrors)
            {
                if (!response.Errors.ContainsKey(error.PropertyName))
                {
                    response.Errors[error.PropertyName] = new string[] { error.ErrorMessage };
                }
                else
                {
                    response.Errors[error.PropertyName] = response.Errors[error.PropertyName].Concat(new string[] { error.ErrorMessage }).ToArray();
                }
            }

            return response;
        }
    }
}
