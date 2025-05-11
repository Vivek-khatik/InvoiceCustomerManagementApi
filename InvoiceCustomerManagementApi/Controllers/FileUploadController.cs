using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccessLayer.Model;
using DataAccessLayer.Service;
using DinkToPdf;
using InvoiceCustomerManagementApi.CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Net;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvoiceCustomerManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private IFileUploadInterface _fileUploadInterface;
        public FileUploadController(IFileUploadInterface fileUploadInterface)
        {
            _fileUploadInterface = fileUploadInterface;
        }
        // POST api/<FileUploadController>
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            var objCommonJson = new CommonJsonResponse();
            var file = Request.Form.Files[0];
            if (file == null || file.Length == 0)
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not selected";
                return BadRequest(objCommonJson);
            }

            var filePath = _fileUploadInterface.CreateFile(file);

            if (System.IO.File.Exists(filePath.Result))
            {
                objCommonJson.responseStatus = 1;
                objCommonJson.message = "File uploaded successfully";
                return Ok(objCommonJson);
            }
            else
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not uploaded";
                return Ok(objCommonJson);
            }
        }

        [HttpPost]
        [Route("uploadData")]
        public async Task<IActionResult> UploadData([FromForm] FileUploadDto model)
        {
            var objCommonJson = new CommonJsonResponse();
            var file = model.File;
            if (file == null || file.Length == 0)
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not selected";
                return BadRequest(objCommonJson);
            }

            var filePath = _fileUploadInterface.CreateFile(model.File);
            var additionalData = model.AdditionalData;

            if (System.IO.File.Exists(filePath.Result))
            {
                objCommonJson.responseStatus = 1;
                objCommonJson.message = "File uploaded successfully";
                return Ok(objCommonJson);
            }
            else
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not uploaded";
                return Ok(objCommonJson);
            }

        }

        [HttpPost]
        [Route("pdfUpload")]
        public async Task<IActionResult> PdfUpload()
        {
            var objCommonJson = new CommonJsonResponse();
            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings() { Top = 10 },
                //Out = @"Pdf\test.pdf",
            },
                Objects = {
                new ObjectSettings()
                {
                    PagesCount=true,
                    HtmlContent = "<html><body><h1>Hello there, i am using DinkToPdf</h1></body></html>",
                    WebSettings = {DefaultEncoding = "utf-8"},
                    HeaderSettings = {FontSize = 9,Right="Page[page]of[toPage]",Line=true,Spacing=2.812}
                    //Page = "https://om.satvasolutions.com/",
                },
            }
            };
            byte[] pdfFile = converter.Convert(doc);
            return File(pdfFile, "application/octet-stream", "abc.pdf");
        }

        //[HttpPost]
        //[Route("pdfFileRotativa")]
        //public IActionResult DemoPageMarginsPDF()
        //{
        //    var report = new ViewAsPdf(@"Pdf\DemoPageMarginsPDF")
        //    {
        //        PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
        //    };
        //    return report;
        //}

        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsLiving { get; set; }
        }

        [HttpGet]
        [Route("exportToCsv")]
        public void GetStream()
        {
            var myPersonObjects = new List<Person>()
            {
                new Person { Id = 1, IsLiving = true, Name = "Vivek" },
                new Person { Id = 2, IsLiving = true, Name = "Dipal" },
                new Person { Id = 3, IsLiving = true, Name = "Unnati" }
            };

            using (var writer = new StreamWriter(@"Csv\filePersons.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(myPersonObjects);
            }
        }

    }
}
