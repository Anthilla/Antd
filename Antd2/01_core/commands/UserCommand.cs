using Antd2.cmds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2 {
    public class UserCommand {

        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "check", CheckFunc },
                { "add", AddFunc },
            };

        private static string[] RequiredGroups = new string[] {
            "wheel",
        };

        private static string[] RequiredUsers = new string[] {
            "deus",
            "obse",
            "obsi",
            "visor",
        };

        public static void CheckFunc(string[] args) {
            Console.WriteLine("  Check groups");
            Console.WriteLine("--------------");
            var existingGroups = Getent.GetGroups();
            foreach (var group in RequiredGroups) {
                var isInstalled = existingGroups.Contains(group);
                if (isInstalled) {
                    CheckFunc_PrintInstalled(group);
                }
                else {
                    CheckFunc_PrintNotInstalled(group);
                }
            }
            Console.WriteLine("");
            Console.WriteLine("  Check users");
            Console.WriteLine("-------------");
            var existingUsers = Getent.GetUsers();
            foreach (var user in RequiredUsers) {
                var isInstalled = existingUsers.Contains(user);
                if (isInstalled) {
                    CheckFunc_PrintInstalled(user);
                }
                else {
                    CheckFunc_PrintNotInstalled(user);
                }
            }
        }

        private static void CheckFunc_PrintInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("exists");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("does not exist");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AddFunc(string[] args) {
            Console.WriteLine("  Create groups");
            Console.WriteLine("---------------");
            foreach (var group in RequiredGroups) {
                Console.WriteLine($"  add group {group}");
                Getent.AddGroup(group);
            }
            Console.WriteLine("");
            Console.WriteLine("  Create users");
            Console.WriteLine("--------------");
            foreach (var user in RequiredUsers) {
                Console.WriteLine($"  add user {user}");
                Getent.AddUser(user);
                foreach (var group in RequiredGroups) {
                    Console.WriteLine($"    assign group {group} to user {user}");
                    Getent.AssignGroup(user, group);
                }
            }
        }
    }
}