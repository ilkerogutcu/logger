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
        Task<IResult> ConfirmEmail(string token, string email);
    }
}