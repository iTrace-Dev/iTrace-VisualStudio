﻿namespace iTraceVS
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for iTraceWindowControl.
    /// </summary>
    public partial class iTraceWindowControl : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="iTraceWindowControl"/> class.
        /// </summary>
        public iTraceWindowControl() {
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
        public static bool highlighting = false;

        private void attemptConnection(object sender, RoutedEventArgs e) {
            if (!connected) {
                SocketManager.port = OptionPageGrid.portNum;
                SocketManager.GetSocket();
                if (connected)
                    button1.Content = "Disconnect";
            }
            else {
                //highlightBox.IsChecked = false;
                SocketManager.CloseSocket();
                connected = false;
                button1.Content = "Connect to Core";
            } 
        }

        //private void Highlight_Checked(object sender, RoutedEventArgs e) {
        //    highlighting = true;
        //}

        //private void Highlight_Unchecked(object sender, RoutedEventArgs e) {
        //    highlighting = false;
        //}
    }
}