using System;
using System.Xml;
using System.Windows.Forms;

namespace iTraceVS {

    public sealed class XMLDataWriter {

        private static readonly Lazy<XMLDataWriter> Singleton =
        new Lazy<XMLDataWriter>(() => new XMLDataWriter());

        private System.Collections.Concurrent.BlockingCollection<XMLJob> XMLDataQueue;

        public static XMLDataWriter Instance { get { return Singleton.Value; } }

        public XmlTextWriter writer;


        public void StartWriter()
        {
            XMLDataQueue = new System.Collections.Concurrent.BlockingCollection<XMLJob>(new System.Collections.Concurrent.ConcurrentQueue<XMLJob>());
            new System.Threading.Thread(() =>
            {
                DequeueData();
            }).Start();
        }

        public void EnqueueData(XMLJob job)
        {
            XMLDataQueue.Add(job);
        }

        private void DequeueData()
        {
            XMLJob job = XMLDataQueue.Take();
            while (job.JobID != 0)
            {
                WriteJobData(job);
                job = XMLDataQueue.Take();
            }
        }

        private void WriteJobData(XMLJob job)
        {
            if (job.JobType == XMLJob.GAZE_DATA)
            {
                WriteResponseData(job);
            }
            else if (job.JobType == XMLJob.SESSION_START)
            {
                StartXML(job);
            }
            else
            {
                EndXML();
            }
        }

        private void StartXML(XMLJob job)
        {
            writer = new XmlTextWriter(job.OutputRootDir + "/itrace_msvs-" + job.SessionTimeStamp + ".xml", System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("itrace_plugin");
            writer.WriteAttributeString("session_id", job.SessionID.ToString());
            WriteEnvironmentData();
            writer.WriteStartElement("gazes");
        }

        private void WriteEnvironmentData()
        {
            writer.WriteStartElement("environment");
            writer.WriteAttributeString("screen_width", Screen.PrimaryScreen.WorkingArea.Width.ToString());
            writer.WriteAttributeString("screen_height", Screen.PrimaryScreen.WorkingArea.Height.ToString());
            writer.WriteAttributeString("plugin_type", "MSVS");
            writer.WriteEndElement();
        }

        private void WriteResponseData(XMLJob job)
        {
            writer.WriteStartElement("response");
            writer.WriteAttributeString("event_id", job.EventID.ToString());
            writer.WriteAttributeString("plugin_time", job.PluginTime.ToString());
            writer.WriteAttributeString("x", job.EyeX.ToString());
            writer.WriteAttributeString("y", job.EyeY.ToString());
            writer.WriteAttributeString("gaze_target", job.GazeTarget != null ? job.GazeTarget : "" );
            writer.WriteAttributeString("gaze_target_type", job.GazeTargetType != null ? job.GazeTargetType : "" );
            writer.WriteAttributeString("source_file_path", job.SourceFilePath != null ? job.SourceFilePath : "" );
            writer.WriteAttributeString("source_file_line", job.SourceFileLine != null ? job.SourceFileLine.Value.ToString() : "" );
            writer.WriteAttributeString("source_file_col", job.SourceFileCol != null ? job.SourceFileCol.Value.ToString() : "");
            writer.WriteAttributeString("editor_line_height", job.EditorLineHeight != null ? job.EditorLineHeight.Value.ToString() : "");
            writer.WriteAttributeString("editor_font_height", job.EditorFontHeight != null ? job.EditorFontHeight.Value.ToString() : "");
            writer.WriteAttributeString("editor_line_base_x", job.EditorLineBaseX != null ? job.EditorLineBaseX.Value.ToString() : "");
            writer.WriteAttributeString("editor_line_base_y", job.EditorLineBaseY != null ? job.EditorLineBaseY.Value.ToString() : "");
            writer.WriteEndElement();
        }

        public void EndXML()
        {
            writer.WriteEndElement(); // End Gazes
            writer.WriteEndElement(); // End itrace_plugin
            writer.Close();
        }
    }
}
