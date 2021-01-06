﻿using System;
using System.Text;
using System.Diagnostics;
using JR.Utils.GUI.Forms;
using System.Net;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using AndroidSideloader;

namespace AndroidSideloader
{
    class Updater
    {

        public static string AppName { get; set; }
        public static string Repostory { get; set; }
        private static string RawGitHubUrl;
        private static string GitHubUrl;

        static readonly public string LocalVersion = "1.17SU1";
        public static string currentVersion = string.Empty;
        public static string changelog = string.Empty;

        private static bool IsUpdateAvailable()
        {
            HttpClient client = new HttpClient();
            try
            {
                currentVersion = client.GetStringAsync($"{RawGitHubUrl}/master/version").Result;
                currentVersion = currentVersion.Remove(currentVersion.Length - 1);
                changelog = client.GetStringAsync($"{RawGitHubUrl}/master/changelog.txt").Result;
            }
            catch { return false; }
            return LocalVersion != currentVersion;
        }
        public static void Update()
        {
            RawGitHubUrl = $"https://raw.githubusercontent.com/{Repostory}";
            GitHubUrl = $"https://github.com/{Repostory}";
            if (IsUpdateAvailable())
                doUpdate();
        }
        private static void doUpdate()
        {
            DialogResult dialogResult = FlexibleMessageBox.Show($"There is a new update you have version {LocalVersion}, do you want to update?\nCHANGELOG\n{changelog}", $"Version {currentVersion} is available", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes)
                return;

            try
            {
                using (var fileClient = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    Logger.Log($"Downloading update from {RawGitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe to {AppName} v{currentVersion}.exe");
                    fileClient.DownloadFile($"{GitHubUrl}/releases/download/v{currentVersion}/{AppName}.exe", $"{AppName} v{currentVersion}.exe");
                }

                AndroidSideloader.Utilities.GeneralUtilities.Melt();
                Logger.Log($"Starting {AppName} v{currentVersion}.exe");
                Process.Start($"{AppName} v{currentVersion}.exe");
            }
            catch { }

            Environment.Exit(0);
        }
    }
}
