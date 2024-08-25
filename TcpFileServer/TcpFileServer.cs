using System.Net.Sockets;
using System.Net;
using System.Text;



//Task #1
namespace TcpFileServer
{
    internal class TcpFileServer
    {
        const int Port = 12345;
        const int BufferSize = 1024;

        static void Main()
        {
          
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start(); 

            Console.WriteLine("Сервер запущен, ждет соединений...");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Клиент подключен.");

                
                using (var networkStream = client.GetStream())
                {
                    var buffer = new byte[BufferSize];
                    int bytesRead;
                    int fileCount = 0;

          
                    while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (fileCount >= 4) break; 

                        string fileName = $"sample{fileCount + 1}.txt";
                        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                        Console.WriteLine($"Получено и сохранено {fileName}");
                        fileCount++;
                    }

                    var ackMessage = System.Text.Encoding.ASCII.GetBytes("Файлы получены и успешно сохранены.");
                    networkStream.Write(ackMessage, 0, ackMessage.Length);
                }

                client.Close(); 
            }
        }
    }
}