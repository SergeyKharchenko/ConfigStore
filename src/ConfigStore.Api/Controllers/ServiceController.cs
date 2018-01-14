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
using ConfigStore.Api.Infrastructure.ActionHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")] 
    [Authorize("application")]
    public class ServiceController : Controller {
        private readonly ConfigStoreContext _context;
        private readonly ConfigClient _client;
        private readonly DefaultDataInitializer _defaultDataInitializer;
        private readonly RenameModelActionHandler<ApplicationService> _renameModelActionHandler;
        private readonly CanAddModelActionHandler<ApplicationService> _canAddModelActionHandler;

        public ServiceController(ConfigStoreContext context,
                                 ConfigClient client,
                                 DefaultDataInitializer defaultDataInitializer,
                                 RenameModelActionHandler<ApplicationService> renameModelActionHandler,
                                 CanAddModelActionHandler<ApplicationService> canAddModelActionHandler) {
            _context = context;
            _client = client;
            _defaultDataInitializer = defaultDataInitializer;
            _renameModelActionHandler = renameModelActionHandler;
            _canAddModelActionHandler = canAddModelActionHandler;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }

            Application app = this.GetApplication();
            Guid servKey = Guid.NewGuid();

            try {
                var service = new ApplicationService {
                    Name = nameDto.Name,
                    Key = servKey,
                    Environments = await _defaultDataInitializer.CreateDefaultEnvironments(app.Key, servKey),
                    ApplicationId = app.Id
                };
                await _context.Services.AddAsync(service);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return Json(ErrorDto.Create(ErrorCodes.ServiceNameAleadyBusy));
            }

            return Ok(new { ServiceKey = servKey });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] KeyDto keyDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }

            Application app = this.GetApplication();
            ApplicationService service = await _context.Services.FirstOrDefaultAsync(serv => serv.Key == keyDto.Key);
            if (service == null) {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            List<ServiceEnvironment> envs = 
                await _context.Environments.Where(env => env.ServiceId == service.Id).ToListAsync();

            IEnumerable<Task> removeConfigTasks =
                envs.Select(async env => await _client.RemoveConfigsAsync(app.Key, service.Key, env.Key));
            await Task.WhenAll(removeConfigTasks);

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("rename")]
        public async Task<IActionResult> Rename([FromBody] KeyNameDto keyNameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            bool result = await _renameModelActionHandler.Do(keyNameDto.Key, keyNameDto.Name);
            return result ? Ok() : StatusCode((int)HttpStatusCode.Unauthorized);
        }

        [HttpPost("canAdd")]
        public async Task<IActionResult> CanRegister([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            bool canAdd = await _canAddModelActionHandler.Do(nameDto.Name);
            return Json(new { canAdd });
        }
    }
}