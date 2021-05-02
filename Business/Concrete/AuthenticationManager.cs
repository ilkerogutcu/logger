using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Mail;
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
        private readonly IMailService _mailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationManager(IConfiguration configuration, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, IMailService mailService,
            SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
            _mailService = mailService;
            _signInManager = signInManager;
        }

        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> Register(UserForRegisterDto model, string url)
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
            await SendEmailForConfirmation(user, url);
            await _userManager.AddToRolesAsync(user, new List<string> {UserRoles.User});

            return !result.Succeeded
                ? new ErrorResult(Messages.FailedToRegisterNewUser)
                : new SuccessResult(Messages.UserCreatedSuccessfully);
        }

        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> RegisterAdmin(UserForRegisterDto model, string url)
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
            await _signInManager.SignOutAsync();
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password,
                false, false);
            if (!signInResult.Succeeded) return new ErrorDataResult<TokenResponseDto>(Messages.SignInFailed);

            var token = await GenerateJwtSecurityToken(user);

            return new SuccessDataResult<TokenResponseDto>(new TokenResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss")
            }, Messages.TokenCreatedSuccessfully);
        }

        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new ErrorResult(Messages.UserNotFound);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded
                ? new SuccessResult(Messages.EmailSuccessfullyConfirmed)
                : new ErrorResult(Messages.ErrorVerifyingMail);
        }

        private async Task<JwtSecurityToken> GenerateJwtSecurityToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            var authSigninKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenOptions:SecurityKey"]));

            return new JwtSecurityToken(
                _configuration["TokenOptions:Issuer"],
                _configuration["TokenOptions:Audience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha512Signature)
            );
        }

        [LogAspect(typeof(FileLogger))]
        private async Task SendEmailForConfirmation(ApplicationUser user, string url)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = $"{url}?token={token}&email={user.Email}";
            await _mailService.SendEmailAsync(new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Confirm your mail!",
                Body = confirmationLink
            });
        }
    }
}