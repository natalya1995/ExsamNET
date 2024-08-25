using MailKit;
using MailKit.Net.Imap;

namespace Task10
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string imapHost = "imap.mail.ru";
            int imapPort = 993;
            string imapUsername = "natalya.tyagay@mail.ru";
            string imapPassword = "Dr0p7CdrNTyd6euXHuwc";

            try
            {
                using (var client = new ImapClient())
                {
               
                    await client.ConnectAsync(imapHost, imapPort, true);
                    await client.AuthenticateAsync(imapUsername, imapPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);

                    var messageIds = inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);

                    var sortedMessages = messageIds.OrderBy(m => m.Date).Take(100);

                    Console.WriteLine("Темы первых 100 писем:");
                    foreach (var message in sortedMessages)
                    {
                        Console.WriteLine($"Письмо: {message.Envelope.Subject}");
                    }

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
