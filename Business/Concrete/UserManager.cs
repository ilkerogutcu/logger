using System;
using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public List<OperationClaim> GetClaims(User user)
        {
            return _userDal.GetClaims(user);
        }

        [SecuredOperation("admin")]
        public void AddAuthorizedUser(User user, List<string> roles)
        {
            //todo these codes should be DataAccess Layer. I'll update here
            using var context = new LoggerContext();
            foreach (var role in roles)
            {
                if (context.OperationClaims.Any(x => x.Name == role))
                {
                    context.UserOperationClaims.Add(new UserOperationClaim
                    {
                        User = user,
                        OperationClaim = context.OperationClaims.First(x => x.Name == role)
                    });
                    context.SaveChanges();
                }
            }
        }

        public void AddUnauthorizedUser(User user)
        {
            //todo These codes should be DataAccess layer. I'll update here

            using var context = new LoggerContext();
            context.UserOperationClaims.Add(new UserOperationClaim
            {
                User = user,
                OperationClaim = context.OperationClaims.First(x => x.Name == "user")
            });
            context.SaveChanges();
        }

        public User GetByMail(string email)
        {
            return _userDal.Get(u => u.Email == email);
        }
    }
}