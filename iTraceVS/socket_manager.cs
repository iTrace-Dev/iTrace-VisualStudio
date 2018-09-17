using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace iTraceVS
{
    class socket_manager
    {
        private static int port = 8008;
        public static bool active = false;

        static TcpClient client;
        static StreamReader clientIn;
        private static Thread readWorker;
        public static reticle ret = null;
        public static status_bar statusBar;

        public static void getSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                xml_writer.xmlStart();
                ret = new reticle();
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
        
        public static void reticleShow(bool show) {
            ret.toDraw(show);
        }

    }
}
