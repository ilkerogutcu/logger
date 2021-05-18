using System.Threading.Tasks;
using Business.Abstract;
using Core.Utilities.Results;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto model)
        {
            var result = await _authenticationService.Register(model, Url.Content("~/auth/confirm-mail"));
            return result.Success
                ? StatusCode(StatusCodes.Status201Created, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserForRegisterDto model)
        {
            var result = await _authenticationService.RegisterAdmin(model, Url.Content("~/auth/confirm-mail"));
            return result.Success
                ? StatusCode(StatusCodes.Status201Created, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto model)
        {
            var result = await _authenticationService.Login(model);
            return result.Success
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [HttpPost]
        [Route("sign-out")]
        public new void SignOut()
        {
            _authenticationService.SignOut();
        }


        [HttpPost]
        [Route("confirm-mail")]
        public Task<IResult> ConfirmMail(string token, string email)
        {
            return _authenticationService.ConfirmEmail(token, email);
        }

        [HttpPost]
        [Route("enable-two-factor-security")]
        public async Task<IActionResult> EnableTwoFactorSecurity(string id)
        {
            var result = await _authenticationService.EnableTwoFactorSecurity(id);
            return result.Success
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [HttpPost]
        [Route("disable-two-factor-security")]
        public async Task<IActionResult> DisableTwoFactorSecurity(string id)
        {
            var result = await _authenticationService.DisableTwoFactorSecurity(id);
            return result.Success
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }

        [HttpPost]
        [Route("login-with-two-factor-security")]
        public async Task<IActionResult> LoginWithTwoFactorSecurity(string code)
        {
            var result = await _authenticationService.LoginWithTwoFactorSecurity(code);
            return result.Success
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, result);
        }
    }
}