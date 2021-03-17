using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Core;
using Core.Entities.Concrete;
using Core.Utilities.ElasticSearch;
using Core.Utilities.ElasticSearch.Models;
using Core.Utilities.Results;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IElasticSearch _elasticSearch;
        public HomeController(IEmployeeService employeeService, IElasticSearch elasticSearch)
        {
            _employeeService = employeeService;
            _elasticSearch = elasticSearch;
        }

        [HttpGet]
        public IDataResult<List<Employee>> Get()
        {
            return _employeeService.GetAll();
        }

        [HttpGet]
        [Route("GetLogs")]
        public async Task<IDataResult<IEnumerable<ElasticSearchGetModel<Log>>>> GetLogsByDate(DateTime logDate, int size=10,
            int from = 0)
        {
            var list = await _elasticSearch.GetAllSearch<Log>(new SearchParameters()
            {
                IndexName = "logger-"+logDate.ToString("yyyy.MM.dd"),
                From = from,
                Size = size
            });
            return new SuccessDataResult<List<ElasticSearchGetModel<Log>>>(list);
        }
    }
}