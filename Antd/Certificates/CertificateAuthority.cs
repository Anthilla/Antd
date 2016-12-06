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
using System.IO;
using antdlib;
using antdlib.common;
using antdlib.common.Tool;

namespace Antd.Certificates {
    public class CertificateAuthority {
        public bool IsActive => true;

        private static readonly string CaDirectory = new ApplicationSetting().CaPath();

        private readonly string CaRootConfFile = $"{CaDirectory}/openssl.cnf";
        private readonly string CaRootPrivateKey = $"{CaDirectory}/private/ca.key.pem";
        private readonly string CaRootCertificate = $"{CaDirectory}/certs/ca.cert.pem";

        private string _caCountry;
        private string _caProvince;
        private string _caLocality;
        private string _caOrganization;
        private string _caOrganizationalUnit;
        private string _caCommonName;
        private string _caEmail;

        private string _caIntermediateCommonName;

        private static readonly string CaIntermediateDirectory = $"{CaDirectory}/intermediate";
        private static readonly string CaIntermediateConfFile = $"{CaIntermediateDirectory}/openssl.cnf";
        private readonly string CaIntermediatePrivateKey = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
        private readonly string CaIntermediateCertificateReq = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
        private readonly string CaIntermediateCertificate = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
        private readonly string CaIntermediateRevocationList = $"{CaIntermediateDirectory}/crl/intermediate.crl.pem";
        private readonly string CaIntermediateChain = $"{CaIntermediateDirectory}/certs/ca-chain.cert.pem";

        private const string SambaDomainDir = "/var/lib/samba/private";
        private readonly string SambaCaCert = $"{SambaDomainDir}/cacert.pem";
        private readonly string SambaCaCrl = $"{SambaDomainDir}/ca.crl";
        private readonly string SambaDcCert = $"{SambaDomainDir}/dc-cert.pem";
        private readonly string SambaDcParams = $"{SambaDomainDir}/dc-dhparams.pem";
        private readonly string SambaDcKey = $"{SambaDomainDir}/secure/dc-privkey.pem";
        public readonly string NginxCrl = $"{SambaDomainDir}/intermediate.crl.pem";

        //public class DomainControllerCertificate {
        //    private Bash _bash = new Bash();

        //    private readonly string CertCnfTemplate = $"{Parameter.Resources}/openssl-dc-tmplate.cnf";
        //    private string _certCurrentConfigurationFile;
        //    public void Create(string crlDistPt, string domainGuid, string domainDnsName, string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string commonName, string emailAddress, string passphrase) {
        //        try {
        //            const string replaceCrlDistPtd = "$crlDitributionPoint$";
        //            const string replaceDomainControllerGuid = "$domainControllerGuid$";
        //            const string replaceDomainDnsname = "$domainDnsName$";
        //            var cnfText = File.ReadAllText(CertCnfTemplate)
        //                .Replace(replaceCrlDistPtd, crlDistPt)
        //                .Replace(replaceDomainControllerGuid, domainGuid)
        //                .Replace(replaceDomainDnsname, domainDnsName);
        //            _certCurrentConfigurationFile = $"{CaIntermediateDirectory}/openssl-dc-{domainGuid}.cnf";
        //            if(File.Exists(_certCurrentConfigurationFile)) {
        //                File.Delete(_certCurrentConfigurationFile);
        //            }
        //            File.WriteAllText(_certCurrentConfigurationFile, cnfText);
        //            ConsoleLogger.Log($"certificate configuration file set for {domainGuid}");
        //            const int days = 740;
        //            var certificateKeyPath = $"{CaIntermediateDirectory}/private/dc-{domainGuid}.key.pem";
        //            var certificateRequestPath = $"{CaIntermediateDirectory}/csr/dc-{domainGuid}.csr.pem";
        //            var certificatePath = $"{CaIntermediateDirectory}/certs/dc-{domainGuid}.cert.pem";
        //            _bash.Execute($"openssl req -new -newkey rsa:2048 -keyout {certificateKeyPath} -out {certificateRequestPath} -config {_certCurrentConfigurationFile} -passout pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={commonName}/emailAddress={emailAddress}\"");
        //            _bash.Execute($"openssl ca -batch -config {_certCurrentConfigurationFile} -days {days} -in {certificateRequestPath} -out {certificatePath} -passin pass:{ApplicationSetting.X509()}");
        //            var privDcKey = $"{CaIntermediateDirectory}/private/dc-privkey.pem";
        //            _bash.Execute($"openssl rsa -in {certificateKeyPath} -inform PEM -out {privDcKey} -outform PEM -passin pass:{ApplicationSetting.X509()}");
        //            var paramFile = $"{CaIntermediateDirectory}/params/dc-dhparams.pem";
        //            _bash.Execute($"openssl dhparam 2048 -outform PEM -out {paramFile}");

        //            if(File.Exists(SambaDcCert)) {
        //                File.Delete(SambaDcCert);
        //            }
        //            _bash.Execute($"cp {certificatePath} {SambaDcCert}");

        //            if(File.Exists(SambaDcParams)) {
        //                File.Delete(SambaDcParams);
        //            }
        //            _bash.Execute($"cp {paramFile} {SambaDcParams}");

        //            if(File.Exists(SambaDcKey)) {
        //                File.Delete(SambaDcKey);
        //            }
        //            _bash.Execute($"cp {privDcKey} {SambaDcKey}");

        //            _bash.Execute("systemctl restart samba");

        //            var dt = DateTime.Now;
        //            var model = new CertificateModel {
        //                IsPresent = true,
        //                IsRevoked = false,
        //                Guid = Guid.NewGuid().ToString(),
        //                CertificateGuid = Guid.NewGuid().ToString(),
        //                CertificatePath = certificatePath,
        //                CertificateCountryName = countryName,
        //                CertificateStateProvinceNameh = stateProvinceName,
        //                CertificateLocalityName = localityName,
        //                CertificateOrganizationName = organizationName,
        //                CertificateOrganizationalUnitName = organizationalUnitName,
        //                CertificateCommonName = commonName,
        //                CertificateEmailAddress = emailAddress,
        //                CertificatePassphrase = passphrase,
        //                CertificateAuthorityLevel = CertificateAuthorityLevel.Common,
        //                CertificateAssignment = CertificateAssignment.DomainController,
        //                AssignmentGuid = domainGuid,
        //                ReleaseDateTime = dt,
        //                ExpirationDateTime = dt.AddDays(days)
        //            };
        //        }
        //        catch(Exception ex) {
        //            ConsoleLogger.Warn(ex.Message);
        //        }
        //    }
        //}

        //public class SmartCardCertificate {
        //    private Bash _bash = new Bash();

        //    private readonly string CertCnfTemplate = $"{Parameter.Resources}/openssl-sc-tmplate.cnf";
        //    private string _certCurrentConfigurationFile;
        //    public void Create(string crlDistPt, string userPrincipalName, string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string passphrase) {
        //        try {
        //            const string replaceCrlDistPtd = "$crlDitributionPoint$";
        //            const string replaceUserPrincipalName = "$userPrincipalName$";
        //            var cnfText = File.ReadAllText(CertCnfTemplate)
        //                .Replace(replaceCrlDistPtd, crlDistPt)
        //                .Replace(replaceUserPrincipalName, userPrincipalName);
        //            _certCurrentConfigurationFile = $"{CaIntermediateDirectory}/openssl-dc-{userPrincipalName}.cnf";
        //            if(File.Exists(_certCurrentConfigurationFile)) {
        //                File.Delete(_certCurrentConfigurationFile);
        //            }
        //            File.WriteAllText(_certCurrentConfigurationFile, cnfText);
        //            ConsoleLogger.Log($"certificate configuration file set for {userPrincipalName}");
        //            const int days = 740;
        //            var certificateKeyPath = $"{CaIntermediateDirectory}/private/dc-{userPrincipalName}.key.pem";
        //            var certificateRequestPath = $"{CaIntermediateDirectory}/csr/dc-{userPrincipalName}.csr.pem";
        //            var certificatePath = $"{CaIntermediateDirectory}/certs/dc-{userPrincipalName}.cert.pem";
        //            _bash.Execute($"openssl req -new -newkey rsa:2048 -keyout {certificateKeyPath} -out {certificateRequestPath} -config {_certCurrentConfigurationFile} -passout pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={userPrincipalName}/emailAddress={userPrincipalName}\"");
        //            _bash.Execute($"openssl ca -batch -config {_certCurrentConfigurationFile} -days {days} -in {certificateRequestPath} -out {certificatePath} -passin pass:{new ApplicationSetting().X509()}");
        //            var certificateDerPath = $"{CaIntermediateDirectory}/certs/{userPrincipalName}.cert.cer";
        //            _bash.Execute($"openssl x509 -in {certificatePath} -inform PEM -out {certificateDerPath} -outform DER");
        //            _bash.Execute($"chmod 444 {certificateDerPath}");
        //            var certificatePfxPath = $"{CaIntermediateDirectory}/certs/{userPrincipalName}.cert.pfx";
        //            _bash.Execute($"openssl pkcs12 -export -in {certificatePath} -inkey {certificateKeyPath} -out {certificatePfxPath} -passin pass:{passphrase} -passout pass:{passphrase} -nodes");
        //            _bash.Execute($"chmod 444 {certificatePfxPath}");
        //            var dt = DateTime.Now;
        //            var model = new CertificateModel {
        //                IsPresent = true,
        //                IsRevoked = false,
        //                CertificateGuid = Guid.NewGuid().ToString(),
        //                CertificatePath = certificatePath,
        //                CertificateDerPath = certificateDerPath,
        //                CertificatePfxPath = certificatePfxPath,
        //                CertificateCountryName = countryName,
        //                CertificateStateProvinceNameh = stateProvinceName,
        //                CertificateLocalityName = localityName,
        //                CertificateOrganizationName = organizationName,
        //                CertificateOrganizationalUnitName = organizationalUnitName,
        //                CertificateCommonName = userPrincipalName,
        //                CertificateEmailAddress = userPrincipalName,
        //                CertificatePassphrase = passphrase,
        //                CertificateAuthorityLevel = CertificateAuthorityLevel.Common,
        //                CertificateAssignment = CertificateAssignment.SmartCard,
        //                AssignmentGuid = userPrincipalName,
        //                ReleaseDateTime = dt,
        //                ExpirationDateTime = dt.AddDays(days)
        //            };
        //        }
        //        catch(Exception ex) {
        //            ConsoleLogger.Warn(ex.Message);
        //        }
        //    }
        //}

        public class Certificate {
            private Bash _bash = new Bash();

            public void Create(string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string commonName, string emailAddress, string passphrase, CertificateAssignment assignment, string bytesLength, string userGuid, string serviceGuid, string serviceAlias) {
                try {
                    var certName = commonName;
                    var usePassphraseForPrivateKey = passphrase.Length > 0;
                    var certificateKeyPath = $"{CaIntermediateDirectory}/private/{certName}.key.pem";
                    var certificateRequestPath = $"{CaIntermediateDirectory}/csr/{certName}.csr.pem";
                    var certificatePath = $"{CaIntermediateDirectory}/certs/{certName}.cert.pem";
                    if(usePassphraseForPrivateKey == false) {
                        _bash.Execute($"openssl genrsa -out {certificateKeyPath} {bytesLength}");
                        _bash.Execute($"chmod 400 {certificateKeyPath}");
                        _bash.Execute($"openssl req -config {CaIntermediateConfFile} -key {certificateKeyPath} -new -sha256 -out {certificateRequestPath} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={certName}/emailAddress={emailAddress}\"");
                    }
                    else {
                        _bash.Execute(
                            $"openssl genrsa -aes256 -passout pass:{passphrase} -out {certificateKeyPath} {bytesLength}");
                        _bash.Execute($"chmod 400 {certificateKeyPath}");
                        _bash.Execute($"openssl req -config {CaIntermediateConfFile} -key {certificateKeyPath} -new -sha256 -out {certificateRequestPath} -passin pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={certName}/emailAddress={emailAddress}\"");
                    }
                    var certExtension = "usr_cert";
                    if(assignment == CertificateAssignment.Service) {
                        certExtension = "server_cert";
                    }
                    const int days = 375;
                    _bash.Execute($"openssl ca -batch -config {CaIntermediateConfFile} -extensions {certExtension} -days {days} -notext -md sha256 -passin pass:{new ApplicationSetting().X509()} -in {certificateRequestPath} -out {certificatePath}");
                    _bash.Execute($"chmod 444 {certificatePath}");
                    var certificateDerPath = $"{CaIntermediateDirectory}/certs/{certName}.cert.cer";
                    _bash.Execute($"openssl x509 -in {certificatePath} -inform PEM -out {certificateDerPath} -outform DER");
                    _bash.Execute($"chmod 444 {certificateDerPath}");
                    var certificatePfxPath = $"{CaIntermediateDirectory}/certs/{certName}.cert.pfx";
                    _bash.Execute($"openssl pkcs12 -export -in {certificatePath} -inkey {certificateKeyPath} -out {certificatePfxPath} -passin pass:{passphrase} -passout pass:{passphrase} -nodes");
                    _bash.Execute($"chmod 444 {certificatePfxPath}");
                    var dt = DateTime.Now;
                    var model = new CertificateModel {
                        IsPresent = true,
                        IsRevoked = false,
                        Guid = Guid.NewGuid().ToString(),
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
                        AssignmentGuid = "",
                        AssignmentUserGuids = userGuid.Split(','),
                        AssignmentServiceGuid = serviceGuid,
                        AssignmentServiceAlias = serviceAlias,
                        CertificateBytes = bytesLength,
                        ReleaseDateTime = dt,
                        ExpirationDateTime = dt.AddDays(days)
                    };
                }
                catch(Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
        }
    }
}
