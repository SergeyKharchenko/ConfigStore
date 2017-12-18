using System;
using System.Threading.Tasks;
using ConfigStore.Api.Authorization;
using ConfigStore.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Azure.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
                                var builder = new AuthorizationPolicyBuilder();
                                builder.Requirements.Add(new AuthorizationHandler(func => {
                                    using (var context = services.BuildServiceProvider().GetService<ConfigStoreContext>()) {
                                        return func(context);
                                    }
                                }));
                                options.Filters.Add(new AuthorizeFilter(builder.Build()));
                                if (!Environment.IsEnvironment("Localhost")) {
                                    options.Filters.Add(new RequireHttpsAttribute());
                                }
                            })
                    .AddJsonOptions(options => {
                                        options.SerializerSettings.ContractResolver =
                                            new CamelCasePropertyNamesContractResolver();
                                    });

            services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => options.RequireHttpsMetadata = false);

            services.AddSwaggerGen(options => {
                                       options.SwaggerDoc("v1", new Info { Title = "Storage API", Version = "v1" });
                                       options.OperationFilter<SwaggerAuthorizationHeaderParameters>();
                                   });

            services.AddSingleton(Configuration);

        services.AddDbContext<ConfigStoreContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(provider => new KeyVaultClient(GetAccessToken));
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

            ExecuteWithContext(app, context => context.Database.Migrate());
        }

        private static void ExecuteWithContext(IApplicationBuilder app, Action<ConfigStoreContext> action) {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
                ConfigStoreContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigStoreContext>();
                action(context);
            }
        }
    }
}