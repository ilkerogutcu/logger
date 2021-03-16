using System.Collections.Generic;
using Core.Utilities.Results;
using Entities;

namespace Business.Abstract
{
    public interface IEmployeeService
    {
        public IDataResult<List<Employee>> GetAll();
    }
}