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
    [AllowAnonymous]
    public class ApplicationController : Controller {
        private readonly ConfigStoreContext _context;

        public ApplicationController(ConfigStoreContext context) {
            _context = context;
        }

        [HttpGet("canRegister")]
        public async Task<IActionResult> CanRegisterApplication([FromQuery] RegisterApplicationDto registerApplicationDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = registerApplicationDto.Name.ToLower();
            bool canRegisterApplication = !await _context.Applications.AnyAsync(app => Equals(app.Name, name));
            return Json(new { canRegisterApplication });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterApplication([FromBody] RegisterApplicationDto registerApplicationDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = registerApplicationDto.Name.ToLower();
            Guid key = Guid.NewGuid();
            try {
                await _context.Applications.AddAsync(new Application {
                    Name = name,
                    Key = Guid.NewGuid()
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return this.Json(ErrorDto.Create(ErrorCodes.ApplicationNameAleadyBusy));
            }
            return Json(new { ApplicationKey = key });
        }
    }
}