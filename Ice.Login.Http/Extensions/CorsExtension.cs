namespace Ice.Login.Http.Extensions;

public static class CorsExtension
{
    public static WebApplicationBuilder SetupCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
            options.AddPolicy("Cors",
                cpBuilder =>
                    cpBuilder.SetIsOriginAllowed(t => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
            )
        );
        return builder;
    }
}