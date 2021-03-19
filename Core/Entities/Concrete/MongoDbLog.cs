using System;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Entities.Concrete
{
    public class MongoDbLog:IEntity
    {
        [BsonId]
        [DataMember]
        public ObjectId _Id { get; set; }

        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string RenderedMessage { get; set; }
        public string UtcTimestamp { get; set; }
    }
}