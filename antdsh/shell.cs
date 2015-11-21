//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using antdlib;
using System;
using System.IO;
using System.Linq;
using antdlib.Common;
using antdlib.MountPoint;
using antdlib.Terminal;
using static System.Console;

namespace antdsh {
    public class Shell {
        public static void Start() {
            if (antdlib.Antdsh.Execute.IsAntdRunning()) {
                WriteLine("Cannot start antd becaust it's already running!");
            }
            else {
                WriteLine("Antd is not running, so we can start it.");
                WriteLine("Looking for antds in {0}", Folder.AntdVersionsDir);
                var newestVersionFound = antdlib.Antdsh.Execute.GetNewestVersion();
                if (newestVersionFound.Key != null) {
                    antdlib.Antdsh.Execute.LinkVersionToRunning(newestVersionFound.Key);
                    WriteLine($"New antd '{newestVersionFound.Key}' linked to running version");
                    WriteLine("Restarting services now...");
                    antdlib.Antdsh.Execute.RestartSystemctlAntdServices();
                    if (antdlib.Antdsh.Execute.IsAntdRunning()) {
                        WriteLine("Antd is running now!");
                    }
                    else {
                        WriteLine("Something went wrong starting antd... retrying starting it...");
                        StartLoop(newestVersionFound.Key);
                    }
                }
                else {
                    WriteLine("There's no antd on this machine, you can try use update-url command to dowload the latest version...");
                }
            }
        }

        private static int _startCount;
        private static void StartLoop(string versionToRun) {
            while (true) {
                _startCount++;
                WriteLine($"Retry #{_startCount}");
                if (_startCount < 5) {
                    antdlib.Antdsh.Execute.LinkVersionToRunning(versionToRun);
                    WriteLine($"New antd '{versionToRun}' linked to running version");
                    WriteLine("Restarting services now...");
                    antdlib.Antdsh.Execute.RestartSystemctlAntdServices();
                    if (antdlib.Antdsh.Execute.IsAntdRunning()) {
                        WriteLine("Antd is running now!");
                    }
                    else {
                        WriteLine("Something went wrong starting antd... retrying starting it...");
                        continue;
                    }
                }
                else {
                    WriteLine("Error: too many retries...");
                }
                break;
            }
        }

        public static void Stop() {
            WriteLine("Checking whether antd is running or not...");
            if (antdlib.Antdsh.Execute.IsAntdRunning() == false) {
                WriteLine("Cannot stop antd becaust it isn't running!");
            }
            else {
                WriteLine("Removing everything and stopping antd.");
                antdlib.Antdsh.Execute.StopServices();
                UmountAll();
                if (antdlib.Antdsh.Execute.IsAntdRunning() == false) {
                    WriteLine("Antd has been stopped now!");
                }
                else {
                    WriteLine("Something went wrong starting antd... retrying starting it...");
                    StopLoop();
                }
            }
        }

        private static int _stopCount;
        private static void StopLoop() {
            while (true) {
                _stopCount++;
                WriteLine($"Retry #{_stopCount}");
                if (_stopCount < 5) {
                    WriteLine("Removing everything and stopping antd.");
                    antdlib.Antdsh.Execute.StopServices();
                    UmountAll();
                    if (antdlib.Antdsh.Execute.IsAntdRunning() == false) {
                        WriteLine("Antd has been stopped now!");
                    }
                    else {
                        WriteLine("Something went wrong stopping antd... retrying stopping it...");
                        continue;
                    }
                }
                else {
                    WriteLine("Error: too many retries...");
                }
                break;
            }
        }

        public static void Restart() {
            WriteLine("Checking whether antd is running or not...");
            if (antdlib.Antdsh.Execute.IsAntdRunning() == false) {
                WriteLine("Cannot restart antd because it isn't running! Try the 'start' command instead!");
            }
            else {
                Stop();
                Start();
            }
        }

        public static void Status() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            if (res.Length == 0) {
                WriteLine("No antd process found.");
                WriteLine(Terminal.Execute($"systemctl status {Units.Name.NameLauncher}"));
            }
            else {
                WriteLine(res);
                WriteLine(Terminal.Execute($"systemctl status {Units.Name.NameLauncher}"));
            }
        }

        public static void UmountAll() {
            WriteLine("Unmounting all antd-related directories...");
            antdlib.Antdsh.Execute.UmountAntd();
        }

        public static void UpdateCheck() {
            var linkedVersionName = antdlib.Antdsh.Execute.GetRunningVersion();
            if (linkedVersionName == null)
                return;
            var linkedVersion = antdlib.Antdsh.Execute.SetVersionKeyValuePair(linkedVersionName);
            var newestVersionFound = antdlib.Antdsh.Execute.GetNewestVersion();
            if (linkedVersion.Key == null || newestVersionFound.Key == null)
                return;
            WriteLine($"You are running {linkedVersion.Value} and the latest version is {newestVersionFound.Value}.");
            var linkedDate = Convert.ToInt32(linkedVersion.Value);
            var newestDate = Convert.ToInt32(newestVersionFound.Value);
            if (linkedVersion.Value == newestVersionFound.Value) {
                WriteLine("Antd is up to date!");
                return;
            }
            if (newestDate > linkedDate) {
                WriteLine($"New version of antd found!! -> {newestDate}");
                return;
            }
            WriteLine("There's nothing to update.");
        }

        public static void UpdateLaunch() {
            var linkedVersionName = antdlib.Antdsh.Execute.GetRunningVersion();
            if (linkedVersionName == null)
                return;
            var linkedVersion = antdlib.Antdsh.Execute.SetVersionKeyValuePair(linkedVersionName);
            var newestVersionFound = antdlib.Antdsh.Execute.GetNewestVersion();
            if (linkedVersion.Key == null || newestVersionFound.Key == null)
                return;
            WriteLine($"You are running {linkedVersion.Value} and the latest version is {newestVersionFound.Value}.");
            var linkedDate = Convert.ToInt32(linkedVersion.Value);
            var newestDate = Convert.ToInt32(newestVersionFound.Value);
            if (linkedVersion.Value == newestVersionFound.Value) {
                WriteLine("Antd is already up to date!");
                return;
            }
            if (newestDate > linkedDate) {
                WriteLine("New version of antd found!! -> {0}", newestDate);
                WriteLine("Updating!");
                antdlib.Antdsh.Execute.StopServices();
                antdlib.Antdsh.Execute.CleanTmp();
                if (newestVersionFound.Key.Contains(AntdFile.SquashEndsWith)) {
                    antdlib.Antdsh.Execute.RemoveLink();
                    antdlib.Antdsh.Execute.LinkVersionToRunning(newestVersionFound.Key);
                }
                else if (newestVersionFound.Key.Contains(AntdFile.ZipEndsWith)) {
                    var squashName = Folder.AntdVersionsDir + "/" + AntdFile.SquashStartsWith + newestVersionFound.Value + AntdFile.SquashEndsWith;
                    antdlib.Antdsh.Execute.MountTmpRam();
                    antdlib.Antdsh.Execute.CopyToTmp(newestVersionFound.Key);
                    antdlib.Antdsh.Execute.ExtractZipTmp(newestVersionFound.Key);
                    antdlib.Antdsh.Execute.RemoveTmpZips();
                    antdlib.Antdsh.Execute.CreateSquash(squashName);
                    antdlib.Antdsh.Execute.CleanTmp();
                    antdlib.Antdsh.Execute.UmountTmpRam();
                    antdlib.Antdsh.Execute.RemoveLink();
                    antdlib.Antdsh.Execute.LinkVersionToRunning(squashName);
                }
                else {
                    WriteLine("Update failed unexpectedly");
                    return;
                }
                antdlib.Antdsh.Execute.RestartSystemctlAntdServices();
                return;
            }
            WriteLine("There's nothing to update.");
        }

        public static void UpdateFromUrl() {
            antdlib.Antdsh.Execute.StopServices();
            antdlib.Antdsh.Execute.CleanTmp();
            var squashName = $"{Folder.AntdVersionsDir}/{AntdFile.SquashStartsWith}{DateTime.Now.ToString("yyyyMMdd")}{AntdFile.SquashEndsWith}";
            antdlib.Antdsh.Execute.MountTmpRam();
            antdlib.Antdsh.Execute.DownloadFromUrl("https://github.com/Anthilla/Antd/archive/master.zip");
            antdlib.Antdsh.Execute.ExtractDownloadedFile();
            antdlib.Antdsh.Execute.RemoveTmpZips();
            antdlib.Antdsh.Execute.PickAndMoveZipFileInDownloadedDirectory();
            antdlib.Antdsh.Execute.RemoveDownloadedFile();
            antdlib.Antdsh.Execute.ExtractPickedZip();
            antdlib.Antdsh.Execute.RemoveTmpZips();
            antdlib.Antdsh.Execute.CreateSquash(squashName);
            antdlib.Antdsh.Execute.CleanTmp();
            antdlib.Antdsh.Execute.UmountTmpRam();
            antdlib.Antdsh.Execute.RemoveLink();
            antdlib.Antdsh.Execute.LinkVersionToRunning(squashName);
            antdlib.Antdsh.Execute.RestartSystemctlAntdServices();
        }

        public static void UpdateFromPublicRepo() {
            WriteLine("Update From Public Repo ...");
            WriteLine("   Stopping services");
            antdlib.Antdsh.Execute.StopServices();
            WriteLine("   Cleaning directories and mounts");

            Terminal.Execute("umount /framework/antd");
            if (Mount.IsAlreadyMounted("/framework/antd")) {
                Mount.Umount("/framework/antd");
            }

            antdlib.Antdsh.Execute.CleanTmp();
            WriteLine("   Mounting tmp ram");
            antdlib.Antdsh.Execute.MountTmpRam();
            var antdRepoUrl = $"{Update.RemoteRepo}/{Update.RemoteAntdDir}";
            var updateFileUrl = $"{antdRepoUrl}/{Update.RemoteUpdateInfo}";
            var updateFile = $"{Folder.AntdTmpDir}/{Update.RemoteUpdateInfo}";
            WriteLine($"   Downloading from: {updateFileUrl}");
            WriteLine($"                 to: {updateFile}");
            antdlib.Antdsh.Execute.DownloadFromUrl(updateFileUrl, updateFile);
            if (!File.Exists(updateFile)) {
                WriteLine("   Download failed!");
                return;
            }
            WriteLine("   Download complete!");
            var updateText = FileSystem.ReadFile(updateFile);
            var squashName = updateText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            WriteLine($"   Version found: {squashName}");
            var squashUrl = $"{antdRepoUrl}/{squashName}";
            var squashFile = $"{Folder.AntdVersionsDir}/{squashName}";
            WriteLine($"   Downloading from: {squashUrl}");
            WriteLine($"                 to: {squashFile}");
            antdlib.Antdsh.Execute.DownloadFromUrl(squashUrl, squashFile);
            if (!File.Exists(squashFile)) {
                WriteLine("   Download failed!");
                return;
            }
            WriteLine("   Download complete!");
            antdlib.Antdsh.Execute.RemoveLink();
            antdlib.Antdsh.Execute.LinkVersionToRunning(squashName);
            antdlib.Antdsh.Execute.CleanTmp();
            antdlib.Antdsh.Execute.UmountTmpRam();

            Terminal.Execute("umount /framework/antd");
            if (Mount.IsAlreadyMounted("/framework/antd")) {
                Mount.Umount("/framework/antd");
            }

            antdlib.Antdsh.Execute.RestartSystemctlAntdServices();
            WriteLine("   Update complete!");
        }

        public static void UpdateSelect() {
            var linkedVersionName = antdlib.Antdsh.Execute.GetRunningVersion();
            if (linkedVersionName == null)
                return;
            var linkedVersion = antdlib.Antdsh.Execute.SetVersionKeyValuePair(linkedVersionName);
            WriteLine("Select a version (from its number) from this list:");
            antdlib.Antdsh.Execute.PrintVersions();
            var number = ReadLine();
            var selectedVersion = antdlib.Antdsh.Execute.GetVersionByNumber(number);
            if (linkedVersion.Key == null || selectedVersion.Key == null)
                return;
            WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, selectedVersion.Value);
            var selectedtDate = Convert.ToInt32(selectedVersion.Value);
            WriteLine("New version of antd found!! -> {0}", selectedtDate);
            WriteLine("Updating!");
            antdlib.Antdsh.Execute.StopServices();
            antdlib.Antdsh.Execute.CleanTmp();
            if (selectedVersion.Key.Contains(AntdFile.SquashEndsWith)) {
                antdlib.Antdsh.Execute.RemoveLink();
                antdlib.Antdsh.Execute.LinkVersionToRunning(selectedVersion.Key);
            }
            else if (selectedVersion.Key.Contains(AntdFile.ZipEndsWith)) {
                var squashName = Folder.AntdVersionsDir + "/" + AntdFile.SquashStartsWith + selectedVersion.Value + AntdFile.SquashEndsWith;
                antdlib.Antdsh.Execute.MountTmpRam();
                antdlib.Antdsh.Execute.CopyToTmp(selectedVersion.Key);
                antdlib.Antdsh.Execute.ExtractZipTmp(selectedVersion.Key);
                antdlib.Antdsh.Execute.RemoveTmpZips();
                antdlib.Antdsh.Execute.CreateSquash(squashName);
                antdlib.Antdsh.Execute.CleanTmp();
                antdlib.Antdsh.Execute.UmountTmpRam();
                antdlib.Antdsh.Execute.RemoveLink();
                antdlib.Antdsh.Execute.LinkVersionToRunning(squashName);
            }
            else {
                WriteLine("Update failed unexpectedly");
                return;
            }
            antdlib.Antdsh.Execute.RestartSystemctlAntdServices();
        }

        public static void ReloadSystemctl() {
            Terminal.Execute("systemctl daemon-reload");
        }

        public static void IsRunning() {
            WriteLine(antdlib.Antdsh.Execute.IsAntdRunning() ? "Yes, is running." : "No.");
        }

        public static void CleanTmp() {
            WriteLine("Cleaning tmp.");
            antdlib.Antdsh.Execute.CleanTmp();
        }

        public static void Info() {
            WriteLine("This is a shell for antd :)");
        }

        public static void Exit() {
            Environment.Exit(1);
        }

        public static void Execute(string command) {
            WriteLine($"Executing external command: {command}");
            WriteLine(Terminal.Execute(command));
        }
    }
}
