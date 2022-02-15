using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SilverScreen.Models.Tables;
using SilverScreen.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = Configuration["Jwt:Issuer"],
               ValidAudience = Configuration["Jwt:Issuer"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
           };
       });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SilverScreen", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        new string[] {}
                    }
                };

                c.AddSecurityDefinition("bearerAuth", securityScheme);
                c.AddSecurityRequirement(securityRequirement);

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //IMDbAPIService.LoadMovieIntoDB("The Revenant"); It slows down program startup by 2 seconds
        }
    }
}
