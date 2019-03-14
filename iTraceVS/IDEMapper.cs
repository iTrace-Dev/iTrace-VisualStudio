using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using System.Windows;

namespace iTraceVS
{
    class IDEMapper
    {
        private DTE dte;

        public IDEMapper()
        {
            CoreDataHandler.Instance.OnCoreDataReceived += ProcessCoreData;
        }

        private void ProcessCoreData(object sender, CoreDataReceivedEventArgs e)
        {
            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            int line = -1, col = -1, offsetWindow = -1;
            string fileName = "", type = "", path = "", lineHeight= "", fontHeight = "";
            SnapshotPoint? bufferPos;

            offsetWindow = (offsetWindow + 1) % 15;

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
                        Point localPoint = new Point(Convert.ToInt32(e.ReceivedCoreData.eyeX), Convert.ToInt32(e.ReceivedCoreData.eyeY));

                        try
                        {
                            localPoint = wpfTextView.VisualElement.PointFromScreen(new Point(e.ReceivedCoreData.eyeX, e.ReceivedCoreData.eyeY));
                        }
                        catch { }

                        bufferPos = ConvertToPosition(wpfTextView, localPoint);

                        if (bufferPos != null)
                        {
                            fileName = window.Document.Name;
                            type = fileName.Split('.')[1];
                            path = openWindowPath;
                            line = bufferPos.Value.GetContainingLine().LineNumber + 1;
                            col = bufferPos.Value.Position - bufferPos.Value.GetContainingLine().Start.Position + 1;

                            var textLine = wpfTextView.TextViewLines.GetTextViewLineContainingYCoordinate(localPoint.Y + wpfTextView.ViewportTop);
                            //lineBaseY = (textLine.Bottom + wpfTextView.ViewportTop).ToString(); //still needs refining to
                            //lineBaseX = (textLine.Left + wpfTextView.ViewportLeft).ToString();  //ensure correct values
                            lineHeight = textLine.Height.ToString();
                            fontHeight = textLine.TextHeight.ToString();
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("({0}, {1}) - {2} {3} {4} [{5}, {6}]", e.ReceivedCoreData.eyeX, e.ReceivedCoreData.eyeY, fileName, type, path, line, col);
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
