using Renci.SshNet;
using System.Net;

namespace Task3_FTP_SFTP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string url = "https://beeline.kz/binaries/content/assets/public_offer/public_offer_ru.pdf";
            string localFilePath = "public_offer_ru.pdf";
            string ftpUrl = "ftp://ftp.dlptest.com/public_offer_ru.pdf";
            string sftpHost = "test.rebex.net";
            string sftpUsername = "demo";
            string sftpPassword = "password";
            string sftpPath = "/public_offer_ru.pdf";

            await DownloadFileAsync(url, localFilePath);
            Console.WriteLine($"Файл загружен и сохранен как {localFilePath}.");
            await UploadFileToFtpAsync(url, ftpUrl, "dlpuser", "rNrKYTX9g7z3RgJRmI7c0w==");
            Console.WriteLine("Файл загружен на FTP-сервер.");
            await UploadFileToSftpAsync(url, sftpHost, sftpUsername, sftpPassword, sftpPath);
            Console.WriteLine("Файл загружен на SFTP-сервер.");
        }

        private static async Task DownloadFileAsync(string url, string filePath)
        {
            using (var webClient = new WebClient())
            {
                await webClient.DownloadFileTaskAsync(new Uri(url), filePath);
            }
        }

        private static async Task UploadFileToFtpAsync(string fileUrl, string ftpUrl, string username, string password)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Credentials = new NetworkCredential(username, password);

                    using (var stream = await webClient.OpenReadTaskAsync(fileUrl))
                    {
                        webClient.UploadData(ftpUrl, "STOR", ReadStreamToByteArray(stream));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки файла на FTP: {ex.Message}");
            }
        }

        private static async Task UploadFileToSftpAsync(string fileUrl, string host, string username, string password, string sftpPath)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    using (var stream = await webClient.OpenReadTaskAsync(fileUrl))
                    {
                        using (var sftpClient = new SftpClient(host, username, password))
                        {
                            sftpClient.Connect();
                            sftpClient.UploadFile(stream, sftpPath);
                            sftpClient.Disconnect();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки файла на SFTP: {ex.Message}");
            }
        }

        private static byte[] ReadStreamToByteArray(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
