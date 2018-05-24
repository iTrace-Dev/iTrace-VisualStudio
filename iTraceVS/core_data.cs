﻿using System;
//using System.Xml;

namespace iTraceVS
{
    class core_data
    {
        public double eyeX;
        public double eyeY;
        public Int64 sessionTime;

        public core_data() {
            eyeX = -1;
            eyeY = -1;
            sessionTime = -1;
        }

        public core_data(string data) {
            int i = 0;
            string type = "", x = "", y = "", session = "";
            string path = "", id = "";

            while (data[i] != ',')
            {
                type += data[i];
                ++i;
            }
            ++i;

            if (type == "gaze") {
                while (data[i] != ',')
                {
                    session += data[i];
                    ++i;
                }
                sessionTime = Convert.ToInt64(session);
                ++i; //move past the ','

                while (data[i] != ',')
                {
                    x += data[i];
                    ++i;
                }
                ++i; //move past the ','

                while (i < data.Length)
                {
                    y += data[i];
                    ++i;
                }

                if (x == "-nan(ind)" || y == "-nan(ind")
                {
                    eyeX = -1;
                    eyeY = -1;
                }
                else
                {
                    eyeX = Convert.ToDouble(x);
                    eyeY = Convert.ToDouble(y);
                }
            }
            else if (type == "session") {
                while (i < data.Length) {
                    path += data[i];
                    ++i;
                }
                --i;

                while (data[i] != '\\') {
                    id = id.Insert(0, data[i].ToString());
                    --i;
                }
                
                eyeX = -1;
                eyeY = -1;
                sessionTime = -1;

                xml_writer.filePath = path + "/visualStudio_" + id + ".xml";
                xml_writer.xmlStart();
            }
        }

        ~core_data() { }      
    }
}
