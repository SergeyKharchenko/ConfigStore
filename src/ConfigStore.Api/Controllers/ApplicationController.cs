using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using ConfigStore.Api.Dto.Input;
using ConfigStore.Api.Dto.Output;
using ConfigStore.Api.Enums;
using ConfigStore.Api.Extensions;
using ConfigStore.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")] 
    [AllowAnonymous]
    public class ApplicationController : Controller {
        private readonly ConfigStoreContext _context;
        private readonly ConfigClient _client;
        private readonly DefaultDataInitializer _defaultDataInitializer;

        public ApplicationController(ConfigStoreContext context, ConfigClient client, DefaultDataInitializer defaultDataInitializer) {
            _context = context;
            _client = client;
            _defaultDataInitializer = defaultDataInitializer;
        }

        [HttpPost("canRegister")]
        public async Task<IActionResult> CanRegister([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = nameDto.Name.ToLower();
            bool canRegisterApplication = !await _context.Applications.AnyAsync(app => app.Name == name);
            return Json(new { canRegisterApplication });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            Guid appKey = Guid.NewGuid();
            try {
                await _context.Applications.AddAsync(new Application {
                    Name = nameDto.Name,
                    Key = appKey,
                    Services = await _defaultDataInitializer.CreateDefaultServices(appKey)
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return Json(ErrorDto.Create(ErrorCodes.ApplicationNameAleadyBusy));
            }
            return Json(new { ApplicationKey = appKey });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] KeyDto keyDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            Application app = await GetApplication(keyDto.Key);
            if (app == null) {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            Dictionary<(Guid AppKey, Guid ServKey, Guid EnvKey), IEnumerable<string>> configs = await CollectConfigs(app);

            return Json(new {
                ApplicationKey = app.Key,
                ApplicationName = app.Name,
                Services = app.Services.Select(serv => new {
                    ServiceKey = serv.Key,
                    ServiceName = serv.Name,
                    Environments = serv.Environments.Select(env => new {
                        EnvironmentKey = env.Key,
                        EnvironmentName = env.Name,
                        Configs = configs[(app.Key, serv.Key, env.Key)]
                    })
                })
            });
        }

        private async Task<Dictionary<(Guid AppKey, Guid ServKey, Guid EnvKey), IEnumerable<string>>> CollectConfigs(Application app) {
            async Task<((Guid AppKey, Guid ServKey, Guid EnvKey) Key, IEnumerable<string> ConfigNames)> CollectConfigTasks(
                ApplicationService serv, ServiceEnvironment env) {
                return (
                    (app.Key, serv.Key, env.Key),
                    await _client.GetConfigNamesAsync(app.Key, serv.Key, env.Key)
                );
            }

            List<Task<((Guid AppKey, Guid ServKey, Guid EnvKey) Key, IEnumerable<string> ConfigNames)>> configTasks =
                app.Services.SelectMany(serv => serv.Environments, CollectConfigTasks)
                   .ToList();

            await Task.WhenAll(configTasks);
            return configTasks.ToDictionary(task => task.Result.Key, task => task.Result.ConfigNames);
        }

        private async Task<Application> GetApplication(Guid appKey) {
            return await _context.Applications.Include(app => app.Services)
                                 .ThenInclude(serv => serv.Environments)
                                 .FirstOrDefaultAsync(app => app.Key == appKey);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] KeyDto keyDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }

            Application app = await GetApplication(keyDto.Key);
            if (app == null) {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            IEnumerable<Task> removeConfigTasks =
                app.Services.SelectMany(serv => serv.Environments,
                                        async (serv, env) =>
                                            await _client.RemoveConfigsAsync(app.Key, serv.Key, env.Key));
            await Task.WhenAll(removeConfigTasks);

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}