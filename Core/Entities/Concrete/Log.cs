using System;
using Nest;


namespace Core.Entities.Concrete
{
    public class Log : IEntity
    {
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        [Date(Name = "@timestamp")]
        public DateTime Timestamp { get; set; }
    }
}