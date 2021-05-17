using System.Threading.Tasks;
using Business.Abstract;
using Core.Utilities.Results;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("register")]
        public Task<IResult> Register([FromBody] UserForRegisterDto model)
        {
            return _authenticationService.Register(model, Url.Content("~/auth/confirm-mail"));
        }

        [HttpPost]
        [Route("register-admin")]
        public Task<IResult> RegisterAdmin([FromBody] UserForRegisterDto model)
        {
            return _authenticationService.RegisterAdmin(model, Url.Content("~/auth/confirm-mail"));
        }

        [HttpPost]
        [Route("login")]
        public Task<IDataResult<TokenResponseDto>> Login([FromBody] UserForLoginDto model)
        {
            return _authenticationService.Login(model);
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
        public Task<IResult> EnableTwoFactorSecurity(string id)
        {
            return _authenticationService.EnableTwoFactorSecurity(id);
        }

        [HttpPost]
        [Route("disable-two-factor-security")]
        public Task<IResult> DisableTwoFactorSecurity(string id)
        {
            return _authenticationService.DisableTwoFactorSecurity(id);
        }

        [HttpPost]
        [Route("login-with-two-factor-security")]
        public Task<IResult> LoginWithTwoFactorSecurity(string code)
        {
            return _authenticationService.LoginWithTwoFactorSecurity(code);
        }
    }
}