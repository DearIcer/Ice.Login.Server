using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ice.Login.Repository.IRepository.Base;
using Ice.Login.Service.Service.Base;

namespace Ice.Login.Http.Extensions;


public static class AutofacExtension
{
    public static WebApplicationBuilder SetupAutofac(this WebApplicationBuilder builder)
    {
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
        return builder;
    }
}