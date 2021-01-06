﻿using System;
using System.Diagnostics;
using System.Text;

namespace AndroidSideloader
{
    class RCLONE
    {
        public static void killRclone()
        {
            foreach (var process in Process.GetProcessesByName("rclone"))
                process.Kill();
        }

        public static string rclonepw = "0aE0$D61#avO";

        private static Process rclone = new Process();

        public static ProcessOutput runRcloneCommand(string command, string bandwithLimit = "")
        {
            Environment.SetEnvironmentVariable("RCLONE_CRYPT_REMOTE", rclonepw);
            Environment.SetEnvironmentVariable("RCLONE_CONFIG_PASS", rclonepw);

            rclone.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            if (bandwithLimit.Length>0)
            {
                command += $" --bwlimit={bandwithLimit}";
            }

            if (rclonepw.Length > 0)
                command += " --ask-password=false";

            Logger.Log($"Running Rclone command: {command}");

            rclone.StartInfo.FileName = Environment.CurrentDirectory + "\\rclone\\rclone.exe";
            rclone.StartInfo.Arguments = command;
            rclone.StartInfo.RedirectStandardInput = true;
            rclone.StartInfo.RedirectStandardError = true;
            rclone.StartInfo.RedirectStandardOutput = true;
            rclone.StartInfo.WorkingDirectory = Environment.CurrentDirectory + "\\rclone";
            rclone.StartInfo.CreateNoWindow = true;
            if (Form1.debugMode == true)
                rclone.StartInfo.CreateNoWindow = false;
            rclone.StartInfo.UseShellExecute = false;
            rclone.Start();

            rclone.StandardInput.WriteLine(command);
            rclone.StandardInput.Flush();
            rclone.StandardInput.Close();

            string output = rclone.StandardOutput.ReadToEnd();
            string error = rclone.StandardError.ReadToEnd();
            rclone.WaitForExit();
            Logger.Log($"Rclone error: {error} {error.Length}\nRclone Output: {output}");
            return new ProcessOutput(output,error);
        }
    }
}
