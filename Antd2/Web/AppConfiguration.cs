namespace Antd2.Web {
    /// <summary>
    /// Esempio di app configuration
    /// Leggo il file, deserializzo i dati, importo l'oggetto AppConfiguration
    /// Lo passo <see cref="Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder)"/>
    /// che lo usa per inizializzare <see cref="AntdBootstrapper.AntdBootstrapper(IAppConfiguration)"/>
    /// </summary>
    public class AppConfiguration : IAppConfiguration {
        public Logging Logging { get; set; }
        public Smtp Smtp { get; set; }
    }

    public class LogLevel {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class Logging {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public class Smtp {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Port { get; set; }
    }
}
