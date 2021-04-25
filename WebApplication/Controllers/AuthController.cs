using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("Register")]
        public Task<IResult> Register([FromBody] UserForRegisterDto model)
        {
            return _authenticationService.Register(model);
        }

        [HttpPost]
        [Route("RegisterAdmin")]
        public Task<IResult> RegisterAdmin([FromBody] UserForRegisterDto model)
        {
            return _authenticationService.RegisterAdmin(model);
        }

        [HttpPost]
        [Route("Login")]
        [LogAspect(typeof(FileLogger))]
        public Task<IDataResult<TokenResponseDto>> Login([FromBody] UserForLoginDto model)
        {
            return _authenticationService.Login(model);
        }
    }
}