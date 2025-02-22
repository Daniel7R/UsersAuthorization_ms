using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersAuthorization.Application.DTO;
using UsersAuthorization.Application.Interfaces;

namespace UsersAuthorization.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a user and return it 
        /// </summary>
        [HttpPost]
        [Route("register", Name = "RegisterUser")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = (typeof(ResponseDTO<string?>)))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<object?>))]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            var responseRegister = await _authService.Register(model);
            ResponseDTO<string?> _response = new(); 
            if (!string.IsNullOrEmpty(responseRegister))
            {
                _response.IsSuccess = false;
                _response.Message = responseRegister;

                return BadRequest(_response);
            }

            return Ok(_response);
        }

        /// <summary>
        /// Log in the user and return a 
        /// </summary>
        [HttpPost]
        [Route("login", Name ="LoginUser")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK,Type =(typeof(ResponseDTO<LoginResponseDTO?>)))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseDTO<object?>))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _authService.Login(model);
            ResponseDTO<LoginResponseDTO?> _response = new();

            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";

                return BadRequest(_response);
            }
            _response.Result = loginResponse;

            return Ok(_response);
        }


    }
}
