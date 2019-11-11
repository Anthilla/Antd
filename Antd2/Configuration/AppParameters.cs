namespace Antd2.Configuration {
    public class AppParameters {
        /// <summary>
        /// Indirizzo su cui pubblicare l'applicazione
        /// Default tutti gli ip
        /// </summary>
        public string Address { get; set; } = "0.0.0.0";

        /// <summary>
        /// Porta http su cui pubblicare l'applicazione
        /// Default "8085"
        /// </summary>
        public string HttpPort { get; set; } = "8085";

        /// <summary>
        /// Cartella root
        /// Default /cfg/antd
        /// </summary>
        public string Root { get; set; } = "/cfg/antd";
    }
}
