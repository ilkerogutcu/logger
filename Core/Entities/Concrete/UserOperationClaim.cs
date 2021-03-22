using System;

namespace Core.Entities.Concrete
{
    public class UserOperationClaim:IEntity
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public OperationClaim OperationClaim { get; set; }
    }
}