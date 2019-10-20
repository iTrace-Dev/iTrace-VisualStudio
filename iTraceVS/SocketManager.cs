using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace iTraceVS
{
    class SocketManager
    {
        public static int port = 8008;
        public static bool active = false;

        static TcpClient client;
        static StreamReader clientIn;
        private static Thread readWorker;
        //public static StatusBar statusBar;

        public static void GetSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                XmlWriter.filePath = "default.xml";
                
                XmlWriter.XmlStart("0"); //Hack to get XML data to write...
                active = true;
                WindowControl.connected = true;
                XmlWriter.gazeStart = false;
                //statusBar = new StatusBar();
                //statusBar.startUpdating();

                readWorker = new Thread(ReadData);
                readWorker.Start();                
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void ReadData() {
            while (active) {
                if (client.GetStream().DataAvailable == true) {
                    string data = clientIn.ReadLine();
                    XmlWriter.dataReady = true;
                    XmlWriter.data = new CoreData(data);
                }
            }
        }

        public static void CloseSocket() {
            active = false;
            client = null;
            clientIn = null;
            XmlWriter.XmlEnd();
        }
    }
}
