using System;

namespace AntdUi.Database {

    /// <summary>
    /// Modello base per le azioni del database
    /// Tutte le classi che dovranno essere salvate nel database dovranno ereditare questa classe
    /// This model includes the basic logics described in Actions.cs
    /// </summary>
    public class BaseModel {

        /// <summary>
        /// Parametro usato per l'indicizzazione del database
        /// </summary>
        public Guid Guid { get; set; } = Guid.Empty;

        /// <summary>
        /// Parametro usato per le interrogazioni al database
        /// Impostando questo valore a True preverrà all'oggetto in questione di comparire tra i risultati dell'interrogazione
        /// </summary>
        public bool Removed { get; set; } = false;
    }
}
