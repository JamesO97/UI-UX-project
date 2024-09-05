Directories:
AndroidPluginProject: the Android Studio project folder for all Android plugins
   androidfilebrowser: module for the File Browser plugin
      src/main/java/com/fiu/androidfilebrowser: main source files for the file browser plugin
   androidvoicerecognition: module for the Voice Recognition plugin
      src/main: working directory for this plugin
         assets/snowboy: Universal Voice Model files necessary for Snowboy's hotword recognition
         java: all source files
               com/fiu/androidvoicerecognition: main source files for the android voice recognition plugin
               ai/kitt/snowboy: source files for the 3rd party hotword library, Snowboy
         jniLibs: precompiled shared libraries used by Snowboy
Voice Controlled VR App: The main directory that contains the Unity code.
   Assets: The Unity files.
      .meta files: Necessary files for project to execute in Unity
      Editor: Contains files used for giving instructions to Unity after compilation. It is used mainly to grant certain permissions on iOS via alteration of “.plist” file
      Icons: Contains the icons used in the project.
      Materials: Contains material files for the look and feel of the app.
      Plugins: Contains the iOS, Android and webGL plugins for various functionalities such as voice recognition and file management.
      Prefabs: Contains prefab files used to combine objects in Unity.
      Resources: Contains the Progress bar image, and files for the Video settings.
      Scenes: The Scenes of the project
      Scripts: Contains the code for the unity project.
         AndroidVoiceControl.cs: Opens File Browser on Android.
         BrowseFiles.cs: Open File Browser on Windows and Mac.
         CameraController.cs: Operates the camera, turn in any direction on Desktop/Mobile.
         CameraControllerManager.cs: Used for the camera on Android devices.
         CompassControl.cs: Used to control the horizontal indicator (compass) on the UI.
         HideUI.cs: (Note: Contains unfixed bug) Hides the UI. Should have been attached to VideoManager.
         HorizontalSandwichScaler.cs: Adjusts dynamically the size of an UI object in between 2 other UI gameobjects.
         InterpretCommand.cs: Receives result from different voice controllers and translates them into video player actions.
         IOSVoiceControl.cs: Communicates with iOS plugin to handle iOS voice recognition.
         LoadVideoURL.cs: Loads the video when selected from file browser.
         MobileTouchCameraController.cs: Used for Android devices. Improved camera operation.
         PlayerSettings.cs: Contains the settings to change the video type.
         ResolutionChangeDetector.cs: Used to detect the screen resolution of the device.
         TEST_DynamicScaler.cs: Prototype of a dynamic scaling script for the UI.
         ToggleMenuStrip: Opens and closes the menu on the application.
         VerticalIndicatorControl.cs: Used to control the vertical indicator on the UI.
         VideoManager.cs: Operates the playback controls.
         VideoProgress.cs: Operates the scrubber at the bottom, similar to YouTube.
         VoiceControlManager.cs: Used to start the different per platform voice recognition controllers.
         VRToggle.cs: Used to toggle VR mode on Android.
         WebGLVoiceControl.cs: Communicates with webGL plugin to handle webGL voice recognition.
         WindowsVoiceControl.cs: Handles windows voice recognition.
      Video: Contains a test video used to confirm functionality of the app.
      .DS_Store: file necessary to operate on Apple Macbooks
   Packages: Contains manifest.json
   ProjectSettings: Contains the asset files for VCVR.
   .DS_Store: file necessary to operate on Apple Macbooks
