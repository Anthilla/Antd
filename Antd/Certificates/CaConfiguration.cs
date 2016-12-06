using System;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Tool;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.Certificates {
    public class CaConfiguration {

        private readonly CaConfigurationModel _serviceModel;
        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/ca.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/ca.conf.bck";

        public CaConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            IoDir.CreateDirectory(_caMainDirectory);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new CaConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<CaConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new CaConfigurationModel();
                }

            }
        }

        public void Save(CaConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
        }

        public void Set() {
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

            Enable();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public CaConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
        }

        private readonly string _caMainDirectory = $"{Parameter.AntdCfg}/ca";
        private readonly string[] _caMainSubdirectories = {
            "certs",
            "crl",
            "newcerts",
            "private"
        };

        private readonly string _caIntermediateDirectory = $"{Parameter.AntdCfg}/ca/intermediate";
        private readonly string[] _caIntermediateSubdirectories = {
            "certs",
            "crl",
            "csr",
            "newcerts",
            "private"
        };

        #region [    ca - Root    ]
        public void PrepareDirectory() {
            //mkdir /data/ca => /cfg/antd/ca
            //cd /data/ca
            //mkdir certs crl newcerts private
            //chmod 700 private
            //touch index.txt
            //echo 1000 > serial
            foreach(var dir in _caMainSubdirectories) {
                IoDir.CreateDirectory($"{_caMainDirectory}/{dir}");
            }
            var bash = new Bash();
            bash.Execute($"chmod 700 ${_caMainDirectory}/private");
            if(!File.Exists($"{_caMainDirectory}/index.txt")) {
                File.WriteAllText($"{_caMainDirectory}/index.txt", "");
            }
            if(!File.Exists($"{_caMainDirectory}/serial")) {
                File.WriteAllText($"{_caMainDirectory}/serial", "1000");
            }
        }

        public void PrepareConfigurationFile() {
            // /data/ca/openssl.cnf
            if(!File.Exists($"{_caMainDirectory}/openssl.cnf")) {
                File.WriteAllLines($"{_caMainDirectory}/openssl.cnf", CaConfigurationFiles.RootCaOpensslCnf(_caMainDirectory));
            }
        }

        public void PrepareRootKey() {
            //#Create the root key
            //cd /data/ca
            //openssl genrsa -aes256 -out private/ca.key.pem -passout pass:$passout 4096
            //	Enter pass phrase for ca.key.pem: secretpassword
            //	Verifying - Enter pass phrase for ca.key.pem: secretpassword
            //chmod 400 private/ca.key.pem
            var file = $"{_caMainDirectory}/private/ca.key.pem";
            var bash = new Bash();
            if(!File.Exists(file)) {
                bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{_serviceModel.KeyPassout} 4096");
                bash.Execute($"chmod 400 ${file}");
            }
        }

        public void PrepareRootCertificate() {
            //cd /data/ca
            //openssl req -config openssl.cnf -key private/ca.key.pem -new -x509 -days 7300 -sha256 -extensions v3_ca -out certs/ca.cert.pem -passin pass:Anthilla -subj "/C=IT/ST=Milano/L=casamia/O=anthilla/OU=anthilla/CN=root/emailAddress=damiano.zanardi@anthilla.com"
            //chmod 444 certs/ca.cert.pem
            var key = $"{_caMainDirectory}/private/ca.key.pem";
            var config = $"{_caMainDirectory}/openssl.cnf";
            var file = $"{_caMainDirectory}/certs/ca.cert.pem";
            var bash = new Bash();
            if(!File.Exists(file)) {
                bash.Execute($"openssl req -config {config} -key {key} -new -x509 -days 7300 -sha256 -extensions v3_ca -out {file} -passin pass:{_serviceModel.KeyPassout} -subj \"/C={_serviceModel.RootCountryName}/ST={_serviceModel.RootStateOrProvinceName}/L={_serviceModel.RootLocalityName}/O={_serviceModel.RootOrganizationName}/OU={_serviceModel.RootOrganizationalUnitName}/CN={_serviceModel.RootCommonName}/emailAddress={_serviceModel.RootEmailAddress}\"");
                bash.Execute($"chmod 444 ${file}");
            }
        }

        public bool VerifyRootCertificate() {
            //openssl x509 -noout -text -in certs/ca.cert.pem
            var bash = new Bash();
            var file = $"{_caMainDirectory}/certs/ca.cert.pem";
            bash.Execute($"openssl x509 -noout -text -in {file}");
            return true;
        }
        #endregion

        #region [    ca - Intermediate    ]
        public void PrepareIntermediateDirectory() {
            //mkdir /root/ca/intermediate
            //cd /root/ca/intermediate
            //mkdir certs crl csr newcerts private
            //chmod 700 private
            //touch index.txt
            //echo 1000 > serial
            //echo 1000 > /root/ca/intermediate/crlnumber
            IoDir.CreateDirectory(_caIntermediateDirectory);
            foreach(var dir in _caIntermediateSubdirectories) {
                IoDir.CreateDirectory($"{_caIntermediateDirectory}/{dir}");
            }
            var bash = new Bash();
            bash.Execute($"chmod 700 ${_caIntermediateDirectory}/private");
            if(!File.Exists($"{_caIntermediateDirectory}/index.txt")) {
                File.WriteAllText($"{_caIntermediateDirectory}/index.txt", "");
            }
            if(!File.Exists($"{_caIntermediateDirectory}/serial")) {
                File.WriteAllText($"{_caIntermediateDirectory}/serial", "1000");
            }
            if(!File.Exists($"{_caIntermediateDirectory}/crlnumber")) {
                File.WriteAllText($"{_caIntermediateDirectory}/crlnumber", "1000");
            }
        }

        public void PrepareIntermediateConfigurationFile() {
            // /data/ca/intermediate/openssl.cnf
            if(!File.Exists($"{_caIntermediateDirectory}/openssl.cnf")) {
                File.WriteAllLines($"{_caIntermediateDirectory}/openssl.cnf", CaConfigurationFiles.IntermediateCaOpensslCnf(_caIntermediateDirectory));
            }
        }

        public void PrepareIntermediateKey() {
            //#Create the root key
            //cd /data/ca
            //openssl genrsa -aes256 -out private/ca.key.pem -passout pass:$passout 4096
            //	Enter pass phrase for ca.key.pem: secretpassword
            //	Verifying - Enter pass phrase for ca.key.pem: secretpassword
            //chmod 400 private/ca.key.pem
            var file = $"{_caIntermediateDirectory}/private/intermediate.key.pem";
            var bash = new Bash();
            if(!File.Exists(file)) {
                bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{_serviceModel.KeyPassout} 4096");
                bash.Execute($"chmod 400 ${file}");
            }
        }

        public void PrepareIntermediateCertificate() {
            var key = $"{_caIntermediateDirectory}/private/intermediate.key.pem";
            var config = $"{_caIntermediateDirectory}/openssl.cnf";
            var file = $"{_caIntermediateDirectory}/csr/intermediate.csr.pem";
            var bash = new Bash();
            if(!File.Exists(file)) {
                //ConsoleLogger.Log($"openssl req -config {config} -new -sha256 -key {key} -out {file} -passin pass:{_serviceModel.KeyPassout} -subj \"/C={_serviceModel.RootCountryName}/ST={_serviceModel.RootStateOrProvinceName}/L={_serviceModel.RootLocalityName}/O={_serviceModel.RootOrganizationName}/OU={_serviceModel.RootOrganizationalUnitName}/CN={_serviceModel.RootCommonName}/emailAddress={_serviceModel.RootEmailAddress}\"");
                bash.Execute($"openssl req -config {config} -new -sha256 -key {key} -out {file} -passin pass:{_serviceModel.KeyPassout} -subj \"/C={_serviceModel.RootCountryName}/ST={_serviceModel.RootStateOrProvinceName}/L={_serviceModel.RootLocalityName}/O={_serviceModel.RootOrganizationName}/OU={_serviceModel.RootOrganizationalUnitName}/CN={_serviceModel.RootCommonName}/emailAddress={_serviceModel.RootEmailAddress}\"");
            }
            config = $"{_caMainDirectory}/openssl.cnf";
            var fileOut = $"{_caIntermediateDirectory}/certs/intermediate.cert.pem";
            if(!File.Exists(fileOut)) {
                //ConsoleLogger.Log($"openssl ca -batch -config {config} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{_serviceModel.KeyPassout} -in {file} -out {fileOut}");
                bash.Execute($"openssl ca -batch -config {config} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{_serviceModel.KeyPassout} -in {file} -out {fileOut}");
                bash.Execute($"chmod 444 ${file}");
            }
        }

        public bool VerifyIntermediateCertificate() {
            var bash = new Bash();
            var file = $"{_caIntermediateDirectory}/certs/intermediate.cert.pem";
            bash.Execute($"openssl x509 -noout -text -in {file}");
            var fileCa = $"{_caMainDirectory}/certs/ca.cert.pem";
            bash.Execute($"openssl verify -CAfile {fileCa} {file}");
            return true;
        }

        public void CreateCertificateChain() {
            var file1 = $"{_caMainDirectory}/certs/ca.cert.pem";
            var line1 = File.ReadAllLines(file1);
            var file2 = $"{_caIntermediateDirectory}/certs/intermediate.cert.pem";
            var line2 = File.ReadAllLines(file2);
            line2.ToList().AddRange(line1);
            var chain = $"{_caIntermediateDirectory}/certs/ca-chain.cert.pem";
            if(!File.Exists(chain)) {
                File.WriteAllLines(chain, line2);
            }
        }
        #endregion

        #region [    ca - Certificate    ]

        

        #endregion
    }
}
