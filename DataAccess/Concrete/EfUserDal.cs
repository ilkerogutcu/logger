using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete
{
    public class EfUserDal : EfEntityRepositoryBase<User, LoggerContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using var context = new LoggerContext();
            var result = from operationClaim in context.OperationClaims
                join userOperationClaim in context.UserOperationClaims
                    on operationClaim.Id equals userOperationClaim.OperationClaim.Id
                where userOperationClaim.User.Id == user.Id
                select new OperationClaim {Id = operationClaim.Id, Name = operationClaim.Name};
            return result.ToList();
        }
    }
}