using System;
using System.Threading.Tasks;
using ConfigStore.Api.Authorization;
using ConfigStore.Api.Data;
using ConfigStore.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Azure.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace ConfigStore.Api {
    public class Startup {
        private readonly IConfiguration Configuration;
        private readonly IHostingEnvironment Environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment) {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc(options => {
                                if (!Environment.IsEnvironment("Localhost")) {
                                    options.Filters.Add(new RequireHttpsAttribute());
                                }
                            })
                    .AddJsonOptions(options => {
                                        options.SerializerSettings.ContractResolver =
                                            new CamelCasePropertyNamesContractResolver();
                                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                    });

            services.AddAuthorization(options => {
                void ActionWithContext(Action<ConfigStoreContext> action) {
                    using (ConfigStoreContext context = services.BuildServiceProvider().GetService<ConfigStoreContext>()) {
                        action(context);
                    }
                }

                options.AddPolicy("application", builder => {
                        builder.Requirements.Add(new AuthorizationApplicationHandler(ActionWithContext));
                    });

                options.AddPolicy("service", builder => {
                        builder.Requirements.Add(new AuthorizationServiceHandler(ActionWithContext));
                    });

                options.AddPolicy("environment", builder => {
                        builder.Requirements.Add(new AuthorizationEnvironmentHandler(ActionWithContext));
                    });
            });

            services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer();

            services.AddSwaggerGen(options => {
                                       options.SwaggerDoc("v1", new Info { Title = "Storage API", Version = "v1" });
                                       options.OperationFilter<SwaggerAuthorizationHeaderParameters>();
                                   });

            services.AddSingleton(Configuration);
            services.AddSingleton(Environment);

            services.AddDbContext<ConfigStoreContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(provider => new KeyVaultClient(GetAccessToken));

            services.AddSingleton(Configuration);
            services.AddScoped<ConfigClient>();
            services.AddScoped<DefaultDataInitializer>();
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope) {
            var clientCredential = new ClientCredential(Configuration["ClientId"], Configuration["ClientSecret"]);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
        }

        public void Configure(IApplicationBuilder app) {
            if (Environment.IsDevelopment() || Environment.IsEnvironment("Localhost")) {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
            if (!Environment.IsEnvironment("Localhost")) {
                app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "Storage API v1");
                             });

            InitializeDbAsync(app).Wait();
        }

        private static async Task InitializeDbAsync(IApplicationBuilder app) {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
                ConfigStoreContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigStoreContext>();
                context.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
                await context.Database.MigrateAsync();

                ConfigClient client = serviceScope.ServiceProvider.GetRequiredService<ConfigClient>();
                await client.ClearAsync();

                DbInitializer initializer = new DbInitializer(context, client);
                await initializer.SeedAsync();
            }
        }
    }
}