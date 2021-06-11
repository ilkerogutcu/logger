using System.Collections.Generic;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Entities;

namespace Business.Concrete
{
    public class EmployeeManager : IEmployeeService
    {
        /// <summary>
        ///     Get all employees for example log output
        /// </summary>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetEmployee")]
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

        /// <summary>
        ///     Update employee for example log output
        /// </summary>
        /// <returns></returns>
        [LogAspect(typeof(ElasticsearchLogger), "UpdateEmployee")]
        public IDataResult<Employee> Update(Employee employee)
        {
            employee.Lastname = "öğütcü";
            return new SuccessDataResult<Employee>(employee, Messages.EmployeesListed);
        }
    }
}