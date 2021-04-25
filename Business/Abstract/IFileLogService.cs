using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results;

namespace Business.Abstract
{
    public interface IFileLogService
    {
        public Task<FileContentResultModel> GetLogFilesByDateRange(DateTime startDate, DateTime endDate);
        public Task<FileContentResultModel> GetLogsByDate(DateTime logDate);
        public IDataResult<List<FileModel>> GetAllLogs();
    }
}