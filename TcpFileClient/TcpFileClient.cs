using System.Net.Sockets;


//Task #1
namespace TcpFileClient
{
    class TcpFileClient
    {
        const string ServerAddress = "127.0.0.1"; 
        const int Port = 12345;
        const int BufferSize = 1024; 

        static void Main()
        {
            try
            {
                using (var client = new TcpClient(ServerAddress, Port))
                using (var networkStream = client.GetStream())
                {
                   
                    for (int i = 1; i <= 4; i++)
                    {
                        string fileName = $"sample{i}.txt";
                        if (File.Exists(fileName))
                        {
                            byte[] fileData = File.ReadAllBytes(fileName);
                            networkStream.Write(fileData, 0, fileData.Length);
                            Console.WriteLine($"Отправил {fileName}");
                        }
                        else
                        {
                            Console.WriteLine($"Файл {fileName} не существует");
                        }
                    }

                    
                    var buffer = new byte[BufferSize];
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    string response = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Ответ сервера: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
