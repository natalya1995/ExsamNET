using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Task7_phone
{
    internal class Program
    {
        static void Main(string[] args)
        {
    
            var url = "https://beeline.kz/ru";

            var web = new HtmlWeb();
            var doc = web.Load(url);

       
            var phoneNodes = doc.DocumentNode.SelectNodes("//footer//text()");

            if (phoneNodes != null)
            {
    
                var phonePattern = @"\+?\d[\d\s\(\)-]{4,}";

                foreach (var node in phoneNodes)
                {
                    var text = node.InnerText.Trim();

                    var matches = Regex.Matches(text, phonePattern);

                    foreach (Match match in matches)
                    {
                        Console.WriteLine(match.Value);
                    }
                }
            }
            else
            {
                Console.WriteLine("Телефоны не найдены.");
            }
        }
    }
}

