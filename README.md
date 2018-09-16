iTrace Visual Studio Plugin
===========================

Eye tracking in Visual Studio using the iTrace Engine

This extension allows you to know what code file, the line / column position, and the toke that was looked at during an eye tracking session.

## Building

1. Clone or download this repository and open the .sln file in Visual Studio 2017. It may or may not work in other versions of Visual Studio. 
1. Once Visual Studio opens, you may be prompted to install a few Visual Studio extension development tools. Accept the installs. 
1. Once the solution is loaded, press build and run the app. You may want to configure the build to be in release mode for increased optimization.
1. A new instance of Visual Studio will pop up in which the iTrace Visual Studio extension is available for use. 

## Usage

To get eye tracking data, this plugin uses the iTrace-Core. You must have it before you can use the extension. It is available here: https://github.com/iTrace-Dev/iTrace-Core

1. Load up a project to run eye tracking on. 
1. You should see an iTrace window pane with controls. If you do not, open it up by going to View > Other Windows > itrace_window
1. When you are ready, click on Connect to Core. **After** that, open up the iTrace Core app, configure it, and then click Start Tracker. This way, the plugin can recieve from the core where to output the data.  
1. Now eye tracking data is being recorded. To stop it, press Disconnect. To enable a live reticle that shows where you are looking, enable Display Reticle. 
