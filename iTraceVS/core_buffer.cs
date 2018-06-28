using System;
using System.Collections.Generic;
using System.Threading;

namespace iTraceVS
{
    class core_buffer
    {
        public static Queue<core_data> buffer;
        private static Mutex mutex;
        private static core_buffer singleton;

        private core_buffer() {
            mutex = new Mutex();
            buffer = new Queue<core_data>();
        }

        public static core_buffer Instance {
            get {
                if (singleton == null) {
                    singleton = new core_buffer();
                }
                return singleton;
            }
        }

        public core_data dequeue() {
            core_data cd = new core_data();

            if (buffer.Count == 0)
                ; //Do nothing if there is no data in the buffer
            else if (mutex.WaitOne(5)) {
                cd = buffer.Dequeue();
                mutex.ReleaseMutex();
            }

            return cd;
        }

        public void enqueue(core_data cd) {
            if (mutex.WaitOne(20)) {
                buffer.Enqueue(cd);
                mutex.ReleaseMutex();
                if (cd.sessionTime > 0)
                    socket_manager.ret.updateReticle(Convert.ToInt32(cd.eyeX), Convert.ToInt32(cd.eyeY));
            }
        }
        
        ~core_buffer() {
            mutex.Dispose();
        }
    }
}
