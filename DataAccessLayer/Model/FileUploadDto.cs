using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string AdditionalData { get; set; }
    }
}
