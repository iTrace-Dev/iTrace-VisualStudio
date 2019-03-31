using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

namespace iTraceVS
{
    public class SourceWindow
    {
        public String DocPath { get; protected set; }
        public String DocName { get; protected set; }
        public IWpfTextView TextView { get; protected set; }
        public SnapshotPoint? TextBuffer { get; set; }


        public SourceWindow(IVsWindowFrame frame, string DocPath)
        {
            this.DocPath = DocPath;
            DocName = GetNameFromPath(DocPath);
            TextView = GetTextViewFromFrame(frame);
            TextBuffer = null;
        }

        private IWpfTextView GetTextViewFromFrame(IVsWindowFrame frame)
        {
            IVsTextView textView = VsShellUtilities.GetTextView(frame);
            object holder;
            Guid guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
            IVsUserData userData = textView as IVsUserData;
            userData.GetData(ref guidViewHost, out holder);
            IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;
            return viewHost.TextView;
        }

        private String GetNameFromPath(String path)
        {
            string[] temp = path.Split('\\');
            return temp[temp.Length - 1];
        }
    }
}
