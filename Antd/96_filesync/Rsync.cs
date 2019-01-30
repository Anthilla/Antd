using Antd.sync;
using anthilla.core;
using anthilla.fs.Client;
using System;
using System.IO;

namespace Antd {

    /// <summary>
    /// ho un file                      {Source.file}
    /// genero la sua signature         {Source.file.sign}
    /// genero il suo delta             {Source.file.delta}
    /// 
    /// ho una destinazione             {Destination.file}
    /// che può esistere o no
    /// applico la patch usando         {Source.file} + {Source.file.delta} + {Destination.file}
    /// genero la signature di dst      {Destination.file.sign}
    /// confronto le due signarure      {Source.file.sign} + {Destination.file.sign}
    /// </summary>
    public class Rsync {

        private static string SignatureFileExtension = ".sign";
        private static string DeltaFileExtension = ".delta";
        private static string TmpFileExtension = ".tmp";

        #region [    Sync: local ←→ local    ]
        public static void SyncDirectory(string sourcePath, string destinationPath, bool recursive = false) {
            if(!Directory.Exists(sourcePath)) {
                throw new DirectoryNotFoundException(sourcePath);
            }
            var sourceType = FileSystem.GetPathType(sourcePath);
            if(sourceType != PathType.Directory) {
                throw new WrongPathTypeException(sourcePath);
            }
            if(!Directory.Exists(destinationPath)) {
                Directory.CreateDirectory(destinationPath);
            }
            var sourceContent = Directory.EnumerateFiles(sourcePath);
            foreach(var sourceFile in sourceContent) {
                var destinationFile = sourceFile.Replace(sourcePath, destinationPath);
                try {
                    SyncFile(sourceFile, destinationFile);
                }
                catch(Exception) {
                    //continue
                }
            }
            if(recursive) {
                var sourceDirectories = Directory.EnumerateDirectories(sourcePath);
                foreach(var sourceDir in sourceDirectories) {
                    var destinationDir = sourceDir.Replace(sourcePath, destinationPath);
                    if(!Directory.Exists(destinationDir)) {
                        Directory.CreateDirectory(destinationDir);
                    }
                    SyncDirectory(sourceDir, destinationDir, recursive);
                }
            }
        }

        public static bool SyncFile(string sourcePath, string destinationPath) {
            if(!File.Exists(sourcePath)) {
                throw new FileNotFoundException(sourcePath);
            }
            var sourceType = FileSystem.GetPathType(sourcePath);
            if(sourceType != PathType.File) {
                throw new WrongPathTypeException(sourcePath);
            }
            var sourceSignaturePath = $@"{sourcePath}{SignatureFileExtension}";
            //sign del file sorgente
            GenerateSignature(sourcePath, sourceSignaturePath);
            var destinationSignaturePath = $@"{destinationPath}{SignatureFileExtension}";
            if(File.Exists(destinationPath)) {
                //sign del file di destinazione
                GenerateSignature(destinationPath, destinationSignaturePath);
                var firstCheck = CompareSignature(sourceSignaturePath, destinationSignaturePath);
                if(firstCheck) {
                    //qui il file di destinazione esiste e la sua signature corrisponde
                    File.Delete(sourceSignaturePath);
                    File.Delete(destinationSignaturePath);
                    return true;
                }
            }
            //genero il delta
            var sourceDeltaPath = $@"{sourcePath}{DeltaFileExtension}";
            GenerateDelta(sourceSignaturePath, sourcePath, sourceDeltaPath);
            //applico la patch
            PatchFile(sourcePath, sourceDeltaPath, destinationPath);
            //verifico aggiornamento
            GenerateSignature(destinationPath, destinationSignaturePath);
            var secondCheck = CompareSignature(sourceSignaturePath, destinationSignaturePath);
            if(secondCheck) {
                File.Delete(sourceDeltaPath);
                File.Delete(sourceSignaturePath);
                File.Delete(destinationSignaturePath);
                return true;
            }
            //il sync non ha funzionato (?)
            //throw new SyncFailedException();
            return false;
        }

        #endregion

        #region [    Sync: local ←→ remote    ]
        //public static void SyncRemoteDirectory(string sourcePath, string destinationPath, bool recursive = false) {
        //    if(!Directory.Exists(sourcePath)) {
        //        throw new DirectoryNotFoundException(sourcePath);
        //    }
        //    var sourceType = FileSystem.GetPathType(sourcePath);
        //    if(sourceType != PathType.Directory) {
        //        throw new WrongPathTypeException(sourcePath);
        //    }
        //    if(!Directory.Exists(destinationPath)) {
        //        Directory.CreateDirectory(destinationPath);
        //    }
        //    var sourceContent = Directory.EnumerateFiles(sourcePath);
        //    foreach(var sourceFile in sourceContent) {
        //        var destinationFile = sourceFile.Replace(sourcePath, destinationPath);
        //        try {
        //            SyncRemoteFile(sourceFile, destinationFile);
        //        }
        //        catch(Exception) {
        //            //continue
        //        }
        //    }
        //    if(recursive) {
        //        var sourceDirectories = Directory.EnumerateDirectories(sourcePath);
        //        foreach(var sourceDir in sourceDirectories) {
        //            var destinationDir = sourceDir.Replace(sourcePath, destinationPath);
        //            if(!Directory.Exists(destinationDir)) {
        //                Directory.CreateDirectory(destinationDir);
        //            }
        //            SyncRemoteDirectory(sourceDir, destinationDir, recursive);
        //        }
        //    }
        //}

        /// <summary>
        /// Sincronizzazione di file da una sorgente locale a una destinazione remota
        /// Utilizzo l'algoritmo "rsync" basato su signature+patch
        /// </summary>
        /// <param name="sourcePath">File Path locale</param>
        /// <param name="destinationPath">File Path remoto</param>
        /// <param name="destinationAddress"></param>
        /// <param name="destinationPort"></param>
        public static void SyncRemoteFile(string sourcePath, string destinationPath, string destinationAddress, int destinationPort) {
            if(!File.Exists(sourcePath)) {
                throw new FileNotFoundException(sourcePath);
            }
            var sourceType = FileSystem.GetPathType(sourcePath);
            if(sourceType != PathType.File) {
                throw new WrongPathTypeException(sourcePath);
            }
            var client = new FileManagerClient(destinationAddress, destinationPort);
            var fileName = Path.GetFileName(sourcePath);
            var tmpDestinationPath = $"{destinationPath}{TmpFileExtension}";
            client.FileUpload(sourcePath, tmpDestinationPath);
            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            var sourceSignaturePath = $@"{sourcePath}{SignatureFileExtension}";
            var destinationSignaturePath = $"{destinationDirectory}/{fileName}{SignatureFileExtension}";
            //sign del file sorgente
            GenerateSignature(sourcePath, sourceSignaturePath);
            client.FileUpload(sourceSignaturePath, destinationSignaturePath);
            //genero il delta
            var sourceDeltaPath = $@"{sourcePath}{DeltaFileExtension}";
            var destinationDeltaPath = $"{destinationDirectory}/{fileName}{DeltaFileExtension}";
            GenerateDelta(sourceSignaturePath, sourcePath, sourceDeltaPath);
            client.FileUpload(sourceDeltaPath, destinationDeltaPath);
            client.FileSync(tmpDestinationPath, destinationSignaturePath, destinationDeltaPath, new[] { sourceSignaturePath, destinationDeltaPath });
            //File.Delete(sourceDeltaPath);
            //File.Delete(sourceSignaturePath);
        }
        #endregion

        #region [    Signature    ]
        /// <summary>
        /// Generates a signature from a stream, and writes it to another stream
        /// </summary>
        /// <param name="input">The stream to create the signature for</param>
        /// <param name="output">The stream to write the signature into</param>
        public static Stream GetSignature(string sourcePath) {
            var sourceSignaturePath = $@"{sourcePath}{SignatureFileExtension}";
            GenerateSignature(sourcePath, sourceSignaturePath);
            var signatureStream = new MemoryStream();
            using(FileStream fileStream = File.OpenRead(sourceSignaturePath)) {
                fileStream.CopyTo(signatureStream);
            }
            File.Delete(sourceSignaturePath);
            return signatureStream;
        }

        /// <summary>
        /// Generates a signature from a stream, and writes it to another stream
        /// </summary>
        /// <param name="input">The stream to create the signature for</param>
        /// <param name="output">The stream to write the signature into</param>
        public static void GenerateSignature(Stream input, Stream output) {
            Signature signatureModel;

            // Generating the object model to store the specific signature parameters and stream
            signatureModel = new Signature(output);

            ChecksumFileWriter.WriteCommonHeader(signatureModel);
            ChecksumFileWriter.GenerateSignatureData(signatureModel, input);
        }

        /// <summary>
        /// Generates a signature file from existing file, and writes it to another file
        /// </summary>
        /// <param name="filename">The file to create the signature for</param>
        /// <param name="outputfile">The file write the signature to</param>
        public static void GenerateSignature(string filename, string outputfile) {
            if(File.Exists(outputfile)) {
                File.Delete(outputfile);
            }

            using(FileStream fs1 = File.OpenRead(filename)) {
                using(FileStream fs2 = File.Create(outputfile)) {
                    GenerateSignature(fs1, fs2);
                }
            }
        }

        /// <summary>
        /// Generates a signature file from existing file, and write it to a given stream
        /// </summary>
        /// <param name="filename">The file to create the signature for</param>
        /// <param name="outputfile">The file write the signature to</param>
        public static void GenerateSignature(string filename, Stream outputStream) {
            using(FileStream fs1 = File.OpenRead(filename)) {
                GenerateSignature(fs1, outputStream);
            }
        }

        /// <summary>
        /// Generates a signature from existing stream, and write it to a given file
        /// </summary>
        /// <param name="filename">The file to create the signature for</param>
        /// <param name="outputfile">The file write the signature to</param>
        public static void GenerateSignature(Stream inputStream, string outputFile) {
            using(FileStream fs1 = File.OpenRead(outputFile)) {
                GenerateSignature(inputStream, fs1);
            }
        }
        #endregion

        #region [    Compare Signature    ]
        /// <summary>
        /// Compare two signature file
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        public static bool CompareSignature(string sourceFile, string destinationFile) {
            if(!File.Exists(sourceFile)) {
                return false;
            }
            if(!File.Exists(destinationFile)) {
                return false;
            }
            const int buffer = sizeof(long);
            using(FileStream fs1 = File.OpenRead(sourceFile))
            using(FileStream fs2 = File.OpenRead(destinationFile)) {
                byte[] one = new byte[buffer];
                byte[] two = new byte[buffer];
                for(int i = 0; i < fs1.Length; i += buffer) {
                    fs1.Read(one, 0, buffer);
                    fs2.Read(two, 0, buffer);
                    if(BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compare two signature bytes
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        public static bool CompareSignature(byte[] sourceFile, byte[] destinationFile) {
            const int buffer = sizeof(long);
            using(Stream fs1 = new MemoryStream(sourceFile))
            using(Stream fs2 = new MemoryStream(destinationFile)) {
                byte[] one = new byte[buffer];
                byte[] two = new byte[buffer];
                for(int i = 0; i < fs1.Length; i += buffer) {
                    fs1.Read(one, 0, buffer);
                    fs2.Read(two, 0, buffer);
                    if(BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compare two signature Stream
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        public static bool CompareSignature(Stream sourceFile, Stream destinationFile) {
            const int buffer = sizeof(long);
            byte[] one = new byte[buffer];
            byte[] two = new byte[buffer];
            for(int i = 0; i < sourceFile.Length; i += buffer) {
                sourceFile.Read(one, 0, buffer);
                destinationFile.Read(two, 0, buffer);
                if(BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    return false;
            }
            return true;
        }
        #endregion

        #region [    Delta   ]
        /// <summary>
        /// Generates a delta stream
        /// </summary>
        /// <param name="signature">The signature for the stream</param>
        /// <param name="filename">The (possibly) altered stream to create the delta for</param>
        /// <param name="output">The delta output</param>
        public static void GenerateDelta(Stream signature, Stream input, Stream output) {
            ChecksumFileReader checksum;

            //Write header into output file
            output.Write(RDiffBinary.DELTA_MAGIC, 0, RDiffBinary.DELTA_MAGIC.Length);

            checksum = new ChecksumFileReader(signature);

            DeltaFile.GenerateDeltaFile(input, output, checksum);
        }

        /// <summary>
        /// Generates a delta stream
        /// </summary>
        /// <param name="signature">The signature for the stream</param>
        /// <param name="filename">The (possibly) altered stream to create the delta for</param>
        /// <param name="output">The delta output</param>
        public static void GenerateDelta(Stream signature, string inputFile, Stream output) {
            FileStream inputStream;

            using(inputStream = File.OpenRead(inputFile)) {
                GenerateDelta(signature, inputStream, output);
            }
        }


        /// <summary>
        /// Generates a delta stream
        /// </summary>
        /// <param name="signature">The signature for the stream</param>
        /// <param name="filename">The (possibly) altered stream to create the delta for</param>
        /// <param name="output">The delta output</param>
        public static void GenerateDelta(byte[] signature, string inputFile, Stream output) {
            FileStream inputStream;
            MemoryStream signatureStream;

            using(inputStream = File.OpenRead(inputFile)) {
                using(signatureStream = new MemoryStream(signature)) {
                    GenerateDelta(signatureStream, inputStream, output);
                }
            }
        }


        /// <summary>
        /// Generates a delta file
        /// </summary>
        /// <param name="signaturefile">The signature for the file</param>
        /// <param name="sourceFile">The file to create the delta for</param>
        /// <param name="deltafile">The delta output file</param>
        public static void GenerateDelta(string signaturefile, string sourceFile, string deltafile) {
            using(FileStream signatureStream = File.OpenRead(signaturefile)) {
                using(FileStream sourceStream = File.OpenRead(sourceFile)) {
                    using(FileStream deltaStream = File.Create(deltafile)) {
                        GenerateDelta(signatureStream, sourceStream, deltaStream);
                    }
                }
            }
        }
        #endregion

        #region [    Patch    ]
        /// <summary>
        /// Patches a file
        /// </summary>
        /// <param name="basefile">The most recent full copy of the file</param>
        /// <param name="deltafile">The delta file</param>
        /// <param name="outputfile">The restored file</param>
        public static void PatchFile(string basefile, string deltafile, string outputfile) {
            using(FileStream input = File.OpenRead(basefile)) {
                using(FileStream delta = File.OpenRead(deltafile)) {
                    using(FileStream output = File.Create(outputfile)) {
                        PatchFile(input, delta, output);
                    }
                }
            }
        }

        /// <summary>
        /// Patches a file
        /// </summary>
        /// <param name="basefile">The most recent full copy of the file</param>
        /// <param name="deltafile">The delta file</param>
        /// <param name="outputfile">The restored file</param>
        public static void PatchFile(string basefile, Stream delta, string outputfile) {
            using(FileStream input = File.OpenRead(basefile)) {
                using(FileStream output = File.Create(outputfile)) {
                    PatchFile(input, delta, output);
                }
            }
        }

        /// <summary>
        /// Constructs a stream from a basestream and a delta stream
        /// </summary>
        /// <param name="basefile">The most recent full copy of the file</param>
        /// <param name="deltafile">The delta file</param>
        /// <param name="outputfile">The restored file</param>
        public static void PatchFile(Stream basestream, Stream delta, Stream output) {
            DeltaFile.PatchFile(basestream, output, delta);
        }
        #endregion
    }
}
