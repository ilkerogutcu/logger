using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results;

namespace Business.Abstract
{
    public interface IMongoDbLogService
    {
        public Task<IDataResult<List<MongoDbLog>>> GetLogFilesByDateRange(DateTime startDate, DateTime endDate);
        public Task<IDataResult<List<MongoDbLog>>> GetLogsByDate(DateTime logDate);
        public Task<IDataResult<List<MongoDbLog>>> GetAllLogs();
    }
}