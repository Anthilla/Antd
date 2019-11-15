using Octodiff.Core;
using Octodiff.Diagnostics;
using System;
using System.IO;

namespace Antd2.Sync {
    public static class OSync {

        public const string SignatureFileExtension = ".sign";
        public const string DeltaFileExtension = ".delta";
        public const string TmpFileExtension = ".tmps";

        public static void CreateSignatureFile(string baseFilePath, string signatureFilePath) {
            var signatureOutputDirectory = Path.GetDirectoryName(signatureFilePath);
            if (!Directory.Exists(signatureOutputDirectory))
                Directory.CreateDirectory(signatureOutputDirectory);
            var signatureBuilder = new SignatureBuilder();
            using (var basisStream = new FileStream(baseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
            }
        }
        public static void CreateDeltaFile(string signatureFilePath, string deltaFilePath, string newFilePath) {
            var deltaOutputDirectory = Path.GetDirectoryName(deltaFilePath);
            if (!Directory.Exists(deltaOutputDirectory))
                Directory.CreateDirectory(deltaOutputDirectory);
            var deltaBuilder = new DeltaBuilder();
            using (var newFileStream = new FileStream(newFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            using (var signatureFileStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, new ConsoleProgressReporter()), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            }
        }
        public static void CreateDeltaFile(byte[] signatureFile, string deltaFilePath, string newFilePath) {
            var deltaOutputDirectory = Path.GetDirectoryName(deltaFilePath);
            if (!Directory.Exists(deltaOutputDirectory))
                Directory.CreateDirectory(deltaOutputDirectory);
            var deltaBuilder = new DeltaBuilder();
            using (var newFileStream = new FileStream(newFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            using (var signatureFileStream = new MemoryStream(signatureFile))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, new ConsoleProgressReporter()), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            }
        }
        public static void ApplyPatch(string baseFilePath, string signatureFilePath, string deltaFilePath, string newFilePath) {
            // Apply delta file to create new file
            var newFileOutputDirectory = Path.GetDirectoryName(newFilePath);
            if (!Directory.Exists(newFileOutputDirectory))
                Directory.CreateDirectory(newFileOutputDirectory);
            var deltaApplier = new DeltaApplier { SkipHashCheck = false };
            using (var basisStream = new FileStream(baseFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var newFileStream = new FileStream(newFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()), newFileStream);
            }
            File.Delete(signatureFilePath);
            File.Delete(deltaFilePath);
        }

        /// <summary>
        /// osync file /Data/Data02/test.txt /Data/Data02/test2.txt
        /// </summary>
        /// <param name="signatureBaseFilePath"></param>
        /// <param name="newFilePath"></param>
        public static void SyncFiles(string signatureBaseFilePath, string newFilePath) {
            if (!File.Exists(signatureBaseFilePath)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  file not found: {signatureBaseFilePath}");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            if (!File.Exists(newFilePath)) {
                File.Copy(signatureBaseFilePath, newFilePath);
                return;
            }
            // Create signature file
            var signatureFilePath = signatureBaseFilePath + ".sign";
            var signatureOutputDirectory = Path.GetDirectoryName(signatureFilePath);
            if (!Directory.Exists(signatureOutputDirectory))
                Directory.CreateDirectory(signatureOutputDirectory);
            var signatureBuilder = new SignatureBuilder();
            using (var basisStream = new FileStream(signatureBaseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
            }

            // Create delta file
            var deltaFilePath = newFilePath + ".delta";
            var deltaOutputDirectory = Path.GetDirectoryName(deltaFilePath);
            if (!Directory.Exists(deltaOutputDirectory))
                Directory.CreateDirectory(deltaOutputDirectory);
            var deltaBuilder = new DeltaBuilder();
            using (var newFileStream = new FileStream(signatureBaseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureFileStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, new ConsoleProgressReporter()), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            }

            // Apply delta file to create new file
            var newFileOutputDirectory = Path.GetDirectoryName(newFilePath);
            if (!Directory.Exists(newFileOutputDirectory))
                Directory.CreateDirectory(newFileOutputDirectory);
            var deltaApplier = new DeltaApplier { SkipHashCheck = false };
            using (var basisStream = new FileStream(signatureBaseFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var newFileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
                deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()), newFileStream);
            }

            if (File.Exists(signatureFilePath)) {
                File.Delete(signatureFilePath);
            }
            if (File.Exists(deltaFilePath)) {
                File.Delete(deltaFilePath);
            }
        }

        public static void SyncDirectories(string sourceDirectory, string destinationDirectory) {
            if (!Directory.Exists(sourceDirectory)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  directory not found: {sourceDirectory}");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            if (!Directory.Exists(destinationDirectory)) {
                Directory.CreateDirectory(destinationDirectory);
            }

            var subDirs = Directory.EnumerateDirectories(sourceDirectory);
            foreach (var source in subDirs) {
                var destination = source.Replace(sourceDirectory, destinationDirectory);
                Console.WriteLine($"  sync subdir {source} {destination}");
                SyncDirectories(source, destination);
            }

            var subFile = Directory.EnumerateFiles(sourceDirectory);
            foreach (var source in subFile) {
                var destination = source.Replace(sourceDirectory, destinationDirectory);
                Console.WriteLine($"  sync subfile {source} {destination}");
                SyncFiles(source, destination);
            }
        }
    }
}
