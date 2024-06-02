using System.Reflection;
using AutoMapper;

namespace Ice.Login.Http.Extensions;


public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperProfilesFromAssembly(this IServiceCollection services,
        string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        var profileTypes = assembly.GetTypes()
            .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

        var config = new MapperConfiguration(cfg =>
        {
            foreach (var profileType in profileTypes)
            {
                cfg.AddProfile((Profile)Activator.CreateInstance(profileType));
            }
        });

        var mapper = config.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
}