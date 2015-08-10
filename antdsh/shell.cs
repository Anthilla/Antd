using antdlib;
using System;
using System.Threading;
using static System.Console;
//using static antdsh.execute;

namespace antdsh {
    public class shell {

        /// <summary>
        /// ok
        /// </summary>
        public static void Start() {
            if (execute.IsAntdRunning() == true) {
                WriteLine("Cannot start antd becaust it's already running!");
            }
            else {
                WriteLine("Antd is not running, so we can start it.");
                WriteLine("Looking for antds in {0}", global.versionsDir);
                var newestVersionFound = execute.GetNewestVersion();
                if (newestVersionFound.Key != null) {
                    execute.LinkVersionToRunning(newestVersionFound.Key);
                    WriteLine($"New antd '{newestVersionFound.Key}' linked to running version");
                    WriteLine("Restarting services now...");
                    execute.RestartSystemctlAntdServices();
                    if (execute.IsAntdRunning() == true) {
                        WriteLine("Antd is running now!");
                    }
                    else {
                        WriteLine("Something went wrong starting antd... retrying starting it...");
                        StartLoop(newestVersionFound.Key);
                    }
                }
                else {
                    WriteLine("There's no antd on this machine, you can try use update-url command to dowload the latest version...");
                    return;
                }
            }
        }

        private static int startCount = 0;

        /// <summary>
        /// ok
        /// </summary>
        private static void StartLoop(string versionToRun) {
            startCount++;
            WriteLine($"Retry #{startCount.ToString()}");
            if (startCount < 5) {
                execute.LinkVersionToRunning(versionToRun);
                WriteLine($"New antd '{versionToRun}' linked to running version");
                WriteLine("Restarting services now...");
                execute.RestartSystemctlAntdServices();
                if (execute.IsAntdRunning() == true) {
                    WriteLine("Antd is running now!");
                }
                else {
                    WriteLine("Something went wrong starting antd... retrying starting it...");
                    StartLoop(versionToRun);
                }
            }
            else {
                WriteLine("Error: too many retries...");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Stop() {
            WriteLine("Checking whether antd is running or not...");
            if (execute.IsAntdRunning() == false) {
                WriteLine("Cannot stop antd becaust it isn't running!");
            }
            else {
                WriteLine("Removing everything and stopping antd.");
                execute.StopServices();
                UmountAll();
                if (execute.IsAntdRunning() == false) {
                    WriteLine("Antd has been stopped now!");
                }
                else {
                    WriteLine("Something went wrong starting antd... retrying starting it...");
                    StopLoop();
                }
            }
        }

        private static int stopCount = 0;

        /// <summary>
        /// ok
        /// </summary>
        private static void StopLoop() {
            stopCount++;
            WriteLine($"Retry #{stopCount.ToString()}");
            if (stopCount < 5) {
                WriteLine("Removing everything and stopping antd.");
                execute.StopServices();
                UmountAll();
                if (execute.IsAntdRunning() == false) {
                    WriteLine("Antd has been stopped now!");
                }
                else {
                    WriteLine("Something went wrong stopping antd... retrying stopping it...");
                    StopLoop();
                }
            }
            else {
                WriteLine("Error: too many retries...");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Restart() {
            WriteLine("Checking whether antd is running or not...");
            if (execute.IsAntdRunning() == false) {
                WriteLine("Cannot restart antd because it isn't running! Try the 'start' command instead!");
            }
            else {
                Stop();
                Start();
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Status() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            if (res.Length == 0) {
                WriteLine("No antd process found.");
                WriteLine(Terminal.Execute("systemctl status antd-launcher.service"));
            }
            else {
                WriteLine(res);
                WriteLine(Terminal.Execute("systemctl status antd-launcher.service"));
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UmountAll() {
            WriteLine("Unmounting all antd-related directories...");
            execute.UmountAntd();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateCheck() {
            var linkedVersionName = execute.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = execute.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = execute.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        WriteLine("Antd is up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        WriteLine("New version of antd found!! -> {0}", newestDate);
                        return;
                    }
                    else {
                        WriteLine("There's nothing to update.");
                        return;
                    }
                }
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateLaunch() {
            var linkedVersionName = execute.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = execute.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = execute.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        WriteLine("Antd is already up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        WriteLine("New version of antd found!! -> {0}", newestDate);
                        WriteLine("Updating!");
                        execute.StopServices();
                        execute.CleanTmp();
                        if (newestVersionFound.Key.Contains(global.squashEndsWith)) {
                            execute.RemoveLink();
                            execute.LinkVersionToRunning(newestVersionFound.Key);
                        }
                        else if (newestVersionFound.Key.Contains(global.zipEndsWith)) {
                            var squashName = global.versionsDir + "/" + global.squashStartsWith + newestVersionFound.Value + global.squashEndsWith;
                            execute.MountTmpRam();
                            execute.CopyToTmp(newestVersionFound.Key);
                            execute.ExtractZipTmp(newestVersionFound.Key);
                            execute.RemoveTmpZips();
                            execute.CreateSquash(squashName);
                            execute.CleanTmp();
                            execute.UmountTmpRam();
                            execute.RemoveLink();
                            execute.LinkVersionToRunning(squashName);
                        }
                        else {
                            WriteLine("Update failed unexpectedly");
                            return;
                        }
                        execute.RestartSystemctlAntdServices();
                        return;
                    }
                    else {
                        WriteLine("There's nothing to update.");
                        return;
                    }
                }
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateFromUrl() {
            execute.StopServices();
            execute.CleanTmp();
            var squashName = global.versionsDir + "/" + global.squashStartsWith + DateTime.Now.ToString("yyyyMMdd") + global.squashEndsWith;
            execute.MountTmpRam();
            execute.DownloadFromUrl("https://github.com/Anthilla/Antd/archive/master.zip");
            execute.ExtractDownloadedFile();
            execute.RemoveTmpZips();
            execute.PickAndMoveZipFileInDownloadedDirectory();
            execute.RemoveDownloadedFile();
            execute.ExtractPickedZip();
            execute.RemoveTmpZips();
            execute.CreateSquash(squashName);
            execute.CleanTmp();
            execute.UmountTmpRam();
            execute.RemoveLink();
            execute.LinkVersionToRunning(squashName);
            execute.RestartSystemctlAntdServices();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateSelect() {
            var linkedVersionName = execute.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = execute.SetVersionKeyValuePair(linkedVersionName);
                WriteLine("Select a version (from its number) from this list:");
                execute.PrintVersions();
                var number = ReadLine();
                var selectedVersion = execute.GetVersionByNumber(number);
                if (linkedVersion.Key != null && selectedVersion.Key != null) {
                    WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, selectedVersion.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var selectedtDate = Convert.ToInt32(selectedVersion.Value);
                    WriteLine("New version of antd found!! -> {0}", selectedtDate);
                    WriteLine("Updating!");
                    execute.StopServices();
                    execute.CleanTmp();
                    if (selectedVersion.Key.Contains(global.squashEndsWith)) {
                        execute.RemoveLink();
                        execute.LinkVersionToRunning(selectedVersion.Key);
                    }
                    else if (selectedVersion.Key.Contains(global.zipEndsWith)) {
                        var squashName = global.versionsDir + "/" + global.squashStartsWith + selectedVersion.Value + global.squashEndsWith;
                        execute.MountTmpRam();
                        execute.CopyToTmp(selectedVersion.Key);
                        execute.ExtractZipTmp(selectedVersion.Key);
                        execute.RemoveTmpZips();
                        execute.CreateSquash(squashName);
                        execute.CleanTmp();
                        execute.UmountTmpRam();
                        execute.RemoveLink();
                        execute.LinkVersionToRunning(squashName);
                    }
                    else {
                        WriteLine("Update failed unexpectedly");
                        return;
                    }
                    execute.RestartSystemctlAntdServices();
                    return;
                }
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ReloadSystemctl() {
            Terminal.Execute("systemctl daemon-reload");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void IsRunning() {
            if (execute.IsAntdRunning() == true) {
                WriteLine("Yes, is running.");
            }
            else {
                WriteLine("No.");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void CleanTmp() {
            WriteLine("Cleaning tmp.");
            execute.CleanTmp();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Info() {
            WriteLine("This is a shell for antd :)");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Exit() {
            WriteLine("Bye bye");
            Environment.Exit(1);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Execute(string command) {
            WriteLine("Executing external command: {0}", command);
            WriteLine(Terminal.Execute(command));
            return;
        }
    }
}
