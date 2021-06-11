using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logger;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using Ionic.Zip;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Concrete
{
    /// <summary>
    ///     File Log Manager
    /// </summary>
    public class FileLogManager : IFileLogService
    {
        /// <summary>
        ///     Get log files by date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetFileLogs")]
        [CacheAspect]
        public async Task<FileContentResultModel> GetLogFilesByDateRange(DateTime startDate, DateTime endDate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var iterator = Utilities.GetDateRange(startDate, endDate).GetEnumerator();

            using var zip = new ZipFile {AlternateEncodingUsage = ZipOption.AsNecessary};

            var filePaths = Directory.GetFiles(GetFilePath()).ToList();
            zip.AddDirectoryByName("Files");

            while (iterator.MoveNext())
            {
                var item = iterator.Current.ToString("yyyy-MM-dd");
                var files = filePaths.FindAll(x => x.EndsWith(item + ".txt"));
                if (files.Count > 0)
                    files.ForEach(x => zip.AddFile($"{x}", "Files"));
            }

            var zipName = $"Zip_{DateTime.Now:yyyy-MMM-dd-HHmmss}.zip";
            await using var memoryStream = new MemoryStream();
            zip.Save(memoryStream);

            return new FileContentResultModel
            {
                FileContents = memoryStream.ToArray(),
                ContentType = "application/zip",
                FileDownloadName = zipName
            };
        }

        /// <summary>
        ///     Get logs by date
        /// </summary>
        /// <param name="logDate"></param>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetFileLogs")]
        [CacheAspect]
        public async Task<FileContentResultModel> GetLogsByDate(DateTime logDate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var fileName = logDate.ToString("yyyy-MM-dd");

            using var zip = new ZipFile {AlternateEncodingUsage = ZipOption.AsNecessary};
            zip.AddDirectoryByName("Files");
            zip.AddFile($"{GetFilePath()}/{fileName}.txt", "Files");
            var zipName = $"Zip_{DateTime.Now:yyyy-MMM-dd-HHmmss}.zip";
            await using var memoryStream = new MemoryStream();
            zip.Save(memoryStream);

            return new FileContentResultModel
            {
                FileContents = memoryStream.ToArray(),
                ContentType = "application/zip",
                FileDownloadName = zipName
            };
        }

        /// <summary>
        ///     Get all logs
        /// </summary>
        /// <returns></returns>
        [LogAspect(typeof(FileLogger), "GetFileLogs")]
        [CacheAspect]
        public IDataResult<List<FileModel>> GetAllLogs()
        {
            var filePaths = Directory.GetFiles(GetFilePath());
            var files = filePaths.Select(filePath => new FileModel
                {FileName = Path.GetFileName(filePath), FilePath = filePath}).ToList();

            return new SuccessDataResult<List<FileModel>>(files, Messages.LogsListed);
        }

        /// <summary>
        ///     Returns the file path where the logs
        /// </summary>
        /// <returns></returns>
        private static string GetFilePath()
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            return configuration?.GetSection("SeriLogConfigurations:FileLogConfiguration:FolderPath").Value;
        }

        /*[LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<List<FileModel>> GetLogsByDateList(DateTime startDate, DateTime endDate)
        {
            using var iterator = Utilities.GetDateRange(startDate, endDate).GetEnumerator();
            var filePaths = Directory.GetFiles("./logs/");
            var files = new List<FileModel>();
            foreach (var filePath in filePaths)
                while (iterator.MoveNext())
                {
                    var item = iterator.Current.ToString("yyyy.MM.dd").Replace(".", "");
                    if (Path.GetFileName(filePath).Equals(item))
                        files.Add(new FileModel
                        {
                            FileName = item,
                            FilePath = filePath
                        });
                }

            return new SuccessDataResult<List<FileModel>>(files, Messages.LogsListed);
        }*/
    }
}