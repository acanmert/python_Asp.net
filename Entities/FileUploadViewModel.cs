using System;
using System.Collections.Generic;

namespace Entities
{
    public class FileUploadViewModel
    {
        public List<string> FileNames { get; set; }
        public List<string> FieldList { get; set; }
        public string ThisFileName { get; set; }
        // Diğer gerekli özellikler buraya eklenebilir
    }
}
