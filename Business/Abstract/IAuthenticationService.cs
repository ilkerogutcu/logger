using System.Threading.Tasks;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthenticationService
    {
        Task<IResult> Register(UserForRegisterDto model);
        Task<IResult> RegisterAdmin(UserForRegisterDto model);
        Task<IDataResult<TokenResponseDto>> Login(UserForLoginDto model);
    }
}