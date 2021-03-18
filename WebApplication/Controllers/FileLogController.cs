using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Aspects.Autofac;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileLogController : ControllerBase
    {
        private readonly IFileLogService _fileLogService;

        public FileLogController(IFileLogService fileLogService)
        {
            _fileLogService = fileLogService;
        }

        [HttpGet]
        [Route("GetLogsByDate")]
        public async Task<FileContentResult> GetLogsByDate(DateTime logDate)
        {
            var file = _fileLogService.GetLogsByDate(logDate).Result;
            return File(file.FileContents, file.ContentType, file.FileDownloadName);
        }

        [HttpGet]
        [Route("GetLogsByDateRange")]
        public async Task<FileContentResult> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            var file = _fileLogService.GetLogFilesByDateRange(startDate, endDate).Result;
            return File(file.FileContents, file.ContentType, file.FileDownloadName);
        }

        [HttpGet]
        [Route("GetAllLogs")]
        public IDataResult<List<FileModel>> GetAllLogs()
        {
            return _fileLogService.GetAllLogs();
        }
    }
}