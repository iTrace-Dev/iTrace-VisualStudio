using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;

namespace iTraceVS {

    /// <summary>
    /// Interaction logic for itrace_windowControl.
    /// </summary>
    public partial class itrace_windowControl : UserControl {
        DispatcherTimer connectionTextRefreshTimer;
        /// <summary>
        /// Initializes a new instance of the <see cref="itrace_windowControl"/> class.
        /// </summary>
        public itrace_windowControl() {
            this.InitializeComponent();

            connectionTextRefreshTimer = new DispatcherTimer();
            connectionTextRefreshTimer.Tick += connectionTextRefreshTimer_Tick;
            connectionTextRefreshTimer.Interval = TimeSpan.FromSeconds(1);
            connectionTextRefreshTimer.Start();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]

        public static bool connected = false;
        public static bool highlighting = false;

        public void attemptConnection(object sender, RoutedEventArgs e) {
            if (!connected) {
                socket_manager.port = OptionPageGrid.portNum;
                socket_manager.getSocket();
                if (connected) {
                    button1.Content = "Disconnect";
                }
            }
            else {
                highlightBox.IsChecked = false;
                socket_manager.closeSocket();
                connected = false;
                button1.Content = "Connect to Core";
            }
        }

        private void Highlight_Checked(object sender, RoutedEventArgs e) {
            highlighting = true;
        }

        private void Highlight_Unchecked(object sender, RoutedEventArgs e) {
            highlighting = false;
        }

        public void connectionTextRefreshTimer_Tick(object sender, object e) {
            //Debug.WriteLine("timer");
            if (!connected) {
                button1.Content = "Connect to Core";
            }
            else {
                button1.Content = "Disconnect";
            }
        }
    }
}