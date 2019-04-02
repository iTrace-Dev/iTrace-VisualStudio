using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace iTraceVS
{
    public class DocumentEventHandler : IVsRunningDocTableEvents3
    {
        // RDT
        uint rdtCookie;
        RunningDocumentTable rdt;
        string CurrentDocPath = "";

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

        ~ DocumentEventHandler()
        {
            try
            {
                if (rdtCookie != 0) rdt.Unadvise(rdtCookie);
            }
            catch {}
        }
        
        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            // VISIBLE WINDOW!
            RunningDocumentInfo rdi = rdt.GetDocumentInfo(docCookie);

            if (CurrentDocPath != rdi.Moniker)
            {
                System.Diagnostics.Debug.WriteLine(rdi.Moniker);
                CoreDataHandler.Instance.SetActiveSourceWindow(new SourceWindow(pFrame, rdt.GetDocumentInfo(docCookie).Moniker));
                CurrentDocPath = rdi.Moniker;
            }
                
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        { 
            return VSConstants.S_OK;
        }
    }
}
