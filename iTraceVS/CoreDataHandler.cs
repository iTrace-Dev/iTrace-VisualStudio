using System;

namespace iTraceVS
{
    public sealed class CoreDataHandler
    {
        public event EventHandler<CoreDataReceivedEventArgs> OnCoreDataReceived;

        private static readonly Lazy<CoreDataHandler> Singleton =
        new Lazy<CoreDataHandler>(() => new CoreDataHandler());

        private System.Collections.Concurrent.BlockingCollection<CoreData> CoreDataQueue;
        
        public static CoreDataHandler Instance { get { return Singleton.Value; } }

        public void StartHandler()
        {
            CoreDataQueue = new System.Collections.Concurrent.BlockingCollection<CoreData>(new System.Collections.Concurrent.ConcurrentQueue<CoreData>());
            new System.Threading.Thread(() =>
            {
                DequeueData();
            }).Start();
        }

        public void EnqueueData(CoreData cd)
        {
            CoreDataQueue.Add(cd);
        }

        private void DequeueData()
        {
            System.Diagnostics.Debug.WriteLine("START DEQUEUE");
            CoreData cd = CoreDataQueue.Take();
            while (cd != null)
            {
                System.Diagnostics.Debug.WriteLine("DEQUEUE");
                if (OnCoreDataReceived != null)
                {
                    System.Diagnostics.Debug.WriteLine("EVENT!");
                    OnCoreDataReceived(this, new CoreDataReceivedEventArgs(cd));
                }
                cd = CoreDataQueue.Take();
            }
            System.Diagnostics.Debug.WriteLine("Queue Thread Done!");
        }
    }

    public class CoreDataReceivedEventArgs : EventArgs
    {
        public CoreData ReceivedCoreData { get; private set; }

        public CoreDataReceivedEventArgs(CoreData coreData)
        {
            ReceivedCoreData = coreData;
        }
    }
}
