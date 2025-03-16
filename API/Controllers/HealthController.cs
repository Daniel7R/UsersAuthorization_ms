using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersAuthorization.Application.DTO;
using UsersAuthorization.Application.Interfaces;

namespace UsersAuthorization.API.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok("Healthy");
        }
    }
}
