/********************************************************************************************************************************************************
* @file core_data.cs
*
* @Copyright (C) 2022 i-trace.org
*
* This file is part of iTrace Infrastructure http://www.i-trace.org/.
* iTrace Infrastructure is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
* iTrace Infrastructure is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
* You should have received a copy of the GNU General Public License along with iTrace Infrastructure. If not, see <https://www.gnu.org/licenses/>.
********************************************************************************************************************************************************/

using System;

namespace iTraceVS {
    class core_data {

        public double eyeX;
        public double eyeY;
        public Int64 eventID;

        public core_data() {
            eyeX = -1;
            eyeY = -1;
            eventID = -1;
        }

        public core_data(string data) {

            string[] data_string;

            data_string = data.Split(',');

            if (data_string[0] == "gaze") {
                eventID = Convert.ToInt64(data_string[1]);

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
                eventID = -1;
                System.Diagnostics.Debug.WriteLine(data);
                if (xml_writer.filePath == "default.xml") {
                    xml_writer.filePath = data_string[3] + "/itrace_msvs_" + data_string[2] + ".xml";
                    xml_writer.xmlStart(data_string[1]);
                }
                else {
                    xml_writer.xmlEnd();
                    xml_writer.filePath = data_string[3] + "/itrace_msvs_" + data_string[2] + ".xml";
                    xml_writer.xmlStart(data_string[1]);
                }
            }
        }
    }
}
