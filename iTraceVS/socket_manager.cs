using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace iTraceVS
{
    class socket_manager
    {
        private static int port = 8008;
        static bool active = false;

        static TcpClient client;
        static StreamReader clientIn;
        static core_buffer buffer;
        private static Thread readWorker;
        private static Thread writeWorker;
        private static reticle ret;

        public static void getSocket() {
            try {
                client = new TcpClient("localhost", port);
                clientIn = new StreamReader(client.GetStream());
                xml_writer.xmlStart();
                buffer = core_buffer.Instance;
                ret = new reticle();
                active = true;

                readWorker = new Thread(readData);
                readWorker.Start();
                writeWorker = new Thread(writeData);
                writeWorker.Start();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void readData() {
            while (active) {
                if (client.GetStream().DataAvailable == true) {
                    string data = clientIn.ReadLine();
                    buffer.enqueue(new core_data(data));
                }
            }
        }

        static void writeData() {
            core_data data;
            while (active) {
                data = buffer.dequeue();
                if (data.sessionTime != -1) {
                    xml_writer.writeResponse(data.sessionTime, data.eyeX, data.eyeY);
                    ret.updateReticle(Convert.ToInt32(data.eyeX), Convert.ToInt32(data.eyeY));
                }
            }
            xml_writer.xmlEnd();
        }

        public static void closeSocket() {
            active = false;
            client = null;
            clientIn = null;
        }
        
        public static void reticleShow(bool show) {
            ret.toDraw(show);
        }

    }
}
