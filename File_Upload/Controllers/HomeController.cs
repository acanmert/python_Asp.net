using File_Upload.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Suggestions.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace File_Upload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly HttpClient _client = new HttpClient();
        private ISuggestionsService _suggestionsService;
        public HomeController(ILogger<HomeController> logger, ISuggestionsService suggestionsService)
        {
            _logger = logger;
            _suggestionsService = suggestionsService;
        }

        public IActionResult Index()
        {
            return View(_suggestionsService.GetFile());
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DataUpload()
        {

            return View(_suggestionsService.GetFile());
        }
        [HttpPost]
        public async Task<IActionResult> DataUpload(IFormFile formFile)
        {
            var dataUpload = _suggestionsService.DataUpload(formFile);
            return View(dataUpload);

        }



        public string Python(string fileName, List<string> benzerlikName, string productName, string getProductName, string p_type)
        {
            string output;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Python yorumlayıcısının yolu.
            start.Arguments = "oneri.py"; // Python scriptinizin yolu.
            start.UseShellExecute = false;
            start.RedirectStandardInput = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.StandardOutputEncoding = Encoding.UTF8;
            start.StandardErrorEncoding = Encoding.UTF8;
            using (Process process = Process.Start(start))
            {
                using (StreamWriter writer = process.StandardInput)
                {
                    string benzerlik = string.Join(",", benzerlikName);
                    writer.WriteLine(fileName.ToString()); // Örneğin, veri seti seçimi için.
                    writer.WriteLine(benzerlik.ToString());
                    writer.WriteLine(getProductName.ToString());
                    writer.WriteLine(productName.ToString());
                    writer.WriteLine(p_type.ToString());
                }

                output = process.StandardOutput.ReadToEnd();


            }
            return output;
        }


        public IActionResult Suggestions(string fileName)
        {
            FileUploadViewModel file = new FileUploadViewModel();

            file.FieldList = _suggestionsService.Header(fileName);
            file.FileNames = _suggestionsService.GetFile();
            file.ThisFileName = fileName;
            return View(file);
        }
        public async Task<IActionResult> ProcessSuggestions(string fileName, List<string> selectedFeatures, string p_pk, string p_name, string p_type)
        {
            // async Task<IActionResult>
            //IActionResult
            //var recommendations = Python(fileName, benzerlikName, productName, getProductName, p_type);
            //var recommendationList = recommendations.Split(new string[] { "dtype: object," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //return View(recommendationList);


            var recommendations = await _suggestionsService.Get_recommendations(fileName, selectedFeatures, p_pk, p_name, p_type);
            return View(recommendations);
        }

        public IActionResult Download(List<string> data)
        {
            var csvContent = string.Join(Environment.NewLine, data);

            var fileName = "suggestions.csv";
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
            Response.ContentType = "text/csv";


            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
