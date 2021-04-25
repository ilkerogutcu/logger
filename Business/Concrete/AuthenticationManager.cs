using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business.Concrete
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationManager(IConfiguration configuration, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IResult> Register(UserForRegisterDto model)
        {
            if (await _userManager.FindByNameAsync(model.Username) != null)
                return new ErrorResult(Messages.UserAlreadyExist);
            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            return !result.Succeeded
                ? new ErrorResult(Messages.FailedToRegisterNewUser)
                : new SuccessResult(Messages.UserCreatedSuccessfully);
        }

        public async Task<IResult> RegisterAdmin(UserForRegisterDto model)
        {
            if (await _userManager.FindByNameAsync(model.Username) != null)
                return new ErrorResult(Messages.UserAlreadyExist);
            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return new ErrorResult($"{result.Errors.ToList()[0].Description}");
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _userManager.AddToRolesAsync(user, new List<string> {UserRoles.Admin});
            return new SuccessResult(Messages.UserCreatedSuccessfully);
        }

        public async Task<IDataResult<TokenResponseDto>> Login(UserForLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new ErrorDataResult<TokenResponseDto>(Messages.FailedToCreateToken);
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            var authSigninKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenOptions:SecurityKey"]));
            var token = new JwtSecurityToken(
                _configuration["TokenOptions:Issuer"],
                _configuration["TokenOptions:Audience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return new SuccessDataResult<TokenResponseDto>(new TokenResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss")
            }, Messages.TokenCreatedSuccessfully);
        }
    }
}