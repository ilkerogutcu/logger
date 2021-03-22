using System.Collections.Generic;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.JWT;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        [LogAspect(typeof(FileLogger))]
        public IDataResult<User> RegisterAuthorizedUser(UserForRegisterDto userForRegisterDto, string password)
        {
            return UserRegisterDataResult(userForRegisterDto, password);

        }

        public IDataResult<User> RegisterUnauthorizedUser(UserForRegisterDto userForRegisterDto, string password)
        {
            return UserRegisterDataResult(userForRegisterDto, password);
        }

        private IDataResult<User> UserRegisterDataResult(UserForRegisterDto userForRegisterDto, string password)
        {
            HashingHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };
            _userService.AddAuthorizedUser(user, userForRegisterDto.Roles);
            return new SuccessDataResult<User>(user, Messages.UserRegistered);
        }

        [LogAspect(typeof(FileLogger))]
        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email);
            if (userToCheck == null || !HashingHelper.VerifyPasswordHash(userForLoginDto.Password,
                userToCheck.PasswordHash,
                userToCheck.PasswordSalt)) return new ErrorDataResult<User>(Messages.UserOrPasswordIsIncorrect);

            return new SuccessDataResult<User>(userToCheck, Messages.SuccessfulLogin);
        }

        [LogAspect(typeof(FileLogger))]
        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null) return new ErrorResult(Messages.UserAlreadyExists);
            return new SuccessResult();
        }

        [LogAspect(typeof(FileLogger))]
        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
        }
    }
}