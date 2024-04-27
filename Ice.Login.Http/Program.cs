using System.Reflection;
using System.Text;
using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Error;
using Common.Model;
using Ice.Login.Http.Filter;
using Ice.Login.Http.Middleware;
using Ice.Login.Repository.Context;
using Ice.Login.Repository.IRepository.Base;
using Ice.Login.Service.Common.JWT;
using Ice.Login.Service.Service.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });
    builder.Services.AddSwaggerGen(options =>
    {
        var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        var xmlFiles = new DirectoryInfo(basePath).GetFiles().Where(x => x.Name.Contains(".xml")).ToList();
        xmlFiles.ForEach(x => options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, x.Name), true));
    });
}

builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
    options.AddPolicy("Cors",
        cpBuilder =>
            cpBuilder.SetIsOriginAllowed(t => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
    )
);
builder.Services.AddAutoMapper(options =>
{
    var assembly = Assembly.Load("Ice.Login.Http");
    var profileTypes = assembly.GetTypes()
        .Where(t => typeof(Profile).IsAssignableFrom(t));
    foreach (var profileType in profileTypes) options.AddProfile(profileType);
});
builder.Services.AddDbContext<IceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ).UseLoggerFactory(LoggerFactory.Create(it => { })));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterAssemblyTypes(Assembly.Load("Ice.Login.Repository"))
            .Where(t => typeof(IBaseRepository).IsAssignableFrom(t))
            .AsImplementedInterfaces()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
            .InstancePerLifetimeScope();
    });

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterAssemblyTypes(Assembly.Load("Ice.Login.Service"))
            .Where(t => typeof(IBaseService).IsAssignableFrom(t))
            .AsImplementedInterfaces()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
            .InstancePerLifetimeScope();
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
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
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
        };

        return new BadRequestObjectResult(result);
    };
});

builder.Services.AddLogging();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
var jwt = builder.Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>()!;
builder.Services.AddSingleton(jwt);

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.Secret)),
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

app.UseMiddleware<RequestMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
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
        context.Response.ContentType = "application/json; charset=utf-8";
        var rst = new ApiResult
        {
            Success = false,
            Message = knownException.Message,
            ErrorCode = knownException.ErrorCode,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
            Data = knownException.ErrorData
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(rst, options));
    });
});


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();