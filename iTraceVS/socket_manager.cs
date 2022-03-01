/********************************************************************************************************************************************************
* @file socket_manager.cs
*
* @Copyright (C) 2022 i-trace.org
*
* This file is part of iTrace Infrastructure http://www.i-trace.org/.
* iTrace Infrastructure is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
* iTrace Infrastructure is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
* You should have received a copy of the GNU General Public License along with iTrace Infrastructure. If not, see <https://www.gnu.org/licenses/>.
********************************************************************************************************************************************************/

using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace iTraceVS
{
    class socket_manager
    {
        public static int port = 8008;
        public static bool active = false;

        static TcpClient client;
        static StreamReader clientIn;
        private static Thread readWorker;
        //public static status_bar statusBar;

        public static void getSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                xml_writer.filePath = "default.xml";
                
                xml_writer.xmlStart("0"); //Hack to get XML data to write...
                active = true;
                itrace_windowControl.connected = true;
                xml_writer.gazeStart = false;
                //statusBar = new status_bar();
                //statusBar.startUpdating();

                readWorker = new Thread(readData);
                readWorker.Start();                
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void readData() {
            while (active) {
                if (client.GetStream().DataAvailable == true) {
                    string data = clientIn.ReadLine();
                    xml_writer.dataReady = true;
                    xml_writer.data = new core_data(data);
                }
            }
        }

        public static void closeSocket() {
            active = false;
            client = null;
            clientIn = null;
            xml_writer.xmlEnd();
        }
    }
}
