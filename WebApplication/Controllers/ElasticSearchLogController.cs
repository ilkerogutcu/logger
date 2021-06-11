using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.ElasticSearch.Models;
using Core.Utilities.Results;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/elastic-search-log")]
    public class ElasticSearchLogController : ControllerBase
    {
        private readonly IElasticSearchLogService _elasticSearchLogService;

        public ElasticSearchLogController(IElasticSearchLogService elasticSearchLogService)
        {
            _elasticSearchLogService = elasticSearchLogService;
        }
        
        [HttpGet]
        [Route("get-logs-by-date")]
        public async Task<IDataResult<IEnumerable<ElasticSearchGetModel<Log>>>> GetLogsByDate(DateTime logDate,
            int size = 10,
            int from = 0)
        {
            return await _elasticSearchLogService.GetLogsByDate(logDate, from, size);
        }


        [HttpGet]
        [Route("get-logs-by-date-range")]
        public async Task<IDataResult<List<List<ElasticSearchGetModel<Log>>>>> GetLogsByDateRange(
            DateTime startDate, DateTime endDate, int from = 0, int size = 10)
        {
            return await _elasticSearchLogService.GetLogsByDateRange(startDate, endDate, from, size);
        }

        [HttpPost]
        [Route("get-search-by-field")]
        public async Task<IDataResult<List<ElasticSearchGetModel<Log>>>> GetSearchByField(
            ElasticSearchRequestDto elasticSearchRequestDto)
        {
            return await _elasticSearchLogService.GetSearchByField(elasticSearchRequestDto);
        }
    }
}