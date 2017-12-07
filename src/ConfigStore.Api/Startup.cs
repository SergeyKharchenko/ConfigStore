using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Swashbuckle.AspNetCore.Swagger;

namespace ConfigStore.Api {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();

            services.AddSwaggerGen(options => {
                                       options.SwaggerDoc("v1", new Info { Title = "Storage API", Version = "v1" });
                                   });

            services.AddSingleton(Configuration);
            services.AddScoped(provider => new KeyVaultClient(GetAccessToken));
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope) {
            var clientCredential = new ClientCredential(Configuration["ClientId"], Configuration["ClientSecret"]);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "Storage API v1");
                             });
        }
    }
}