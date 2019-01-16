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
using System.Diagnostics;


namespace iTraceVS {

    class xml_writer {

        public static XmlWriter writer;
        public static XmlWriterSettings prefs;
        //private static System.Timers.Timer timer;
        public static String filePath = "default.xml";
        public static SnapshotPoint? bufferPos;

        public static bool dataReady = true;
        public static core_data data = new core_data();

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

            //timer = new System.Timers.Timer() { Interval = 6, AutoReset = true, Enabled = true };
            //timer.Elapsed += timerTick;
        }

        static void timerTick(object sender, EventArgs e) {
            if (socket_manager.active && dataReady) {
                if (data.sessionTime != -1) {
                    writeResponse(data.sessionTime, data.eyeX, data.eyeY);
                    //socket_manager.statusBar.setText(data.sessionTime.ToString());
                }
                dataReady = false;
            }
        }

        public static void writeResponse(long sessionTime, double x, double y) {
            dataReady = false;
            socket_manager.statusBar.setText(Convert.ToString(sessionTime));

            writer.WriteStartElement("response");
            writer.WriteAttributeString("x", Convert.ToString(x));
            writer.WriteAttributeString("y", Convert.ToString(y));
            writer.WriteAttributeString("timestamp", DateTime.Now.ToString());
            writer.WriteAttributeString("event_time", Convert.ToString(sessionTime));
            //Debug.WriteLine(Convert.ToString(x) + ", " + Convert.ToString(y));

            getVSData(x, y);

            writer.WriteEndElement();
            writer.Flush();

            dataReady = true;
        }

        static void getVSData(double x, double y) {            
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            //Variables to print
            String lineHeight = "", fontHeight = "", fileName = "", type = "", path = ""; //lineBaseX = "", lineBaseY = ""; 
            int line = -1, col = -1;

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
                        Point localPoint = new Point(Convert.ToInt32(x), Convert.ToInt32(y));

                        //Debug.WriteLine("before try");
                        //try {
                        //    localPoint = wpfTextView.VisualElement.PointFromScreen(new Point(x, y));
                        //}
                        //catch {  }
                        //Debug.WriteLine("pre bufferPos");

                        bufferPos = ConvertToPosition(wpfTextView, localPoint);

                        if (bufferPos != null) {
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

            writer.WriteAttributeString("object_name", fileName);
            writer.WriteAttributeString("type", type);
            writer.WriteAttributeString("path", path);

            writer.WriteAttributeString("line", line.ToString());
            writer.WriteAttributeString("col", col.ToString());

            writer.WriteAttributeString("line_height", lineHeight);
            writer.WriteAttributeString("font_height", fontHeight);
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
        }
    }
}
