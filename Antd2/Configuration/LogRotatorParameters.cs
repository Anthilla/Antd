namespace Antd2.Configuration {
    public class LogRotatorParameters {

        public bool Enable { get; set; }
        public bool EnableSystemRotation { get; set; }
        public bool EnableAppRotation { get; set; }

        public string Source { get; set; }


        public string Destination { get; set; }


    }
}
