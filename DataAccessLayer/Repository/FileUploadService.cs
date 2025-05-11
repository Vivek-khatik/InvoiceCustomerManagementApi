using Amazon.Runtime.Internal;
using DataAccessLayer.Model;
using DataAccessLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class FileUploadService : IFileUploadInterface
    {
        private string _fileDirectory;
        public FileUploadService(string fileDirectory)
        {
            _fileDirectory = fileDirectory;
        }
        public async Task<string> CreateFile(IFormFile file)
        {
            string fileExtension = System.IO.Path.GetExtension(file.FileName);
            var fileName = DateTime.Now.ToString("yyyyymmddhhmmss") + fileExtension;
            var filePath = Path.Combine(_fileDirectory, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return filePath.ToString();
        }
    }
}
