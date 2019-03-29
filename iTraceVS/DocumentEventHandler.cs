using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace iTraceVS
{
    public partial class DocumentEventHandler : IVsRunningDocTableEvents3
    {
        // RDT
        uint rdtCookie;
        RunningDocumentTable rdt;

        public DocumentEventHandler()
        {
            // Advise the RDT of this event sink.
            IOleServiceProvider sp =
                Package.GetGlobalService(typeof(IOleServiceProvider)) as IOleServiceProvider;
            if (sp == null) return;

            rdt = new RunningDocumentTable(new ServiceProvider(sp));
            if (rdt == null) return;

            rdtCookie = rdt.Advise(this);
        }
        
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
            CoreDataHandler.Instance.SetActiveSourceWindow(new SourceWindow(pFrame, rdt.GetDocumentInfo(docCookie).Moniker));
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
