namespace iTraceVS
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for itrace_windowControl.
    /// </summary>
    public partial class itrace_windowControl : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="itrace_windowControl"/> class.
        /// </summary>
        public itrace_windowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]

        public static bool connected = false;

        private void attemptConnection(object sender, RoutedEventArgs e)
        {
            if (!connected) {
                socket_manager.getSocket();
                if (connected)
                    button1.Content = "Disconnect";
            }
            else {
                socket_manager.closeSocket();
                connected = false;
                button1.Content = "Connect to Core";
            } 
        }

        private void Reticle_Checked(object sender, RoutedEventArgs e)
        {
            socket_manager.reticleShow(true);
        }

        private void Reticle_Unchecked(object sender, RoutedEventArgs e)
        {
            socket_manager.reticleShow(false);
        }

        private void Highlight_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Highlight_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}