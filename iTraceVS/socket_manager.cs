using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace iTraceVS {

    class socket_manager {
        private static int port = 8008;
        public static bool active = false;
        static string rawData = "";

        static TcpClient client;
        static StreamReader clientIn;
        private static Thread readWorker;
        public static BackgroundWorker bgWorker;
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

                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(processData);
                bgWorker.RunWorkerAsync();
                //bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(postProcess);
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());               
            }
        }

        static void readData() {
            while (active) {
                if (client.GetStream().DataAvailable && xml_writer.dataReady) {
                    if (!bgWorker.IsBusy) {
                        bgWorker.RunWorkerAsync();
                        //break;
                    }
                }
            }
        }

        static void processData(object sender, DoWorkEventArgs e) {
            //while (active) {
                //if (/*xml_writer.dataReady &&*/ client.GetStream().DataAvailable) {
                    string rawData = clientIn.ReadLine();
                    core_data gaze = new core_data(rawData);
                    if (gaze.sessionTime != -1)
                        xml_writer.writeResponse(gaze.sessionTime, gaze.eyeX, gaze.eyeY);
                //}
            //}
        }

        static void postProcess(object sender, RunWorkerCompletedEventArgs e) {
            if (active) {
                bgWorker.RunWorkerAsync();
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
