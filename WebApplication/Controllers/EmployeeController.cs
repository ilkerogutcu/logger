using System.Collections.Generic;
using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetEmployees")]
        public IDataResult<List<Employee>> Get()
        {
            return _employeeService.GetAll();
        }
    }
}