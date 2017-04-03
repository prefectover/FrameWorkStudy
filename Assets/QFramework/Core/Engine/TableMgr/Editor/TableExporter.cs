using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace QFramework
{
    public class TableExporter
    {
        private static bool IsLinuxSystem()
        {
            PlatformID platformID = System.Environment.OSVersion.Platform;

            if (platformID == PlatformID.MacOSX || platformID == PlatformID.Unix)
            {
                return true;
            }

            return false;
        }

        [MenuItem("Assets/SCEngine/Table/Build C#")]
        public static void BuildCSharpFile()
        {
			string path = ToolPathConfig.ProjectToolsFolderPath;
            if (IsLinuxSystem())
            {
				path += ToolPathConfig.BUILD_CSHARP_LINUX_SHELL;
            }
            else
            {
				path += ToolPathConfig.BUILD_CSHARP_WIN_SHELL;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        [MenuItem("Assets/SCEngine/Table/Build Data(txt)")]
        public static void BuildDataTxtMode()
        {
			string path = ToolPathConfig.ProjectToolsFolderPath;
            if (IsLinuxSystem())
            {
				path += ToolPathConfig.BUILD_TXT_DATA_LINUX_SHELL;
            }
            else
            {
				path += ToolPathConfig.BUILD_TXT_DATA_WIN_SHELL;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        [MenuItem("Assets/SCEngine/Table/Build Data(lrg)")]
        public static void BuildDataLrgMode()
        {
			string path = ToolPathConfig.ProjectToolsFolderPath;
            if (IsLinuxSystem())
            {
				path += ToolPathConfig.BUILD_LRG_DATA_LINUX_SHELL;
            }
            else
            {
				path += ToolPathConfig.BUILD_LRG_DATA_WIN_SHELL;
            }

            Thread newThread = new Thread(new ThreadStart(() =>
            {
                BuildCSharpThreadStart(path);
            }));
            newThread.Start();
        }

        private static void BuildCSharpThreadStart(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
