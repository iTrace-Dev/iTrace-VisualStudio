using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Threading;

namespace iTraceVS
{
    class status_bar
    {
        IVsStatusbar statusBar;
        string text;

        DispatcherTimer statusBarRefreshTimer;

        //Must be created on UI Thread
        public status_bar()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            statusBar = (IVsStatusbar)itrace_windowCommand.Instance.ServiceProvider.GetService(typeof(SVsStatusbar));

            statusBarRefreshTimer = new DispatcherTimer();
            statusBarRefreshTimer.Tick += statusBarRefreshTimer_Tick;
            statusBarRefreshTimer.Interval = TimeSpan.FromSeconds(1);
        }

        public void startUpdating()
        {
            statusBarRefreshTimer.Start();
        }

        public void stopUpdating()
        {
            statusBarRefreshTimer.Stop();
        }

        private void statusBarRefreshTimer_Tick(object sender, object e)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            int frozen;

            statusBar.IsFrozen(out frozen);

            if (frozen != 0)
            {
                statusBar.FreezeOutput(0);
            }

            statusBar.SetText(text);

            statusBar.FreezeOutput(1);
        }

        public void setText(string toSet)
        {
            text = toSet;
        }
    }
}
