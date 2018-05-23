using System;
using System.Windows.Forms;
using System.Xml;
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
    class xml_writer
    {
        public static XmlWriter writer;
        public static XmlWriterSettings prefs;
        private static System.Windows.Forms.Timer timer;

        public static void xmlStart() {
            prefs = new XmlWriterSettings() {
                Indent = true
            };
            writer = XmlWriter.Create("plugin_test.xml", prefs);
            writer.WriteStartDocument();
            writer.WriteStartElement("plugin");

            writer.WriteStartElement("environment");
            //store screen height and width
            writer.WriteStartElement("screen-size");
            writer.WriteAttributeString("width", Screen.PrimaryScreen.WorkingArea.Width.ToString());
            writer.WriteAttributeString("height", Screen.PrimaryScreen.WorkingArea.Height.ToString());
            writer.WriteEndElement();
            //store plugin used
            writer.WriteStartElement("application");
            writer.WriteAttributeString("type", "MSVS");
            writer.WriteEndElement();
            //close environment
            writer.WriteEndElement();

            timer = new System.Windows.Forms.Timer() { Interval = 10, Enabled = true };
            timer.Tick += new EventHandler(timerTick);
        }

        static void timerTick(object sender, EventArgs e) {
            core_data data;
            if (socket_manager.active) {
                data = socket_manager.buffer.dequeue();
                if (data.sessionTime != -1) {
                    writeResponse(data.sessionTime, data.eyeX, data.eyeY);
                    socket_manager.ret.updateReticle(Convert.ToInt32(data.eyeX), Convert.ToInt32(data.eyeY));
                }
            }
        }

        public static void writeResponse(Int64 sessionTime, double x, double y) {
            writer.WriteStartElement("responses");
           
            writer.WriteAttributeString("x", Convert.ToString(x));
            writer.WriteAttributeString("y", Convert.ToString(y));
            writer.WriteAttributeString("session-time", Convert.ToString(sessionTime));

            //additional attributes
            writer.WriteAttributeString("path", getFile());
            getLineCol(x, y);

            writer.WriteEndElement();
            writer.Flush();
        }

        static string getFile() {
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            string filePath = "no active file";
            if (dte.ActiveDocument.FullName != null) {
                filePath = dte.ActiveDocument.FullName;
                //Debug.WriteLine(dte.ActiveDocument.ActiveWindow.Left + ", " + dte.ActiveDocument.ActiveWindow.Top + " : " + x + ", " + y);
            }
            return filePath;
        }

        static void getLineCol(double x, double y) {
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            foreach (EnvDTE.Window window in dte.Windows) {
                if (!window.Visible) {
                    continue;
                }

                // only look at text editor windows
                if (window.Type == vsWindowType.vsWindowTypeDocument || window.Type == vsWindowType.vsWindowTypeCodeWindow) {
                    var openWindowPath = Path.Combine(window.Document.Path, window.Document.Name);
                    ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
                    IVsUIHierarchy uiHierarchy;
                    uint itemID;
                    IVsWindowFrame windowFrame;
                    if (VsShellUtilities.IsDocumentOpen(sp, openWindowPath, Guid.Empty, out uiHierarchy, out itemID, out windowFrame)) {
                        IVsTextView textView = VsShellUtilities.GetTextView(windowFrame);
                        object holder;
                        Guid guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
                        IVsUserData userData = textView as IVsUserData;
                        userData.GetData(ref guidViewHost, out holder);
                        IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;
                        IWpfTextView wpfTextView = viewHost.TextView;
                        Point localPoint = wpfTextView.VisualElement.PointFromScreen(new Point(x, y));

                        SnapshotPoint? bufferPos = ConvertToPosition(wpfTextView, localPoint);
                        if (bufferPos != null) {
                            writer.WriteAttributeString("line", bufferPos.Value.GetContainingLine().LineNumber.ToString()); //0-indexed, the +1 is handled later in the toolchain
                            writer.WriteAttributeString("col", (bufferPos.Value.Position - bufferPos.Value.GetContainingLine().Start.Position).ToString());
                        }
                        else {
                            writer.WriteAttributeString("line", "nan");
                            writer.WriteAttributeString("col", "nan");
                        }
                    }
                }
            }
        }

        static SnapshotPoint? ConvertToPosition(ITextView view, Point pos) {
            SnapshotPoint? position = null;
            {
                // See that we have  view
                if (view != null && view.TextViewLines != null) {
                    if ((pos.X >= 0.0) && (pos.X < view.ViewportWidth) && (pos.Y >= 0.0) && (pos.Y < view.ViewportHeight)) {
                        var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(pos.Y + view.ViewportTop);
                        if (line != null) {
                            double x = pos.X + view.ViewportLeft;
                            position = line.GetBufferPositionFromXCoordinate(x);
                            if ((!position.HasValue) && (line.LineBreakLength == 0) && (line.EndIncludingLineBreak == view.TextSnapshot.Length)) {
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

        public static void xmlEnd() {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

    }
}
