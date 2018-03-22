using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iTraceVS
{
    class core_buffer
    {
        Queue<core_data> buffer;
        private static Mutex mutex = new Mutex();
        private static core_buffer singleton;

        private core_buffer() { }

        public static core_buffer Instance {
            get {
                if (singleton == null) {
                    singleton = new core_buffer();
                }
                return singleton;
            }
        }

        core_data dequeue() {
            core_data cd = new core_data();

            while (buffer.Count == 0) {
                Thread.Sleep(10);
            }

            if (mutex.WaitOne(500)) {
                cd = buffer.Dequeue();
                mutex.ReleaseMutex();
            }

            return cd;
        }

        void enqueue(core_data cd) {
            if (mutex.WaitOne(500)) {
                buffer.Enqueue(cd);
                mutex.ReleaseMutex();
            }
        }
        
        ~core_buffer() {
            mutex.Dispose();
        }
    }
}
