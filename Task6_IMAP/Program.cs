using System;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace Task6_IMAP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var imapHost = "imap.mail.ru";
            var imapPort = 993; 
            var email = "natalya.tyagay@mail.ru";
            var password = "Dr0p7CdrNTyd6euXHuwc"; 

       
            var targetFolderName = "ProcessedMessages"; 

            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync(imapHost, imapPort, true);

                    
                    await client.AuthenticateAsync(email, password);

                    
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite);

                    
                    var subjectToFind = "sample"; 
                    var query = SearchQuery.SubjectContains(subjectToFind);
                    var uids = await inbox.SearchAsync(query);

                    if (uids.Count > 0)
                    {
                        var uid = uids[0];
                        var message = await inbox.GetMessageAsync(uid);

                        Console.WriteLine($"Нашел письмо с темой: {message.Subject}");

               
                        await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);

                        var rootFolder = client.GetFolder("INBOX"); 

                     
                        IMailFolder targetFolder = null;
                        foreach (var folder in rootFolder.GetSubfolders())
                        {
                            if (folder.Name.Equals(targetFolderName, StringComparison.OrdinalIgnoreCase))
                            {
                                targetFolder = folder;
                                break;
                            }
                        }

                        if (targetFolder == null)
                        {
                           
                            targetFolder = await rootFolder.CreateAsync(targetFolderName, true);
                        }

                        await inbox.MoveToAsync(uid, targetFolder);
                        Console.WriteLine($"Email перемещен: {targetFolderName}");
                    }
                    else
                    {
                        Console.WriteLine("Письма с указанной темой не найдено.");
                    }

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error : {ex.Message}");
                }
            }
        }
    }
}
