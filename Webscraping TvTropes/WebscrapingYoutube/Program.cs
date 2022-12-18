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
            Console.WriteLine("Enter search term:");

            string searchterm = Console.ReadLine();
            //string video = "never gonna give you up";

            string cookie = String.Format("https://tvtropes.org/pmwiki/search_result.php?q={0}", searchterm);
            driver.Navigate().GoToUrl(String.Format("https://tvtropes.org/pmwiki/search_result.php?q={0}", searchterm));
            //var popupbutton = driver.FindElement(By.CssSelector(".yt-spec-button-shape-next.yt-spec-button-shape-next--filled.yt-spec-button-shape-next--call-to-action.yt-spec-button-shape-next--size-m"));
            //popupbutton.Click(); //to remove that annoying pop-up

            //var searchbar = driver.FindElement(By.CssSelector(".style-scope.ytd-searchbox"));
            //searchbar.Click(); //to click on the searchbar
            //searchbar.SendKeys("Never gonna give you up");

            //using (StreamWriter writetext = new StreamWriter("write.csv"))
            //{
            //    writetext.Write("Link;Titel;Categorie;Beschrijving\n");
            //}

            //titles collections
            //categories channels
            //descriptions views
            var titles = FindElements(By.ClassName("gs-title"));
            var categories = FindElements(By.ClassName("gs-bidi-start-align gs-visibleUrl gs-visibleUrl-breadcrumb"));
            var descriptions = FindElements(By.ClassName("gs-bidi-start-align gs-snippet"));
            //var a = views.Zip(channels, (n, w) => new { view = n, channel = w });
            string textline = "";
            var collections = descriptions.Zip(titles,  (c, t) => new { title = t, category = c});
            String jsontext = "[\n";
            foreach (var ct in collections) {
                string link = ct.title.GetAttribute("href");
                if (ct.title.Text != "")
                {
                    if (counter1 == 0)
                    {
                        Console.WriteLine(link);
                        textline += String.Format("{0};", link);
                        jsontext += "  {\n    \"Link\": \"" + link + "\",\n    \"Titel\": \"" + ct.title.Text + "\",\n    \"";
                    }
                    if (counter1 == 1)
                    { jsontext += "Categorie\": \"" + ct.title.Text + "\",\n    \""; }
                    if (counter1 == 2)
                    { jsontext += "Beschrijving\": \"" + ct.title.Text + "\"\n  },\n"; }
                    Console.WriteLine(ct.title.Text);
                    counter1 += 1;
                    textline += String.Format("{0};", ct.title.Text);
                    using (StreamWriter writetext = new StreamWriter("write.csv"))
                    {
                        writetext.Write("Link;Titel;Categorie;Beschrijving\n");
                        writetext.Write(textline);
                    }
                    if (counter1 == 3)
                    {
                        Console.WriteLine("--------------------------");
                        counter1 = 0;
                        counter2 += 1;
                        textline += "\n";

                    }
                }

            }



            //foreach (var collection in collections)
            //{
            //string str_blog_link = collection.GetAttribute("href");
            //Console.WriteLine(str_blog_link);
            //Console.WriteLine(collection.Text);
            //}
            jsontext = jsontext;
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
                var elements = driver.FindElements(By.CssSelector(".gs-title a, .gs-bidi-start-align.gs-visibleUrl.gs-visibleUrl-breadcrumb, .gs-bidi-start-align.gs-snippet"));
                //var elements = driver.FindElements(By.CssSelector(".yt-simple-endpoint.style-scope.ytd-video-renderer"));
                //var elements1 = driver.FindElements(By.CssSelector(".long-byline.style-scope.ytd-video-renderer"));
                //for views: .style-scope.ytd-video-meta-block .inline-metadata-item.style-scope.ytd-video-meta-block:nth-of-type(1)
                if (elements.Count > 0)
                    return elements;
                Thread.Sleep(10);
                counter += 1;
            }
            return new ReadOnlyCollection<IWebElement>(new List<IWebElement>()); //used if you want to control internet speed (is return null if you're not doing something with the speed of the internet)

        }
    }
}