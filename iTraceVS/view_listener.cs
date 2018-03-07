using System.ComponentModel.Composition;
using System.Collections.Generic;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE;

//Listener for new code windows
[Export(typeof(IVsTextViewCreationListener))]
//Specify types of windows to listen for, or set to any
[Microsoft.VisualStudio.Utilities.ContentType("any")]
/*
[Microsoft.VisualStudio.Utilities.ContentType("c/c++")]
[Microsoft.VisualStudio.Utilities.ContentType("text")]
[Microsoft.VisualStudio.Utilities.ContentType("csharp")]
*/
[Microsoft.VisualStudio.Text.Editor.TextViewRole(Microsoft.VisualStudio.Text.Editor.PredefinedTextViewRoles.Editable)]

class CreationListener : IVsTextViewCreationListener
{
    //currently: list of all files that have been opened, not neccessarily currently open
    public List<string> openFiles = new List<string>();

    //The callback function for a new window being created
    public void VsTextViewCreated(IVsTextView textView)
    {
        DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
        Documents docs = dte.Documents;

        int count = docs.Count;
        for (int i = 1; i <= count; ++i)
        {
            if (!openFiles.Contains(docs.Item(i).FullName))
            {
                openFiles.Add(docs.Item(i).FullName);

                //System.Windows.Forms.MessageBox.Show("File " + openFiles.Count + " added " + docs.Item(i).FullName);
            }
        }
    }
}
