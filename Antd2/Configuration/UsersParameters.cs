namespace Antd2.Configuration {
    /// <summary>
    /// Informazioni sugli utenti di sistema, da applicare al boot della macchina
    /// </summary>
    public class SystemUser {

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// La password deve essere applicata come hashing compatibile con shadow e passwd
        /// Se l'utente non è abilitato la password viene settata vuota
        /// </summary>
        public string Password { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;
    }
}
