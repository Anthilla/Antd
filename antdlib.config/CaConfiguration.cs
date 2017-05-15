using antdlib.common;
using antdlib.config.shared;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.config {
    public class CaConfiguration {

        private static CaConfigurationModel ServiceModel => Load();
        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/ca.conf";

        private static CaConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new CaConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<CaConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new CaConfigurationModel();
            }
        }

        public static void Save(CaConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[ca] configuration saved");
        }

        public static void Set() {
            PrepareDirectory();
            PrepareConfigurationFile();
            PrepareRootKey();
            PrepareRootCertificate();
            VerifyRootCertificate();
            PrepareIntermediateDirectory();
            PrepareIntermediateConfigurationFile();
            PrepareIntermediateKey();
            PrepareIntermediateCertificate();
            VerifyIntermediateCertificate();
            CreateCertificateChain();
            CreateCrl();
            Enable();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static CaConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[ca] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[ca] disabled");
        }

        private static readonly string CaMainDirectory = $"{Parameter.AntdCfg}/ca";
        private static readonly string[] CaMainSubdirectories = {
            "certs",
            "crl",
            "newcerts",
            "private"
        };

        private static readonly string CaIntermediateDirectory = $"{Parameter.AntdCfg}/ca/intermediate";
        private static readonly string[] CaIntermediateSubdirectories = {
            "certs",
            "crl",
            "csr",
            "newcerts",
            "private"
        };

        #region [    ca - Root    ]
        public static void PrepareDirectory() {
            //mkdir /data/ca => /cfg/antd/ca
            //cd /data/ca
            //mkdir certs crl newcerts private
            //chmod 700 private
            //touch index.txt
            //echo 1000 > serial
            DirectoryWithAcl.CreateDirectory(CaMainDirectory, "755", "root", "wheel");
            foreach(var dir in CaMainSubdirectories) {
                DirectoryWithAcl.CreateDirectory($"{CaMainDirectory}/{dir}", "755", "root", "wheel");
            }
            Bash.Execute($"chmod 700 ${CaMainDirectory}/private");
            if(!File.Exists($"{CaMainDirectory}/index.txt")) {
                FileWithAcl.WriteAllText($"{CaMainDirectory}/index.txt", "", "644", "root", "wheel");
            }
            if(!File.Exists($"{CaMainDirectory}/serial")) {
                FileWithAcl.WriteAllText($"{CaMainDirectory}/serial", "1000", "644", "root", "wheel");
            }
        }

        public static void PrepareConfigurationFile() {
            // /data/ca/openssl.cnf
            if(!File.Exists($"{CaMainDirectory}/openssl.cnf")) {
                FileWithAcl.WriteAllLines($"{CaMainDirectory}/openssl.cnf", CaConfigurationFiles.RootCaOpensslCnf(CaMainDirectory), "644", "root", "wheel");
            }
        }

        public static void PrepareRootKey() {
            //#Create the root key
            //cd /data/ca
            //openssl genrsa -aes256 -out private/ca.key.pem -passout pass:$passout 4096
            //	Enter pass phrase for ca.key.pem: secretpassword
            //	Verifying - Enter pass phrase for ca.key.pem: secretpassword
            //chmod 400 private/ca.key.pem
            var file = $"{CaMainDirectory}/private/ca.key.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{ServiceModel.KeyPassout} 4096");
                Bash.Execute($"chmod 400 ${file}");
            }
        }

        public static void PrepareRootCertificate() {
            //cd /data/ca
            //openssl req -config openssl.cnf -key private/ca.key.pem -new -x509 -days 7300 -sha256 -extensions v3_ca -out certs/ca.cert.pem -passin pass:Anthilla -subj "/C=IT/ST=Milano/L=casamia/O=anthilla/OU=anthilla/CN=root/emailAddress=damiano.zanardi@anthilla.com"
            //chmod 444 certs/ca.cert.pem
            var key = $"{CaMainDirectory}/private/ca.key.pem";
            var config = $"{CaMainDirectory}/openssl.cnf";
            var file = $"{CaMainDirectory}/certs/ca.cert.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -x509 -days 7300 -sha256 -extensions v3_ca -out {file} -passin pass:{ServiceModel.KeyPassout} -subj \"/C={ServiceModel.RootCountryName}/ST={ServiceModel.RootStateOrProvinceName}/L={ServiceModel.RootLocalityName}/O={ServiceModel.RootOrganizationName}/OU={ServiceModel.RootOrganizationalUnitName}/CN={ServiceModel.RootCommonName}/emailAddress={ServiceModel.RootEmailAddress}\"");
                Bash.Execute($"chmod 444 ${file}");
            }
        }

        public static bool VerifyRootCertificate() {
            //openssl x509 -noout -text -in certs/ca.cert.pem
            var file = $"{CaMainDirectory}/certs/ca.cert.pem";
            Bash.Execute($"openssl x509 -noout -text -in {file}");
            return true;
        }
        #endregion

        #region [    ca - Intermediate    ]
        public static void PrepareIntermediateDirectory() {
            DirectoryWithAcl.CreateDirectory(CaIntermediateDirectory, "755", "root", "wheel");
            foreach(var dir in CaIntermediateSubdirectories) {
                DirectoryWithAcl.CreateDirectory($"{CaIntermediateDirectory}/{dir}", "755", "root", "wheel");
            }
            Bash.Execute($"chmod 700 ${CaIntermediateDirectory}/private");
            if(!File.Exists($"{CaIntermediateDirectory}/index.txt")) {
                FileWithAcl.WriteAllText($"{CaIntermediateDirectory}/index.txt", "", "644", "root", "wheel");
            }
            if(!File.Exists($"{CaIntermediateDirectory}/serial")) {
                FileWithAcl.WriteAllText($"{CaIntermediateDirectory}/serial", "1000", "644", "root", "wheel");
            }
            if(!File.Exists($"{CaIntermediateDirectory}/crlnumber")) {
                FileWithAcl.WriteAllText($"{CaIntermediateDirectory}/crlnumber", "1000", "644", "root", "wheel");
            }
        }

        private static string GetThisIp() {
            var ifconfig = Bash.Execute("ifconfig").SplitBash();
            var first = ifconfig.FirstOrDefault(_ => _.ToLower().Contains("inet"));
            if(first == null) {
                return "127.0.0.1";
            }
            var ip = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            var result = ip.Matches(first);
            if(result.Count < 1) {
                return "127.0.0.1";
            }
            return result[0].ToString();
        }

        public static void PrepareIntermediateConfigurationFile() {
            if(!File.Exists($"{CaIntermediateDirectory}/openssl.cnf")) {
                var applicationSetting = new AppConfiguration().Get();
                FileWithAcl.WriteAllLines($"{CaIntermediateDirectory}/openssl.cnf", CaConfigurationFiles.IntermediateCaOpensslCnf(CaIntermediateDirectory, $"http://{GetThisIp()}:{applicationSetting.AntdPort}/services/ca/crl"), "644", "root", "wheel");
            }
        }

        public static void PrepareIntermediateKey() {
            var file = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{ServiceModel.KeyPassout} 4096");
                Bash.Execute($"chmod 400 ${file}");
            }
        }

        public static void PrepareIntermediateCertificate() {
            var key = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
            var config = $"{CaIntermediateDirectory}/openssl.cnf";
            var file = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl req -config {config} -new -sha256 -key {key} -out {file} -passin pass:{ServiceModel.KeyPassout} -subj \"/C={ServiceModel.RootCountryName}/ST={ServiceModel.RootStateOrProvinceName}/L={ServiceModel.RootLocalityName}/O={ServiceModel.RootOrganizationName}/OU={ServiceModel.RootOrganizationalUnitName}/CN={ServiceModel.RootCommonName}/emailAddress={ServiceModel.RootEmailAddress}\"");
            }
            config = $"{CaMainDirectory}/openssl.cnf";
            var fileOut = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
            if(!File.Exists(fileOut)) {
                Bash.Execute($"openssl ca -batch -config {config} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{ServiceModel.KeyPassout} -in {file} -out {fileOut}");
                Bash.Execute($"chmod 444 ${file}");
            }
        }

        public static bool VerifyIntermediateCertificate() {
            var file = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
            Bash.Execute($"openssl x509 -noout -text -in {file}");
            var fileCa = $"{CaMainDirectory}/certs/ca.cert.pem";
            Bash.Execute($"openssl verify -CAfile {fileCa} {file}");
            return true;
        }

        public static void CreateCertificateChain() {
            var file1 = $"{CaMainDirectory}/certs/ca.cert.pem";
            var line1 = File.ReadAllLines(file1);
            var file2 = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
            var line2 = File.ReadAllLines(file2);
            line2.ToList().AddRange(line1);
            var chain = $"{CaIntermediateDirectory}/certs/ca-chain.cert.pem";
            if(!File.Exists(chain)) {
                FileWithAcl.WriteAllLines(chain, line2, "644", "root", "wheel");
            }
        }
        #endregion

        #region [    ca - Certificate    ]
        public static void CreateUserCertificate(string name, string passphrase, string email, string c, string st, string l, string o, string ou) {
            var config = $"{CaIntermediateDirectory}/openssl.cnf";
            var key = $"{CaIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{CaIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{CaIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions usr_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static void CreateServerCertificate(string name, string passphrase, string email, string c, string st, string l, string o, string ou) {
            var config = $"{CaIntermediateDirectory}/openssl.cnf";
            var key = $"{CaIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{CaIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{CaIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions server_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static void CreateDomainControllerCertificate(string name, string passphrase, string dcGuid, string dcDns, string email, string c, string st, string l, string o, string ou) {
            var config = $"{CaIntermediateDirectory}/{name}.openssl.cnf";
            if(!File.Exists(config)) {
                var applicationSetting = new AppConfiguration().Get();
                FileWithAcl.WriteAllLines(config, CaConfigurationFiles.IntermediateCaDomainControllerOpensslCnf(
                        CaIntermediateDirectory,
                        $"http://{GetThisIp()}:{applicationSetting.AntdPort}/services/ca/crl",
                        dcGuid,
                        dcDns
                    ), "644", "root", "wheel");
            }
            var key = $"{CaIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{CaIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{CaIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions usr_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static void CreateSmartCardCertificate(string name, string passphrase, string upn, string email, string c, string st, string l, string o, string ou) {
            var config = $"{CaIntermediateDirectory}/{name}.openssl.cnf";
            if(!File.Exists(config)) {
                var applicationSetting = new AppConfiguration().Get();
                FileWithAcl.WriteAllLines(config, CaConfigurationFiles.IntermediateCaSmartCardOpensslCnf(
                        CaIntermediateDirectory,
                        $"http://{GetThisIp()}:{applicationSetting.AntdPort}/services/ca/crl",
                        upn
                    ), "644", "root", "wheel");
            }
            var key = $"{CaIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{CaIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{CaIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions usr_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static bool VerifyCertificate(string name) {
            var cert = $"{CaIntermediateDirectory}/certs/{name}.cert.pem";
            Bash.Execute($"openssl x509 -noout -text -in {cert}");
            var chain = $"{CaIntermediateDirectory}/certs/ca-chain.cert.pem";
            Bash.Execute($"openssl verify -CAfile {chain} {cert}");
            return true;
        }
        #endregion

        #region [    ca - Revocation List    ]
        public static void CreateCrl() {
            var config = $"{CaIntermediateDirectory}/openssl.cnf";
            var crl = $"{CaIntermediateDirectory}/crl/intermediate.crl.pem";
            if(!File.Exists(crl)) {
                Bash.Execute($"openssl ca -config {config} -gencrl -out {crl}");
            }
        }

        public static string[] CheckCrl() {
            var crl = $"{CaIntermediateDirectory}/crl/intermediate.crl.pem";
            return Bash.Execute($"openssl crl -in {crl} -noout -text").SplitBash().ToArray();
        }
        #endregion
    }
}
