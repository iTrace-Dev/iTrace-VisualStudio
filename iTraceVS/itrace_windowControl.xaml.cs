/********************************************************************************************************************************************************
* @file itrace_windowCommand.xaml.cs
*
* @Copyright (C) 2022 i-trace.org
*
* This file is part of iTrace Infrastructure http://www.i-trace.org/.
* iTrace Infrastructure is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
* iTrace Infrastructure is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
* You should have received a copy of the GNU General Public License along with iTrace Infrastructure. If not, see <https://www.gnu.org/licenses/>.
********************************************************************************************************************************************************/

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
        public itrace_windowControl() {
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
                socket_manager.port = OptionPageGrid.portNum;
                socket_manager.getSocket();
                if (connected)
                    button1.Content = "Disconnect";
            }
            else {
                //highlightBox.IsChecked = false;
                socket_manager.closeSocket();
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