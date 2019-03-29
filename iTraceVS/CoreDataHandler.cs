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

                SnapshotPoint? bufferPos = null;

                try
                {
                    bufferPos = ConvertToPosition(ActiveWindow.TextView, localPoint);
                }
                catch (InvalidOperationException e)
                {
                    return;
                }
                catch (Exception e) {}

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
    }
}
