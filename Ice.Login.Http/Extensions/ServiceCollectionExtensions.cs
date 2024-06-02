using System.Text;
using Common.Model;
using Ice.Login.Repository.Context;
using Ice.Login.Service.Common.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ice.Login.Http.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIceDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {});
        services.AddDbContext<IceDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .UseLoggerFactory(loggerFactory); 
        });

        return services;
    }
    public static IServiceCollection ConfigureCustomApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState
                    .Where(s => s.Value != null && s.Value.ValidationState == ModelValidationState.Invalid)
                    .SelectMany(s => s.Value!.Errors.ToList())
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var result = new ApiResult
                {
                    ErrorCode = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Model validation fails",
                    Data = errors,
                    Success = false,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                };

                return new BadRequestObjectResult(result);
            };
        });

        return services;
    }
    
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>()!;

        services.AddSingleton(jwtConfig);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
