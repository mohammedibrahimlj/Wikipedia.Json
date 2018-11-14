using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using System.Web.Script.Serialization;

namespace WikiData
{
    public class WikiDataLib
    {
        private string HomeURL = "https://en.wikipedia.org/wiki/";
        private string DownlaodData = string.Empty;
        private Dictionary<string, object> Infoboxval;
        public string GetData(string query)
        {
            string GetDdata;
            string json=string.Empty;
            try
            {
                GetDdata = DownloadData(query);
                ScrapWIKIData(GetDdata);
                json = new JavaScriptSerializer().Serialize(Infoboxval.ToDictionary(item => item.Key.ToString(), item => item.Value.ToString()));
                
            }
            catch (Exception ex)
            {
                return "{'response':'Error while processing the data'}";
            }
            return json;
        }
        private string DownloadData(string Query)
        {

            try
            {
                using (WebClient wb = new WebClient())
                {
                    DownlaodData = wb.DownloadString(HomeURL + Query);
                }
            }
            catch (Exception ex)
            {
                DownlaodData = "";
            }
            return DownlaodData;
        }
        private void ScrapWIKIData(string Pagestring)
        {
            string value1;
            string value2;
            Infoboxval = new Dictionary<string, object>();
            try
            {
                HtmlDocument hd1 = new HtmlDocument();
                hd1.LoadHtml(Pagestring);
                var ScrapData = hd1.DocumentNode.SelectNodes("//table")[0];//infobox vcard
                var countNode = ScrapData.SelectNodes(".//th").Count;
                bool DownloadImage = true;
               
                for (int i = 0; i < countNode; i++)
                {
                    if (DownloadImage)
                    {
                        string imagepath = ScrapData.SelectNodes(".//td")[i].SelectSingleNode(".//img").Attributes[1].Value;

                        value1 = "Image";
                        value2 = "https:" + imagepath;
                        Infoboxval.Add(value1, value2);
                        DownloadImage = false;
                    }

                    value1 = ScrapData.SelectNodes(".//th")[i].InnerText.ToString().Replace("&#160;", " ");
                    if (value1.Contains("Trade"))
                    {
                        value2 = "[" + ScrapData.SelectNodes(".//td")[i + 1].InnerText.ToString().Replace("&amp;", " , ").Replace("&#160;", " ").Replace("&#59;", " ").Replace("&#93;", "]").Replace("&#91;", " [").Replace("&#32;", "").Replace("\n", " ") + "]";
                    }
                    else if (value1.Contains("Industry"))
                    {
                        value2 = "[" + ScrapData.SelectNodes(".//td")[i + 1].InnerText.ToString().Substring(1, ScrapData.SelectNodes(".//td")[i + 1].InnerText.ToString().Length - 1).Substring(0, ScrapData.SelectNodes(".//td")[i + 1].InnerText.ToString().Length - 2).Replace("\n", ",") + "]";
                    }
                    else
                    {
                        value2 =  ScrapData.SelectNodes(".//td")[i + 1].InnerText.ToString().Replace("&amp;", " | ").Replace("&#160;", " ").Replace("&#59;", " ").Replace("&#93;", "]").Replace("&#91;", " [").Replace("&#32;", "");
                    }
                    Infoboxval.Add(value1.Replace(" ",""), value2);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
