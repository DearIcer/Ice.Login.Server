using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Ice.Login.Repository.Context;
using Ice.Login.Repository.IRepository.Base;
using Ice.Login.Service.IService.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
