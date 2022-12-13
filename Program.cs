using System;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeiLiving.Quotes.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development"))
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<ApplicationDbContext>();


                    AddTestData(context);
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void AddTestData(ApplicationDbContext context)
        {
            var roleSA = new Role { Id = Guid.NewGuid(), Name = "super_admin", Description = "Super Administrador" };
            var roleAdmin = new Role { Id = Guid.NewGuid(), Name = "admin", Description = "Administrador" };
            var roleAgent = new Role { Id = Guid.NewGuid(), Name = "agent", Description = "Agente" };

            context.Roles.AddRange(new Role[] { roleSA, roleAdmin, roleAgent });

            var user1 = new User
            {
                Id = new Guid("641d7639-ea52-4628-83d2-8a5189f8d3ea"),
                Profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Email = "jdoe@heicommunity.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Password = "1000.C+sce5uUP3L8Tet335Mg/A==.hBln1W+UA4Gp3dlTIzzOkmhEWyENG3nz2xas9YR85RI="
                },
                IsActive = true,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };

            var agent1 = new User
            {
                Id = new Guid("b2e7cdbd-fa4b-48c2-991f-c3b9e6a6f486"),
                Profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Email = "agente@heicommunity.com",
                    FirstName = "Agente",
                    LastName = "Levy Holding",
                    Password = "1000.C+sce5uUP3L8Tet335Mg/A==.hBln1W+UA4Gp3dlTIzzOkmhEWyENG3nz2xas9YR85RI="
                },
                IsActive = true,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                UserRoles = new UserRole[] { new UserRole { Role = roleAgent } }
            };

            var admin1 = new User
            {
                Id = new Guid("953e1346-bc0c-eb11-8ce1-0003ff5661cd"),
                Profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@heicommunity.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Password = "1000.C+sce5uUP3L8Tet335Mg/A==.hBln1W+UA4Gp3dlTIzzOkmhEWyENG3nz2xas9YR85RI="
                },
                IsActive = true,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                UserRoles = new UserRole[] { new UserRole { Role = roleSA } }
            };

            context.Users.AddRange(new User[] { user1, agent1, admin1 });

            context.SaveChanges();
        }
    }
}
