using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTraceVS
{
    class core_data
    {
        double eyeX;
        double eyeY;
        Int64 sessionTime;

        public core_data() {
            eyeX = -1;
            eyeY = -1;
            sessionTime = -1;
        }

        public core_data(string data) {
            int i = 0;
            string x = "", y = "", session = "";

            while (data[i] != ',') {
                session += data[i];
                ++i;
            }
            sessionTime = Convert.ToInt64(session);
            ++i; //move past the ','
            
            while (data[i] != ',') {
                x += data[i];
                ++i;
            }
            ++i; //move past the ','

            while (i < data.Length) {
                y += data[i];
                ++i;
            }

            if (x == "-nan(ind)" || y == "-nan(ind") {
                eyeX = -1;
                eyeY = -1;
            } else {
                eyeX = Convert.ToDouble(x);
                eyeY = Convert.ToDouble(y);
            }
        }

        ~core_data() { }      
    }
}
