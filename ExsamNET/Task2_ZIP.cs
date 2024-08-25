using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ExsamNET
{
    internal class Task2_ZIP
    {
        static void Main(string[] args)
        {
        
            string[] filesToZip = { "sample1.txt", "sample2.txt", "sample3.txt", "sample4.txt" };
            string zipFilePath = "archive.zip";
            string ftpUrl = "ftp://ftp.dlptest.com/archive.zip";
            string localZipFilePath = "archive.zip";
            string ftpUsername = "dlpuser";
            string ftpPassword = "rNrKYTX9g7z3RgJRmI7c0w==";
            string extractPath = "ExtractedFiles";

            // Создание архива
            ZipManager.CreateZip(filesToZip, zipFilePath);
            // Загрузка архива на FTP
            FtpManager.UploadToFtp(zipFilePath, ftpUrl, ftpUsername, ftpPassword);
            // Скачивание архива с FTP
            FtpManager.DownloadFromFtp(ftpUrl, localZipFilePath, ftpUsername, ftpPassword);
            // Разархивирование архива
            ZipManager.ExtractZip(localZipFilePath, extractPath);
            Console.WriteLine("Задача выполнена успешно.");
        }
    }

    internal class ZipManager
    {
        public static void CreateZip(string[] files, string zipFilePath)
        {
            try
            {
                using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var file in files)
                    {
                        if (File.Exists(file))
                        {
                            archive.CreateEntryFromFile(file, Path.GetFileName(file));
                            Console.WriteLine($"Добавлен {file} в {zipFilePath}");
                        }
                        else
                        {
                            Console.WriteLine($"Файл {file} не существует.");
                        }
                    }
                }
                Console.WriteLine($"Архив {zipFilePath} успешно создан.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания архива: {ex.Message}");
            }
        }

        public static void ExtractZip(string zipFilePath, string extractPath)
        {
            try
            {
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                Console.WriteLine($"Файлы извлечены в {extractPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при извлечении архива: {ex.Message}");
            }
        }
    }

    internal class FtpManager
    {
        public static void UploadToFtp(string filePath, string ftpUrl, string username, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);

                byte[] fileContents = File.ReadAllBytes(filePath);
                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Статус загрузки: {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки файла:{ex.Message}");
            }
        }

        public static void DownloadFromFtp(string ftpUrl, string filePath, string username, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(username, password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    responseStream.CopyTo(fileStream);
                    Console.WriteLine($"Статус загрузки: {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки файла: {ex.Message}");
            }
        }
    }
}
