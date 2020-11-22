// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Data;
using IdentityServer.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            const string connectionStringAppServices = @"UserID=postgres;Password=postgres;Host=localhost;Port=5432;Database=app_services;";
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options => 
            {
                options.UseNpgsql(connectionStringAppServices,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                }
                );
                }
            );

            services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

            const string connectionString = @"UserID=postgres;Password=postgres;Host=localhost;Port=5432;Database=pe_ident;";

            const string connectionStringOperationalStore = @"UserID=postgres;Password=postgres;Host=localhost;Port=5432;Database=pe_ident;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                //options.EmitStaticAudienceClaim = true;
            })
                //.AddInMemoryIdentityResources(Config.IdentityResources)
                //.AddInMemoryApiScopes(Config.ApiScopes)
                //.AddInMemoryApiResources(Config.GetApis())
                //.AddInMemoryClients(Config.Clients)
                 //.AddTestUsers(Config.GetUsers())
                 .AddConfigurationStore(options => //ConfigurationDbContext
                 {
                     options.ConfigureDbContext = builder =>
                         builder.UseNpgsql(connectionStringOperationalStore,
                             sql => sql.MigrationsAssembly(migrationsAssembly));
                 })
                 .AddAspNetIdentity<ApplicationUser>()
                 .AddOperationalStore(options =>  //PersistedGrantDbContext
                 {
                     options.ConfigureDbContext = builder =>
                         builder.UseNpgsql(connectionString,
                             sql => sql.MigrationsAssembly(migrationsAssembly));

                     // this enables automatic token cleanup. this is optional.
                     options.EnableTokenCleanup = true;
                     options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                 });
            

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            //app.UseStaticFiles();
            //app.UseRouting();
            
            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}
