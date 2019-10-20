using System;

namespace iTraceVS {
    class CoreData {

        public double eyeX;
        public double eyeY;
        public Int64 eventID;

        public CoreData() {
            eyeX = -1;
            eyeY = -1;
            eventID = -1;
        }

        public CoreData(string data) {

            string[] dataString;

            dataString = data.Split(',');

            if (dataString[0] == "gaze") {
                eventID = Convert.ToInt64(dataString[1]);

                if (dataString[2] == "nan" || dataString[3] == "nan") {
                    eyeX = -1;
                    eyeY = -1;
                }
                else {
                    eyeX = Convert.ToDouble(dataString[2]);
                    eyeY = Convert.ToDouble(dataString[3]);
                }
            }
            else if (dataString[0] == "session_start") {
                eyeX = -1;
                eyeY = -1;
                eventID = -1;
                System.Diagnostics.Debug.WriteLine(data);
                if (XmlWriter.filePath == "default.xml") {
                    XmlWriter.filePath = dataString[3] + "/itrace_msvs_" + dataString[2] + ".xml";
                    XmlWriter.XmlStart(dataString[1]);
                }
                else {
                    XmlWriter.XmlEnd();
                    XmlWriter.filePath = dataString[3] + "/itrace_msvs_" + dataString[2] + ".xml";
                    XmlWriter.XmlStart(dataString[1]);
                }
            }
        }
    }
}
