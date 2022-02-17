using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if(isProd)
            {
                Console.WriteLine("--> Applying Migrations");

                try 
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Migration Failed: {ex.Message}");
                }
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("......Seeding data.....");
                context.Platforms.AddRange(
                    new Platform() {Name = "Dot Net", Publisher="Microsoft", Cost = "Free"},
                    new Platform() {Name = "SQL Server Express", Publisher="Microsoft", Cost = "Free"},
                    new Platform() {Name = "Kubernetes", Publisher="Cloud Native Computing Foundation", Cost = "Free"}
                );

                context.SaveChanges();
            }
        }
    }
}