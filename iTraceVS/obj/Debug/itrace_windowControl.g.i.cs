﻿#pragma checksum "..\..\itrace_windowControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DDE710338D01DDDBC9D26C4A20253EE8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace iTraceVS {
    
    
    /// <summary>
    /// itrace_windowControl
    /// </summary>
    public partial class itrace_windowControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\itrace_windowControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal iTraceVS.itrace_windowControl MyToolWindow;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\itrace_windowControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button1;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\itrace_windowControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox displayBox;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\itrace_windowControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox highlightBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/iTraceVS;component/itrace_windowcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\itrace_windowControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MyToolWindow = ((iTraceVS.itrace_windowControl)(target));
            return;
            case 2:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\itrace_windowControl.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.attemptConnection);
            
            #line default
            #line hidden
            return;
            case 3:
            this.displayBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 15 "..\..\itrace_windowControl.xaml"
            this.displayBox.Checked += new System.Windows.RoutedEventHandler(this.Reticle_Checked);
            
            #line default
            #line hidden
            
            #line 15 "..\..\itrace_windowControl.xaml"
            this.displayBox.Unchecked += new System.Windows.RoutedEventHandler(this.Reticle_Unchecked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.highlightBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 16 "..\..\itrace_windowControl.xaml"
            this.highlightBox.Checked += new System.Windows.RoutedEventHandler(this.Highlight_Checked);
            
            #line default
            #line hidden
            
            #line 16 "..\..\itrace_windowControl.xaml"
            this.highlightBox.Unchecked += new System.Windows.RoutedEventHandler(this.Highlight_Unchecked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

