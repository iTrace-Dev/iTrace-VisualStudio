using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Windows;

namespace iTraceVS
{
    public sealed class CoreDataHandler
    {

        private static readonly Lazy<CoreDataHandler> Singleton =
        new Lazy<CoreDataHandler>(() => new CoreDataHandler());

        private volatile SourceWindow ActiveWindow = null;

        private System.Collections.Concurrent.BlockingCollection<string> CoreDataQueue;

        public static CoreDataHandler Instance { get { return Singleton.Value; } }

        //private static DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

        public void StartHandler()
        {
            CoreDataQueue = new System.Collections.Concurrent.BlockingCollection<string>(new System.Collections.Concurrent.ConcurrentQueue<string>());
            new System.Threading.Thread(() =>
            {
                DequeueData();
            }).Start();
        }

        public void EnqueueData(string cd)
        {
            CoreDataQueue.Add(cd);
        }

        public void SetActiveSourceWindow(SourceWindow sc)
        {
            if (ActiveWindow == null || sc.DocPath != ActiveWindow.DocPath)
            {
                ActiveWindow = sc;
            }
        }

        private void DequeueData()
        {
            string cd = CoreDataQueue.Take();
            while (cd != null)
            {
                ProcessCoreData(new XMLJob(cd, DateTime.UtcNow.Ticks));
                cd = CoreDataQueue.Take();
            }
            System.Diagnostics.Debug.WriteLine("QUEUE EMPTY!");
            
            // Kill the Writer thread (HACK)
            ProcessCoreData(new XMLJob("session_end\n", 0));
        }

        private void ProcessCoreData(XMLJob j)
        {
            if (j.JobType == XMLJob.GAZE_DATA)
            {
                
                Point localPoint = new Point(Convert.ToInt32(j.EyeX), Convert.ToInt32(j.EyeY));

                try
                {
                    localPoint = ActiveWindow.TextView.VisualElement.PointFromScreen(new Point(j.EyeX.GetValueOrDefault(), j.EyeY.GetValueOrDefault()));
                }
                catch { }

                SnapshotPoint? bufferPos = ConvertToPosition(ActiveWindow.TextView, localPoint);

                if (bufferPos != null)
                {
                    j.GazeTarget = ActiveWindow.DocName;
                    j.GazeTargetType = j.GazeTarget.Split('.')[1];
                    j.SourceFilePath = ActiveWindow.DocPath;
                    j.SourceFileLine = bufferPos.Value.GetContainingLine().LineNumber + 1;
                    j.SourceFileCol = bufferPos.Value.Position - bufferPos.Value.GetContainingLine().Start.Position + 1;

                    var textLine = ActiveWindow.TextView.TextViewLines.GetTextViewLineContainingYCoordinate(localPoint.Y + ActiveWindow.TextView.ViewportTop);
                    //lineBaseY = (textLine.Bottom + wpfTextView.ViewportTop).ToString(); //still needs refining to
                    //lineBaseX = (textLine.Left + wpfTextView.ViewportLeft).ToString();  //ensure correct values
                    j.EditorFontHeight = textLine.TextHeight;
                    j.EditorLineHeight = textLine.Height;
                    j.PluginTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
            XMLDataWriter.Instance.EnqueueData(j);
        }

        private SnapshotPoint? ConvertToPosition(ITextView view, Point pos)
        {
            SnapshotPoint? position = null;
            {
                // See that we have a view
                if (view != null && view.TextViewLines != null)
                {
                    if ((pos.X >= 0.0) && (pos.X < view.ViewportWidth) && (pos.Y >= 0.0) && (pos.Y < view.ViewportHeight))
                    {
                        var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(pos.Y + view.ViewportTop);
                        if (line != null)
                        {
                            double x = pos.X + view.ViewportLeft;
                            position = line.GetBufferPositionFromXCoordinate(x);
                            if ((!position.HasValue) && (line.LineBreakLength == 0) && (line.EndIncludingLineBreak == view.TextSnapshot.Length))
                            {
                                //For purposes of hover events, pretend the last line in the buffer
                                //actually is padded by the EndOfLineWidth (even though it is not).
                                if ((line.Left <= x) && (x < line.TextRight + line.EndOfLineWidth))
                                    position = line.End;
                            }
                        }
                    }
                }
            }
            return position;
        }

        // Helper Function for Threads
        /*private void ProcessCoreData(XMLJob j)
        {
            if (j.JobType == XMLJob.GAZE_DATA)
            {
                //long start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                foreach (EnvDTE.Window window in dte.Windows)
                {
                    if (!window.Visible)
                    {
                        continue;
                    }
                    // only look at text editor windows
                    if (window.Type == vsWindowType.vsWindowTypeDocument || window.Type == vsWindowType.vsWindowTypeCodeWindow)
                    {
                        var openWindowPath = Path.Combine(window.Document.Path, window.Document.Name);
                        ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
                        IVsUIHierarchy uiHierarchy;
                        uint itemID;
                        IVsWindowFrame windowFrame;
                        if (VsShellUtilities.IsDocumentOpen(sp, openWindowPath, Guid.Empty, out uiHierarchy, out itemID, out windowFrame))
                        {
                            IVsTextView textView = VsShellUtilities.GetTextView(windowFrame);
                            object holder;
                            Guid guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
                            IVsUserData userData = textView as IVsUserData;
                            userData.GetData(ref guidViewHost, out holder);
                            IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;
                            IWpfTextView wpfTextView = viewHost.TextView;
                            Point localPoint = new Point(Convert.ToInt32(j.EyeX), Convert.ToInt32(j.EyeY));

                            try
                            {
                                localPoint = wpfTextView.VisualElement.PointFromScreen(new Point(j.EyeX.GetValueOrDefault(), j.EyeY.GetValueOrDefault()));
                            }
                            catch { }

                            SnapshotPoint? bufferPos = ConvertToPosition(wpfTextView, localPoint);

                            if (bufferPos != null)
                            {
                                j.GazeTarget = window.Document.Name;
                                j.GazeTargetType = j.GazeTarget.Split('.')[1];
                                j.SourceFilePath = openWindowPath;
                                j.SourceFileLine = bufferPos.Value.GetContainingLine().LineNumber + 1;
                                j.SourceFileCol = bufferPos.Value.Position - bufferPos.Value.GetContainingLine().Start.Position + 1;

                                var textLine = wpfTextView.TextViewLines.GetTextViewLineContainingYCoordinate(localPoint.Y + wpfTextView.ViewportTop);
                                //lineBaseY = (textLine.Bottom + wpfTextView.ViewportTop).ToString(); //still needs refining to
                                //lineBaseX = (textLine.Left + wpfTextView.ViewportLeft).ToString();  //ensure correct values
                                j.EditorFontHeight = textLine.TextHeight;
                                j.EditorLineHeight = textLine.Height;
                                j.PluginTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            }
                        }
                        //System.Diagnostics.Debug.WriteLine("RUNTIME: " + Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start));
                    }
                }
            }
            XMLDataWriter.Instance.EnqueueData(j);
        }*/
    }
}
