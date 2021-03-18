using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities;
using Ionic.Zip;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("GetEmployees")]
        public IDataResult<List<Employee>> Get()
        {
            return _employeeService.GetAll();
        }
        
        [HttpGet]
        [Route("Files")]
        public ActionResult Gets()
        {
            string[] filePaths = Directory.GetFiles("./logs/");
            var files = new List<FileModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new FileModel()
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath
                });
            }

            return null;
        }
        
        [HttpPost]
        [Route("Download")]
        public ActionResult Index(DateTime startDate,DateTime endDate)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var iterator = Utilities.GetDateRange(startDate, endDate).GetEnumerator();
            using var zip = new ZipFile {AlternateEncodingUsage = ZipOption.AsNecessary};
            zip.AddDirectoryByName("Files");

            while (iterator.MoveNext())
            {
                var logDate = iterator.Current.ToString("yyyy.MM.dd").Replace(".","");
                zip.AddFile($"./logs/{logDate}.txt", "Files");
            }
          
            var zipName = $"Zip_{DateTime.Now:yyyy-MMM-dd-HHmmss}.zip";
            using var memoryStream = new MemoryStream();
            zip.Save(memoryStream);
         
            return File(memoryStream.ToArray(), "application/zip", zipName);
        }
    }
}