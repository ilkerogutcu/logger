using System.Collections.Generic;
using Business.Abstract;
using Core.Utilities.Results;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("get-employees")]
        // [Authorize(Roles = UserRoles.Admin)]
        public IDataResult<List<Employee>> Get()
        {
            return _employeeService.GetAll();
        }

        [HttpPost]
        [Route("update-employee")]
        // [Authorize(Roles = UserRoles.Admin)]
        public IDataResult<Employee> Update(Employee employee)
        {
            return _employeeService.Update(employee);
        }
    }
}