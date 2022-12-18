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
            ChromeNetworkConditions conditions = new ChromeNetworkConditions();
            conditions.DownloadThroughput = 999 * 1000; 
            conditions.UploadThroughput = 10 * 1000; 
            conditions.Latency = TimeSpan.FromMilliseconds(1);

            int counter1 = 0;
            int counter2 = 1;
            driver = new ChromeDriver();
            (driver as ChromeDriver).NetworkConditions = conditions;
            driver.Navigate().GoToUrl(String.Format("https://www.ictjob.be/en/search-it-jobs?keywords=a"));
            Console.WriteLine("Enter search term:");

            string searchterm = Console.ReadLine();

            string cookie = String.Format("https://www.ictjob.be/en/search-it-jobs?keywords={0}", searchterm);
            driver.Navigate().GoToUrl(String.Format("https://www.ictjob.be/en/search-it-jobs?keywords={0}", searchterm));


            var titles = FindElements(By.ClassName("job-title search-item-link"));
            var companies = FindElements(By.ClassName("job-company"));
            var locations = FindElements(By.ClassName("job-location"));
            var keywords = FindElements(By.ClassName("job-keywords"));
            var collections1 = locations.Zip(companies, (l, c) => new { location = l, company = c });
            var collections2 = collections1.Zip(keywords, (c, k) => new { collection1 = c, keyword = k });
            string textline = "";
            var collections3 = collections2.Zip(titles, (c, t) => new { collection2 = c, title = t });
            String jsontext = "[\n";
            foreach (var ct in collections3)
            {
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
                    { jsontext += "Bedrijf\": \"" + ct.title.Text + "\",\n    \""; }
                    if (counter1 == 2)
                    { jsontext += "Locatie\": \"" + ct.title.Text + "\",\n    \""; }
                    if (counter1 == 3)
                    { jsontext += "Sleutelwoorden\": \"" + ct.title.Text + "\"\n  },\n"; }
                    Console.WriteLine(ct.title.Text);
                    counter1 += 1;
                    textline += String.Format("\"{0}\";", ct.title.Text);
                    using (StreamWriter writetext = new StreamWriter("write.csv"))
                    {
                        writetext.WriteLine("Link;Titel;Bedrijf;Locatie;Sleutelwoorden");
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
                var elements = driver.FindElements(By.CssSelector(".job-title.search-item-link, .job-company, .job-location, .job-keywords"));
                if (elements.Count > 0)
                    return elements;
                Thread.Sleep(10);
                counter += 1;
            }
            return new ReadOnlyCollection<IWebElement>(new List<IWebElement>()); 

        }
    }
}