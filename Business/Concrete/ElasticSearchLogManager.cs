﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.ElasticSearch;
using Core.Utilities.ElasticSearch.Models;
using Core.Utilities.Results;

namespace Business.Concrete
{
    public class ElasticSearchLogManager : IElasticSearchLogService
    {
        private readonly IElasticSearch _elasticSearch;

        public ElasticSearchLogManager(IElasticSearch elasticSearch)
        {
            _elasticSearch = elasticSearch;
        }
        
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<IEnumerable<ElasticSearchGetModel<Log>>>> GetLogsByDate(DateTime logDate,
            int from = 0, int size = 10)
        {
            var list = await ElasticSearchGetModels(logDate, from, size);
            return new SuccessDataResult<List<ElasticSearchGetModel<Log>>>(list);
        }
        
        [LogAspect(typeof(FileLogger))]
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
            return new SuccessDataResult<List<List<ElasticSearchGetModel<Log>>>>(logList);
        }
        
        [LogAspect(typeof(FileLogger))]
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