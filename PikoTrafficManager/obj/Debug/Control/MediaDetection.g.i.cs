﻿#pragma checksum "..\..\..\Control\MediaDetection.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E33867F3CA3CF8AFB6043228BC00DB1B86F18200"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PikoTrafficManager.Control;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace PikoTrafficManager.Control {
    
    
    /// <summary>
    /// MediaDetection
    /// </summary>
    public partial class MediaDetection : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\Control\MediaDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PikoTrafficManager.Control.MediaDetection MediaDetectionForm;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\Control\MediaDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MediaDetectionLayout;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Control\MediaDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer svNewFiles;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Control\MediaDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel spNewFilesInVolumes;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Control\MediaDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btAddSelected;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Control\MediaDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/PikoTrafficManager;component/control/mediadetection.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Control\MediaDetection.xaml"
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
            this.MediaDetectionForm = ((PikoTrafficManager.Control.MediaDetection)(target));
            
            #line 9 "..\..\..\Control\MediaDetection.xaml"
            this.MediaDetectionForm.Loaded += new System.Windows.RoutedEventHandler(this.MediaDetectionForm_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MediaDetectionLayout = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.svNewFiles = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 4:
            this.spNewFilesInVolumes = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.btAddSelected = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Control\MediaDetection.xaml"
            this.btAddSelected.Click += new System.Windows.RoutedEventHandler(this.btAddSelected_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btCancel = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\Control\MediaDetection.xaml"
            this.btCancel.Click += new System.Windows.RoutedEventHandler(this.btCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

