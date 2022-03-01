/********************************************************************************************************************************************************
* @file status_bar.cs
*
* @Copyright (C) 2022 i-trace.org
*
* This file is part of iTrace Infrastructure http://www.i-trace.org/.
* iTrace Infrastructure is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
* iTrace Infrastructure is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
* You should have received a copy of the GNU General Public License along with iTrace Infrastructure. If not, see <https://www.gnu.org/licenses/>.
********************************************************************************************************************************************************/

using Microsoft;
using Microsoft.VisualStudio.Shell;
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
            ThreadHelper.ThrowIfNotOnUIThread();
            statusBar = (IVsStatusbar)itrace_windowCommand.Instance.ServiceProvider.GetService(typeof(SVsStatusbar));
            Assumes.Present(statusBar);
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

        public void setText(string toSet)
        {
            text = toSet;
        }
    }
}
