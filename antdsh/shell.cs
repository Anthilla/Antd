using antdlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class shell {

        /// <summary>
        /// ok
        /// </summary>
        public static void Info() {
            Console.WriteLine("> This is a shell for antd :)");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Start() {
            Console.WriteLine("> Looking for antds in {0}", global.versionsDir);
            var newestVersionFound = ex.GetNewestVersion();
            if (newestVersionFound.Key != null) {
                ex.LinkVersionToRunning(newestVersionFound.Key);
                Console.WriteLine("> New antd '{0}' linked to running version", newestVersionFound.Key);
            }
            else {
                Console.WriteLine("> There's no antd to link.");
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
                    Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        Console.WriteLine("> Antd is up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                        return;
                    }
                    else {
                        Console.WriteLine("> There's nothing to update.");
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
                    Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        Console.WriteLine("> Antd is already up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                        Console.WriteLine("> Updating!");
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
                            Console.WriteLine("> Update failed unexpectedly");
                            return;
                        }
                        Terminal.Execute("systemctl restart antd-prepare.service");
                        Terminal.Execute("systemctl restart framework-antd.mount");
                        Terminal.Execute("systemctl antd-launcher.service");
                        return;
                    }
                    else {
                        Console.WriteLine("> There's nothing to update.");
                        return;
                    }
                }
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UpdateFromUrl() {
            var squashName = global.versionsDir + "/" + global.squashStartsWith + DateTime.Now.ToString("yyyyMMdd") + global.squashEndsWith;
            //monta ram in tmp
            ex.MountTmpRam();
            //scarica file in tmp
            ex.DownloadFromUrl("https://github.com/Anthilla/Antd/archive/master.zip");
            //estrai file scaricato
            ex.ExtractDownloadedFile();
            Console.WriteLine(">>>> ExtractDownloadedFile");
            //cancella downloaded file
            ex.RemoveDownloadedFile();
            Console.WriteLine(">>>> RemoveDownloadedFile");
            //mv l'altro zip da qui dentroa a tmp
            ex.MoveDownloadedZip();
            Console.WriteLine(">>>> MoveDownloadedZip");
            //da qui estrai zip e monta e lancia, ecc
            ex.ExtractDownloadedZip();
            Console.WriteLine(">>>> ExtractDownloadedZip");
            ex.RemoveTmpZips();
            Console.WriteLine(">>>> RemoveTmpZips");
            ex.CreateSquash(squashName);
            Console.WriteLine(">>>> CreateSquash");
            //pulisci tmp
            ex.CleanTmp();
            //smonta ram
            ex.UmountTmpRam();
            ex.RemoveLink();
            ex.LinkVersionToRunning(squashName);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateSelect() {
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                Console.WriteLine("> Select a version (from its number) from this list:");
                ex.PrintVersions();
                var number = Console.ReadLine();
                var selectedVersion = ex.GetVersionByNumber(number);
                if (linkedVersion.Key != null && selectedVersion.Key != null) {
                    Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, selectedVersion.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var selectedtDate = Convert.ToInt32(selectedVersion.Value);

                    Console.WriteLine("> New version of antd found!! -> {0}", selectedtDate);
                    Console.WriteLine("> Updating!");
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
                        Console.WriteLine("> Update failed unexpectedly");
                        return;
                    }
                    Terminal.Execute("systemctl restart antd-prepare.service");
                    Terminal.Execute("systemctl restart framework-antd.mount");
                    Terminal.Execute("systemctl antd-launcher.service");
                    return;
                }
                return;
            }
        }

        public static void SetDirectoryDownload() {
            Console.WriteLine("> Write a path to download-directory:");
            var path = Console.ReadLine();
            config.downloadDirectory.Set(path);
        }
    }
}
