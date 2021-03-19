using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.DataAccess.MongoDb.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using MongoDB.Driver;

namespace Business.Concrete
{
    public class MongoDbLogManager : IMongoDbLogService
    {
        private readonly IMongoCollection<MongoDbLog> _collection;

        public MongoDbLogManager(IMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<MongoDbLog>(settings.CollectionName);
        }

        [LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        [CacheAspect]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogFilesByDateRange(DateTime startDate, DateTime endDate)
        {
            var min = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var max = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 0, 0);

            var result = await _collection
                .FindAsync(x => (x.Timestamp > min) & (x.Timestamp < max)).Result.ToListAsync();
            return new SuccessDataResult<List<MongoDbLog>>(result, Messages.LogsListed);
        }

        [LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        [CacheAspect]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogsByDate(DateTime logDate)
        {
            var min = new DateTime(logDate.Year, logDate.Month, logDate.Day, 0, 0, 0);
            var max = new DateTime(logDate.Year, logDate.Month, logDate.Day, 23, 0, 0);
            var result = await _collection
                .FindAsync(x => (x.Timestamp > min) & (x.Timestamp < max)).Result.ToListAsync();
            return new SuccessDataResult<List<MongoDbLog>>(result, Messages.LogsListed);
        }

        [LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        [CacheAspect]
        public async Task<IDataResult<List<MongoDbLog>>> GetAllLogs()
        {
            var result = (await _collection.FindAsync(x => true))
                .ToListAsync().Result;
            return new SuccessDataResult<List<MongoDbLog>>(result, Messages.LogsListed);
        }
    }
}