namespace antdlib.models {
    public class ResponseAssetCommandModel {

        public ResponseAssetCommandModel() {

        }

        public ResponseAssetCommandModel(string commandGuid, string command) {
            CommandGuid = commandGuid;
            Command = command;
        }

        public string CommandGuid { get; set; }
        public string Command { get; set; }
    }
}
