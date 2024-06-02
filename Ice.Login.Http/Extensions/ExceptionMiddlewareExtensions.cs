using System.Text.Json;
using System.Xml;
using Common.Error;
using Common.Model;
using Microsoft.AspNetCore.Diagnostics;

namespace Ice.Login.Http.Extensions;


public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var knownException = exceptionHandlerPathFeature.Error as IKnownException;

                if (knownException == null)
                {
                    var logger = context.RequestServices.GetService<ILogger<KnownException>>()!;
                    logger.LogError(exceptionHandlerPathFeature.Error, exceptionHandlerPathFeature.Error.Message);
                    knownException = KnownException.Unknown;
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
                else
                {
                    knownException = KnownException.FromKnownException(knownException);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var response = new ApiResult
                {
                    Success = false,
                    Message = knownException.Message,
                    ErrorCode = knownException.ErrorCode,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    Data = knownException.ErrorData
                };

                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
            });
        });
    }
}
