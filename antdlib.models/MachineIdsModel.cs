using System;

namespace antdlib.models {
    public class MachineIdsModel {

        /// <summary>
        /// fill values with machine values
        /// </summary>
        public MachineIdsModel() {
            PartNumber = Guid.NewGuid().ToString();
            SerialNumber = Guid.NewGuid().ToString();
            MachineUid = Guid.NewGuid().ToString();
        }

        public string PartNumber { get; set; }
        public string SerialNumber { get; set; }
        public string MachineUid { get; set; }
    }
}
