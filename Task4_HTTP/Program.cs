using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RectangularPrismSurfaceAreaCalculator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8083/"); 
            listener.Start();
            Console.WriteLine("HTTP сервер запущен. Ожидание запросов...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

          
                if (request.HttpMethod == "POST")
                {
                    try
                    {
                        string body;
                        using (var reader = new System.IO.StreamReader(request.InputStream, Encoding.UTF8))
                        {
                            body = await reader.ReadToEndAsync();
                        }

                        string[] parameters = body.Split('&');
                        double length, width, height;

                        if (parameters.Length == 3 &&
                            TryParseParameter(parameters[0], out length) &&
                            TryParseParameter(parameters[1], out width) &&
                            TryParseParameter(parameters[2], out height))
                        {
                            double surfaceArea = 2 * (length * width + width * height + height * length);
                            string responseString = $@"
                                <html>
    <body>
        <h1>Surface Area of a Rectangular Prism</h1>
        <p>Length: {length}</p>
        <p>Width: {width}</p>
        <p>Height: {height}</p>
        <p><strong>Surface Area: {surfaceArea}</strong></p>
    </body>
</html>";

                            SendResponse(response, responseString);
                        }
                        else
                        {
                            string responseString = @"
                                <html>
    <body>
        <h1>Input Error</h1>
        <p>Please provide all three values (length, width, and height) in the correct format.</p>
    </body>
</html>";

                            SendResponse(response, responseString);
                        }
                    }
                    catch (Exception ex)
                    {
                        string responseString = $@"
                            <html>
    <body>
        <h1>Server Error</h1>
        <p>An error occurred while processing the request: {ex.Message}</p>
    </body>
</html>
";

                        SendResponse(response, responseString);
                    }
                }
                else
                {
                    string responseString = @"
                       <html>
    <body>
        <h1>Method Not Supported</h1>
        <p>This server only supports POST requests.</p>
    </body>
</html>
";

                    SendResponse(response, responseString);
                }

                response.Close();
            }
        }

        private static bool TryParseParameter(string parameter, out double result)
        {
            result = 0;
            string[] parts = parameter.Split('=');
            return parts.Length == 2 && double.TryParse(parts[1], out result);
        }

        private static void SendResponse(HttpListenerResponse response, string responseString)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
