using System;

namespace iTraceVS
{
    public sealed class SocketManager
    {
        public event EventHandler<long> OnSocketConnect;

        private static readonly Lazy<SocketManager> Singleton =
            new Lazy<SocketManager>(() => new SocketManager());

        private long CoreConnected = 0;
        private System.Net.Sockets.TcpClient Client;
        private System.IO.StreamReader Reader;

        private readonly string CORE_ADDRESS = "127.0.0.1";
        private int CORE_PORT = 8008;

        private XMLDataWriter XMLOutput;

        public static SocketManager Instance { get { return Singleton.Value; } }

        public void Connect()
        {
            if (!IsConnected())
            {
                try
                {
                    Client = new System.Net.Sockets.TcpClient();
                    Client.Connect(CORE_ADDRESS, CORE_PORT);
                    Reader = new System.IO.StreamReader(Client.GetStream());

                    System.Threading.Interlocked.Exchange(ref CoreConnected, 1);
                    OnSocketConnect(this, CoreConnected);

                    CoreDataHandler.Instance.StartHandler();

                    new System.Threading.Thread(() =>
                    {
                        System.Threading.Thread.CurrentThread.IsBackground = true;
                        ReadCoreData();
                    }).Start();
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    System.Diagnostics.Debug.WriteLine("Core not connected!");
                    System.Diagnostics.Debug.WriteLine(e);
                    Client = null;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Core connection Failed!");
                    System.Diagnostics.Debug.WriteLine(e);
                    Client = null;
                }
            }
        }

        public void Connect(int port)
        {
            CORE_PORT = port;
            Connect();
        }

        public void Disconnect()
        {
            // Kill Threading in DataHandler
            CoreDataHandler.Instance.EnqueueData(null);

            // Notify UT listeners for application state change
            System.Threading.Interlocked.Exchange(ref CoreConnected, 0);
            OnSocketConnect(this, CoreConnected);
        }

        public bool IsConnected()
        {
            return System.Threading.Interlocked.Read(ref CoreConnected) == 1;
        }

        public void SetCorePort(int port)
        {
            CORE_PORT = port;
        }

        private void ReadCoreData()
        {
            System.Diagnostics.Debug.WriteLine(IsConnected());
            while (IsConnected())
            {
                if (Client.GetStream().DataAvailable)
                {
                    String data = Reader.ReadLine();
                    if (data.StartsWith("session_start"))
                    {
                        XMLOutput = new XMLDataWriter(data);
                    }
                    else if (data.StartsWith("session_end"))
                    {
                        XMLOutput.EndXML();
                    }
                    else
                    {
                        CoreDataHandler.Instance.EnqueueData(new CoreData(data));
                    }
                }
            }
        }
    }
}
