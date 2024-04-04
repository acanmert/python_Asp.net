using File_Upload.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Upload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DataUpload()
        {

            return View(GetFile());
        }
        [HttpPost]
        public async Task<IActionResult> DataUpload(IFormFile formFile)
        {
            //...ekleme işlemleri   
            if (formFile != null && formFile.FileName.EndsWith(".csv"))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\dataset", formFile.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
            return View(GetFile());
        }
        public List<string> GetFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\dataset");
            var dataSet = Directory.GetFiles(filePath);

            List<string> dataFileNames = new List<string>();
            foreach (var data in dataSet)
            {
                    dataFileNames.Add(Path.GetFileName(data));
            }
            return dataFileNames;
        }
        public string Python(string fileName, List<string> benzerlikName, string productName, string getProductName, string p_type)
        {
            string output;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Python yorumlayıcısının yolu.
            start.Arguments = "deneme.py"; // Python scriptinizin yolu.
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

            file.FieldList=Header(fileName);
            file.FileNames = GetFile();
            file.ThisFileName = fileName;
            return View(file);
        }
        public IActionResult ProcessSuggestions(string fileName, List<string> benzerlikName, string productName, string getProductName, string p_type)
        {
            var recommendations = Python(fileName, benzerlikName, productName, getProductName, p_type);
            var recommendationList = recommendations.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return View(recommendationList);
        }

        public List<string> Header(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\dataset", fileName);
            using (var reader = new StreamReader(path))
            {
                // İlk satırı oku
                string ilkSatir = reader.ReadLine();
                List<string> liste = ilkSatir.Split(',').ToList();
                return liste;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
