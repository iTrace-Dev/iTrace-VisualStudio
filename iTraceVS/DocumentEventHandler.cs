using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace iTraceVS
{
    internal class DocumentEventHandler : IVsRunningDocTableEvents3
    {
        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            System.Diagnostics.Debug.WriteLine("OnAfterFirstDocumentLock");
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            System.Diagnostics.Debug.WriteLine("OnBeforeLastDocumentUnlock");
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            System.Diagnostics.Debug.WriteLine("OnAfterSave");
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            System.Diagnostics.Debug.WriteLine("OnAfterAttributeChange");
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            // VISIBLE WINDOW!
            System.Diagnostics.Debug.WriteLine("OnBeforeDocumentWindowShow");
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            System.Diagnostics.Debug.WriteLine("OnAfterDocumentWindowHide");
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            System.Diagnostics.Debug.WriteLine("OnAfterAttributeChangeEx");
            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            System.Diagnostics.Debug.WriteLine("OnBeforeSave");
            return VSConstants.S_OK;
        }
    }
}
