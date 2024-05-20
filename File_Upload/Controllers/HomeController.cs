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


            var recommendations = await _suggestionsService.Get_recommendations(fileName, selectedFeatures, p_pk, p_name, p_type);
            return View(recommendations);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
