using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Threading;

namespace iTraceVS
{
    class StatusBar
    {
        IVsStatusbar statusBar;
        string text;

        DispatcherTimer statusBarRefreshTimer;

        //Must be created on UI Thread
        public StatusBar()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            statusBar = (IVsStatusbar)WindowCommand.Instance.ServiceProvider.GetService(typeof(SVsStatusbar));
            Assumes.Present(statusBar);
            statusBarRefreshTimer = new DispatcherTimer();
            statusBarRefreshTimer.Tick += StatusBarRefreshTimerTick;
            statusBarRefreshTimer.Interval = TimeSpan.FromSeconds(1);
        }

        public void StartUpdating()
        {
            statusBarRefreshTimer.Start();
        }

        public void StopUpdating()
        {
            statusBarRefreshTimer.Stop();
        }

        private void StatusBarRefreshTimerTick(object sender, object e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            int frozen;

            statusBar.IsFrozen(out frozen);

            if (frozen != 0)
            {
                statusBar.FreezeOutput(0);
            }

            statusBar.SetText(text);

            statusBar.FreezeOutput(1);
        }

        public void SetText(string toSet)
        {
            text = toSet;
        }
    }
}
