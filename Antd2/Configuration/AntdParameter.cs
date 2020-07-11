namespace Antd2.Configuration {


    public class AntdParameter {

        public ConfType ConfType { get; set; } = ConfType.Deploy;


    }

    public enum ConfType {
        Deploy,
        Overmount
    }
}
