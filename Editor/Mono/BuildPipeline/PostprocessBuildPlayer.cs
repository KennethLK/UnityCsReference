// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System.Diagnostics;
using System.Linq;
using UnityEditor.Utils;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using UnityEditorInternal;
using System;
using System.Text.RegularExpressions;
using Mono.Cecil;
using UnityEditor.Modules;

namespace UnityEditor
{
    internal class PostprocessBuildPlayer
    {
        internal const string StreamingAssets = "Assets/StreamingAssets";

        ///@TODO: This should be moved to C++ since playerprefs has it there already
        internal static string GenerateBundleIdentifier(string companyName, string productName)
        {
            return "unity" + "." + companyName + "." + productName;
        }

        // Seems to be used only by PlatformDependent\AndroidPlayer\Editor\Managed\PostProcessAndroidPlayer.cs
        internal static bool InstallPluginsByExtension(string pluginSourceFolder, string extension, string debugExtension, string destPluginFolder, bool copyDirectories)
        {
            bool installedPlugins = false;

            if (!Directory.Exists(pluginSourceFolder))
                return installedPlugins;

            string[] contents = Directory.GetFileSystemEntries(pluginSourceFolder);
            foreach (string path in contents)
            {
                string fileName = Path.GetFileName(path);
                string fileExtension = Path.GetExtension(path);

                bool filenameMatch =    fileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase) ||
                    fileName.Equals(extension, StringComparison.OrdinalIgnoreCase);
                bool debugMatch =       debugExtension != null && debugExtension.Length != 0 &&
                    (fileExtension.Equals(debugExtension, StringComparison.OrdinalIgnoreCase) ||
                     fileName.Equals(debugExtension, StringComparison.OrdinalIgnoreCase));

                // Do we really need to check the file name here?
                if (filenameMatch || debugMatch)
                {
                    if (!Directory.Exists(destPluginFolder))
                        Directory.CreateDirectory(destPluginFolder);

                    string targetPath = Path.Combine(destPluginFolder, fileName);
                    if (copyDirectories)
                        FileUtil.CopyDirectoryRecursive(path, targetPath);
                    else if (!Directory.Exists(path))
                        FileUtil.UnityFileCopy(path, targetPath);

                    installedPlugins = true;
                }
            }
            return installedPlugins;
        }

        internal static void InstallStreamingAssets(string stagingAreaDataPath)
        {
            InstallStreamingAssets(stagingAreaDataPath, null);
        }

        internal static void InstallStreamingAssets(string stagingAreaDataPath, BuildReporting.BuildReport report)
        {
            if (Directory.Exists(StreamingAssets))
            {
                const string kFileTypeStreamingAssets = "Streaming Assets";
                var outputPath = Path.Combine(stagingAreaDataPath, "StreamingAssets");
                FileUtil.CopyDirectoryRecursiveForPostprocess(StreamingAssets, outputPath, true);
                if (report != null)
                    report.AddFilesRecursive(outputPath, kFileTypeStreamingAssets);
            }
        }

        static public string PrepareForBuild(BuildOptions options, BuildTargetGroup targetGroup, BuildTarget target)
        {
            var postprocessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (postprocessor == null)
                return null;
            return postprocessor.PrepareForBuild(options, target);
        }

        static public bool SupportsScriptsOnlyBuild(BuildTargetGroup targetGroup, BuildTarget target)
        {
            IBuildPostprocessor postprocessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (postprocessor != null)
            {
                return postprocessor.SupportsScriptsOnlyBuild();
            }

            return false;
        }

        static public string GetExtensionForBuildTarget(BuildTargetGroup targetGroup, BuildTarget target, BuildOptions options)
        {
            IBuildPostprocessor postprocessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (postprocessor == null)
                return string.Empty;
            return postprocessor.GetExtension(target, options);
        }

        static public bool SupportsInstallInBuildFolder(BuildTargetGroup targetGroup, BuildTarget target)
        {
            IBuildPostprocessor postprocessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (postprocessor != null)
            {
                return postprocessor.SupportsInstallInBuildFolder();
            }

            switch (target)
            {
                case BuildTarget.PSP2:
                case BuildTarget.PSM:
                case BuildTarget.Android:
                case BuildTarget.WSAPlayer:
                    return true;
                default:
                    return false;
            }
        }

        static public void Launch(BuildTargetGroup targetGroup, BuildTarget target, string path, string productName, BuildOptions options)
        {
            IBuildPostprocessor postprocessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (postprocessor != null)
            {
                BuildLaunchPlayerArgs args;
                args.target = target;
                args.playerPackage = BuildPipeline.GetPlaybackEngineDirectory(target, options);
                args.installPath = path;
                args.productName = productName;
                args.options = options;
                postprocessor.LaunchPlayer(args);
            }
            else
                throw new UnityException(String.Format("Launching {0} build target via mono is not supported", target));
        }

        static public void Postprocess(BuildTargetGroup targetGroup, BuildTarget target, string installPath, string companyName, string productName,
            int width, int height, string downloadWebplayerUrl, string manualDownloadWebplayerUrl, BuildOptions options,
            RuntimeClassRegistry usedClassRegistry, BuildReporting.BuildReport report)
        {
            string stagingArea = "Temp/StagingArea";
            string stagingAreaData = "Temp/StagingArea/Data";
            string stagingAreaDataManaged = "Temp/StagingArea/Data/Managed";
            string playerPackage = BuildPipeline.GetPlaybackEngineDirectory(target, options);

            // Disallow providing an empty string as the installPath
            bool willInstallInBuildFolder = (options & BuildOptions.InstallInBuildFolder) != 0 && SupportsInstallInBuildFolder(targetGroup, target);
            if (installPath == String.Empty && !willInstallInBuildFolder)
                throw new Exception(installPath + " must not be an empty string");

            IBuildPostprocessor postprocessor = ModuleManager.GetBuildPostProcessor(targetGroup, target);
            if (postprocessor != null)
            {
                BuildPostProcessArgs args;
                args.target = target;
                args.stagingAreaData = stagingAreaData;
                args.stagingArea = stagingArea;
                args.stagingAreaDataManaged = stagingAreaDataManaged;
                args.playerPackage = playerPackage;
                args.installPath = installPath;
                args.companyName = companyName;
                args.productName = productName;
                args.productGUID = PlayerSettings.productGUID;
                args.options = options;
                args.usedClassRegistry = usedClassRegistry;
                args.report = report;
                postprocessor.PostProcess(args);
                return;
            }

            // If postprocessor is not provided, build target is not supported
            throw new UnityException(string.Format("Build target '{0}' not supported", target));
        }

        internal static string ExecuteSystemProcess(string command, string args, string workingdir)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true
            };
            var p = new Program(psi);
            p.Start();
            while (!p.WaitForExit(100))
                ;

            string output = p.GetStandardOutputAsString();
            p.Dispose();
            return output;
        }

        public static string subDir32Bit
        {
            get
            {
                return "x86";
            }
        }

        public static string subDir64Bit
        {
            get
            {
                return "x86_64";
            }
        }

        internal static string GetArchitectureForTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneWindows64:
                    return "x86_64";
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneWindows:
                    return "x86";
                default:
                    return String.Empty;
            }
        }
    }
}
