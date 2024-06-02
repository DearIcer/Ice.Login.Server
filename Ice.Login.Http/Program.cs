using Ice.Login.Http.Extensions;
using Ice.Login.Http.Middleware;
using Ice.Login.Service.Common.JWT;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = SerilogExtensions.SetSerilog(builder.Configuration);
try
{

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    if (builder.Environment.IsDevelopment())
    {
        builder.UseSwaggerSetup(builder.Configuration);
    }

    builder.Services.AddMemoryCache();
    builder.SetupCors();
    builder.Services.AddAutoMapperProfilesFromAssembly("Ice.Login.Http");
    builder.Services.AddIceDbContext(builder.Configuration);
    builder.SetupAutofac();
    builder.Services.ConfigureCustomApiBehavior();
    builder.Services.AddSerilog(dispose: true); 
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    var app = builder.Build();
    app.UseCors("Cors");
    app.UseRequestPeakMiddleware();
    app.UseMiddleware<AskMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseMiddleware<SessionValidationMiddleware>();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCustomExceptionHandler();
    
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}