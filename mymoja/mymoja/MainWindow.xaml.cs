using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mymoja
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<WeatherServer> wservers = new List<WeatherServer>();
        WeatherServer w1,w2;

        public MainWindow()
        {
            InitializeComponent();

            lb_state.Content = "Načítání ze souboru..";
            load();
            lb_state.Content = "Načteno";
            
            Timer tm_downloadNew = new Timer(5000);
            tm_downloadNew.AutoReset = true;
            tm_downloadNew.Start();
            tm_downloadNew.Elapsed += downloadCheckTimer;
            checkForDownload();
        }

        private void downloadCheckTimer(object sender, ElapsedEventArgs e)
        {
            checkForDownload();
        }

        private void checkForDownload()
        {

            DateTime dateNow = DateTime.Today;
            int daytime = 0;
            if (DateTime.Now.Hour < 8)
            {
                daytime = 1;
            }
            else if (DateTime.Now.Hour < 16)
            {
                daytime = 2;
            }
            else
            {
                daytime = 3;
            }

            bool alreadyDownloaded = false;
            foreach (WeatherServer ws in wservers)
            {
                if (ws.date.Year == dateNow.Year && ws.date.Month == dateNow.Month && ws.date.Day == dateNow.Day && daytime == ws.daytime)
                {
                    alreadyDownloaded = true;

                }

            }


            if (!alreadyDownloaded)
            {
                // http://pr-asv.chmi.cz/synopy-map/pocasina.php?indstanice=11603
                w1 = new WeatherServer("chmi.cz",
                    "http://pr-asv.chmi.cz/synopy-map/pocasina.php?indstanice=11406",
                    Encoding.Default,
                    "/html/body/center[3]/table/tr[9]/td[2]",
                    "/html/body/center[3]/table/tr[14]/td[1]",
                    "/html/body/center[3]/table/tr[7]/td[2]");
                wservers.Add(w1);
            
                /*http://www.yr.no/place/Czech_Republic/Liberec/Liberec/
                 * "//*[@id=\"ctl00_ctl00_contentBody\"]/div[2]/div[2]/table[1]/tbody/tr[1]/td[3]",
                    "//*[@id=\"ctl00_ctl00_contentBody\"]/div[2]/div[2]/table[1]/tbody/tr[1]/td[4]",
                    "//*[@id=\"ctl00_ctl00_contentBody\"]/div[2]/div[2]/table[1]/tbody/tr[1]/td[5]");
                 * */
                w2 = new WeatherServer("yr.no",
                    "http://www.yr.no/place/Czech_Republic/Liberec/Liberec/forecast.xml",
                    Encoding.Default,
                    "/weatherdata/forecast/tabular/time[1]/temperature",
                    "/weatherdata/forecast/tabular/time[1]/precipitation",
                    "/weatherdata/forecast/tabular/time[1]/pressure");
                wservers.Add(w2);


                download();
            }

        }

        private void bt_download_Click(object sender, RoutedEventArgs e)
        {
            download();            
        }

        public void download()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                lb_state.Content = "Stahování...";

                Timer dwntimer = new Timer(1000);
                dwntimer.AutoReset = true;
                dwntimer.Enabled = true;
                dwntimer.Elapsed += dwntimer_Elapsed;

                foreach (WeatherServer ws in wservers)
                {
                    ws.download();
                }
            }));
        }

        private void dwntimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (w1.gethtml() != "" && w2.gethtml() != "")
            {
                Timer tmr = (Timer)sender;
                tmr.Stop();
                tmr.Close();
                parse();
            }


        }

        public void parse()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                lb_state.Content = "Parsování...";
                foreach (WeatherServer ws in wservers)
                {
                    ws.parse();
                }
                show();
            }));
        }

        public void show()
        {

            lb_state.Content = "Zobrazování...";

            lb_namel.Content = w1.getname();
            //tb_htmll.Text = w1.gethtml();
            lb_temperaturel.Content = w1.gettemp() + "° C";
            lb_precipitationl.Content = w1.getprecipitation() + " mm";
            lb_windl.Content = w1.getwind() + " hPa";

            lb_namer.Content = w2.getname();
            //tb_htmlr.Text = w2.gethtml();
            lb_temperaturer.Content = w2.gettemp() + "° C";
            lb_precipitationr.Content = w2.getprecipitation() + " mm";
            lb_windr.Content = w2.getwind() + " hPa";

            lb_temperaturec.Content = Math.Round(Math.Abs(w1.gettemp() - w2.gettemp()),1) + "° C";
            lb_precipitationc.Content = Math.Round(Math.Abs(w1.getprecipitation() - w2.getprecipitation()),1) + " mm";
            lb_windc.Content = Math.Round(Math.Abs(w1.getwind() - w2.getwind()),1) + " hPa";


            if(!w1.hasPoint && !w2.hasPoint)
            {
                bt_vote1.IsEnabled = true;
                bt_vote2.IsEnabled = true;
            } else
            {
                bt_vote1.IsEnabled = false;
                bt_vote2.IsEnabled = false;

            }

            int points1 = 0;
            int points2 = 0;
            foreach(WeatherServer ws in wservers)
            {
                if(ws.getname() == "yr.no" && ws.hasPoint)
                {
                    points2++;
                }

                if (ws.getname() != "yr.no" && ws.hasPoint)
                {
                    points1++;
                }

            }

            if(points1 > points2)
            {
                lb_namel.Foreground = Brushes.Green;
                lb_namer.Foreground = Brushes.Black;
            } else if (points1 < points2)
            {
                lb_namer.Foreground = Brushes.Green;
                lb_namel.Foreground = Brushes.Black;
            } else
            {
                lb_namer.Foreground = Brushes.Black;
                lb_namel.Foreground = Brushes.Black;
            }

            lb_points1.Content = points1.ToString();
            lb_points2.Content = points2.ToString();

            lb_state.Content = "Zobrazeno";
        }

        private void btn_parse_Click(object sender, RoutedEventArgs e)
        {
            parse();
        }

        private void bt_vote1_Click(object sender, RoutedEventArgs e)
        {
            w1.hasPoint = true;
            show();
        }

        private void bt_vote2_Click(object sender, RoutedEventArgs e)
        {
            w2.hasPoint = true;
            show();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private void load()
        {
            string dir = @"./";
            string serializationFile = System.IO.Path.Combine(dir, "serverdata.bin");
            try
            {
                using (Stream stream = File.Open(serializationFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    wservers = (List<WeatherServer>)bformatter.Deserialize(stream);

                    if(wservers != null && wservers.Count > 1)
                    {
                        w1 = wservers[wservers.Count - 2];
                        w2 = wservers[wservers.Count - 1];
                        show();
                    }

                }
            }
            catch (Exception ex)
            {

            }
            
        }

        private void save()
        {
            string dir = @"./";
            string serializationFile = System.IO.Path.Combine(dir, "serverdata.bin");
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, wservers);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            save();
        }

        private void bt_showStats(object sender, RoutedEventArgs e)
        {
            History hs = new History(wservers);
            hs.ShowDialog();
        }

        private void btn_show_Click(object sender, RoutedEventArgs e)
        {
            show();
        }
    }
}
