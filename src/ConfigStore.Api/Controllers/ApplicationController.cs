using System;
using System.Data.SqlClient;
using System.Net;
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

        [HttpPost("canRegister")]
        public async Task<IActionResult> CanRegister([FromBody] ApplicationDto applicationDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = applicationDto.Name.ToLower();
            bool canRegisterApplication = !await _context.Applications.AnyAsync(app => Equals(app.Name, name));
            return Json(new { canRegisterApplication });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApplicationDto applicationDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = applicationDto.Name.ToLower();
            Guid key = Guid.NewGuid();
            try {
                await _context.Applications.AddAsync(new Application {
                    Name = name,
                    Key = key
                });
                await _context.SaveChangesAsync();
            } catch (DbUpdateException e) when ((e.InnerException as SqlException)?.ErrorCode == -2146232060) {
                return this.Json(ErrorDto.Create(ErrorCodes.ApplicationNameAleadyBusy));
            }
            return Json(new { ApplicationKey = key });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApplicationDto applicationDto) {
            if (!ModelState.IsValid) {
                return this.ValidationError();
            }
            string name = applicationDto.Name.ToLower();
            bool canLogin = !await _context.Applications.AnyAsync(app => Equals(app.Name, name));
            return canLogin ? Ok() : StatusCode((int)HttpStatusCode.Unauthorized);
        }
    }
}