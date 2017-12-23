using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ConfigStore.Api.Data;
using ConfigStore.Api.Dto.Input;
using ConfigStore.Api.Dto.Output;
using ConfigStore.Api.Enums;
using ConfigStore.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Controllers {
    [Route("api/[controller]")] 
    [Authorize("application")]
    public class EnvironmentController : Controller {
        private readonly ConfigStoreContext _context;

        public EnvironmentController(ConfigStoreContext context) {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }

            Application application = this.GetApplication();
            string name = nameDto.Name.ToLower();

            try {   
                await _context.Environments.AddAsync(new ApplicationEnvironment {
                    Name = name,
                    ApplicationId = application.Id
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return Json(ErrorDto.Create(ErrorCodes.EnvironmentNameAleadyBusy));
            }
            return Ok();
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] NameDto nameDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }

            Application application = this.GetApplication();
            string name = nameDto.Name.ToLower();

            try {   
                _context.Environments.Remove(new ApplicationEnvironment {
                    Name = name,
                    ApplicationId = application.Id
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                return Json(ErrorDto.Create(ErrorCodes.EnvironmentNameNotFound));
            }
            return Ok();
        }
    }
}