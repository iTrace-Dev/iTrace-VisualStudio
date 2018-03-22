using System;
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

        public static void writeResponse(Int64 sessionTime, double x, double y) {
            writer.WriteStartElement("responses");
           
            writer.WriteAttributeString("x", Convert.ToString(x));
            writer.WriteAttributeString("y", Convert.ToString(y));

            writer.WriteAttributeString("session-time", Convert.ToString(sessionTime));
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
