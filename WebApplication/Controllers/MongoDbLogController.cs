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
    [Route("[controller]")]
    public class MongoDbLogController: ControllerBase
    {
        private readonly IMongoDbLogService _mongoDbLogService;

        public MongoDbLogController(IMongoDbLogService mongoDbLogService)
        {
            _mongoDbLogService = mongoDbLogService;
        }
        [HttpGet]
        [Route("GetLogs")]
        public async Task<IDataResult<List<MongoDbLog>>> GetAllLogs()
        {
            return await _mongoDbLogService.GetAllLogs();
        }
        [HttpGet]
        [Route("GetLogsByDate")]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogsByDate(DateTime logDate)
        {
            return await _mongoDbLogService.GetLogsByDate(logDate);
        }
        [HttpGet]
        [Route("GetLogsByDateRange")]
        public async Task<IDataResult<List<MongoDbLog>>> GetLogsByDateRange(DateTime startDate,DateTime endDate)
        {
            return await _mongoDbLogService.GetLogFilesByDateRange(startDate,endDate);
        }
    }
}