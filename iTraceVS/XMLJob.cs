using System;

namespace iTraceVS {

    /* POD Class to act as a struct for the data fields */
    public class XMLJob {
        public readonly static int SESSION_START = 1;
        public readonly static int GAZE_DATA = 2;
        public readonly static int SESSION_END = 3;

        // Sent by Core
        public int? JobType { get; protected set; } = null;
        public long JobID { get; protected set; }

        // Gaze
        public double? EyeX { get; protected set; } = null;
        public double? EyeY { get; protected set; } = null;
        public long? EventID { get; protected set; } = null;

        // Session Start
        public long? SessionID { get; protected set; } = null;
        public long? SessionTimeStamp { get; protected set; } = null;
        public string OutputRootDir { get; protected set; } = null;

        // Calculated By Plugin
        public long? PluginTime { get; set; } = null;
        public string GazeTarget { get; set; } = null;
        public string GazeTargetType { get; set; } = null;
        public string SourceFilePath { get; set; } = null;
        public int? SourceFileLine { get; set; } = null;
        public int? SourceFileCol { get; set; } = null;
        public double? EditorLineHeight { get; set; } = null;
        public double? EditorFontHeight { get; set; } = null;
        public int? EditorLineBaseX { get; set; } = null;
        public int? EditorLineBaseY { get; set; } = null;

        public XMLJob(string data, long id) {
            string[] data_string = data.Split(',');

            JobID = id;

            if (data_string[0] == "gaze")
            {
                //gaze,[event_id],[x],[y]
                JobType = GAZE_DATA;
                EventID = Convert.ToInt64(data_string[1]);
                EyeX = Convert.ToDouble(data_string[2]);
                EyeY = Convert.ToDouble(data_string[3]);

            }
            else if (data_string[0] == "session_start")
            {
                //session_start,[session_id],[session_timestamp],[output_root_directory]
                JobType = SESSION_START;
                SessionID = Convert.ToInt64(data_string[1]);
                SessionTimeStamp = Convert.ToInt64(data_string[2]);
                OutputRootDir = data_string[3];
            }
            else
            {
                JobType = SESSION_END;
            }            
        }
    }
}
