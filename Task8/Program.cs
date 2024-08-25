using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Task8
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Параметры подключения SFTP
            string sftpHost = "test.rebex.net";
            string sftpUsername = "demo";
            string sftpPassword = "password";
            string remoteFilePath = "pub/example/WinFormClientSmall.png"; 

            // Параметры для SMTP
            string fromAddress = "natalya.tyagay@mail.ru";
            string toAddress = "natalya.tyagay@mail.ru";
            string smtpHost = "smtp.mail.ru";
            int smtpPort = 465;
            string smtpUsername = "natalya.tyagay@mail.ru";
            string smtpPassword = "Dr0p7CdrNTyd6euXHuwc";

            // Параметры для FTP
            string ftpUrl = "ftp://ftp.dlptest.com/uploadedfile.zip";
            string ftpUsername = "dlpuser";
            string ftpPassword = "rNrKYTX9g7z3RgJRmI7c0w==";

            try
            {
                
                using (var sftpClient = new SftpClient(sftpHost, sftpUsername, sftpPassword))
                {
                    sftpClient.Connect();
                    Console.WriteLine("Подключено к SFTP-серверу.");

                    using (var fileStream = new MemoryStream())
                    {
                        sftpClient.DownloadFile(remoteFilePath, fileStream);
                        Console.WriteLine("Файл загружен с SFTP-сервера.");

                        fileStream.Position = 0;

                    
                        using (var zipStream = new MemoryStream())
                        {
                            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                            {
                                var archiveEntry = archive.CreateEntry(Path.GetFileName(remoteFilePath));
                                using (var entryStream = archiveEntry.Open())
                                {
                                    fileStream.CopyTo(entryStream);
                                }
                            }

                            zipStream.Position = 0;
                            Console.WriteLine("Архив создан.");

                    
                            await SendEmailWithAttachmentAsync(fromAddress, toAddress, "Файловый архив", "Прилагается файловый архив.", zipStream, "file.zip", smtpHost, smtpPort, smtpUsername, smtpPassword);
                            Console.WriteLine("Письмо успешно отправлено.");

                            await UploadFileToFtpAsync(ftpUrl, zipStream, ftpUsername, ftpPassword);
                            Console.WriteLine("Файл загружен на FTP-сервер.");
                        }
                    }
                    sftpClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task SendEmailWithAttachmentAsync(string fromAddress, string toAddress, string subject, string body, Stream attachmentStream, string attachmentName, string smtpHost, int smtpPort, string smtpUsername, string smtpPassword)
        {
            try
            {
                using (var message = new MailMessage(fromAddress, toAddress))
                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    message.Subject = subject;
                    message.Body = body;

                    
                    attachmentStream.Position = 0;
                    message.Attachments.Add(new Attachment(attachmentStream, attachmentName));

                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true; 

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось отправить электронное письмо: {ex.Message}");
            }
        }

        static async Task UploadFileToFtpAsync(string ftpUrl, Stream fileStream, string username, string password)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    fileStream.Position = 0;
                    await fileStream.CopyToAsync(requestStream);
                }

                using (var response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    Console.WriteLine($"Статус загрузки:{response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки на FTP: {ex.Message}");
            }
        }
    }
}
