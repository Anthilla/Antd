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

        private static CaConfigurationModel _serviceModel => Load();
        private static readonly string _cfgFile = $"{Parameter.AntdCfgServices}/ca.conf";
        private static readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/ca.conf.bck";

        private static CaConfigurationModel Load() {
            if(!File.Exists(_cfgFile)) {
                return new CaConfigurationModel();
            }
            try {
                var text = File.ReadAllText(_cfgFile);
                var obj = JsonConvert.DeserializeObject<CaConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new CaConfigurationModel();
            }
        }

        public static void Save(CaConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(_cfgFile, text, "644", "root", "wheel");
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
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public static CaConfigurationModel Get() {
            return _serviceModel;
        }

        public static void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[ca] enabled");
        }

        public static void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[ca] disabled");
        }

        private static readonly string _caMainDirectory = $"{Parameter.AntdCfg}/ca";
        private static readonly string[] _caMainSubdirectories = {
            "certs",
            "crl",
            "newcerts",
            "private"
        };

        private static readonly string _caIntermediateDirectory = $"{Parameter.AntdCfg}/ca/intermediate";
        private static readonly string[] _caIntermediateSubdirectories = {
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
            DirectoryWithAcl.CreateDirectory(_caMainDirectory, "755", "root", "wheel");
            foreach(var dir in _caMainSubdirectories) {
                DirectoryWithAcl.CreateDirectory($"{_caMainDirectory}/{dir}", "755", "root", "wheel");
            }
            Bash.Execute($"chmod 700 ${_caMainDirectory}/private");
            if(!File.Exists($"{_caMainDirectory}/index.txt")) {
                FileWithAcl.WriteAllText($"{_caMainDirectory}/index.txt", "", "644", "root", "wheel");
            }
            if(!File.Exists($"{_caMainDirectory}/serial")) {
                FileWithAcl.WriteAllText($"{_caMainDirectory}/serial", "1000", "644", "root", "wheel");
            }
        }

        public static void PrepareConfigurationFile() {
            // /data/ca/openssl.cnf
            if(!File.Exists($"{_caMainDirectory}/openssl.cnf")) {
                FileWithAcl.WriteAllLines($"{_caMainDirectory}/openssl.cnf", CaConfigurationFiles.RootCaOpensslCnf(_caMainDirectory), "644", "root", "wheel");
            }
        }

        public static void PrepareRootKey() {
            //#Create the root key
            //cd /data/ca
            //openssl genrsa -aes256 -out private/ca.key.pem -passout pass:$passout 4096
            //	Enter pass phrase for ca.key.pem: secretpassword
            //	Verifying - Enter pass phrase for ca.key.pem: secretpassword
            //chmod 400 private/ca.key.pem
            var file = $"{_caMainDirectory}/private/ca.key.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{_serviceModel.KeyPassout} 4096");
                Bash.Execute($"chmod 400 ${file}");
            }
        }

        public static void PrepareRootCertificate() {
            //cd /data/ca
            //openssl req -config openssl.cnf -key private/ca.key.pem -new -x509 -days 7300 -sha256 -extensions v3_ca -out certs/ca.cert.pem -passin pass:Anthilla -subj "/C=IT/ST=Milano/L=casamia/O=anthilla/OU=anthilla/CN=root/emailAddress=damiano.zanardi@anthilla.com"
            //chmod 444 certs/ca.cert.pem
            var key = $"{_caMainDirectory}/private/ca.key.pem";
            var config = $"{_caMainDirectory}/openssl.cnf";
            var file = $"{_caMainDirectory}/certs/ca.cert.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -x509 -days 7300 -sha256 -extensions v3_ca -out {file} -passin pass:{_serviceModel.KeyPassout} -subj \"/C={_serviceModel.RootCountryName}/ST={_serviceModel.RootStateOrProvinceName}/L={_serviceModel.RootLocalityName}/O={_serviceModel.RootOrganizationName}/OU={_serviceModel.RootOrganizationalUnitName}/CN={_serviceModel.RootCommonName}/emailAddress={_serviceModel.RootEmailAddress}\"");
                Bash.Execute($"chmod 444 ${file}");
            }
        }

        public static bool VerifyRootCertificate() {
            //openssl x509 -noout -text -in certs/ca.cert.pem
            var file = $"{_caMainDirectory}/certs/ca.cert.pem";
            Bash.Execute($"openssl x509 -noout -text -in {file}");
            return true;
        }
        #endregion

        #region [    ca - Intermediate    ]
        public static void PrepareIntermediateDirectory() {
            DirectoryWithAcl.CreateDirectory(_caIntermediateDirectory, "755", "root", "wheel");
            foreach(var dir in _caIntermediateSubdirectories) {
                DirectoryWithAcl.CreateDirectory($"{_caIntermediateDirectory}/{dir}", "755", "root", "wheel");
            }
            Bash.Execute($"chmod 700 ${_caIntermediateDirectory}/private");
            if(!File.Exists($"{_caIntermediateDirectory}/index.txt")) {
                FileWithAcl.WriteAllText($"{_caIntermediateDirectory}/index.txt", "", "644", "root", "wheel");
            }
            if(!File.Exists($"{_caIntermediateDirectory}/serial")) {
                FileWithAcl.WriteAllText($"{_caIntermediateDirectory}/serial", "1000", "644", "root", "wheel");
            }
            if(!File.Exists($"{_caIntermediateDirectory}/crlnumber")) {
                FileWithAcl.WriteAllText($"{_caIntermediateDirectory}/crlnumber", "1000", "644", "root", "wheel");
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
            if(!File.Exists($"{_caIntermediateDirectory}/openssl.cnf")) {
                var applicationSetting = new AppConfiguration().Get();
                FileWithAcl.WriteAllLines($"{_caIntermediateDirectory}/openssl.cnf", CaConfigurationFiles.IntermediateCaOpensslCnf(_caIntermediateDirectory, $"http://{GetThisIp()}:{applicationSetting.AntdPort}/services/ca/crl"), "644", "root", "wheel");
            }
        }

        public static void PrepareIntermediateKey() {
            var file = $"{_caIntermediateDirectory}/private/intermediate.key.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{_serviceModel.KeyPassout} 4096");
                Bash.Execute($"chmod 400 ${file}");
            }
        }

        public static void PrepareIntermediateCertificate() {
            var key = $"{_caIntermediateDirectory}/private/intermediate.key.pem";
            var config = $"{_caIntermediateDirectory}/openssl.cnf";
            var file = $"{_caIntermediateDirectory}/csr/intermediate.csr.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl req -config {config} -new -sha256 -key {key} -out {file} -passin pass:{_serviceModel.KeyPassout} -subj \"/C={_serviceModel.RootCountryName}/ST={_serviceModel.RootStateOrProvinceName}/L={_serviceModel.RootLocalityName}/O={_serviceModel.RootOrganizationName}/OU={_serviceModel.RootOrganizationalUnitName}/CN={_serviceModel.RootCommonName}/emailAddress={_serviceModel.RootEmailAddress}\"");
            }
            config = $"{_caMainDirectory}/openssl.cnf";
            var fileOut = $"{_caIntermediateDirectory}/certs/intermediate.cert.pem";
            if(!File.Exists(fileOut)) {
                Bash.Execute($"openssl ca -batch -config {config} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{_serviceModel.KeyPassout} -in {file} -out {fileOut}");
                Bash.Execute($"chmod 444 ${file}");
            }
        }

        public static bool VerifyIntermediateCertificate() {
            var file = $"{_caIntermediateDirectory}/certs/intermediate.cert.pem";
            Bash.Execute($"openssl x509 -noout -text -in {file}");
            var fileCa = $"{_caMainDirectory}/certs/ca.cert.pem";
            Bash.Execute($"openssl verify -CAfile {fileCa} {file}");
            return true;
        }

        public static void CreateCertificateChain() {
            var file1 = $"{_caMainDirectory}/certs/ca.cert.pem";
            var line1 = File.ReadAllLines(file1);
            var file2 = $"{_caIntermediateDirectory}/certs/intermediate.cert.pem";
            var line2 = File.ReadAllLines(file2);
            line2.ToList().AddRange(line1);
            var chain = $"{_caIntermediateDirectory}/certs/ca-chain.cert.pem";
            if(!File.Exists(chain)) {
                FileWithAcl.WriteAllLines(chain, line2, "644", "root", "wheel");
            }
        }
        #endregion

        #region [    ca - Certificate    ]
        public static void CreateUserCertificate(string name, string passphrase, string email, string c, string st, string l, string o, string ou) {
            var config = $"{_caIntermediateDirectory}/openssl.cnf";
            var key = $"{_caIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{_caIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{_caIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions usr_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static void CreateServerCertificate(string name, string passphrase, string email, string c, string st, string l, string o, string ou) {
            var config = $"{_caIntermediateDirectory}/openssl.cnf";
            var key = $"{_caIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{_caIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{_caIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions server_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static void CreateDomainControllerCertificate(string name, string passphrase, string dcGuid, string dcDns, string email, string c, string st, string l, string o, string ou) {
            var config = $"{_caIntermediateDirectory}/{name}.openssl.cnf";
            if(!File.Exists(config)) {
                var applicationSetting = new AppConfiguration().Get();
                FileWithAcl.WriteAllLines(config, CaConfigurationFiles.IntermediateCaDomainControllerOpensslCnf(
                        _caIntermediateDirectory,
                        $"http://{GetThisIp()}:{applicationSetting.AntdPort}/services/ca/crl",
                        dcGuid,
                        dcDns
                    ), "644", "root", "wheel");
            }
            var key = $"{_caIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{_caIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{_caIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions usr_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static void CreateSmartCardCertificate(string name, string passphrase, string upn, string email, string c, string st, string l, string o, string ou) {
            var config = $"{_caIntermediateDirectory}/{name}.openssl.cnf";
            if(!File.Exists(config)) {
                var applicationSetting = new AppConfiguration().Get();
                FileWithAcl.WriteAllLines(config, CaConfigurationFiles.IntermediateCaSmartCardOpensslCnf(
                        _caIntermediateDirectory,
                        $"http://{GetThisIp()}:{applicationSetting.AntdPort}/services/ca/crl",
                        upn
                    ), "644", "root", "wheel");
            }
            var key = $"{_caIntermediateDirectory}/private/{name}.key.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl genrsa -aes256 -out {key} -passout pass:{passphrase} 2048");
                Bash.Execute($"chmod 400 ${key}");
            }
            var csr = $"{_caIntermediateDirectory}/csr/{name}.csr.pem";
            if(!File.Exists(key)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -sha256 -out {csr} -passin pass:{passphrase} -subj \"/C={c}/ST={st}/L={l}/O={o}/OU={ou}/CN={name}/emailAddress={email}\"");
            }
            var cert = $"{_caIntermediateDirectory}/certs/{name}.cert.pem";
            if(!File.Exists(cert)) {
                Bash.Execute($"openssl ca -config {config} -extensions usr_cert -days 375 -notext -md sha256 -in {csr} -out {cert}");
                Bash.Execute($"chmod 444 ${cert}");
            }
        }

        public static bool VerifyCertificate(string name) {
            var cert = $"{_caIntermediateDirectory}/certs/{name}.cert.pem";
            Bash.Execute($"openssl x509 -noout -text -in {cert}");
            var chain = $"{_caIntermediateDirectory}/certs/ca-chain.cert.pem";
            Bash.Execute($"openssl verify -CAfile {chain} {cert}");
            return true;
        }
        #endregion

        #region [    ca - Revocation List    ]
        public static void CreateCrl() {
            var config = $"{_caIntermediateDirectory}/openssl.cnf";
            var crl = $"{_caIntermediateDirectory}/crl/intermediate.crl.pem";
            if(!File.Exists(crl)) {
                Bash.Execute($"openssl ca -config {config} -gencrl -out {crl}");
            }
        }

        public static string[] CheckCrl() {
            var crl = $"{_caIntermediateDirectory}/crl/intermediate.crl.pem";
            return Bash.Execute($"openssl crl -in {crl} -noout -text").SplitBash().ToArray();
        }
        #endregion
    }
}
