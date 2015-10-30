using System;

namespace antdlib.Database {
    public class Entity {
        public string _Id { get; set; } = Guid.NewGuid().ToString();

        public string Timestamp { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        public EntityStatus Status { get; set; } = EntityStatus.New;

        public string EntityCode { get { return $"{Status}-{_Id}-{Timestamp}"; } }
    }
}
