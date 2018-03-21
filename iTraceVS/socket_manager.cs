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
        private static reticle ret;

        public static void getSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                xml_writer.xmlStart();
                toRead = true;
                ret = new reticle();
                worker = new Thread(writeData);
                worker.Start();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void writeData() {
            while (toRead) {
                if (client.GetStream().DataAvailable == true) {
                    string data = clientIn.ReadLine();
                    xml_writer.writeResponse(data);
                    
                    updateReticle(data);
                }
            }

            xml_writer.xmlEnd();
            return;
        }

        public static void closeSocket() {
            toRead = false;
            client = null;
            clientIn = null;
        }

        static void updateReticle(string data) {
            int i = 0;
            string x = "";
            string y = "";
            while (data[i] != ',')
                ++i;
            ++i; //move past the ','

            while (data[i] != '.' && data[i] != ',') {
                x += data[i];
                ++i;
            }
            while (data[i] != ',')
                ++i;
            ++i; //move past the ','

            while (data[i] != '.' && i < data.Length - 1) {
                y += data[i];
                ++i;
            }

            if (x == "-nan(ind)" || y == "-nan(ind")
                return;

            ret.updateReticle(Convert.ToInt32(x), Convert.ToInt32(y));
        }

        public static void reticleShow(bool show) {
            ret.toDraw(show);
        }

    }
}
