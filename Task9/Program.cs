using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Task9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           
            string url = "https://github.com/mbaibatyr/SEP_221_NET/archive/refs/heads/master.zip";
            string zipFilePath = "master.zip";
            string extractPath = "SEP_221_NET-master";

            try
            {
              
                await DownloadFileAsync(url, zipFilePath);
                Console.WriteLine("ZIP файл скачан.");

                ExtractZipFile(zipFilePath, extractPath);
                Console.WriteLine("ZIP файл извлечен.");

                string gitignoreFilePath = Path.Combine(extractPath, Path.Combine(extractPath, ".gitignore"));

                if (File.Exists(gitignoreFilePath))
                {
                    string gitignoreContent = File.ReadAllText(gitignoreFilePath);
                    Console.WriteLine("Содержимое .gitignore:");
                    Console.WriteLine(gitignoreContent);
                }
                else
                {
                    Console.WriteLine(".gitignore не найден по пути: " + gitignoreFilePath);
                }

              
                DeleteFile(zipFilePath);
                DeleteDirectory(extractPath);
                Console.WriteLine("Файлы удалены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static async Task DownloadFileAsync(string url, string filePath)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                await using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }

        static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true); 
            }

            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
        }

        static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        static void DeleteDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true); 
            }
        }
    }
}
