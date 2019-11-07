using Antd2.cmds;
using anthilla.core;
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
            ConsoleLogger.Log("  Check groups");
            ConsoleLogger.Log("--------------");
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
            ConsoleLogger.Log("");
            ConsoleLogger.Log("  Check users");
            ConsoleLogger.Log("-------------");
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
            ConsoleLogger.Log("exists");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CheckFunc_PrintNotInstalled(string package) {
            Console.Write($"  {package}: ");
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleLogger.Log("does not exist");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AddFunc(string[] args) {
            ConsoleLogger.Log("  Create groups");
            ConsoleLogger.Log("---------------");
            foreach (var group in RequiredGroups) {
                ConsoleLogger.Log($"  add group {group}");
                Getent.AddGroup(group);
            }
            ConsoleLogger.Log("");
            ConsoleLogger.Log("  Create users");
            ConsoleLogger.Log("--------------");
            foreach (var user in RequiredUsers) {
                ConsoleLogger.Log($"  add user {user}");
                Getent.AddUser(user);
                foreach (var group in RequiredGroups) {
                    ConsoleLogger.Log($"    assign group {group} to user {user}");
                    Getent.AssignGroup(user, group);
                }
            }
        }
    }
}