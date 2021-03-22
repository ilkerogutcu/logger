using System.Collections.Generic;
using Core.Entities.Concrete;
using Core.Utilities.JWT;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> RegisterAuthorizedUser(UserForRegisterDto userForRegisterDto, string password);
        IDataResult<User> RegisterUnauthorizedUser(UserForRegisterDto userForRegisterDto, string password);

        IDataResult<User> Login(UserForLoginDto userForLoginDto);
        IResult UserExists(string email);
        IDataResult<AccessToken> CreateAccessToken(User user);
    }
}