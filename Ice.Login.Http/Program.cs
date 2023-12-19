using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Error;
using Common.Model;
using Ice.Login.Repository.Context;
using Ice.Login.Repository.IRepository.Base;
using Ice.Login.Service.Service.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    foreach (var profileType in profileTypes)
    {
        options.AddProfile(profileType);
    }
});
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

//builder.Services.AddDbContext<IceDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
//    //options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//    //                 new MySqlServerVersion(new Version(5, 7, 26)))

//    ));

builder.Services.AddDbContext<IceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<DbContext, IceDbContext>();

var app = builder.Build();

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
        IExceptionHandlerPathFeature exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
        IKnownException? knownException = exceptionHandlerPathFeature.Error as IKnownException;
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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        context.Response.ContentType = "application/json; charset=utf-8";
        ApiResult rst = new ApiResult
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

app.UseAuthorization();

app.MapControllers();

app.Run();
