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

        [LogAspect(typeof(FileLogger), "Register")]
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
            if (!result.Succeeded)
                return !result.Succeeded
                    ? new ErrorResult(result.Errors.ToList()[0].Description)
                    : new SuccessResult(Messages.UserCreatedSuccessfully);
            await SendEmailForConfirmation(user, url);
                
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            await _userManager.AddToRolesAsync(user, new List<string> {UserRoles.User});


            return !result.Succeeded
                ? new ErrorResult(Messages.FailedToRegisterNewUser)
                : new SuccessResult(Messages.UserCreatedSuccessfully);
        }

        [LogAspect(typeof(FileLogger), "Register")]
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

        [LogAspect(typeof(FileLogger), "Login")]
        public async Task<IDataResult<TokenResponseDto>> Login(UserForLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new ErrorDataResult<TokenResponseDto>(Messages.FailedToCreateToken);
            if (!result) return new ErrorDataResult<TokenResponseDto>(Messages.SignInFailed);
            await _signInManager.SignOutAsync();
            var token = await GenerateJwtSecurityToken(user);

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password,
                false, false);

            if (!signInResult.RequiresTwoFactor)
                return new SuccessDataResult<TokenResponseDto>(new TokenResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss"),
                }, Messages.TokenCreatedSuccessfully);
            await SendTwoFactorToken(user.Email);
            return new ErrorDataResult<TokenResponseDto>(Messages.RequiredTwoFactoryCode);
        }

        [LogAspect(typeof(FileLogger), "Signout")]
        public async void SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        [LogAspect(typeof(FileLogger), "Email")]
        public async Task<IResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new ErrorResult(Messages.UserNotFound);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded
                ? new SuccessResult(Messages.EmailSuccessfullyConfirmed)
                : new ErrorResult(Messages.ErrorVerifyingMail);
        }

        [LogAspect(typeof(FileLogger), "Enable")]
        public async Task<IResult> EnableTwoFactorSecurity(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.TwoFactorEnabled = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? new SuccessResult(Messages.UpdatedUserSuccessfully)
                : new ErrorResult(Messages.FailedToUpdateUser);
        }

        [LogAspect(typeof(FileLogger), "Disable")]
        public async Task<IResult> DisableTwoFactorSecurity(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.TwoFactorEnabled = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? new SuccessResult(Messages.UpdatedUserSuccessfully)
                : new ErrorResult(Messages.FailedToUpdateUser);
        }

        public async Task<IResult> LoginWithTwoFactorSecurity(string code)
        {
            var result = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            return result.Succeeded
                ? new SuccessResult(Messages.SignInSuccessfully)
                : new ErrorResult(Messages.SignInFailed);
        }

        private async Task SendTwoFactorToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _mailService.SendEmailAsync(new MailRequest
            {
                ToEmail = user.Email,
                Subject = $"Your code:{token}",
                Body = $"<p style='text-align:center;'>Your code:{token}</p>" + Messages.HtmlDontShareVerificationToken
            });
        }

        [LogAspect(typeof(FileLogger), "Generate")]
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

        [LogAspect(typeof(FileLogger), "Register")]
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