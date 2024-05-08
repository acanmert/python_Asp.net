using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suggestions.Abstract
{
    public interface ISuggestionsService

    {
        List<string> GetFile();
        List<string> DataUpload(IFormFile formFile);
        Task<List<string>> Get_recommendations(string fileName, List<string> benzerlikName, string p_primaryKey, string getProductName, string p_type);
        List<string> Header(string fileName);
        FileUploadViewModel Suggestions(string fileName);
    }
}
