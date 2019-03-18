using System.Windows.Forms;
using System.Xml;

namespace iTraceVS {

    class XMLDataWriter {

        public XmlTextWriter writer;

        private readonly int SESSION_ID_POS = 1;
        private readonly int SESSION_TIMESTAMP_POS = 2;
        private readonly int DIRECTORY_DATA_POS = 3;

        public XMLDataWriter(string data)
        {
            string[] sessionInfo = data.Split(',');
            writer = new XmlTextWriter(sessionInfo[DIRECTORY_DATA_POS] + "/itrace_msvs-" + sessionInfo[SESSION_TIMESTAMP_POS] + ".xml", System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("itrace_plugin");
            writer.WriteAttributeString("session_id", sessionInfo[SESSION_ID_POS]);
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

        private void WriteResponseData()
        {
            writer.WriteStartElement("response");
            writer.WriteAttributeString("event_id", "");
            writer.WriteAttributeString("plugin_time", "");
            writer.WriteAttributeString("x", "");
            writer.WriteAttributeString("y", "");
            writer.WriteAttributeString("gaze_target", "");
            writer.WriteAttributeString("gaze_target_type", "");
            writer.WriteAttributeString("source_file_path", "");
            writer.WriteAttributeString("source_file_line", "");
            writer.WriteAttributeString("source_file_col", "");
            writer.WriteAttributeString("editor_line_height", "");
            writer.WriteAttributeString("editor_font_height", "");
            writer.WriteAttributeString("editor_line_base_x", "");
            writer.WriteAttributeString("editor_line_base_y", "");
            writer.WriteEndElement();
        }

        public void EndXML()
        {
            writer.WriteEndElement(); // End Gazes
            writer.WriteEndElement(); // End itrace_plugin
        }
    }
}
