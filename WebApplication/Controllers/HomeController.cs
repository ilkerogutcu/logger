using System.Collections.Generic;
using Business.Abstract;
using Core;
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

        public HomeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IDataResult<List<Employee>> Get()
        {
            return _employeeService.GetAll();
        }
    }
}