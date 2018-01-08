﻿using System;
using System.Data.SqlClient;
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
    [Authorize("service")]
    public class EnvironmentController : Controller {
        private readonly ConfigStoreContext _context;
        private readonly ConfigClient _client;
        private readonly DefaultDataInitializer _defaultDataInitializer;

        public EnvironmentController(ConfigStoreContext context, ConfigClient client, DefaultDataInitializer defaultDataInitializer) {
            _context = context;
            _client = client;
            _defaultDataInitializer = defaultDataInitializer;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            
            Application application = this.GetApplication();
            ApplicationService service = this.GetService();
            Guid envKey = Guid.NewGuid();

            try {
                await _defaultDataInitializer.CreateDefaultConfig(application.Key, service.Key, envKey);
                await _context.Environments.AddAsync(new ServiceEnvironment {
                    Key = envKey,
                    Name = nameDto.Name,
                    ServiceId = service.Id
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return Json(ErrorDto.Create(ErrorCodes.EnvironmentNameAleadyBusy));
            }
            return Ok(new { EnvironmentKey = envKey });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] KeyDto keyDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }

            Application application = this.GetApplication();
            ApplicationService service = this.GetService();
            ServiceEnvironment environment = await _context.Environments.FirstOrDefaultAsync(env => env.Key == keyDto.Key);
            if (environment == null) {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            await _client.RemoveConfigsAsync(application.Key, service.Key, environment.Key);
            _context.Environments.Remove(environment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}