using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mymoja
{
    [Serializable]
    class WeatherServer
    {

        private string html;
        private string url;
        private Encoding encoding;
        private float temp;
        private string tempxpath;
        private float wind;
        private string windxpath; 
        private float precipitation;
        private string precipitationxpath;
        private string name;
        public int daytime;
        public DateTime date;
        public bool hasPoint;




        public WeatherServer(string _name,  string _url, Encoding _encoding, string _tempxpath, string _precipitationxpath, string _windxpath )
        {
            name = _name;
            url = _url;
            encoding = _encoding;
            tempxpath = _tempxpath;
            windxpath = _windxpath;
            precipitationxpath = _precipitationxpath;
            date = DateTime.Today;

            if (DateTime.Now.Hour < 8)
            {
                daytime = 1;
            } else if (DateTime.Now.Hour < 16)
            {
                daytime = 2;
            }  else
            {
                daytime = 3;
            }
            
            hasPoint = false;

        }


        public async void download()
        {
            html = "";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = encoding; //TODO right encoding?
                try{
                    html = await wc.DownloadStringTaskAsync(url);
                } catch (Exception ex)
                {

                }
                
            }
        }


        public void parse()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            if(name == "yr.no")
            {
                try
                {
                    temp = clearregex(doc.DocumentNode.SelectSingleNode(tempxpath).GetAttributeValue("value", "0"));
                    wind = clearregex(doc.DocumentNode.SelectSingleNode(windxpath).GetAttributeValue("value", "0"));
                    precipitation = clearregex(doc.DocumentNode.SelectSingleNode(precipitationxpath).GetAttributeValue("value", "0"));
                }
                catch (Exception ex)
                {

                }
            } else
            {
                try
                {
                    temp = clearregex(doc.DocumentNode.SelectSingleNode(tempxpath).InnerText);
                    wind = clearregex(doc.DocumentNode.SelectSingleNode(windxpath).InnerText);
                    precipitation = clearregex(doc.DocumentNode.SelectSingleNode(precipitationxpath).InnerText);
                }
                catch (Exception ex)
                {

                }
            }
            
            

        }

        private float clearregex(string innerText)
        {
            float numma = 0;
            Regex regex = new Regex(@"[-+]?(\d*[.])?\d+");
            Match match = regex.Match(innerText);
            if (match.Success)
            {
                while(match.NextMatch().Value != "")
                {
                    match = match.NextMatch();
                }

                ;
                numma = float.Parse(rightseparator(match.Value));
            }
            return numma;
        }

        private string rightseparator(string str)
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return str.Replace(".", separator).Replace(",", separator);
        }


        public string gethtml()
        {
            return html;
        }


        public string getname()
        {
            return name;
        }

        public float getwind()
        {
            return wind;
        }

        public float gettemp()
        {
            return temp;
        }

        public float getprecipitation()
        {
            return precipitation;
        }





    }
}
