using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.DataAccess.MongoDb.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.DTOs;
using MongoDB.Driver;

namespace Business.Concrete
{
    /// <summary>
    ///     Mongo Database Log Manager
    /// </summary>
    public class MongoDbLogManager : IMongoDbLogService
    {
        private readonly IMongoCollection<MongoDbLog> _collection;

        /// <summary>
        ///     Mongo Database Log Manager consturctor
        /// </summary>
        /// <param name="settings"></param>
        public MongoDbLogManager(IMongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<MongoDbLog>(settings.CollectionName);
        }

        /// <summary>
        ///     Get logs
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetMongoDbLogs")]
        [CacheAspect]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogs(DateTime startDate, DateTime endDate)
        {
            if (startDate != default && endDate == default) return await GetLogsByDate(startDate);
            if (startDate == default || endDate == default) return await GetAllLogs();

            var min = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var max = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var result = await _collection
                .FindAsync(x => (x.Timestamp > min) & (x.Timestamp < max)).Result.ToListAsync();
            return new SuccessDataResult<List<MongoDbLog>>(result, Messages.LogsListed);
        }

        /// <summary>
        ///     Get filtered logs
        /// </summary>
        /// <param name="searchRequestDto"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetMongoDbLogs")]
        public async Task<IDataResult<List<MongoDbLog>>> GetFilteredLogs(MongoLogSearchRequestDto searchRequestDto)
        {
            var result = await _collection.FindAsync(x =>
                    x.MessageTemplate.StartsWith(
                        $"Project: {searchRequestDto.ProjectName} Key: {searchRequestDto.Key}"))
                .Result.ToListAsync();
            return new SuccessDataResult<List<MongoDbLog>>(result, Messages.LogsListed);
        }

        /// <summary>
        ///     Get logs by date
        /// </summary>
        /// <param name="logDate"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetMongoDbLogs")]
        [CacheAspect]
        private async Task<IDataResult<List<MongoDbLog>>> GetLogsByDate(DateTime logDate)
        {
            var min = new DateTime(logDate.Year, logDate.Month, logDate.Day, 0, 0, 0);
            var max = new DateTime(logDate.Year, logDate.Month, logDate.Day, 23, 59, 59);
            var result = await _collection
                .FindAsync(x => (x.Timestamp > min) & (x.Timestamp < max)).Result.ToListAsync();
            return new SuccessDataResult<List<MongoDbLog>>(result, Messages.LogsListed);
        }

        /// <summary>
        ///     Get all logs
        /// </summary>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetMongoDbLogs")]
        [CacheAspect]
        private async Task<IDataResult<List<MongoDbLog>>> GetAllLogs()
        {
            var result = await _collection.FindAsync(x => true);
            return new SuccessDataResult<List<MongoDbLog>>(await result.ToListAsync(), Messages.LogsListed);
        }
    }
}