using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.ElasticSearch;
using Core.Utilities.ElasticSearch.Models;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Concrete
{
    /// <summary>
    ///     ElasticSearch Log Manager
    /// </summary>
    public class ElasticSearchLogManager : IElasticSearchLogService
    {
        private readonly IElasticSearch _elasticSearch;

        /// <summary>
        ///     ElasticSearch Log Manager Constructor
        /// </summary>
        /// <param name="elasticSearch"></param>
        public ElasticSearchLogManager(IElasticSearch elasticSearch)
        {
            _elasticSearch = elasticSearch;
        }

        /// <summary>
        ///     Get logs by date
        /// </summary>
        /// <param name="logDate"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetElasticSearchLogs")]
        [CacheAspect]
        public async Task<IDataResult<IEnumerable<ElasticSearchGetModel<Log>>>> GetLogsByDate(DateTime logDate,
            int from = 0, int size = 10)
        {
            var list = await ElasticSearchGetModels(logDate, from, size);
            if (list.Count > 0)
                return new SuccessDataResult<List<ElasticSearchGetModel<Log>>>(list, Messages.LogsListed);

            return new ErrorDataResult<List<ElasticSearchGetModel<Log>>>(Messages.LogsNotListed);
        }


        /// <summary>
        ///     Get logs by date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetElasticSearchLogs")]
        [CacheAspect]
        public async Task<IDataResult<List<List<ElasticSearchGetModel<Log>>>>> GetLogsByDateRange(
            DateTime startDate, DateTime endDate, int from = 0, int size = 10)
        {
            var logList = new List<List<ElasticSearchGetModel<Log>>>();
            using var iterator = Utilities.GetDateRange(startDate, endDate).GetEnumerator();
            while (iterator.MoveNext())
            {
                var logDate = iterator.Current;
                logList.Add(await ElasticSearchGetModels(logDate, from, size));
            }

            return new SuccessDataResult<List<List<ElasticSearchGetModel<Log>>>>(logList, Messages.LogsListed);
        }

        /// <summary>
        ///     Get search by field
        /// </summary>
        /// <param name="elasticSearchRequestDto"></param>
        /// <returns></returns>
        public async Task<IDataResult<List<ElasticSearchGetModel<Log>>>> GetSearchByField(
            ElasticSearchRequestDto elasticSearchRequestDto)
        {
            var list = await _elasticSearch.GetSearchByField<Log>(new SearchByFieldParameters
            {
                FieldName = "messageTemplate",
                Value = elasticSearchRequestDto.Value,
                IndexName = "logger-" + elasticSearchRequestDto.LogDate.ToString("yyyy.MM.dd")
            });
            return new SuccessDataResult<List<ElasticSearchGetModel<Log>>>(list, Messages.LogsListed);
        }

        /// <summary>
        ///     Elastic Search get models
        /// </summary>
        /// <param name="logDate"></param>
        /// <param name="from"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetElasticSearchLogs")]
        [CacheAspect]
        private async Task<List<ElasticSearchGetModel<Log>>> ElasticSearchGetModels(DateTime logDate, int from,
            int size)
        {
            var list = await _elasticSearch.GetAllSearch<Log>(new SearchParameters
            {
                IndexName = "logger-" + logDate.ToString("yyyy.MM.dd"),
                From = from,
                Size = size
            });
            return list;
        }
    }
}