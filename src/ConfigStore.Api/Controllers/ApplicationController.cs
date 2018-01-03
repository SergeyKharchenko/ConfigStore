using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ConfigStore.Api.Data;
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

        public ApplicationController(ConfigStoreContext context, ConfigClient client) {
            _context = context;
            _client = client;
        }

        [HttpPost("canRegister")]
        public async Task<IActionResult> CanRegister([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = nameDto.Name.ToLower();
            bool canRegisterApplication = !await _context.Applications.AnyAsync(app => Equals(app.Name, name));
            return Json(new { canRegisterApplication });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = nameDto.Name.ToLower();
            Guid key = Guid.NewGuid();
            try {
                await _context.Applications.AddAsync(new Application {
                    Name = name,
                    Key = key
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return Json(ErrorDto.Create(ErrorCodes.ApplicationNameAleadyBusy));
            }
            return Json(new { ApplicationKey = key });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] NameKeyDto nameKeyDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = nameKeyDto.Name.ToLower();
            Application application =
                await _context.Applications.Include(app => app.Environments)
                              .FirstOrDefaultAsync(app => app.Name == name && app.Key == nameKeyDto.Key);
            if (application == null) {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            var environmentTasks = application.Environments.Select(async env => {
                string prefix = $"{ConfigNameResolver.CreatePrefix(application.Name, env.Name)}{ConfigNameResolver.Separator}";
                return new {
                    EnvironmentName = env.Name,
                    Configs = await _client.GetConfigNamesAsync(prefix)
                };
            }).ToList();
            await Task.WhenAll(environmentTasks);

            return Json(new {
                ApplicationName = application.Name,
                Environments = environmentTasks.Select(task => task.Result)
            });
        }
    }
}