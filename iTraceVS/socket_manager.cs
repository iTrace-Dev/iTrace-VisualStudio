using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace iTraceVS
{
    class socket_manager
    {
        private static int port = 8008;
        static TcpClient client;
        static StreamReader clientIn;
        static bool toRead = false;
        private static Thread worker;

        public static void getSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                xml_writer.xmlStart();
                toRead = true;

                worker = new Thread(writeData);
                worker.Start();
            }
            catch(Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void writeData() {
            while (toRead) {
                if (client.GetStream().DataAvailable == true) {
                    string data = clientIn.ReadLine();
                    xml_writer.writeResponse(data);
                }
            }
            xml_writer.xmlEnd();
        }

        public static void closeSocket() {
            toRead = false;
            client = null;
            clientIn = null;
        }
    }
}
