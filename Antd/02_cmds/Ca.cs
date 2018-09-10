using System.IO;
using anthilla.core;
using System.Linq;

namespace Antd.cmds {

    /// <summary>
    /// TODO
    /// Converti Bash in CommonProcess
    /// </summary>
    public class Ca {

        private const string serviceName = "dhcpd4.service";
        private const string dhcpdFile = "/etc/dhcp/dhcpd.conf";

        public static void Apply() {
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
        }

        private const string localAddress = "127.0.0.1";

        private static readonly string CaMainDirectory = $"{Const.AntdCfg}/ca";
        private static readonly string[] CaMainSubdirectories = {
            "certs",
            "crl",
            "newcerts",
            "private"
        };

        private static readonly string CaIntermediateDirectory = $"{Const.AntdCfg}/ca/intermediate";
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
            Directory.CreateDirectory(CaMainDirectory);
            foreach(var dir in CaMainSubdirectories) {
                Directory.CreateDirectory($"{CaMainDirectory}/{dir}");
            }
            Bash.Execute($"chmod 700 ${CaMainDirectory}/private");
            if(!File.Exists($"{CaMainDirectory}/index.txt")) {
                File.WriteAllText($"{CaMainDirectory}/index.txt", "");
            }
            if(!File.Exists($"{CaMainDirectory}/serial")) {
                File.WriteAllText($"{CaMainDirectory}/serial", "1000");
            }
        }

        public static void PrepareConfigurationFile() {
            // /data/ca/openssl.cnf
            if(!File.Exists($"{CaMainDirectory}/openssl.cnf")) {
                File.WriteAllLines($"{CaMainDirectory}/openssl.cnf", RootCaOpensslCnf(CaMainDirectory));
            }
        }

        public static void PrepareRootKey() {
            var options = Application.CurrentConfiguration.Services.Ca;
            if(options == null) {
                return;
            }
            //#Create the root key
            //cd /data/ca
            //openssl genrsa -aes256 -out private/ca.key.pem -passout pass:$passout 4096
            //	Enter pass phrase for ca.key.pem: secretpassword
            //	Verifying - Enter pass phrase for ca.key.pem: secretpassword
            //chmod 400 private/ca.key.pem
            var file = $"{CaMainDirectory}/private/ca.key.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{options.KeyPassout} 4096");
                Bash.Execute($"chmod 400 ${file}");
            }
        }

        public static void PrepareRootCertificate() {
            var options = Application.CurrentConfiguration.Services.Ca;
            if(options == null) {
                return;
            }
            //cd /data/ca
            //openssl req -config openssl.cnf -key private/ca.key.pem -new -x509 -days 7300 -sha256 -extensions v3_ca -out certs/ca.cert.pem -passin pass:Anthilla -subj "/C=IT/ST=Milano/L=casamia/O=anthilla/OU=anthilla/CN=root/emailAddress=damiano.zanardi@anthilla.com"
            //chmod 444 certs/ca.cert.pem
            var key = $"{CaMainDirectory}/private/ca.key.pem";
            var config = $"{CaMainDirectory}/openssl.cnf";
            var file = $"{CaMainDirectory}/certs/ca.cert.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl req -config {config} -key {key} -new -x509 -days 7300 -sha256 -extensions v3_ca -out {file} -passin pass:{options.KeyPassout} -subj \"/C={options.RootCountryName}/ST={options.RootStateOrProvinceName}/L={options.RootLocalityName}/O={options.RootOrganizationName}/OU={options.RootOrganizationalUnitName}/CN={options.RootCommonName}/emailAddress={options.RootEmailAddress}\"");
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
            Directory.CreateDirectory(CaIntermediateDirectory);
            foreach(var dir in CaIntermediateSubdirectories) {
                Directory.CreateDirectory($"{CaIntermediateDirectory}/{dir}");
            }
            Bash.Execute($"chmod 700 ${CaIntermediateDirectory}/private");
            if(!File.Exists($"{CaIntermediateDirectory}/index.txt")) {
                File.WriteAllText($"{CaIntermediateDirectory}/index.txt", "");
            }
            if(!File.Exists($"{CaIntermediateDirectory}/serial")) {
                File.WriteAllText($"{CaIntermediateDirectory}/serial", "1000");
            }
            if(!File.Exists($"{CaIntermediateDirectory}/crlnumber")) {
                File.WriteAllText($"{CaIntermediateDirectory}/crlnumber", "1000");
            }
        }

        private static string GetThisIp() {
            return Network.GetAllLocalAddress().FirstOrDefault(_ => CommonString.AreEquals(_, localAddress) == false);
        }

        public static void PrepareIntermediateConfigurationFile() {
            if(!File.Exists($"{CaIntermediateDirectory}/openssl.cnf")) {
                var port = Application.CurrentConfiguration.WebService.Port;
                File.WriteAllLines($"{CaIntermediateDirectory}/openssl.cnf", IntermediateCaOpensslCnf(CaIntermediateDirectory, $"http://{GetThisIp()}:{port}/services/ca/crl"));
            }
        }

        public static void PrepareIntermediateKey() {
            var options = Application.CurrentConfiguration.Services.Ca;
            if(options == null) {
                return;
            }
            var file = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl genrsa -aes256 -out {file} -passout pass:{options.KeyPassout} 4096");
                Bash.Execute($"chmod 400 ${file}");
            }
        }

        public static void PrepareIntermediateCertificate() {
            var options = Application.CurrentConfiguration.Services.Ca;
            if(options == null) {
                return;
            }
            var key = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
            var config = $"{CaIntermediateDirectory}/openssl.cnf";
            var file = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
            if(!File.Exists(file)) {
                Bash.Execute($"openssl req -config {config} -new -sha256 -key {key} -out {file} -passin pass:{options.KeyPassout} -subj \"/C={options.RootCountryName}/ST={options.RootStateOrProvinceName}/L={options.RootLocalityName}/O={options.RootOrganizationName}/OU={options.RootOrganizationalUnitName}/CN={options.RootCommonName}/emailAddress={options.RootEmailAddress}\"");
            }
            config = $"{CaMainDirectory}/openssl.cnf";
            var fileOut = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
            if(!File.Exists(fileOut)) {
                Bash.Execute($"openssl ca -batch -config {config} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{options.KeyPassout} -in {file} -out {fileOut}");
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
                File.WriteAllLines(chain, line2);
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
                var port = Application.CurrentConfiguration.WebService.Port;
                File.WriteAllLines(config, IntermediateCaDomainControllerOpensslCnf(
                        CaIntermediateDirectory,
                        $"http://{GetThisIp()}:{port}/services/ca/crl",
                        dcGuid,
                        dcDns
                    ));
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
                var port = Application.CurrentConfiguration.WebService.Port;
                File.WriteAllLines(config, IntermediateCaSmartCardOpensslCnf(
                        CaIntermediateDirectory,
                        $"http://{GetThisIp()}:{port}/services/ca/crl",
                        upn
                    ));
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
            return Bash.Execute($"openssl crl -in {crl} -noout -text").Split().ToArray();
        }
        #endregion

        #region [    ca - Create Files    ]
        public static string[] RootCaOpensslCnf(string root) {
            return new[] {
                "[ ca ]",
                "# `man ca`",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir               = {root}",
                "certs             = $dir/certs",
                "crl_dir           = $dir/crl",
                "new_certs_dir     = $dir/newcerts",
                "database          = $dir/index.txt",
                "serial            = $dir/serial",
                "RANDFILE          = $dir/private/.rand",
                "private_key       = $dir/private/ca.key.pem",
                "certificate       = $dir/certs/ca.cert.pem",
                "",
                "crlnumber         = $dir/crlnumber",
                "crl               = $dir/crl/ca.crl.pem",
                "crl_extensions    = crl_ext",
                "default_crl_days  = 30",
                "",
                "default_md        = sha256",
                "",
                "name_opt          = ca_default",
                "cert_opt          = ca_default",
                "default_days      = 375",
                "preserve          = no",
                "policy            = policy_strict",
                "",
                "[ policy_strict ]",
                "countryName             = match",
                "stateOrProvinceName     = match",
                "organizationName        = match",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ policy_loose ]",
                "countryName             = optional",
                "stateOrProvinceName     = optional",
                "localityName            = optional",
                "organizationName        = optional",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ req ]",
                "default_bits        = 2048",
                "distinguished_name  = req_distinguished_name",
                "string_mask         = utf8only",
                "",
                "default_md          = sha256",
                "",
                "x509_extensions     = v3_ca",
                "",
                "[ req_distinguished_name ]",
                "countryName                     = Country Name (2 letter code)",
                "stateOrProvinceName             = State or Province Name",
                "localityName                    = Locality Name",
                "0.organizationName              = Organization Name",
                "organizationalUnitName          = Organizational Unit Name",
                "commonName                      = Common Name",
                "emailAddress                    = Email Address",
                "",
                "countryName_default             =",
                "stateOrProvinceName_default     =",
                "localityName_default            =",
                "0.organizationName_default      =",
                "organizationalUnitName_default  =",
                "emailAddress_default            =",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ v3_intermediate_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true, pathlen:0",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ usr_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = client, email",
                "nsComment = \"OpenSSL Generated Client Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, nonRepudiation, digitalSignature, keyEncipherment",
                "extendedKeyUsage = clientAuth, emailProtection",
                "",
                "[ server_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = server",
                "nsComment = \"OpenSSL Generated Server Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer:always",
                "keyUsage = critical, digitalSignature, keyEncipherment",
                "extendedKeyUsage = serverAuth",
                "",
                "[ crl_ext ]",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ ocsp ]",
                "basicConstraints = CA:FALSE",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, digitalSignature",
                "extendedKeyUsage = critical, OCSPSigning"
            };
        }

        public static string[] IntermediateCaOpensslCnf(string root, string crl) {
            return new[] {
                "[ ca ]",
                "# `man ca`",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir               = {root}",
                "certs             = $dir/certs",
                "crl_dir           = $dir/crl",
                "new_certs_dir     = $dir/newcerts",
                "database          = $dir/index.txt",
                "serial            = $dir/serial",
                "RANDFILE          = $dir/private/.rand",
                "",
                "private_key       = $dir/private/intermediate.key.pem",
                "certificate       = $dir/certs/intermediate.cert.pem",
                "",
                "crlnumber         = $dir/crlnumber",
                "crl               = $dir/crl/intermediate.crl.pem",
                "crl_extensions    = crl_ext",
                "default_crl_days  = 30",
                "",
                "default_md        = sha256",
                "",
                "name_opt          = ca_default",
                "cert_opt          = ca_default",
                "default_days      = 375",
                "preserve          = no",
                "policy            = policy_loose",
                "",
                "[ policy_strict ]",
                "countryName             = match",
                "stateOrProvinceName     = match",
                "organizationName        = match",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ policy_loose ]",
                "countryName             = optional",
                "stateOrProvinceName     = optional",
                "localityName            = optional",
                "organizationName        = optional",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ req ]",
                "default_bits        = 2048",
                "distinguished_name  = req_distinguished_name",
                "string_mask         = utf8only",
                "",
                "default_md          = sha256",
                "",
                "x509_extensions     = v3_ca",
                "",
                "[ req_distinguished_name ]",
                "countryName                     = Country Name (2 letter code)",
                "stateOrProvinceName             = State or Province Name",
                "localityName                    = Locality Name",
                "0.organizationName              = Organization Name",
                "organizationalUnitName          = Organizational Unit Name",
                "commonName                      = Common Name",
                "emailAddress                    = Email Address",
                "",
                "countryName_default             =",
                "stateOrProvinceName_default     =",
                "localityName_default            =",
                "0.organizationName_default      =",
                "organizationalUnitName_default  =",
                "emailAddress_default            =",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ v3_intermediate_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true, pathlen:0",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ usr_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = client, email",
                "nsComment = \"OpenSSL Generated Client Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, nonRepudiation, digitalSignature, keyEncipherment",
                "extendedKeyUsage = clientAuth, emailProtection",
                $"crlDistributionPoints = URI:{crl}",
                "",
                "[ server_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = server",
                "nsComment = \"OpenSSL Generated Server Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer:always",
                "keyUsage = critical, digitalSignature, keyEncipherment",
                "extendedKeyUsage = serverAuth",
                $"crlDistributionPoints = URI:{crl}",
                "",
                "[ crl_ext ]",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ ocsp ]",
                "basicConstraints = CA:FALSE",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, digitalSignature",
                "extendedKeyUsage = critical, OCSPSigning"
            };
        }

        public static string[] IntermediateCaDomainControllerOpensslCnf(string root, string crl, string dcGuid, string dcDns) {
            return new[] {
                $"CRLDISTPT = {crl}",
                $"DOMAINCONTROLLERGUID = {dcGuid}",
                $"DOMAINDNS = DNS:{dcDns}",
                "",
                "[ ca ]",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir = {root}",
                "certs = $dir/certs",
                "crl_dir = $dir/crl",
                "new_certs_dir = $dir/newcerts",
                "database = $dir/index.txt",
                "serial = $dir/serial",
                "RANDFILE = $dir/private/.rand",
                "private_key = $dir/private/intermediate.key.pem",
                "certificate = $dir/certs/intermediate.cert.pem",
                "crlnumber = $dir/crlnumber",
                "crl = $dir/crl/intermediate.crl.pem",
                "crl_extensions = crl_ext",
                "default_crl_days = 30",
                "default_md = sha256",
                "name_opt = ca_default",
                "cert_opt = ca_default",
                "default_days = 730",
                "preserve = no",
                "policy = policy_loose",
                "x509_extensions = usr_cert_mskdc",
                "",
                "[ policy_loose ]",
                "countryName = optional",
                "stateOrProvinceName = optional",
                "localityName = optional",
                "organizationName = optional",
                "organizationalUnitName = optional",
                "commonName = supplied",
                "emailAddress = optional",
                "",
                "[ req ]",
                "default_bits = 2048",
                "default_keyfile = privkey.pem",
                "distinguished_name = req_distinguished_name",
                "attributes = req_attributes",
                "x509_extensions = v3_ca",
                "string_mask = utf8only",
                "",
                "[ req_distinguished_name ]",
                "countryName = Country Name (2 letter code)",
                "stateOrProvinceName = State or Province Name",
                "localityName = Locality Name",
                "0.organizationName = Organization Name",
                "organizationalUnitName = Organizational Unit Name",
                "commonName = Common Name",
                "emailAddress = Email Address",
                "countryName_default = IT",
                "stateOrProvinceName_default = Varese",
                "localityName_default = Caronno Pertusella",
                "0.organizationName_default = AnthillaSRL",
                "organizationalUnitName_default =",
                "emailAddress_default =",
                "",
                "[ v3_req ]",
                "basicConstraints = CA:FALSE",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid:always,issuer",
                "basicConstraints = CA:true",
                "keyUsage = cRLSign, keyCertSign",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = sslCA, emailCA",
                "subjectAltName=email:copy",
                "issuerAltName=issuer:copy",
                "",
                "[ crl_ext ]",
                "issuerAltName=issuer:copy",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ usr_cert_mskdc ]",
                "basicConstraints=CA:FALSE",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = server",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "nsComment = \"Domain Controller Certificate\"",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid,issuer",
                "subjectAltName=@dc_subjalt",
                "issuerAltName=issuer:copy",
                "nsCaRevocationUrl = $CRLDISTPT",
                "extendedKeyUsage = clientAuth,serverAuth",
                "",
                "[dc_subjalt]",
                "DNS=$DOMAINDNS",
                "otherName=1.3.6.1.4.1.311.25.1;FORMAT:HEX,OCTETSTRING:$DOMAINCONTROLLERGUID"
            };
        }

        public static string[] IntermediateCaSmartCardOpensslCnf(string root, string crl, string upn) {
            return new[] {
                $"CRLDISTPT = {crl}",
                $"USERPRINCIPALNAME = {upn}",
                "",
                "[ ca ]",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir = {root}",
                "certs = $dir/certs",
                "crl_dir = $dir/crl",
                "new_certs_dir = $dir/newcerts",
                "database = $dir/index.txt",
                "serial = $dir/serial",
                "RANDFILE = $dir/private/.rand",
                "private_key = $dir/private/intermediate.key.pem",
                "certificate = $dir/certs/intermediate.cert.pem",
                "crlnumber = $dir/crlnumber",
                "crl = $dir/crl/intermediate.crl.pem",
                "crl_extensions = crl_ext",
                "default_crl_days = 30",
                "default_md = sha256",
                "name_opt = ca_default",
                "cert_opt = ca_default",
                "default_days = 730",
                "preserve = no",
                "policy = policy_loose",
                "x509_extensions = usr_cert_scarduser",
                "",
                "[ policy_loose ]",
                "countryName = optional",
                "stateOrProvinceName = optional",
                "localityName = optional",
                "organizationName = optional",
                "organizationalUnitName = optional",
                "commonName = supplied",
                "emailAddress = optional",
                "",
                "[ req ]",
                "default_bits = 2048",
                "default_keyfile = privkey.pem",
                "distinguished_name = req_distinguished_name",
                "attributes = req_attributes",
                "x509_extensions = v3_ca # The extensions to add to the self signed cert",
                "string_mask = utf8only",
                "",
                "[ req_distinguished_name ]",
                "countryName = Country Name (2 letter code)",
                "stateOrProvinceName = State or Province Name",
                "localityName = Locality Name",
                "0.organizationName = Organization Name",
                "organizationalUnitName = Organizational Unit Name",
                "commonName = Common Name",
                "emailAddress = Email Address",
                "countryName_default = IT",
                "stateOrProvinceName_default = Varese",
                "localityName_default = Caronno Pertusella",
                "0.organizationName_default = AnthillaSRL",
                "organizationalUnitName_default =",
                "emailAddress_default =",
                "",
                "[ v3_req ]",
                "basicConstraints = CA:FALSE",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid:always,issuer",
                "basicConstraints = CA:true",
                "keyUsage = cRLSign, keyCertSign",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = sslCA, emailCA",
                "subjectAltName=email:copy",
                "issuerAltName=issuer:copy",
                "",
                "[ crl_ext ]",
                "issuerAltName=issuer:copy",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ usr_cert_scarduser ]",
                "basicConstraints=CA:FALSE",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = client, email",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "nsComment = \"Smart Card Login Certificate\"",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid,issuer",
                "subjectAltName=email:copy,otherName:1.3.6.1.4.1.311.20.2.3;UTF8:$USERPRINCIPALNAME",
                "issuerAltName=issuer:copy",
                "nsCaRevocationUrl = $CRLDISTPT",
                "extendedKeyUsage = clientAuth,1.3.6.1.4.1.311.20.2.2"
            };
        }
        #endregion

        public class Files {

            public static string[] RootCaOpensslCnf(string root) {
                return new[] {
                "[ ca ]",
                "# `man ca`",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir               = {root}",
                "certs             = $dir/certs",
                "crl_dir           = $dir/crl",
                "new_certs_dir     = $dir/newcerts",
                "database          = $dir/index.txt",
                "serial            = $dir/serial",
                "RANDFILE          = $dir/private/.rand",
                "private_key       = $dir/private/ca.key.pem",
                "certificate       = $dir/certs/ca.cert.pem",
                "",
                "crlnumber         = $dir/crlnumber",
                "crl               = $dir/crl/ca.crl.pem",
                "crl_extensions    = crl_ext",
                "default_crl_days  = 30",
                "",
                "default_md        = sha256",
                "",
                "name_opt          = ca_default",
                "cert_opt          = ca_default",
                "default_days      = 375",
                "preserve          = no",
                "policy            = policy_strict",
                "",
                "[ policy_strict ]",
                "countryName             = match",
                "stateOrProvinceName     = match",
                "organizationName        = match",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ policy_loose ]",
                "countryName             = optional",
                "stateOrProvinceName     = optional",
                "localityName            = optional",
                "organizationName        = optional",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ req ]",
                "default_bits        = 2048",
                "distinguished_name  = req_distinguished_name",
                "string_mask         = utf8only",
                "",
                "default_md          = sha256",
                "",
                "x509_extensions     = v3_ca",
                "",
                "[ req_distinguished_name ]",
                "countryName                     = Country Name (2 letter code)",
                "stateOrProvinceName             = State or Province Name",
                "localityName                    = Locality Name",
                "0.organizationName              = Organization Name",
                "organizationalUnitName          = Organizational Unit Name",
                "commonName                      = Common Name",
                "emailAddress                    = Email Address",
                "",
                "countryName_default             =",
                "stateOrProvinceName_default     =",
                "localityName_default            =",
                "0.organizationName_default      =",
                "organizationalUnitName_default  =",
                "emailAddress_default            =",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ v3_intermediate_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true, pathlen:0",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ usr_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = client, email",
                "nsComment = \"OpenSSL Generated Client Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, nonRepudiation, digitalSignature, keyEncipherment",
                "extendedKeyUsage = clientAuth, emailProtection",
                "",
                "[ server_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = server",
                "nsComment = \"OpenSSL Generated Server Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer:always",
                "keyUsage = critical, digitalSignature, keyEncipherment",
                "extendedKeyUsage = serverAuth",
                "",
                "[ crl_ext ]",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ ocsp ]",
                "basicConstraints = CA:FALSE",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, digitalSignature",
                "extendedKeyUsage = critical, OCSPSigning"
            };
            }

            public static string[] IntermediateCaOpensslCnf(string root, string crl) {
                return new[] {
                "[ ca ]",
                "# `man ca`",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir               = {root}",
                "certs             = $dir/certs",
                "crl_dir           = $dir/crl",
                "new_certs_dir     = $dir/newcerts",
                "database          = $dir/index.txt",
                "serial            = $dir/serial",
                "RANDFILE          = $dir/private/.rand",
                "",
                "private_key       = $dir/private/intermediate.key.pem",
                "certificate       = $dir/certs/intermediate.cert.pem",
                "",
                "crlnumber         = $dir/crlnumber",
                "crl               = $dir/crl/intermediate.crl.pem",
                "crl_extensions    = crl_ext",
                "default_crl_days  = 30",
                "",
                "default_md        = sha256",
                "",
                "name_opt          = ca_default",
                "cert_opt          = ca_default",
                "default_days      = 375",
                "preserve          = no",
                "policy            = policy_loose",
                "",
                "[ policy_strict ]",
                "countryName             = match",
                "stateOrProvinceName     = match",
                "organizationName        = match",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ policy_loose ]",
                "countryName             = optional",
                "stateOrProvinceName     = optional",
                "localityName            = optional",
                "organizationName        = optional",
                "organizationalUnitName  = optional",
                "commonName              = supplied",
                "emailAddress            = optional",
                "",
                "[ req ]",
                "default_bits        = 2048",
                "distinguished_name  = req_distinguished_name",
                "string_mask         = utf8only",
                "",
                "default_md          = sha256",
                "",
                "x509_extensions     = v3_ca",
                "",
                "[ req_distinguished_name ]",
                "countryName                     = Country Name (2 letter code)",
                "stateOrProvinceName             = State or Province Name",
                "localityName                    = Locality Name",
                "0.organizationName              = Organization Name",
                "organizationalUnitName          = Organizational Unit Name",
                "commonName                      = Common Name",
                "emailAddress                    = Email Address",
                "",
                "countryName_default             =",
                "stateOrProvinceName_default     =",
                "localityName_default            =",
                "0.organizationName_default      =",
                "organizationalUnitName_default  =",
                "emailAddress_default            =",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ v3_intermediate_ca ]",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid:always,issuer",
                "basicConstraints = critical, CA:true, pathlen:0",
                "keyUsage = critical, digitalSignature, cRLSign, keyCertSign",
                "",
                "[ usr_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = client, email",
                "nsComment = \"OpenSSL Generated Client Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, nonRepudiation, digitalSignature, keyEncipherment",
                "extendedKeyUsage = clientAuth, emailProtection",
                $"crlDistributionPoints = URI:{crl}",
                "",
                "[ server_cert ]",
                "basicConstraints = CA:FALSE",
                "nsCertType = server",
                "nsComment = \"OpenSSL Generated Server Certificate\"",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer:always",
                "keyUsage = critical, digitalSignature, keyEncipherment",
                "extendedKeyUsage = serverAuth",
                $"crlDistributionPoints = URI:{crl}",
                "",
                "[ crl_ext ]",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ ocsp ]",
                "basicConstraints = CA:FALSE",
                "subjectKeyIdentifier = hash",
                "authorityKeyIdentifier = keyid,issuer",
                "keyUsage = critical, digitalSignature",
                "extendedKeyUsage = critical, OCSPSigning"
            };
            }

            public static string[] IntermediateCaDomainControllerOpensslCnf(string root, string crl, string dcGuid, string dcDns) {
                return new[] {
                $"CRLDISTPT = {crl}",
                $"DOMAINCONTROLLERGUID = {dcGuid}",
                $"DOMAINDNS = DNS:{dcDns}",
                "",
                "[ ca ]",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir = {root}",
                "certs = $dir/certs",
                "crl_dir = $dir/crl",
                "new_certs_dir = $dir/newcerts",
                "database = $dir/index.txt",
                "serial = $dir/serial",
                "RANDFILE = $dir/private/.rand",
                "private_key = $dir/private/intermediate.key.pem",
                "certificate = $dir/certs/intermediate.cert.pem",
                "crlnumber = $dir/crlnumber",
                "crl = $dir/crl/intermediate.crl.pem",
                "crl_extensions = crl_ext",
                "default_crl_days = 30",
                "default_md = sha256",
                "name_opt = ca_default",
                "cert_opt = ca_default",
                "default_days = 730",
                "preserve = no",
                "policy = policy_loose",
                "x509_extensions = usr_cert_mskdc",
                "",
                "[ policy_loose ]",
                "countryName = optional",
                "stateOrProvinceName = optional",
                "localityName = optional",
                "organizationName = optional",
                "organizationalUnitName = optional",
                "commonName = supplied",
                "emailAddress = optional",
                "",
                "[ req ]",
                "default_bits = 2048",
                "default_keyfile = privkey.pem",
                "distinguished_name = req_distinguished_name",
                "attributes = req_attributes",
                "x509_extensions = v3_ca",
                "string_mask = utf8only",
                "",
                "[ req_distinguished_name ]",
                "countryName = Country Name (2 letter code)",
                "stateOrProvinceName = State or Province Name",
                "localityName = Locality Name",
                "0.organizationName = Organization Name",
                "organizationalUnitName = Organizational Unit Name",
                "commonName = Common Name",
                "emailAddress = Email Address",
                "countryName_default = IT",
                "stateOrProvinceName_default = Varese",
                "localityName_default = Caronno Pertusella",
                "0.organizationName_default = AnthillaSRL",
                "organizationalUnitName_default =",
                "emailAddress_default =",
                "",
                "[ v3_req ]",
                "basicConstraints = CA:FALSE",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid:always,issuer",
                "basicConstraints = CA:true",
                "keyUsage = cRLSign, keyCertSign",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = sslCA, emailCA",
                "subjectAltName=email:copy",
                "issuerAltName=issuer:copy",
                "",
                "[ crl_ext ]",
                "issuerAltName=issuer:copy",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ usr_cert_mskdc ]",
                "basicConstraints=CA:FALSE",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = server",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "nsComment = \"Domain Controller Certificate\"",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid,issuer",
                "subjectAltName=@dc_subjalt",
                "issuerAltName=issuer:copy",
                "nsCaRevocationUrl = $CRLDISTPT",
                "extendedKeyUsage = clientAuth,serverAuth",
                "",
                "[dc_subjalt]",
                "DNS=$DOMAINDNS",
                "otherName=1.3.6.1.4.1.311.25.1;FORMAT:HEX,OCTETSTRING:$DOMAINCONTROLLERGUID"
            };
            }

            public static string[] IntermediateCaSmartCardOpensslCnf(string root, string crl, string upn) {
                return new[] {
                $"CRLDISTPT = {crl}",
                $"USERPRINCIPALNAME = {upn}",
                "",
                "[ ca ]",
                "default_ca = CA_default",
                "",
                "[ CA_default ]",
                $"dir = {root}",
                "certs = $dir/certs",
                "crl_dir = $dir/crl",
                "new_certs_dir = $dir/newcerts",
                "database = $dir/index.txt",
                "serial = $dir/serial",
                "RANDFILE = $dir/private/.rand",
                "private_key = $dir/private/intermediate.key.pem",
                "certificate = $dir/certs/intermediate.cert.pem",
                "crlnumber = $dir/crlnumber",
                "crl = $dir/crl/intermediate.crl.pem",
                "crl_extensions = crl_ext",
                "default_crl_days = 30",
                "default_md = sha256",
                "name_opt = ca_default",
                "cert_opt = ca_default",
                "default_days = 730",
                "preserve = no",
                "policy = policy_loose",
                "x509_extensions = usr_cert_scarduser",
                "",
                "[ policy_loose ]",
                "countryName = optional",
                "stateOrProvinceName = optional",
                "localityName = optional",
                "organizationName = optional",
                "organizationalUnitName = optional",
                "commonName = supplied",
                "emailAddress = optional",
                "",
                "[ req ]",
                "default_bits = 2048",
                "default_keyfile = privkey.pem",
                "distinguished_name = req_distinguished_name",
                "attributes = req_attributes",
                "x509_extensions = v3_ca # The extensions to add to the self signed cert",
                "string_mask = utf8only",
                "",
                "[ req_distinguished_name ]",
                "countryName = Country Name (2 letter code)",
                "stateOrProvinceName = State or Province Name",
                "localityName = Locality Name",
                "0.organizationName = Organization Name",
                "organizationalUnitName = Organizational Unit Name",
                "commonName = Common Name",
                "emailAddress = Email Address",
                "countryName_default = IT",
                "stateOrProvinceName_default = Varese",
                "localityName_default = Caronno Pertusella",
                "0.organizationName_default = AnthillaSRL",
                "organizationalUnitName_default =",
                "emailAddress_default =",
                "",
                "[ v3_req ]",
                "basicConstraints = CA:FALSE",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "",
                "[ v3_ca ]",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid:always,issuer",
                "basicConstraints = CA:true",
                "keyUsage = cRLSign, keyCertSign",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = sslCA, emailCA",
                "subjectAltName=email:copy",
                "issuerAltName=issuer:copy",
                "",
                "[ crl_ext ]",
                "issuerAltName=issuer:copy",
                "authorityKeyIdentifier=keyid:always",
                "",
                "[ usr_cert_scarduser ]",
                "basicConstraints=CA:FALSE",
                "crlDistributionPoints=URI:$CRLDISTPT",
                "nsCertType = client, email",
                "keyUsage = nonRepudiation, digitalSignature, keyEncipherment",
                "nsComment = \"Smart Card Login Certificate\"",
                "subjectKeyIdentifier=hash",
                "authorityKeyIdentifier=keyid,issuer",
                "subjectAltName=email:copy,otherName:1.3.6.1.4.1.311.20.2.3;UTF8:$USERPRINCIPALNAME",
                "issuerAltName=issuer:copy",
                "nsCaRevocationUrl = $CRLDISTPT",
                "extendedKeyUsage = clientAuth,1.3.6.1.4.1.311.20.2.2"
            };
            }
        }
    }
}