using System.Windows.Forms;
using System.Xml;

namespace iTraceVS
{
    class xml_writer
    {
        static XmlWriter writer;
        static XmlWriterSettings prefs;

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
        }

        public static void writeResponse(string data) {
            writer.WriteStartElement("responses");

            writer.WriteAttributeString("serverData", data);
            //writer.WriteAttributeString("y", "57");
            //writer.WriteAttributeString("timestamp", "57");
            //additional attributes

            writer.WriteEndElement();
            writer.Flush();
        }

        public static void xmlEnd() {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

    }
}
