using System.Threading.Tasks;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthenticationService
    {
        Task<IResult> Register(UserForRegisterDto model, string url);
        Task<IResult> RegisterAdmin(UserForRegisterDto model, string url);
        Task<IDataResult<TokenResponseDto>> Login(UserForLoginDto model);
        void SignOut();
        Task<IResult> ConfirmEmail(string token, string email);
        Task<IResult> EnableTwoFactorSecurity(string id);
        Task<IResult> DisableTwoFactorSecurity(string id);
        Task<IResult> LoginWithTwoFactorSecurity(string code);
    }
}