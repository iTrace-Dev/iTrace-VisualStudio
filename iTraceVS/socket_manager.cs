using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace iTraceVS {

    class socket_manager {
        public static int port = 8008;
        public static bool active = false;

        static TcpClient client;
        static StreamReader clientIn;
        private static Thread readWorker;
        public static status_bar statusBar;

        public static void getSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                active = true;
                itrace_windowControl.connected = true;

                statusBar = new status_bar();
                statusBar.startUpdating();

                readWorker = new Thread(readData);
                readWorker.Start();  
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void readData() {
            while (active) {
                if (client.GetStream().DataAvailable) {
                    string rawData = clientIn.ReadLine();
                    xml_writer.dataReady = true;
                    xml_writer.data = new core_data(rawData);
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
