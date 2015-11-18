//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using antdlib.Boot;
using antdlib.Common;

namespace antdlib.Certificate {
    public class CertificateAuthority {
        public static void Setup() {
            try {
                if (IsActive)
                    return;
                SetupRootCa();
                SetupIntermediateCa();
                CoreParametersConfig.EnableCa();
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.ToString());
            }
        }

        public static bool IsActive => CoreParametersConfig.GetCa() == "yes" && File.Exists(CaRootCertificate);

        private static readonly string CaDirectory = Folder.CertificateAuthority;
        private static readonly string CaRootConfFile = $"{CaDirectory}/openssl.cnf";
        private static readonly string CaRootPrivateKey = $"{CaDirectory}/private/ca.key.pem";
        private static readonly string CaRootCertificate = $"{CaDirectory}/certs/ca.cert.pem";
        private const string Passphrase = "antdca";

        private static void SetupRootCa() {
            ConsoleLogger.Log("______ Setup Root CA ______");
            ConsoleLogger.Log("1) Create directories: /ca .certs .crl .newcerts .private");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/certs");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/crl");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/newcerts");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/private");
            ConsoleLogger.Log("2) Change .private acl");
            Terminal.Terminal.Execute($"chmod 700 {CaDirectory}/private");
            ConsoleLogger.Log("3) Create index file");
            Terminal.Terminal.Execute($"touch {CaDirectory}/index.txt");
            ConsoleLogger.Log("4) Create serial file");
            Terminal.Terminal.Execute($"echo 1000 > {CaDirectory}/serial");
            ConsoleLogger.Log("5) Copy .conf file");
            if (!File.Exists(CaRootConfFile)) {
                Terminal.Terminal.Execute($"cp {Folder.Resources}/openssl.cnf {CaRootConfFile}");
            }
            ConsoleLogger.Log("6) Generate root private key");
            Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaRootPrivateKey} -passout pass:{Passphrase} 4096");
            if (!File.Exists(CaRootPrivateKey)) {
                throw new FileNotFoundException("File Not Found", CaRootPrivateKey);
            }
            ConsoleLogger.Log("7) Change private key acl");
            Terminal.Terminal.Execute($"chmod 400 {CaRootPrivateKey}");
            ConsoleLogger.Log("8) Generate root certificate");
            Terminal.Terminal.Execute($"openssl req -config {CaRootConfFile} -key {CaRootPrivateKey} -new -x509 -days 10950 -sha256 -extensions v3_ca -out {CaRootCertificate} -passin pass:{Passphrase} -subj \"/C=IT/ST=Milan/L=./O=AnthillaSRL/OU=./CN=Antd Root CA/emailAddress=.\"");
            if (!File.Exists(CaRootCertificate)) {
                throw new FileNotFoundException("File Not Found", CaRootCertificate);
            }
            ConsoleLogger.Log("9) Check root certificate");
            Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaRootCertificate}");
        }

        private static readonly string CaIntermediateDirectory = $"{CaDirectory}/intermediate";
        private static readonly string CaIntermediateConfFile = $"{CaIntermediateDirectory}/openssl.cnf";
        private static readonly string CaIntermediatePrivateKey = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
        private static readonly string CaIntermediateCertificateReq = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
        private static readonly string CaIntermediateCertificate = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
        private static readonly string CaIntermediateChain = $"{CaIntermediateDirectory}/certs/ca-chain.cert.pem";

        private static void SetupIntermediateCa() {
            ConsoleLogger.Log("______ Setup Intermediate CA ______");
            ConsoleLogger.Log("1) Create directories: /ca/intermediate .certs .crl .csr .newcerts .private");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/certs");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/crl");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/csr");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/newcerts");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/private");
            ConsoleLogger.Log("2) Change .private acl");
            Terminal.Terminal.Execute($"chmod 700 {CaIntermediateDirectory}/private");
            ConsoleLogger.Log("3) Create index file");
            Terminal.Terminal.Execute($"touch {CaIntermediateDirectory}/index.txt");
            ConsoleLogger.Log("4) Create serial file");
            Terminal.Terminal.Execute($"echo 1000 > {CaIntermediateDirectory}/serial");
            ConsoleLogger.Log("5) Create crlnumber file");
            Terminal.Terminal.Execute($"echo 1000 > {CaIntermediateDirectory}/crlnumber");
            ConsoleLogger.Log("6) Copy .conf file");
            if (!File.Exists(CaIntermediateConfFile)) {
                Terminal.Terminal.Execute($"cp {Folder.Resources}/openssl-intermediate.cnf {CaIntermediateConfFile}");
            }
            ConsoleLogger.Log("7) Generate intermediate private key");
            Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaIntermediatePrivateKey} -passout pass:{Passphrase} 4096");
            if (!File.Exists(CaIntermediatePrivateKey)) {
                throw new FileNotFoundException("File Not Found", CaIntermediatePrivateKey);
            }
            ConsoleLogger.Log("8) Change private key acl");
            Terminal.Terminal.Execute($"chmod 400 {CaIntermediatePrivateKey}");
            ConsoleLogger.Log("9) Generate intermediate cert request");
            Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {CaIntermediatePrivateKey} -new -sha256 -out {CaIntermediateCertificateReq} -passin pass:{Passphrase} -subj \"/C=IT/ST=Milan/L=./O=AnthillaSRL/OU=./CN=Antd Intermediate CA/emailAddress=.\"");
            if (!File.Exists(CaIntermediateCertificateReq)) {
                throw new FileNotFoundException("File Not Found", CaIntermediateCertificateReq);
            }
            ConsoleLogger.Log("10) Generate intermediate certificate, signed with root certificate");
            Terminal.Terminal.Execute($"openssl ca -batch -config {CaRootConfFile} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{Passphrase} -in {CaIntermediateCertificateReq} -out {CaIntermediateCertificate}");
            if (!File.Exists(CaIntermediateCertificate)) {
                throw new FileNotFoundException("File Not Found", CaIntermediateCertificate);
            }
            ConsoleLogger.Log("11) Change intermediate certificate acl");
            Terminal.Terminal.Execute($"chmod 444 {CaIntermediateCertificate}");
            ConsoleLogger.Log("12) Check intermediate certificate");
            Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaIntermediateCertificate}");
            ConsoleLogger.Log("13) Verify intermediate certificate");
            Terminal.Terminal.Execute($"openssl verify -CAfile {CaRootCertificate} {CaIntermediateCertificate}");
            ConsoleLogger.Log("14) Generate intermediate chain file");
            Terminal.Terminal.Execute($"cat {CaIntermediateCertificate} {CaRootCertificate} > {CaIntermediateChain}");
            if (!File.Exists(CaIntermediateChain)) {
                throw new FileNotFoundException("File Not Found", CaIntermediateChain);
            }
            ConsoleLogger.Log("15) Change intermediate chain file acl");
            Terminal.Terminal.Execute($"chmod 444 {CaIntermediateChain}");
        }

        //private static void Convert() {
        //    throw new NotImplementedException();
        //}

        public class Certificate {
            public static void Create(string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string commonName, string emailAddress, string passphrase, CertificateAssignment assignment, string bytesLength, string userGuid, string serviceGuid, string serviceAlias) {
                try {
                    var certName = commonName;
                    var usePassphraseForPrivateKey = passphrase.Length > 0;
                    var certificateKeyPath = $"{CaIntermediateDirectory}/private/{certName}.key.pem";
                    var certificateRequestPath = $"{CaIntermediateDirectory}/csr/{certName}.csr.pem";
                    var certificatePath = $"{CaIntermediateDirectory}/certs/{certName}.cert.pem";

                    if (usePassphraseForPrivateKey == false) {
                        Terminal.Terminal.Execute($"openssl genrsa -out {certificateKeyPath} {bytesLength}");
                        Terminal.Terminal.Execute($"chmod 400 {certificateKeyPath}");
                        Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {certificateKeyPath} -new -sha256 -out {certificateRequestPath} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={certName}/emailAddress={emailAddress}\"");
                    }
                    else {
                        Terminal.Terminal.Execute(
                            $"openssl genrsa -aes256 -passout pass:{passphrase} -out {certificateKeyPath} {bytesLength}");
                        Terminal.Terminal.Execute($"chmod 400 {certificateKeyPath}");
                        Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {certificateKeyPath} -new -sha256 -out {certificateRequestPath} -passin pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={certName}/emailAddress={emailAddress}\"");
                    }
                    Thread.Sleep(2000);

                    var certExtension = "usr_cert";
                    if (assignment == CertificateAssignment.Service) {
                        certExtension = "server_cert";
                    }
                    const int days = 375;
                    Terminal.Terminal.Execute($"openssl ca -batch -config {CaIntermediateConfFile} -extensions {certExtension} -days {days} -notext -md sha256 -passin pass:{Passphrase} -in {certificateRequestPath} -out {certificatePath}");
                    Thread.Sleep(2000);
                    Terminal.Terminal.Execute($"chmod 444 {certificatePath}");

                    var certificateDerPath = $"{CaIntermediateDirectory}/certs/{certName}.cert.cer";
                    Terminal.Terminal.Execute($"openssl x509 -in {certificatePath} -inform PEM -out {certificateDerPath} -outform DER");
                    Terminal.Terminal.Execute($"chmod 444 {certificateDerPath}");

                    var certificatePfxPath = $"{CaIntermediateDirectory}/certs/{certName}.cert.pfx";
                    Terminal.Terminal.Execute($"openssl pkcs12 -export -in {certificatePath} -inkey {certificateKeyPath} -out {certificatePfxPath} -passin pass:{passphrase} -passout pass:{passphrase}");
                    Terminal.Terminal.Execute($"chmod 444 {certificatePfxPath}");

                    //if (!File.Exists(certificatePath))
                    //    return;
                    var dt = DateTime.Now;
                    var model = new CertificateModel {
                        IsPresent = true,
                        IsRevoked = false,
                        _Id = Guid.NewGuid().ToString(),
                        CertificateGuid = Guid.NewGuid().ToString(),
                        CertificatePath = certificatePath,
                        CertificateDerPath = certificateDerPath,
                        CertificatePfxPath = certificatePfxPath,
                        CertificateCountryName = countryName,
                        CertificateStateProvinceNameh = stateProvinceName,
                        CertificateLocalityName = localityName,
                        CertificateOrganizationName = organizationName,
                        CertificateOrganizationalUnitName = organizationalUnitName,
                        CertificateCommonName = certName,
                        CertificateEmailAddress = emailAddress,
                        CertificatePassphrase = passphrase,
                        IsProtectedByPassphrase = usePassphraseForPrivateKey,
                        CertificateAuthorityLevel = CertificateAuthorityLevel.Common,
                        CertificateAssignment = assignment,
                        AssignmentUserGuid = userGuid.Split(','),
                        AssignmentServiceGuid = serviceGuid,
                        AssignmentServiceAlias = serviceAlias,
                        CertificateBytes = bytesLength,
                        ReleaseDateTime = dt,
                        ExpirationDateTime = dt.AddDays(days)
                    };
                    DeNSo.Session.New.Set(model);
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }

            public string Verify(string certificateCommonName) {
                var certificatePath = $"{CaIntermediateDirectory}/certs/{certificateCommonName}.cert.pem ";
                return Terminal.Terminal.Execute($"openssl x509 -noout -text -in {certificatePath}");
            }

            public string VerifyChained(string certificateCommonName) {
                var certificatePath = $"{CaIntermediateDirectory}/certs/{certificateCommonName}.cert.pem ";
                return Terminal.Terminal.Execute($"openssl verify -CAfile {CaIntermediateChain} {certificatePath}");
            }
        }

        public static IEnumerable<string> GetAllCertificates() {
            var indexFile = $"{CaIntermediateDirectory}/index.txt";
            if (File.Exists(indexFile)) {
                return File.ReadAllLines(indexFile);
            }
            return new List<string>();
        }
    }
}
