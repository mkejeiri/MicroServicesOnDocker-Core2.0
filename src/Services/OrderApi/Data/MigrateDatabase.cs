using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MicroServicesOnDocker.Services.OrderApi.Data
{
    public static class MigrateDatabase
    {
        public static void EnsureCreated(OrdersContext context)
        {
            System.Console.WriteLine("Creating database...");
            //context.Database.EnsureCreated(); //<-- not appropriate since we will run only migration
            //those lines also not needed
            //RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            //databaseCreator.CreateTables();
            context.Database.Migrate();
            System.Console.WriteLine("Database and tables' creation complete.....");
        }
    }
}
