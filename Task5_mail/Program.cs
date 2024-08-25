using System.IO.Compression;
using System.Net.Mail;
using System.Net;

namespace Task5_mail
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string[] files = { "sample1.txt", "sample2.txt", "sample3.txt", "sample4.txt" };
            string zipFilePath = "archive.zip";

            try
            {
                CreateZipArchive(files, zipFilePath);
                Console.WriteLine($"Archive {zipFilePath} created successfully.");

                await SendEmailWithAttachmentAsync("natalya.tyagay@mail.ru", "sample", "body sample", zipFilePath);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void CreateZipArchive(string[] files, string zipFilePath)
        {
            using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                    else
                    {
                        throw new FileNotFoundException($"File {file} does not exist.");
                    }
                }
            }
        }

        static async Task SendEmailWithAttachmentAsync(string toAddress, string subject, string body, string attachmentPath)
        {
            var fromAddress = "natalya.tyagay@mail.ru";
            var smtpHost = "smtp.mail.ru";
            var smtpPort = 465; 
            var smtpUsername = "natalya.tyagay@mail.ru";
            var smtpPassword = "Dr0p7CdrNTyd6euXHuwc"; 
            using (var message = new MailMessage())
            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                message.From = new MailAddress(fromAddress);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false; 

                if (File.Exists(attachmentPath))
                {
                    message.Attachments.Add(new Attachment(attachmentPath));
                }
                else
                {
                    throw new FileNotFoundException($"Attachment file {attachmentPath} does not exist.");
                }

                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                await client.SendMailAsync(message);
            }
        }
    }
}
