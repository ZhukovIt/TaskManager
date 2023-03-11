using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Data;

namespace TaskManager.Api
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
            // get connection string
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Указывает, будет ли валидироваться издатель при валидации токена
                        ValidateIssuer = true,

                        // Строка, представляющая издателя
                        ValidIssuer = AuthOptions.ISSUER,

                        // Будет ли валидироваться потребитель токена
                        ValidateAudience = true,

                        // Установка потребителя токена
                        ValidAudience = AuthOptions.AUDIENCE,

                        // Будет ли валидироваться время существования
                        ValidateLifetime = true,

                        // Установка ключа безопасности
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

                        // Валидация ключа безопасности
                        ValidateIssuerSigningKey = true
                    };
                }
            );

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
