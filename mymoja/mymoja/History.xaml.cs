using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mymoja
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        List<WeatherServer> wservers;
        public History(object _wservers)
        {
            InitializeComponent();
            wservers = (List<WeatherServer>) _wservers;

            dtp1.SelectedDate = DateTime.Today;
            dtp2.SelectedDate = DateTime.Today;
            count();
        }

        private void dtp1_CalendarClosed(object sender, RoutedEventArgs e)
        {
            count();
        }

        private void count()
        {
            List<float> temps = new List<float>();
            List<float> preassures = new List<float>();
            List<float> waters = new List<float>();
            foreach (WeatherServer ws in wservers)
            {
                if(ws.date >= dtp1.SelectedDate && ws.date <= dtp2.SelectedDate)
                {
                    temps.Add(ws.gettemp());
                    preassures.Add(ws.getwind());
                    waters.Add(ws.getprecipitation());
                }
            }


            if (temps.Count != 0)
            {
                temps.Sort();
                lb_medtem.Content = temps[int.Parse(Math.Round((double)(temps.Count / 2), 0).ToString())].ToString();


                preassures.Sort();
                lb_medpress.Content = preassures[int.Parse(Math.Round((double)(preassures.Count / 2), 0).ToString())].ToString();


                waters.Sort();
                lb_medwater.Content = waters[int.Parse(Math.Round((double)(waters.Count / 2), 0).ToString())].ToString();


                float avgtemp = temps.Sum() / temps.Count;
                float avgtpress = preassures.Sum() / preassures.Count;
                float avgwaters = waters.Sum() / waters.Count;

                float atemp = 0;
                float apress = 0;
                float awater = 0;

                foreach (int temp in temps)
                {
                    atemp += (float)Math.Pow(avgtemp - temp, 2);
                }
                foreach (int pre in preassures)
                {
                    apress += (float)Math.Pow(avgtpress - pre, 2);
                }
                foreach (int wat in waters)
                {
                    awater += (float)Math.Pow(avgwaters - wat, 2);
                }

                atemp = atemp / temps.Count;
                apress = apress / preassures.Count;
                awater = awater / waters.Count;

                lb_avgtemp.Content = atemp.ToString();
                lb_avgpress.Content = apress.ToString();
                lb_avgwater.Content = awater.ToString();

            }
            else
            {
                lb_medtem.Content = "";
                lb_medpress.Content = "";
                lb_medwater.Content = "";
                lb_avgtemp.Content = "-";
                lb_avgpress.Content = "-";
                lb_avgwater.Content = "-";
            }



        }

        private void dtp2_CalendarClosed(object sender, RoutedEventArgs e)
        {
            count();
        }
    }
}
