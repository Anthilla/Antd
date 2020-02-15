using Antd2.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {

    public static class LogRotation {

        public static void Rotate(LogRotatorParameters param) {
            Directory.CreateDirectory(param.Destination);

            if (param.EnableAppRotation)
                RotateApplicationLog(param.Source, param.Destination);

            if (param.EnableSystemRotation)
                RotateSystemLog(param.Destination);
        }

        private static void RotateSystemLog(string destination) {
            var sourceDirectory = "/var/log";
            if (!Directory.Exists(sourceDirectory))
                return;

            var destinationFolder = $"{destination}/syslog";
            Directory.CreateDirectory(destinationFolder);

            var syslogFiles = Directory.EnumerateFiles(sourceDirectory, "syslog*.gz", SearchOption.TopDirectoryOnly);

            foreach (var syslogFile in syslogFiles) {
                var fileInfo = new FileInfo(syslogFile);
                var destinationFile = $"{destinationFolder}/syslog.{fileInfo.CreationTime.ToString("yyyyMMdd")}.gz";
                Console.WriteLine($"[logrotate] {syslogFile} to {destinationFile}");
                File.Move(syslogFile, destinationFile);
            }

        }

        private static void RotateApplicationLog(string source, string destination) {
            RL(source, destination);
            RSD(source, destination);
        }

        private static void RL(string source, string destination) {
            var destinationDir = $"{destination}/log";
            Directory.CreateDirectory(destinationDir);

            var sourceDir = $"{source}/log";
            var files = Directory.EnumerateFiles(sourceDir, "*.log", SearchOption.TopDirectoryOnly);
            var todayPrefix = DateTime.Now.ToString("yyyyMMdd");

            foreach (var file in files) {
                if (file.Contains(todayPrefix))
                    continue;

                var fileName = Path.GetFileName(file);
                var newFilePath = $"{destinationDir}/{fileName}";
                Console.WriteLine($"[logrotate] {file} to {newFilePath}");
                File.Move(file, newFilePath);
                Gzip.CompressFile(newFilePath);
            }
        }

        private static void RSD(string source, string destination) {
            var destinationDir = $"{destination}/sensordata";
            Directory.CreateDirectory(destinationDir);

            var sourceDir = $"{source}/sensordata";
            var todayPrefix = DateTime.Now.ToString("yyyyMMdd");
            var directories = Directory.EnumerateDirectories(sourceDir)
                .Where(_ => !_.Contains(todayPrefix)).ToArray();

            foreach (var directory in directories) {
                var directoryName = Path.GetFileName(directory);
                var newDirectoryPath = $"{destinationDir}/{directoryName}";
                Console.WriteLine($"[logrotate] {directory} to {newDirectoryPath}");
                Directory.CreateDirectory(newDirectoryPath);
                MoveDirectory(directory, newDirectoryPath);
                Directory.Delete(directory);
                Gzip.CompressRecursive(newDirectoryPath);
            }
        }

        private static void MoveDirectory(string source, string destination) {
            var sourceFiles = Directory.EnumerateFiles(source);
            foreach (var file in sourceFiles) {
                var fileName = Path.GetFileName(file);
                var newFile = $"{destination}/{fileName}";
                File.Move(file, newFile);
            }
        }
    }
}
