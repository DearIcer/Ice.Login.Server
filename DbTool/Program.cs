using Ice.Login.Repository.Context;
using Microsoft.EntityFrameworkCore;

try
{
    DbContextOptions<IceDbContext> options;
    while (true)
    {
        Console.WriteLine("Use Mysql:1(这个还不能用，我还在处理==)");
        Console.WriteLine("Use SqlServer:2");
        switch (int.Parse(Console.ReadLine()))
        {
            case 1:
                var serverVersion = new MySqlServerVersion(new Version(5, 7, 26));
                options = new DbContextOptionsBuilder<IceDbContext>()
                            .UseMySql(@"Server=localhost;Database=IceLoginDb;Uid=root;Pwd=root;",serverVersion).Options;
                init(options);
                break;
            case 2:
                options = new DbContextOptionsBuilder<IceDbContext>()
                    .UseSqlServer(@"Server=localhost;Database=IceLoginDb;User Id=sa;Password=123456;TrustServerCertificate=True").Options;
                init(options);
                break;
            default:
                continue;
        }
    }
   
}
catch (Exception ex )
{
    Console.WriteLine(ex);
    throw;
}

static void init(DbContextOptions<IceDbContext> dbContext)
{
    try
    {
        IceDbContext db = new(dbContext);
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        Console.WriteLine("IceDbContext init success");
        Environment.Exit(0);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}