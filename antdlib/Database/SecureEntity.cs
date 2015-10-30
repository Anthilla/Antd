using System;

namespace antdlib.Database {
    public class SecureEntity {
        public string _Id { get; set; } = System.Guid.NewGuid().ToString();

        public string Guid { get; set; } = "";

        public string Timestamp { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        public EntityStatus Status { get; set; } = EntityStatus.New;

        public string EntityCode { get { return $"{Status.ToString()}-{_Id}-{Timestamp}"; } }

        public byte[] EntityKey { get; set; }

        public byte[] EntityVector { get; set; }

        public byte[] Dump { get; set; }
    }
}
