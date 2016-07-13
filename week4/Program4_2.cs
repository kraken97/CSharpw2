using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace ConsoleApplication
{

    class PdfBot
    {

        bool firstReq=true;
        const int timeoutSeconds=5;

        public readonly HttpClient client = new HttpClient();

        public PdfBot(){
          
        }
        public void DownloadPageandSave(string url, string folder)
        {
            
            string file_name = Regex.Match(url, @"([^/]+pdf)").Groups[1].Value;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var pathToFile = folder + "/" + file_name;

            var fileAsStream = client.GetByteArrayAsync(url).Result;
            using (var Writer = File.Create(pathToFile))
            {
                Writer.WriteAsync(fileAsStream, 0, fileAsStream.Length);
            }

        }
        public string DownloadPageasString(string url)
        {
            var pageasBytes = client.GetByteArrayAsync(url).Result;
            return Encoding.ASCII.GetString(pageasBytes);
        }


        public static IEnumerable<string> ApplyRegex(string page_body, string pattern)
        {
            var matches = Regex.Matches(page_body, pattern);
            for (int i = 0; i < matches.Count; i++)
            {
                yield return matches[i].Groups[1].Value;
            }
        }
        public IEnumerable<string> FindOnPage(string url, string pattern)
        {
            if(!this.firstReq)Thread.Sleep(TimeSpan.FromSeconds(timeoutSeconds));
            else this.firstReq=false;
            return ApplyRegex(DownloadPageasString(url), pattern);
        }


        public void StartDownLoad(string outFolder)
        {
            string url = "http://nz.ukma.edu.ua/index.php?option=com_content&task=category&sectionid=10&id=60&Itemid=47";
            var links = FindOnPage(url, @"<a href=""([^""]+Itemid=47)"">");
            foreach (string link in links)
            {
                System.Console.WriteLine(link);
                var pdf_files = FindOnPage(link.Replace("amp;", ""), @"(http[^""]+pdf)");

                foreach (string pdf_file_link in pdf_files)
                {
                    System.Console.WriteLine(pdf_file_link);
                    DownloadPageandSave(pdf_file_link, outFolder);
                }
            }
        }
    }
    public class Program2
    {
        public static void Main(string[] args)
        {

            PdfBot bot = new PdfBot();
            using (bot.client)
            {
                bot.StartDownLoad( "pdfFiles");
            }

        }

    }

}
