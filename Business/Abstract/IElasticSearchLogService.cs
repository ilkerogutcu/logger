using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.ElasticSearch.Models;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IElasticSearchLogService
    {
        public Task<IDataResult<IEnumerable<ElasticSearchGetModel<Log>>>> GetLogsByDate(DateTime logDate,
            int from = 0, int size = 10);

        public Task<IDataResult<List<List<ElasticSearchGetModel<Log>>>>> GetLogsByDateRange(
            DateTime startDate, DateTime endDate, int from = 0, int size = 10);

        public Task<IDataResult<List<ElasticSearchGetModel<Log>>>> GetSearchByField(
            ElasticSearchRequestDto elasticSearchRequestDto);
    }
}