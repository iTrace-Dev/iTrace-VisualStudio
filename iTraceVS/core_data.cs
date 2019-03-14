using System;
using System.Diagnostics;

namespace iTraceVS {

    class core_data {
        public double eyeX;
        public double eyeY;
        public long sessionTime;

        public core_data() {
            eyeX = -1;
            eyeY = -1;
            sessionTime = -1;
        }

        public core_data(string data) {
            string[] data_string;

            data_string = data.Split(',');

            if (data_string[0] == "gaze") {
                sessionTime = Convert.ToInt64(data_string[1]);

                if (data_string[2] == "nan" || data_string[3] == "nan") {
                    eyeX = -1;
                    eyeY = -1;
                }
                else {
                    eyeX = Convert.ToDouble(data_string[2]);
                    eyeY = Convert.ToDouble(data_string[3]);
                }
            }
            else if (data_string[0] == "session_start") {                
                eyeX = -1;
                eyeY = -1;
                sessionTime = -1;

                string[] tmp = data_string[1].Split('\\');
                System.Diagnostics.Debug.WriteLine("FILEPATH");
                System.Diagnostics.Debug.WriteLine(data_string[3] + "/itrace_msvs-" + data_string[2] + ".xml");
                if (xml_writer.filePath == "default.xml") {
                    xml_writer.filePath = data_string[3] + "/itrace_msvs-" + data_string[2] + ".xml";
                    xml_writer.xmlStart();
                } else {
                    Debug.WriteLine("sessionTime else");
                    xml_writer.xmlEnd();
                    xml_writer.filePath = data_string[3] + "/itrace_msvs-" + data_string[2] + ".xml";
                    xml_writer.xmlStart();
                }
            }
        }
    }
}
