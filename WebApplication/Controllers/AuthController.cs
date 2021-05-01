using System.Threading.Tasks;
using Business.Abstract;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
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
        [LogAspect(typeof(FileLogger))]
        public Task<IDataResult<TokenResponseDto>> Login([FromBody] UserForLoginDto model)
        {
            return _authenticationService.Login(model);
        }


        [HttpPost]
        [Route("confirm-mail")]
        [LogAspect(typeof(FileLogger))]
        public Task<IResult> ConfirmMail(string token, string email)
        {
            return _authenticationService.ConfirmEmail(token, email);
        }
    }
}