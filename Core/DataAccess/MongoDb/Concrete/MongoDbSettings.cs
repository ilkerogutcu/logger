using Core.DataAccess.MongoDb.Abstract;

namespace Core.DataAccess.MongoDb.Concrete
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}