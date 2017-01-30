using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace AntdUi.Database {

    /// <summary>
    /// Classe che contiene le logiche principali che permettono di interagire con il database
    /// </summary>
    /// <typeparam name="T">
    /// Tipo dell'oggetto preso in gestione dall'istanza di questa classe
    /// L'oggetto dovrà ereditare BaseModel
    /// </typeparam>
    public class Actions<T> where T : BaseModel, new() {

        #region [    Parameters    ]
        /// <summary>
        /// Parametro che identifica la cartella principale del database
        /// </summary>
        private readonly string _databaseDirectory = "dbtest";

        /// <summary>
        /// Parametro che identifica il nome della tabella del database gestita dall'istanza corrente di questa classe
        /// Questo valore viene inizializzato dai costruttori e sarà uguale al Type di T
        /// </summary>
        private readonly string _databaseTable;

        /// <summary>
        /// Parametro che identifica il percorso completo del file del database
        /// Questo valore viene inizializzato dai costruttori
        /// </summary>
        public string DatabasePath { get; }
        #endregion

        #region [    Constructors    ]
        /// <summary>
        /// Verifica l'esistenza della cartella indicata in _databaseDirectory
        /// Inizializza i valori di _databaseTable e DatabasePath
        /// </summary>
        public Actions() {
            SetDatabaseDirectory();
            _databaseTable = SetDatabaseTableName();
            DatabasePath = SetDatabaseFileName();
        }

        /// <summary>
        /// Inizializza il valore di _databaseDirectory con un nuovo valore
        /// Verifica l'esistenza della cartella indicata in _databaseDirectory
        /// Inizializza i valori di _databaseTable e DatabasePath
        /// </summary>
        /// <param name="dbPath">Nuovo percorso del database differente da quello di default</param>
        public Actions(string dbPath) {
            SetDatabaseDirectory();
            _databaseTable = SetDatabaseTableName();
            _databaseDirectory = dbPath;
            DatabasePath = SetDatabaseFileName();
        }
        #endregion

        #region [    Private Methods    ]
        /// <summary>
        /// Imposta il nome della tabella del database gestita dall'istanza corrente di questa classe
        /// </summary>
        /// <returns></returns>
        private static string SetDatabaseTableName() {
            var t = typeof(T).ToString().ToLower();
            return !t.Contains(".") ? t : t.Split('.').LastOrDefault();
        }

        /// <summary>
        /// Crea la cartella indicata in _databaseDirectory
        /// </summary>
        private void SetDatabaseDirectory() {
            System.IO.Directory.CreateDirectory(_databaseDirectory);
        }

        /// <summary>
        /// Concatena i valori di _databaseDirectory e _databaseTable per formare il percorso completo del file del database
        /// </summary>
        /// <returns>Percorso completo del file del database</returns>
        private string SetDatabaseFileName() {
            return _databaseDirectory + "/_" + typeof(T).ToString().ToLower() + ".db";
        }
        #endregion

        #region [    Get Table   ]
        /// <summary>
        /// Ottiene tutte le voci nella tabella di tipo T
        /// </summary>
        /// <returns>Tutte le voci</returns>
        public IEnumerable<T> Get() {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                var result = table.FindAll().Where(x => x.Removed == false);
                return result;
            }
        }

        /// <summary>
        /// Ottiene tutte le voci nella tabella di tipo T ma è possibile specificare se includere quelle rimosse o no
        /// </summary>
        /// <param name="getRemoved">Valore booleano per indicare se includere nel risultato le voci nascoste</param>
        /// <returns>Tutte le voci</returns>
        public IEnumerable<T> Get(bool getRemoved) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                if(!getRemoved) {
                    var result = table.FindAll().Where(x => x.Removed == false);
                    return result;
                }
                else {
                    var result = table.FindAll();
                    return result;
                }
            }
        }

        /// <summary>
        /// Ottiene le voci della tabella T filtrandole con il parametro 'query'
        /// </summary>
        /// <param name="query">Filtro</param>
        /// <returns>Voci filtrate</returns>
        public IEnumerable<T> Get(Func<T, bool> query) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                var result = table.FindAll().Where(query);
                return result;
            }
        }

        /// <summary>
        /// Ottiene le voci della tabella T filtrandole con il parametro 'query' ma è possibile specificare se includere quelle rimosse o no
        /// </summary>
        /// <param name="query">Filtro</param>
        /// <param name="getRemoved">Valore booleano per indicare se includere nel risultato le voci nascoste</param>
        /// <returns>Voci filtrate</returns>
        public IEnumerable<T> Get(Func<T, bool> query, bool getRemoved) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                if(!getRemoved) {
                    var result = table.FindAll().Where(x => x.Removed == false).Where(query);
                    return result;
                }
                else {
                    var result = table.FindAll().Where(query);
                    return result;
                }
            }
        }

        /// <summary>
        /// Ottiene una singola voce nella tabella T con indice uguale al parametro 'guid'
        /// </summary>
        /// <param name="guid">Indice della voce da ottenere</param>
        /// <returns>Singola voce</returns>
        public T Get(Guid guid) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                var result = table.FindAll().FirstOrDefault(x => x.Guid == guid);
                return result;
            }
        }
        #endregion

        #region [    Count Table   ]
        /// <summary>
        /// Ottiene il numero di voci presenti nella tabella T
        /// </summary>
        /// <returns>Numero di voci</returns>
        public int Count() {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                var result = table.FindAll().Where(x => x.Removed == false);
                return result.Count();
            }
        }

        /// <summary>
        /// Ottiene il numero di voci nella tabella di tipo T ma è possibile specificare se includere quelle rimosse o no
        /// </summary>
        /// <param name="getRemoved">Valore booleano per indicare se includere nel risultato le voci nascoste</param>
        /// <returns>Numero di voci</returns>
        public int Count(bool getRemoved) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                if(!getRemoved) {
                    var result = table.FindAll().Where(x => x.Removed == false);
                    return result.Count();
                }
                else {
                    var result = table.FindAll();
                    return result.Count();
                }
            }
        }

        /// <summary>
        /// Ottiene il numero di voci della tabella T filtrandole con il parametro 'query'
        /// </summary>
        /// <param name="query">Filtro</param>
        /// <returns>Numero di voci</returns>
        public int Count(Func<T, bool> query) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                var result = table.FindAll().Where(query);
                return result.Count();
            }
        }

        /// <summary>
        /// Ottiene il numero di voci della tabella T filtrandole con il parametro 'query' ma è possibile specificare se includere quelle rimosse o no
        /// </summary>
        /// <param name="query">Filtro</param>
        /// <param name="getRemoved">Valore booleano per indicare se includere nel risultato le voci nascoste</param>
        /// <returns>Numero di voci</returns>
        public int Count(Func<T, bool> query, bool getRemoved) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                if(!getRemoved) {
                    var result = table.FindAll().Where(x => x.Removed == false).Where(query);
                    return result.Count();
                }
                else {
                    var result = table.FindAll().Where(query);
                    return result.Count();
                }
            }
        }
        #endregion

        #region [    Set Table    ]
        /// <summary>
        /// Inserisce la voce descritta dal parametro 'model' nella tabella T
        /// </summary>
        /// <param name="model">Voce da inserire</param>
        public void Save(T model) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                table.Insert(model);
                table.EnsureIndex(x => x.Guid);
            }
        }

        /// <summary>
        /// Modifica la voce descritta dal parametro 'model' nella tabella T
        /// </summary>
        /// <param name="model">Voce da modificare</param>
        public void Edit(T model) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                table.Delete(x => x.Guid == model.Guid);
                table.Insert(model);
                table.EnsureIndex(x => x.Guid);
            }
        }

        /// <summary>
        /// Rimuove una singola voce della tabella T dai risultati di ricerca
        /// </summary>
        /// <see cref="Get(Guid)">Per ottenere nuovamente questa voce la si può recuperare tramite il suo indice </see> 
        /// <see cref="Get(bool)">Per ottenere nuovamente questa voce si possono includere le voci rimosse dalle interrogazioni </see> 
        /// <param name="guid">Indice della voce</param>
        public void Remove(Guid guid) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                var result = Get(guid);
                if(result != null) {
                    result.Removed = true;
                    table.Delete(x => x.Guid == guid);
                    table.Insert(result);
                    table.EnsureIndex(x => x.Guid);
                }
            }
        }

        /// <summary>
        /// Cancella una singola voce della tabella T con indice uguale al parametro 'guid'
        /// </summary>
        /// <param name="guid">Indice della voce</param>
        public void Delete(Guid guid) {
            using(var db = new LiteDatabase(DatabasePath)) {
                var table = db.GetCollection<T>(_databaseTable);
                table.Delete(guid);
                table.EnsureIndex(x => x.Guid);
            }
        }
        #endregion
    }
}
