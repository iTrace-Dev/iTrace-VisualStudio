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
        public static String filePath = "plugin_test.xml";

        public static void xmlStart() {
            prefs = new XmlWriterSettings() {
                Indent = true
            };
            writer = XmlWriter.Create(filePath, prefs);
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

            writer.WriteStartElement("gazes");

            timer = new System.Windows.Forms.Timer() { Interval = 8, Enabled = true };
            timer.Tick += new EventHandler(timerTick);
        }

        static void timerTick(object sender, EventArgs e) {
            core_data data;
            if (socket_manager.active) {
                data = socket_manager.buffer.dequeue();
                if (data.sessionTime != -1) {
                    writeResponse(data.sessionTime, data.eyeX, data.eyeY);
                }
            }
        }

        public static void writeResponse(Int64 sessionTime, double x, double y) {
            writer.WriteStartElement("response");

            String fileName = getFileName();
            writer.WriteAttributeString("object_name", fileName);
            if (fileName != "")
                writer.WriteAttributeString("type", fileName.Split('.')[1]);
            else
                writer.WriteAttributeString("type", "");

            writer.WriteAttributeString("x", Convert.ToString(x));
            writer.WriteAttributeString("y", Convert.ToString(y));
            writer.WriteAttributeString("timestamp", DateTime.Now.ToString());
            writer.WriteAttributeString("event_time", Convert.ToString(sessionTime));

            writer.WriteAttributeString("path", getFilePath());
            getLineCol(x, y);

            writer.WriteEndElement();
            writer.Flush();
        }

        static string getFileName() {
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            string fileName = "";
            if (dte.ActiveDocument != null) {
                fileName = dte.ActiveDocument.Name;
            }
            return fileName;
        }

        static string getFilePath() {
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            string path = "";
            if (dte.ActiveDocument != null) {
                path = dte.ActiveDocument.FullName;
            }
            return path;
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
                            writer.WriteAttributeString("line_height", "");
                            writer.WriteAttributeString("font_height", "");
                            writer.WriteAttributeString("line", bufferPos.Value.GetContainingLine().LineNumber.ToString()); //0-indexed, the +1 is handled later in the toolchain
                            writer.WriteAttributeString("col", (bufferPos.Value.Position - bufferPos.Value.GetContainingLine().Start.Position).ToString());
                            writer.WriteAttributeString("line_base_x", "");
                            writer.WriteAttributeString("line_base_y", "");
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
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

    }
}
