using System;
using System.Diagnostics;

namespace iTraceVS {

    public class CoreData {
        public double eyeX;
        public double eyeY;
        public long sessionTime;

        private readonly int EVENT_ID = 1;
        private readonly int X_DATA_ID = 2;
        private readonly int Y_DATA_ID = 3;

        public CoreData(string data) {
            string[] data_string = data.Split(',');
            sessionTime = Convert.ToInt64(data_string[EVENT_ID]);
            
            eyeX = Convert.ToDouble(data_string[X_DATA_ID]);
            eyeY = Convert.ToDouble(data_string[Y_DATA_ID]);
            
        }
    }
}
