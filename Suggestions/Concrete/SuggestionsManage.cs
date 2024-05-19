using Entities;
using Microsoft.AspNetCore.Http;
using Suggestions.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Suggestions.Concrete
{
    public class SuggestionsManage : ISuggestionsService

    {
        private static readonly HttpClient _client = new HttpClient();

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

        public List<string> DataUpload(IFormFile formFile)
        {
            //...ekleme işlemleri   
            if (formFile != null && formFile.FileName.EndsWith(".csv"))
            {

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\dataset", formFile.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

            }
            var files = GetFile();
            return files;

        }
        public async Task<List<string>> Get_recommendations(string fileName, List<string> benzerlikName, string p_primaryKey, string getProductName, string p_type)
        {
            string benzerlik = string.Join(",", benzerlikName);
            string secim = fileName;
            string selectedFeatures = benzerlik;
            string p_name = getProductName;
            string p_pk = p_primaryKey;
            string pType = p_type;

            //string secim = "book_data.csv";
            //string[] selectedFeatures = { "Name", "Genre" };
            //string title = "ciglik";
            //string pName = "Name";
            //string pType = "hayir"; string secim = "book_data.csv";


            // Flask API'sine GET isteği gönder
            string apiUrl = $"http://127.0.0.1:5000/recommendations?secim={secim}&selected_features={string.Join(",", selectedFeatures)}&p_name={p_name}&p_pk={p_pk}&p_type={pType}";
            HttpResponseMessage response = await _client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // API'den dönen JSON verisini oku
                string jsonResponse = await response.Content.ReadAsStringAsync();

                //var recommendations = Newtonsoft.Json.JsonConvert.DeserializeObject<string[] >(jsonResponse);
                //List<dynamic> resultList = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);

                //var resultList1 = JsonConvert.DeserializeObject<List<string>[]>(jsonResponse);

                jsonResponse = jsonResponse.TrimStart('[').TrimEnd(']');
                string cleanedData = jsonResponse.Replace("\n", "").Trim();
                var recommendationList = cleanedData.Split(new string[] { "  }," }, StringSplitOptions.RemoveEmptyEntries).ToList(); //dtype: object,

                var jsonList = JsonSerializer.Deserialize<List<string>>(jsonResponse);

                ConvertToCsv(jsonList);

                return recommendationList;
            }
            else
            {
                List<string> error_List = new List<string>();

                error_List.Add("-1");

                return error_List;
            }

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
        public FileUploadViewModel Suggestions(string fileName)
        {
            FileUploadViewModel file = new FileUploadViewModel();

            file.FieldList = Header(fileName);
            file.FileNames = GetFile();
            file.ThisFileName = fileName;
            return file;
        }
        public async static Task ConvertToCsv<T>(IEnumerable<T> items)
        {
            var csv = new StringBuilder();
            var properties = typeof(T).GetProperties();

            // Header row
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            string filePath = "suggestions.csv";
            await File.WriteAllTextAsync(filePath, csv.ToString());
            // Data rows
            foreach (var item in items)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty);
                csv.AppendLine(string.Join(",", values));
            }

        }

    }
}
