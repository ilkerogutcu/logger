using System;

namespace Core.Entities.Concrete
{
    public class Log : IEntity
    {
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}