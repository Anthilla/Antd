using System;
using System.IO.Compression;
using System.Linq;

namespace AntdUi.Database {
    public class Backup {
        #region [    Parameters    ]
        /// <summary>
        /// Parametro che identifica la cartella principale del database su cui effettuare il backup
        /// </summary>
        private readonly string _databaseDirectory = "/tmp/dbtest";

        private readonly string _backupRepoDirectory;

        private string _backupPath;
        private string _tmpPath;

        #endregion

        #region [    Constructors    ]
        /// <summary>
        /// Verifica l'esistenza delle cartelle indicate in _databaseDirectory e in _backupRepoDirectory
        /// Inizializza il valore di _backupRepoDirectory
        /// </summary>
        public Backup() {
            _backupRepoDirectory = SetBackupRepoDirectory();
            SetDirectories();
        }

        /// <summary>
        /// Inizializza il valore di _databaseDirectory con un nuovo valore
        /// Verifica l'esistenza delle cartelle indicate in _databaseDirectory e in _backupRepoDirectory
        /// Inizializza il valore di _backupRepoDirectory
        /// </summary>
        /// <param name="dbPath">Nuovo percorso del database differente da quello di default</param>
        public Backup(string dbPath) {
            _databaseDirectory = dbPath;
            _backupRepoDirectory = SetBackupRepoDirectory();
            SetDirectories();
        }
        #endregion


        #region [    Private Methods    ]
        /// <summary>
        /// Imposta il nome della cartella in cui salvare i backup del database
        /// </summary>
        /// <returns></returns>
        private string SetBackupRepoDirectory() {
            return _databaseDirectory + "/.backup";
        }

        /// <summary>
        /// Crea la cartelle indicate in _databaseDirectory e in _backupRepoDirectory
        /// </summary>
        private void SetDirectories() {
            System.IO.Directory.CreateDirectory(_databaseDirectory);
            System.IO.Directory.CreateDirectory(_backupRepoDirectory);
        }

        /// <summary>
        /// Partendo dal valore di _backupRepoDirectory compone il nome del file di backup
        /// </summary>
        /// <returns>Percorso completo del file del database</returns>
        private string SetBackupFileName() {
            return _backupRepoDirectory + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".dbck";
        }

        /// <summary>
        /// Partendo dal valore di _databaseDirectory compone il nome della cartella temporanea
        /// </summary>
        /// <returns>Percorso completo del file del database</returns>
        private string SetBackupTmpDir() {
            return _databaseDirectory + "/.tmp";
        }

        /// <summary>
        /// Pulizia della cartella temporanea _tmpPath
        /// </summary>
        private void ClanTmp() {
            if(System.IO.Directory.Exists(_tmpPath)) {
                System.IO.Directory.Delete(_tmpPath, true);
            }
        }
        #endregion

        /// <summary>
        /// Effettua un backup del database
        /// </summary>
        public void Save() {
            _backupPath = SetBackupFileName();
            _tmpPath = SetBackupTmpDir();
            ClanTmp();
            System.IO.Directory.CreateDirectory(_tmpPath);

            var files = System.IO.Directory.EnumerateFiles(_databaseDirectory, "*.db");
            foreach(var file in files) {
                var tmpDestination = file.Replace(_databaseDirectory, _tmpPath);
                System.IO.File.Copy(file, tmpDestination, true);
            }

            ZipFile.CreateFromDirectory(_tmpPath, _backupPath, CompressionLevel.Fastest, true);

            //pulizia dei temporanei
            ClanTmp();
        }

        /// <summary>
        /// Ripristina il database all'ultimo backup effettuato
        /// </summary>
        public void Restore() {
            _tmpPath = SetBackupTmpDir();
            ClanTmp();
            System.IO.Directory.CreateDirectory(_tmpPath);

            var files = System.IO.Directory.EnumerateFiles(_backupRepoDirectory, "*.dbck");

            if(!files.Any()) {
                ClanTmp();
                return;
            }



            ClanTmp();
        }
    }
}
