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

        public void EndXML()
        {
            writer.WriteEndElement(); // End Gazes
            writer.WriteEndElement(); // End itrace_plugin
        }
    }
}
