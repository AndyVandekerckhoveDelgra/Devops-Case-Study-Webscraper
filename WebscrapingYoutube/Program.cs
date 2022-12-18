using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using OpenQA.Selenium.Safari;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using static System.Formats.Asn1.AsnWriter;
using System.Net;
using System.Collections;
using Microsoft.VisualBasic;
using System.Threading.Channels;
using System.Xml.Linq;
using System.Reflection.Metadata;
using System.IO;
using Newtonsoft.Json;

namespace WebScrapingYoutube
{
    public class Program
    {
        private static IWebDriver driver;


        static void Main(string[] args)
        {
            ChromeNetworkConditions conditions = new ChromeNetworkConditions(); //controls internet speed
            conditions.DownloadThroughput = 999 * 1000; //adjusts the internet's speed; the lower the "45" to slower
            conditions.UploadThroughput = 10 * 1000; //controls internet speed
            conditions.Latency = TimeSpan.FromMilliseconds(1);//controls internet speed
            int counter1 = 0;
            int counter2 = 1;
            driver = new ChromeDriver();
            (driver as ChromeDriver).NetworkConditions = conditions; //controls internet speed
            // Type your search term and enter
            Console.WriteLine("Enter search term:"); //Vraagt voor zoekterm, user vult het zelf in

            string searchterm = Console.ReadLine(); //string voor de ingevulde zoekterm
            //string video = "never gonna give you up";

            driver.Navigate().GoToUrl(String.Format("https://www.youtube.com/results?search_query={0}", searchterm)); //app leidt naar de pagina waar de 


            var titles = FindElements(By.ClassName("yt-simple-endpoint style-scope ytd-video-renderer"));
            var channels = FindElements(By.ClassName("long-byline style-scope ytd-video-renderer"));
            var views = FindElements(By.ClassName("inline-metadata-item style-scope ytd-video-meta-block"));
            //var a = views.Zip(channels, (t, c) => new { view = t, channel = c });
            string textline = "";
            var collections = views.Zip(titles,  (t, c) => new { title = t, channel = c});
            String jsontext = "[\n";

            foreach (var tc in collections) {
                string link = tc.title.GetAttribute("href");
                if (counter1 == 0) {
                    Console.WriteLine(link);
                    textline += String.Format("{0};", link);
                    jsontext += "  {\n    \"Link\": \"" + link + "\",\n    \"Titel\": \"" + tc.title.Text + "\",\n    \"";
                }
                if (counter1 == 1)
                { jsontext += "Weergaven\": \"" + tc.title.Text + "\",\n    \""; }
                if (counter1 == 2)
                { jsontext += "Kanaal\": \"" + tc.title.Text + "\"\n  },\n"; }
                
                Console.WriteLine(tc.title.Text);
                counter1 += 1;
                textline += String.Format("{0};", tc.title.Text);
                using (StreamWriter writetext = new StreamWriter("write.csv"))
                {
                    writetext.Write("Link;Video;Weergaven;Kanaal\n");
                    writetext.Write(textline);
                }
                if (counter1 == 4)
                {
                    Console.WriteLine("--------------------------");
                    counter1 = 0;
                    counter2 += 1;
                    textline += "\n";

                }
                
            }

            jsontext = jsontext.Substring(0, jsontext.Length - 2);
            jsontext += "\n]";
            using (StreamWriter writetext = new StreamWriter("write.json"))
            {
                writetext.Write(jsontext);
            }
            Console.WriteLine("Done");
        }

        static IReadOnlyCollection<IWebElement> FindElements(By by)
        {
            int counter = 0;
            while (counter < 6)
            {
                var elements = driver.FindElements(By.CssSelector(".yt-simple-endpoint.style-scope.ytd-video-renderer, .long-byline.style-scope.ytd-video-renderer, .style-scope.ytd-video-meta-block .inline-metadata-item.style-scope.ytd-video-meta-block:nth-of-type(1)"));
                if (elements.Count > 0)
                    return elements;
                Thread.Sleep(10);
                counter += 1;
            }
            return new ReadOnlyCollection<IWebElement>(new List<IWebElement>()); //used if you want to control internet speed (is return null if you're not doing something with the speed of the internet)

        }
    }
}