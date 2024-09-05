/*
This is a slightly modified version of https://gist.github.com/eppz/1ebbc1cf6a77741f56d63d3803e57ba3
This file takes care of creating a plist file for iOS when the code is being compiled
through unity for an iOS build. This plist file has the necessary permissions for microphone
checked, which is requeried for the speech api to work.
*/
#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class BuildPostProcessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.iOS)
        {
            // Read.
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            string targetName = PBXProject.GetUnityTargetName();
            string targetGUID = project.TargetGuidByName(targetName);

            AddFrameworks(project, targetGUID);

            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetString("NSSpeechRecognitionUsageDescription", "This app needs access to Speech Recognition");
            plist.WriteToFile(plistPath);
            plist.root.SetString("NSMicrophoneUsageDescription", "This app needs access to Microphone for Speech Recognition");
            plist.WriteToFile(plistPath);
            plist.root.SetString("UIFileSharingEnabled", "This app needs access to file to transfer files between the device and the application");
            plist.WriteToFile(plistPath);
            plist.root.SetString("LSSupportsOpeningDocumentsInPlace", "This app needs access to file to transfer files between the device and the application");
            plist.WriteToFile(plistPath);
            // Write.
            File.WriteAllText(projectPath, project.WriteToString());
        }
    }

    static void AddFrameworks(PBXProject project, string targetGUID)
    {
        project.AddFrameworkToProject(targetGUID, "Speech.framework", false);
        //This project appears to be a default now:
        //project.AddFrameworkToProject(targetGUID, "AVFoundation.framework", false);
        // Add `-ObjC` to "Other Linker Flags".
        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
    }
}
#endif

