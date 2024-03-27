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
        public IActionResult ImageUpload()
        {

            return View(GetFile());
        }
        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile formFile)
        {
            //...ekleme işlemleri   
            if (formFile != null)
            {

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", formFile.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }


            }
            return View(GetFile());
        }
        public List<string> GetFile()
        {
            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img");
            var imgFiles = Directory.GetFiles(imgPath);

            List<string> imgFileNames = new List<string>();
            foreach (var file in imgFiles)
            {
                imgFileNames.Add(Path.GetFileName(file));
            }
            return imgFileNames;
        }
        public string Python(string fileName ,string benzerlikName, string productName, string getProductName, string p_type)
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

                    writer.WriteLine(fileName.ToString()); // Örneğin, veri seti seçimi için.
                    writer.WriteLine(benzerlikName.ToString());
                    writer.WriteLine(getProductName.ToString());
                    writer.WriteLine(productName.ToString());
                    writer.WriteLine(p_type.ToString());


                }
                string errors = process.StandardError.ReadToEnd();
                // Python scriptinin çıktısını oku
                output = process.StandardOutput.ReadToEnd();


            }
            return output;
        }

        string file;
        public IActionResult Suggestions()
        {
            return View(GetFile());
        }
        public IActionResult ProcessSuggestions(string fileName, string benzerlikName,string productName,string getProductName,string p_type)
        {
            var recommendations = Python(fileName, benzerlikName,productName,getProductName,p_type);
            var recommendationList = recommendations.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return View(recommendationList);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
