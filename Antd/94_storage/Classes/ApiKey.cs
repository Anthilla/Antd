using anthilla.core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kvpbase {
    public class ApiKey {
        #region Public-Members

        public int? ApiKeyId { get; set; }
        public int? UserMasterId { get; set; }
        public int? ExpirationSec { get; set; }
        public string Guid { get; set; }
        public string Notes { get; set; }
        public int? Active { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? Expiration { get; set; }

        #endregion

        #region Constructors-and-Factories

        public ApiKey() {

        }

        public static List<ApiKey> FromFile(string filename) {
            if(String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));
            if(!Common.FileExists(filename))
                throw new FileNotFoundException(nameof(filename));

            ConsoleLogger.Log("Reading API keys from " + filename);
            string contents = Common.ReadTextFile(@filename);

            if(String.IsNullOrEmpty(contents)) {
                Common.ExitApplication("ApiKey", "Unable to read contents of " + filename, -1);
                return null;
            }

            ConsoleLogger.Log("Deserializing " + filename);
            List<ApiKey> ret = null;

            try {
                ret = Common.DeserializeJson<List<ApiKey>>(contents);
                if(ret == null) {
                    Common.ExitApplication("ApiKey", "Unable to deserialize " + filename + " (null)", -1);
                    return null;
                }
            }
            catch(Exception e) {
                ConsoleLogger.Warn(("ApiKey", "Deserialization issue with " + filename + e.ToString()));
                Common.ExitApplication("ApiKey", "Unable to deserialize " + filename + " (exception)", -1);
                return null;
            }

            return ret;
        }

        public static List<ApiKey> Default() {
            return new List<ApiKey>() {
                new ApiKey() {
                    ApiKeyId = 1,
                    UserMasterId = 1,
                    Guid = System.Guid.NewGuid().ToString(),
                    Notes = "Created by setup script",
                    Active = 1,
                    Created = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    Expiration = DateTime.Now.AddDays(30)
                }
            };
        }


        #endregion
    }
}
