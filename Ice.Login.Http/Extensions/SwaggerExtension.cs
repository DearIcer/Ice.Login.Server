using Microsoft.OpenApi.Models;

namespace Ice.Login.Http.Extensions;


public static class SwaggerExtension
{
    public static WebApplicationBuilder UseSwaggerSetup(this WebApplicationBuilder builder, IConfiguration configuration)
    {
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
        return builder;
    }
}