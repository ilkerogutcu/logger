﻿using System.Collections.Generic;
using Business.Abstract;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = UserRoles.Admin)]
        [LogAspect(typeof(FileLogger))]
        public IDataResult<List<Employee>> Get()
        {
            return _employeeService.GetAll();
        }
    }
}