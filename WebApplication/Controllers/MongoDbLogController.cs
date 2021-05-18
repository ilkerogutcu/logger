using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.DTOs;
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
        public async Task<IDataResult<List<MongoDbLog>>> GetLogs(DateTime startDate, DateTime endDate)
        {
            return await _mongoDbLogService.GetLogs(startDate, endDate);
        }


        [HttpPost]
        [Route("filtered-logs")]
        public async Task<IDataResult<List<MongoDbLog>>> FilteredLogs(MongoLogSearchRequestDto requestDto)
        {
            return await _mongoDbLogService.GetFilteredLogs(requestDto);
        }
    }
}