namespace AntdUi2.Modules.Auth {

    /// <summary>
    /// Oggetto che contiene i dati necessari per l'autenticazione
    /// </summary>
    public class AuthenticationDataModel {
        public string Id { get; set; }
        public string[] Claims { get; set; }
    }
}
