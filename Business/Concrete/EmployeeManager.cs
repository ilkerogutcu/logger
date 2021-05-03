using System.Collections.Generic;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities;
using Microsoft.AspNetCore.Authorization;

namespace Business.Concrete
{
    public class EmployeeManager : IEmployeeService
    {
        [LogAspect(typeof(FileLogger))]
        public IDataResult<List<Employee>> GetAll()
        {
            return new SuccessDataResult<List<Employee>>(new List<Employee>
            {
                new()
                {
                    Id = 1,
                    Name = "John",
                    Lastname = "Doe",
                    Salary = 1000
                },
                new()
                {
                    Id = 2,
                    Name = "Katty",
                    Lastname = "Doe",
                    Salary = 2000
                }
            }, Messages.EmployeesListed);
        }
    }
}