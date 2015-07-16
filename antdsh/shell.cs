using antdlib;
using System;
using System.Threading;

namespace antdsh {
    public class shell {

        /// <summary>
        /// ok
        /// </summary>
        public static void Info() {
            Console.WriteLine("This is a shell for antd :)");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Exit() {
            Console.WriteLine("Bye bye");
            System.Environment.Exit(1);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Start() {
            Console.WriteLine("Looking for antds in {0}", global.versionsDir);
            var newestVersionFound = ex.GetNewestVersion();
            if (newestVersionFound.Key != null) {
                ex.LinkVersionToRunning(newestVersionFound.Key);
                Console.WriteLine("New antd '{0}' linked to running version", newestVersionFound.Key);
            }
            else {
                Console.WriteLine("There's no antd to link.");
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Stop() {
            ex.StopServices();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void IsRunning() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            if (res.Length > 0) {
                Console.WriteLine("Yes, is running.");
            }
            else {
                Console.WriteLine("No.");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Status() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            if (res.Length == 0) {
                Console.WriteLine("No antd process found.");
                Console.WriteLine("Try to restart or to update again.");
                Console.WriteLine("----------------------------------");
                Console.WriteLine(Terminal.Execute("systemctl status antd-launcher.service"));
            }
            else {
                Console.WriteLine(res);
                Console.WriteLine(Terminal.Execute("systemctl status antd-launcher.service"));
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Remove() {
            Console.Write("Are you sure? (y/n): ");
            var response = Console.ReadLine();
            if (response == "n") {
                Console.WriteLine("Ok, I'm removing everything.");
                ex.StopServices();
                UmountAll();
            }
            else if (response == "n") {
                Console.WriteLine("Ok.");
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateCheck() {
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = ex.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    Console.WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        Console.WriteLine("Antd is up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        Console.WriteLine("New version of antd found!! -> {0}", newestDate);
                        return;
                    }
                    else {
                        Console.WriteLine("There's nothing to update.");
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
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = ex.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    Console.WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        Console.WriteLine("Antd is already up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        Console.WriteLine("New version of antd found!! -> {0}", newestDate);
                        Console.WriteLine("Updating!");
                        ex.StopServices();
                        ex.CleanTmp();
                        if (newestVersionFound.Key.Contains(global.squashEndsWith)) {
                            ex.RemoveLink();
                            ex.LinkVersionToRunning(newestVersionFound.Key);
                        }
                        else if (newestVersionFound.Key.Contains(global.zipEndsWith)) {
                            var squashName = global.versionsDir + "/" + global.squashStartsWith + newestVersionFound.Value + global.squashEndsWith;
                            ex.MountTmpRam();
                            ex.CopyToTmp(newestVersionFound.Key);
                            ex.ExtractZipTmp(newestVersionFound.Key);
                            ex.RemoveTmpZips();
                            ex.CreateSquash(squashName);
                            ex.CleanTmp();
                            ex.UmountTmpRam();
                            ex.RemoveLink();
                            ex.LinkVersionToRunning(squashName);
                        }
                        else {
                            Console.WriteLine("Update failed unexpectedly");
                            return;
                        }
                        ex.RestartSystemctlAntdServices();
                        return;
                    }
                    else {
                        Console.WriteLine("There's nothing to update.");
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
            ex.StopServices();
            ex.CleanTmp();
            var squashName = global.versionsDir + "/" + global.squashStartsWith + DateTime.Now.ToString("yyyyMMdd") + global.squashEndsWith;
            ex.MountTmpRam();
            ex.DownloadFromUrl("https://github.com/Anthilla/Antd/archive/master.zip");
            ex.ExtractDownloadedFile();
            ex.RemoveTmpZips();
            ex.PickAndMoveZipFileInDownloadedDirectory();
            ex.RemoveDownloadedFile();
            ex.ExtractPickedZip();
            ex.RemoveTmpZips();
            ex.CreateSquash(squashName);
            ex.CleanTmp();
            ex.UmountTmpRam();
            ex.RemoveLink();
            ex.LinkVersionToRunning(squashName);
            ex.RestartSystemctlAntdServices();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateSelect() {
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                Console.WriteLine("Select a version (from its number) from this list:");
                ex.PrintVersions();
                var number = Console.ReadLine();
                var selectedVersion = ex.GetVersionByNumber(number);
                if (linkedVersion.Key != null && selectedVersion.Key != null) {
                    Console.WriteLine("You are running {0} and the latest version is {1}.", linkedVersion.Value, selectedVersion.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var selectedtDate = Convert.ToInt32(selectedVersion.Value);
                    Console.WriteLine("New version of antd found!! -> {0}", selectedtDate);
                    Console.WriteLine("Updating!");
                    ex.StopServices();
                    ex.CleanTmp();
                    if (selectedVersion.Key.Contains(global.squashEndsWith)) {
                        ex.RemoveLink();
                        ex.LinkVersionToRunning(selectedVersion.Key);
                    }
                    else if (selectedVersion.Key.Contains(global.zipEndsWith)) {
                        var squashName = global.versionsDir + "/" + global.squashStartsWith + selectedVersion.Value + global.squashEndsWith;
                        ex.MountTmpRam();
                        ex.CopyToTmp(selectedVersion.Key);
                        ex.ExtractZipTmp(selectedVersion.Key);
                        ex.RemoveTmpZips();
                        ex.CreateSquash(squashName);
                        ex.CleanTmp();
                        ex.UmountTmpRam();
                        ex.RemoveLink();
                        ex.LinkVersionToRunning(squashName);
                    }
                    else {
                        Console.WriteLine("Update failed unexpectedly");
                        return;
                    }
                    ex.RestartSystemctlAntdServices();
                    return;
                }
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RestartServices() {
            ex.RestartSystemctlAntdServices();
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
        public static void CleanTmp() {
            Console.WriteLine("Cleaning tmp.");
            ex.CleanTmp();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Execute(string command) {
            Console.WriteLine("Executing external command: {0}", command);
            Console.WriteLine(Terminal.Execute(command));
            //return;
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UmountAll() {
            var r = Terminal.Execute("cat /proc/mounts | grep /antd");
            var f = Terminal.Execute("df | grep /cfg/antd");
            if (r.Length > 0 || f.Length > 0) {
                ex.UmountAntd();
                UmountAll();
            }
            else return;
        }

        /// <summary>
        /// test
        /// </summary>
        public static void Progress() {
            using (var progress = new ProgressBar()) {
                for (int i = 0; i <= 100; i++) {
                    progress.Report((double)i / 100);
                    Thread.Sleep(20);
                }
            }
        }
    }
}
