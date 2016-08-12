using System;
using System.Collections.Generic;

namespace antdlib.views {
    [Serializable]
    public class EntityModel {
        /// <summary>
        /// Use this value to write the object in the Database
        /// This value will be always a "new" value, both in Save and Edit functions
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Use this value to track the object in the Database
        /// In the Edit function this value will be the same as the previous object
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// This value will be always a "new" value, both in Save and Edit functions
        /// </summary>
        public string Timestamp { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        public EntityStatus Status { get; set; } = EntityStatus.New;
        public string EntityCode { get; set; }
        public Guid Key { get; set; }
        public Guid Vector { get; set; }
        /// <summary>
        /// This value will be set to True to encrypt the data during the Save function
        /// </summary>
        public bool IsEncrypted { get; set; }
        public byte[] Dump { get; set; }
        public IEnumerable<string> Tags { get; set; } = new List<string>();
    }
}
