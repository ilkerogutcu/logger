using System.Collections.Generic;
using Core.Entities.Concrete;

namespace Business.Abstract
{
    public interface IUserService
    {
        List<OperationClaim> GetClaims(User user);
        void AddAuthorizedUser(User user,List<string> roles);
        void AddUnauthorizedUser(User user);
        User GetByMail(string email);
    }
}