using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;

namespace iTraceVS {

    /// <summary>
    /// Interaction logic for itrace_windowControl.
    /// </summary>
    public partial class itrace_windowControl : UserControl {
        /// <summary>
        /// Initializes a new instance of the <see cref="itrace_windowControl"/> class.
        /// </summary>

        public itrace_windowControl() {
            ThreadHelper.ThrowIfNotOnUIThread();
            DocumentEventHandler deh = new DocumentEventHandler();

            this.InitializeComponent();
            SocketManager.Instance.OnSocketConnect += ButtonConnnectionText;
            System.Diagnostics.Debug.AutoFlush = true;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        public static bool highlighting = false;

        public void attemptConnection(object sender, RoutedEventArgs e) {
            if (!SocketManager.Instance.IsConnected()) {
                SocketManager.Instance.Connect(OptionPageGrid.portNum);
            }
            else {
                highlightBox.IsChecked = false;
                SocketManager.Instance.Disconnect();
            }
        }

        private void Highlight_Checked(object sender, RoutedEventArgs e) {
            highlighting = true;
        }

        private void Highlight_Unchecked(object sender, RoutedEventArgs e) {
            highlighting = false;
        }

        private void ButtonConnnectionText(object sender, long connected) {
            if (connected == 0) {
                button1.Content = "Connect to Core";
            }
            else {
                button1.Content = "Disconnect";
            }
        }
    }
}