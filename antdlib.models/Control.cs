namespace antdlib.models {
    /// <summary>
    /// al lancio di antd controlla ed esegue una lista di comandi scritti su un file
    /// per ogni comando si deve predisporre un secondo comando di controllo in modo tale da non ri-lanciare il comando principale
    /// 
    /// quindi un possibile flusso sarà:
    ///     1 Lancio ControlCommand e ottengo un risultato
    ///     2 confronto in qualche mood il risultato di (1) con Check
    ///         2a se Check positivo allora la configurazione è ok
    ///         2b se Check negativo allora devo configurare qualcosa
    ///             3 Lancio FirstCommand per configurare qualcosa
    ///             4 Ri-lancio Control command e ottengo un risultato
    ///                 4a se Check positivo allora la configurazione è andata a buon fine
    ///                 4b altrimento lancio nuovamente il FirstCommand (3)
    /// </summary>
    public class Control {
        /// <summary>
        /// definisce l'ordine di esecuzione dei comandi
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Comando che deve essere lanciato per configurare qualcosa
        /// </summary>
        public string FirstCommand { get; set; }
        /// <summary>
        /// Comando che viene lanciato prima E dopo il FirstCommand per controllare che:
        ///     1 - quell'aspetto non sia già stato configurato
        ///     2 - quell'aspetto venga configurato correttamente
        /// </summary>
        public string ControlCommand { get; set; }
        /// <summary>
        /// Risultato che ci si aspetta di ottenere lanciando il ControlCommand
        /// </summary>
        public string Check { get; set; }
    }
}
