using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilverScreen
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SilverScreen", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[!] Running migrations. Please wait...");
            try
            {
                SilverScreenContext context = new SilverScreenContext(Configuration);
                context.Database.Migrate();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) //In case DB has been modified externally it throws an exception
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] There was an error while running migration. Please wait while we fix the error...");

                SilverScreenContext context = new SilverScreenContext(Configuration);
                context.EfmigrationsHistories.RemoveRange(context.EfmigrationsHistories); //Remove previous versions

                string[] migrationsDirectory = Directory.GetFiles(@"Migrations\", "*.cs"); //Get all migrations

                for (int i = 0; i < migrationsDirectory.Length; i++)
                {
                    migrationsDirectory[i] = migrationsDirectory[i].Remove(0, 11); //Remove directory from string
                }

                foreach (var migration in migrationsDirectory)
                {
                    //Regex to exclude the files that are not migrations  
                    if (Regex.IsMatch(migration, "^((?!((SilverScreenContextModelSnapshot.cs)|(Designer.cs))).)*$"))
                    {
                        context.EfmigrationsHistories.Add(new EfmigrationsHistory
                        {
                            ProductVersion = "5.0.13", //Current version that could change
                            MigrationId = migration.Remove(migration.Length - 3) //Removes .cs since we dont need it
                        }); ;
                    }
                }
                context.SaveChanges();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SilverScreen v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
