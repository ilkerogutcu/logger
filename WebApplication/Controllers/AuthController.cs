using System.Collections.Generic;
using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success) return BadRequest(userToLogin.Message);

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success) return Ok(result.Data);

            return BadRequest(result.Message);
        }

        [HttpPost("RegisterAuthorizedUser")]
        public ActionResult RegisterAuthorizedUser(UserForRegisterDto userForRegisterDto)
        {
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success) return BadRequest(userExists.Message);

            var registerResult = _authService.RegisterAuthorizedUser(userForRegisterDto, userForRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success) return Ok(result.Data);

            return BadRequest(result.Message);
        }
        [HttpPost("RegisterUnauthorizedUser")]
        public ActionResult RegisterUnauthorizedUser(UserForRegisterDto userForRegisterDto)
        {
            //todo I'll update here
            userForRegisterDto.Roles = new List<string>
            {
                "user"
            };
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success) return BadRequest(userExists.Message);

            var registerResult = _authService.RegisterUnauthorizedUser(userForRegisterDto, userForRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success) return Ok(result.Data);

            return BadRequest(result.Message);
        }
    }
}