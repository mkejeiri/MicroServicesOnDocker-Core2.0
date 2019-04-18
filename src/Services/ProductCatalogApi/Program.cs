using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductCatalogApi.Data;

namespace ProductCatalogApi
{
    /*
     The configure method in the startup class is devoted to operate in the middleware pipeline! (aka components chain),
     unlike in non dockerized system where we could call the seeder inside configure method, and due the problem encountered by Core,
     the database might not ready in dockerized systems, for that reason Core 2.0 + recommends to inject directly
     the db context for seeding in the program class (main methods).
     */
    public class Program
    {
        //public static async Task Main(string[] args)
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CatalogDbContext>();
                    //await  CatalogSeeder.SeedAsync(context);
                    CatalogSeeder.SeedAsync(context).Wait();
                    Console.WriteLine("seeding the database... OK");
                }
                catch (Exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError("An error occurred while seeding the database");
                    Console.WriteLine("An error occurred while seeding the database");
                }
            }
                host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
