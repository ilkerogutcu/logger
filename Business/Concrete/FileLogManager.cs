using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Ionic.Zip;

namespace Business.Concrete
{
    public class FileLogManager : IFileLogService
    {
        [LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        public async Task<FileContentResultModel> GetLogFilesByDateRange(DateTime startDate, DateTime endDate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var iterator = Utilities.GetDateRange(startDate, endDate).GetEnumerator();

            using var zip = new ZipFile {AlternateEncodingUsage = ZipOption.AsNecessary};
            var filePaths = Directory.GetFiles("./logs/").ToList();
            zip.AddDirectoryByName("Files");

            while (iterator.MoveNext())
            {
                var item = iterator.Current.ToString("yyyy.MM.dd").Replace(".", "");
                if (filePaths.Any(x => x.Substring(7, 8).Equals(item)))
                {
                    zip.AddFile($"./logs/{item}.txt", "Files");
                }
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

        [LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        public async Task<FileContentResultModel> GetLogsByDate(DateTime logDate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var fileName = logDate.ToString("yyyy.MM.dd").Replace(".", "");

            using var zip = new ZipFile {AlternateEncodingUsage = ZipOption.AsNecessary};
            zip.AddDirectoryByName("Files");
            zip.AddFile($"./logs/{fileName}.txt", "Files");
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

        [LogAspect(typeof(FileLogger))]
        [LogAspect(typeof(ElasticsearchLogger))]
        [LogAspect(typeof(MongoDbLogger))]
        public IDataResult<List<FileModel>> GetAllLogs()
        {
            var filePaths = Directory.GetFiles("./logs/");
            var files = new List<FileModel>();
            foreach (var filePath in filePaths)
                files.Add(new FileModel
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath
                });

            return new SuccessDataResult<List<FileModel>>(files, Messages.LogsListed);
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