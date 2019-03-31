using Microsoft;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Threading;

namespace iTraceVS {
    class StatusBar {
        IVsStatusbar statusBar;
        volatile string eventID;

        DispatcherTimer statusBarRefreshTimer;

        //Must be created on UI Thread
        public StatusBar() {
            statusBar = (IVsStatusbar)itrace_windowCommand.Instance.ServiceProvider.GetService(typeof(SVsStatusbar));
            Assumes.Present(statusBar);
            statusBarRefreshTimer = new DispatcherTimer();
            statusBarRefreshTimer.Tick += statusBarRefreshTimer_Tick;
            statusBarRefreshTimer.Interval = TimeSpan.FromSeconds(1);
            eventID = "";
        }

        public void startUpdating() {
            statusBarRefreshTimer.Start();
        }

        public void stopUpdating() {
            statusBarRefreshTimer.Stop();
        }

        private void statusBarRefreshTimer_Tick(object sender, object e) {
            int frozen;
            statusBar.IsFrozen(out frozen);

            if (frozen != 0) {
                statusBar.FreezeOutput(0);
            }

            statusBar.SetText(eventID);
            statusBar.FreezeOutput(1);
        }

        public void setEventID(long? id) {
            if (id.HasValue) {
                eventID = id.ToString();
            }
            else
            {
                eventID = "";
            }
        }
    }
}
