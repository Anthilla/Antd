using Antd2.cmds;
using anthilla.core;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class PackagesCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "install", InstallFunc },
            };

        private static string[] RequiredPackages = new string[] {
            "apt-get",
            "brctl",
            "date",
            "dhclient",
            "killall",
            "dhcpcd",
            "df",
            "dmesg",
            "bash",
            "getent",
            "gluster",
            "glusterd",
            "groupadd",
            "haproxy",
            "hostnamectl",
            "ip",
            "ifenslave",
            "journalctl",
            "keepalived",
            "kill",
            "losetup",
            "lsmod",
            "modprobe",
            "rmmod",
            "mount",
            "umount",
            "nsupdate",
            "ntpdate",
            "useradd",
            "mkpasswd",
            "usermod",
            "qemu-img",
            "rndc",
            "rsync",
            "ssh",
            "ssh-keygen",
            "sysctl",
            "systemctl",
            "timedatectl",
            "hwclock",
            "uname",
            "uptime",
            "zfs",
            "virsh",
            "zpool",
        };

        private static void CheckFunc(string[] args) {
            foreach (var package in RequiredPackages) {
                var isInstalled = Whereis.IsInstalled(package);
                if (isInstalled) {
                    CheckFunc_PrintInstalled(package);
                }
                else {
                    CheckFunc_PrintNotInstalled(package);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("installed");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("not installed");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void InstallFunc(string[] args) {
            foreach (var package in args) {
                Console.WriteLine($"  installing {package}");
                AptGet.Install(package);
            }
        }
    }
}