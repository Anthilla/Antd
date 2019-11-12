using System;

namespace AntdUi2.Modules.Auth {

    /// <summary>
    /// Oggetto che contiene i dati relativi alla sessione autenticata
    /// </summary>
    public class PhlegyasUserModel {
        public Guid User { get; set; }
        public Guid Session { get; set; }
        public string Code { get; set; }
    }
}
