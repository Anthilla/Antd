using antdlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class shell {
        private const string root = "/mnt/cdrom/Apps";  //qui dentro carico tutte le versioni di antd
        private const string antdRunning = "antdRunning";

        private const string zipStartsWith = "antd_";
        private const string zipEndsWith = ".7z";
        private const string squashStartsWith = "DIR_framework_antd_";
        private const string squashEndsWith = ".squashfs.xz";

        private const string dateFormat = "yyyyMMdd";

        public static void Info() {
            Console.WriteLine("> This is a shell for antd :)");
        }

        public static void UpdateCheck() {
            //comandi utili:    ln -s file002 fileRunning
            //                  file fileRunning            return-> fileRunning: symbolic link to file001
            //prendo il contenuto di root -> voglio vedere 1) se c'è una versione running
            //                                             2) se e quante versioni ci sono -> layout del file   ->  7z      antd_*yyyyMMdd*.7z
            //                                                                                                  ->  squash  DIR_framework_antd_*yyyyMMdd*.squashfs.xz
            var running = Directory.EnumerateFiles(root, antdRunning).ToArray();
            //creo una lista di dump dove mettere le versioni trovate
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                //identificare la versionedi antd in running
                //file fileRunning  return  ->  fileRunning: symbolic link to file001
                var linkedVersionName = Terminal.Execute("file " + antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(zipStartsWith, "").Replace(zipEndsWith, "").Replace(squashStartsWith, "").Replace(squashEndsWith, ""));
            }
            else {
                Console.WriteLine("> There's nothing to update -> there's no running version of antd.");
                return;
            }
            var zips = Directory.EnumerateFiles(root, "*.*").Where(s => s.StartsWith(zipStartsWith) && s.EndsWith(zipEndsWith)).ToArray();
            if (zips.Length < 1) {
                Console.WriteLine("> There's nothing to update -> there's no zipped version of antd.");
                return;
            }
            else {
                //ok ci sono degli zip
                //buttali nella lista
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(zipStartsWith, "").Replace(zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(root, "*.*").Where(s => s.StartsWith(squashStartsWith) && s.EndsWith(squashEndsWith)).ToArray();
            if (squashes.Length < 1) {
                Console.WriteLine("> There's nothing to update -> there's no squashed version of antd.");
                return;
            }
            else {
                //ok ci sono degli squash
                //buttali nella lista
                foreach (var squash in squashes) {
                    versions.Add(new KeyValuePair<string, string>(squash, squash.Replace(squashStartsWith, "").Replace(squashEndsWith, "")));
                }
            }
            //ordino la lista
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            //quindi se la versione ordinata è > 0 allora recupero il file col valore più alto
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.Last();
            }
            //ora controllo la linked version e poi la confronto con la newest
            if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                //so che ci sono entrambi i fattori, -> confronto il Value
                //se sono uguali -> no update
                //se sono differenti -> update
                // Parse date and time with custom specifier.
                Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                var linkedDate = DateTime.ParseExact(linkedVersion.Value, dateFormat, CultureInfo.InvariantCulture);
                var newestDate = DateTime.ParseExact(newestVersionFound.Value, dateFormat, CultureInfo.InvariantCulture);
                //ho le date
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

        public static void UpdateLaunch() {
            var running = Directory.EnumerateFiles(root, antdRunning).ToArray();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                var linkedVersionName = Terminal.Execute("file " + antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(zipStartsWith, "").Replace(zipEndsWith, "").Replace(squashStartsWith, "").Replace(squashEndsWith, ""));
            }
            else {
                Console.WriteLine("> Update exited -> there's no running version of antd.");
                return;
            }
            var zips = Directory.EnumerateFiles(root, "*.*").Where(s => s.StartsWith(zipStartsWith) && s.EndsWith(zipEndsWith)).ToArray();
            if (zips.Length < 1) {
                Console.WriteLine("> Update exited -> there's no zipped version of antd.");
                return;
            }
            else {
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(zipStartsWith, "").Replace(zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(root, "*.*").Where(s => s.StartsWith(squashStartsWith) && s.EndsWith(squashEndsWith)).ToArray();
            if (squashes.Length < 1) {
                Console.WriteLine("> Update exited -> there's no squashed version of antd.");
                return;
            }
            else {
                foreach (var squash in squashes) {
                    versions.Add(new KeyValuePair<string, string>(squash, squash.Replace(squashStartsWith, "").Replace(squashEndsWith, "")));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.Last();
            }
            if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                var linkedDate = DateTime.ParseExact(linkedVersion.Value, dateFormat, CultureInfo.InvariantCulture);
                var newestDate = DateTime.ParseExact(newestVersionFound.Value, dateFormat, CultureInfo.InvariantCulture);
                if (linkedVersion.Value == newestVersionFound.Value) {
                    Console.WriteLine("> Update exited -> antd is already up to date!");
                    return;
                }
                else if (newestDate > linkedDate) {
                    Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                    Console.WriteLine("> Updating!");
                    string fileToLink;
                    if (newestVersionFound.Key.Contains(squashEndsWith)) {
                        fileToLink = newestVersionFound.Key;
                    }
                    else if (newestVersionFound.Key.Contains(zipEndsWith)) {
                        fileToLink = Path.GetFullPath(newestVersionFound.Key.Replace(zipStartsWith, squashStartsWith).Replace(zipEndsWith, zipStartsWith));
                        Terminal.Execute("7z x " + Path.GetFullPath(newestVersionFound.Key));
                        Terminal.Execute("mksquashfs " +
                            Path.GetFullPath(newestVersionFound.Key.Replace(zipEndsWith, "")) + " " +
                            fileToLink +  
                            " -comp xz -Xbcj x86 -Xdict-size 75%");
                    }
                    else {
                        Console.WriteLine("> Update failed unexpectedly");
                        return;
                    }
                    Terminal.Execute("ln -s " + Path.GetFullPath(fileToLink) + " " + Path.GetFullPath(antdRunning));

                    //risetta antd, squash, mount, sysctl ecc

                    return;
                }
                else {
                    Console.WriteLine("> Update failed unexpectedly");
                    return;
                }
            }
            return;
        }

        public static void UpdateForce() {

        }
    }
}
