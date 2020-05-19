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
        public static String filePath = "default.xml";
        public static SnapshotPoint? bufferPos;

        public static bool gazeStart = false;
        public static bool dataReady = false;
        public static core_data data = new core_data();

        public static void xmlStart(string sessionId) {
            prefs = new XmlWriterSettings() {
                Indent = true
            };
            writer = XmlWriter.Create(filePath, prefs);
            writer.WriteStartDocument();
            writer.WriteStartElement("itrace_plugin");
            writer.WriteAttributeString("session_id", sessionId);

            writer.WriteStartElement("environment");
            //store screen height and width
            writer.WriteAttributeString("screen_width", Screen.PrimaryScreen.Bounds.Width.ToString());
            writer.WriteAttributeString("screen_height", Screen.PrimaryScreen.Bounds.Height.ToString());
            //store plugin used
            writer.WriteAttributeString("plugin_type", "MSVS");
            //close environment
            writer.WriteEndElement();

            writer.WriteStartElement("gazes");
            gazeStart = true;

            timer = new System.Windows.Forms.Timer() { Interval = 5 };
            timer.Tick += new EventHandler(timerTick);
            timer.Start();
        }

        static void timerTick(object sender, EventArgs e) {
            //core_data data;
            if (socket_manager.active && dataReady && gazeStart) {
                //data = socket_manager.buffer.dequeue();
                if (data.eventID > 0) {
                    writeResponse(data.eventID, data.eyeX, data.eyeY);
                    //socket_manager.statusBar.setText(data.sessionTime.ToString());
                }
                dataReady = false;
            }
        }

        public static void writeResponse(Int64 eventId, double x, double y) {
            writer.WriteStartElement("response");
            writer.WriteAttributeString("event_id", Convert.ToString(eventId));
            writer.WriteAttributeString("plugin_time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            writer.WriteAttributeString("x", Convert.ToString(x));
            writer.WriteAttributeString("y", Convert.ToString(y));

            getVSData(x, y);

            writer.WriteEndElement();
            writer.Flush();
        }

        static void getVSData(double x, double y) {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            //Var to print
            String lineHeight = "", fontHeight = "", lineBaseX = "", lineBaseY = ""; 
            int line = -1, col = -1;
            String fileName = "", type = "", path = "";

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
                        Point localPoint = new Point(-1, -1);
                        try {
                            localPoint = wpfTextView.VisualElement.PointFromScreen(new Point(x, y));
                        }
                        catch {  }
                        
                        bufferPos = ConvertToPosition(wpfTextView, localPoint);
                        if (bufferPos != null) {
                            fileName = window.Document.Name;
                            type = fileName.Split('.')[1];
                            path = openWindowPath;
                            line = bufferPos.Value.GetContainingLine().LineNumber + 1; 
                            col = bufferPos.Value.Position - bufferPos.Value.GetContainingLine().Start.Position + 1;

                            var textLine = wpfTextView.TextViewLines.GetTextViewLineContainingYCoordinate(localPoint.Y + wpfTextView.ViewportTop);
                            lineBaseY = (textLine.Bottom + wpfTextView.ViewportTop).ToString(); //still needs refining to
                            lineBaseX = (textLine.Left + wpfTextView.ViewportLeft).ToString();  //ensure correct values
                            lineHeight = textLine.Height.ToString();
                            fontHeight = textLine.TextHeight.ToString();
                        }
                    }
                }
            }

            writer.WriteAttributeString("gaze_target", fileName);
            writer.WriteAttributeString("gaze_target_type", type);

            writer.WriteAttributeString("source_file_path", path);
            writer.WriteAttributeString("source_file_line", line.ToString());
            writer.WriteAttributeString("source_file_col", col.ToString());

            writer.WriteAttributeString("editor_line_height", lineHeight);
            writer.WriteAttributeString("editor_font_height", fontHeight);
            writer.WriteAttributeString("editor_line_base_x", lineBaseX);
            writer.WriteAttributeString("editor_line_base_y", lineBaseY);
        }

        static SnapshotPoint? ConvertToPosition(ITextView view, Point pos) {
            SnapshotPoint? position = null;
            {
                // See that we have a view
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
            writer.WriteEndElement();   //close gazes
            writer.WriteEndElement();   //close plugin
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            timer.Stop();
            timer.Dispose();
        }

        public static bool runCheck()
        {
            return timer.Enabled;
        }
    }
}
