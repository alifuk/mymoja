using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mymoja
{
    interface IWeatherServer
    {

        void download();
        void parse();
        string gethtml();

        string getname();
        float getwind();

        float gettemp();

        float getprecipitation();
    }
}
