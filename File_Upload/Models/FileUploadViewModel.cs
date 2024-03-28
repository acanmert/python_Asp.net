using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace File_Upload.Models
{
    public class FileUploadViewModel
    {
        public List<string> FileNames { get; set; }
        public string FieldList { get; set; }
        public string ThisFileName { get; set; }
        // Diğer gerekli özellikler buraya eklenebilir
    }
}
