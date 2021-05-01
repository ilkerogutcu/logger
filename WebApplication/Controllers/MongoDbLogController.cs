using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/mongo-db-log")]
    public class MongoDbLogController : ControllerBase
    {
        private readonly IMongoDbLogService _mongoDbLogService;

        public MongoDbLogController(IMongoDbLogService mongoDbLogService)
        {
            _mongoDbLogService = mongoDbLogService;
        }

        [HttpGet]
        [Route("get-logs")]
        public async Task<IDataResult<List<MongoDbLog>>> GetAllLogs()
        {
            return await _mongoDbLogService.GetAllLogs();
        }

        [HttpGet]
        [Route("get-logs-by-date")]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogsByDate(DateTime logDate)
        {
            return await _mongoDbLogService.GetLogsByDate(logDate);
        }

        [HttpGet]
        [Route("get-logs-by-date-range")]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _mongoDbLogService.GetLogFilesByDateRange(startDate, endDate);
        }
    }
}