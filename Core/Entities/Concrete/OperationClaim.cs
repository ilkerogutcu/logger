using System;
using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class OperationClaim:IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}