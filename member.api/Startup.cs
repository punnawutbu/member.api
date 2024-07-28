﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using member.api.Models;
using NLog;
using member.api.Shared.DefaultException;

namespace member.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services for controllers
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            // Adds health check service
            services.AddHealthChecks();

            #region Swagger
            // Read Swagger settings
            var swagger = this.Configuration.GetSection("Swagger").Get<SwaggerSettings>();

            // Add Swagger generator
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    swagger.Version,
                    new OpenApiInfo
                    {
                        Title = swagger.Title,
                        Version = swagger.Version,
                        Contact = new OpenApiContact()
                        {
                            Name = swagger.Name,
                            Email = swagger.Email
                        }
                    }
                );
            });
            #endregion

            #region Dependency Injection
            // For more information, visit https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                var swaggerSettings = this.Configuration.GetSection("Swagger").Get<SwaggerSettings>();
                var serviceName = this.Configuration.GetValue<string>("Consul:Discovery:ServiceName");

                // Use Swagger middleware
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        var forwardedHost = httpReq.Headers["X-Forwarded-Host"].FirstOrDefault();

                        if (string.IsNullOrEmpty(forwardedHost))
                            swagger.Servers = new List<OpenApiServer>
                            {
                                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                            };
                        else
                            swagger.Servers = new List<OpenApiServer>
                            {
                                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"].FirstOrDefault()}/{serviceName}/" }
                            };
                    });
                });

                // Use Swagger UI middleware
                app.UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint($"{swaggerSettings.Endpoint}", swaggerSettings.Title);
                    }
                );
                app.UseDefaultException(_logger, true);
            }
            else
            {
                app.UseDefaultException(_logger, false);
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}