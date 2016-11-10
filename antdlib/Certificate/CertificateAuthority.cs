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
using antdlib.common;

namespace antdlib.Certificate {
    public class CertificateAuthority {
        public static bool IsActive => ApplicationSetting.CertificateAuthority() == "yes" && File.Exists(CaRootCertificate);

        private static readonly string CaDirectory = ApplicationSetting.CaPath();

        private static readonly string CaRootConfFile = $"{CaDirectory}/openssl.cnf";
        private static readonly string CaRootPrivateKey = $"{CaDirectory}/private/ca.key.pem";
        private static readonly string CaRootCertificate = $"{CaDirectory}/certs/ca.cert.pem";

        private static string _caCountry;
        private static string _caProvince;
        private static string _caLocality;
        private static string _caOrganization;
        private static string _caOrganizationalUnit;
        private static string _caCommonName;
        private static string _caEmail;

        private static string _caIntermediateCommonName;

        private static readonly string CaIntermediateDirectory = $"{CaDirectory}/intermediate";
        private static readonly string CaIntermediateConfFile = $"{CaIntermediateDirectory}/openssl.cnf";
        private static readonly string CaIntermediatePrivateKey = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
        private static readonly string CaIntermediateCertificateReq = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
        private static readonly string CaIntermediateCertificate = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
        private static readonly string CaIntermediateRevocationList = $"{CaIntermediateDirectory}/crl/intermediate.crl.pem";
        private static readonly string CaIntermediateChain = $"{CaIntermediateDirectory}/certs/ca-chain.cert.pem";

        private const string SambaDomainDir = "/var/lib/samba/private";
        private static readonly string SambaCaCert = $"{SambaDomainDir}/cacert.pem";
        private static readonly string SambaCaCrl = $"{SambaDomainDir}/ca.crl";
        private static readonly string SambaDcCert = $"{SambaDomainDir}/dc-cert.pem";
        private static readonly string SambaDcParams = $"{SambaDomainDir}/dc-dhparams.pem";
        private static readonly string SambaDcKey = $"{SambaDomainDir}/secure/dc-privkey.pem";
        public static readonly string NginxCrl = $"{SambaDomainDir}/intermediate.crl.pem";

        private static readonly Bash Bash = new Bash();

        public static void Setup(string directory, string passphrase, string caCountry, string caProvince, string caLocality, string caOrganization, string caOrganizationalUnit, string caCommonName, string caEmail) {
            ConsoleLogger.Log("setting up root ca structure");

            if(string.IsNullOrEmpty(directory)) {
                ApplicationSetting.SetCaPath(directory);
            }
            if(string.IsNullOrEmpty(passphrase)) {
                ApplicationSetting.SetX509(passphrase);
            }

            _caCountry = caCountry;
            _caProvince = caProvince;
            _caLocality = caLocality;
            _caOrganization = caOrganization;
            _caOrganizationalUnit = caOrganizationalUnit;
            _caCommonName = caCommonName;
            _caEmail = caEmail;
            _caIntermediateCommonName = $"Intermediate {caCommonName}";

            Bash.Execute($"mkdir -p {CaDirectory}", false);
            Bash.Execute($"mkdir -p {CaDirectory}/certs", false);
            Bash.Execute($"mkdir -p {CaDirectory}/crl", false);
            Bash.Execute($"mkdir -p {CaDirectory}/newcerts", false);
            Bash.Execute($"mkdir -p {CaDirectory}/private", false);
            Bash.Execute($"chmod 700 {CaDirectory}/private", false);
            Bash.Execute($"touch {CaDirectory}/index.txt", false);
            Bash.Execute($"echo 1000 > {CaDirectory}/serial", false);
            Bash.Execute($"cp {Parameter.Resources}/openssl.cnf {CaRootConfFile}", false);
            Bash.Execute($"openssl genrsa -aes256 -out {CaRootPrivateKey} -passout pass:{passphrase} 4096", false);
            Bash.Execute($"chmod 400 {CaRootPrivateKey}", false);
            Bash.Execute($"openssl req -config {CaRootConfFile} -key {CaRootPrivateKey} -new -x509 -days 10950 -sha256 -extensions v3_ca -out {CaRootCertificate} -passin pass:{passphrase} -subj \"/C={_caCountry}/ST={_caProvince}/L={_caLocality}/O={_caOrganization}/OU={_caOrganizationalUnit}/CN={_caCommonName}/emailAddress={_caEmail}\"", false);
            Bash.Execute($"openssl x509 -noout -text -in {CaRootCertificate}", false);

            ConsoleLogger.Log("setting up intermediate ca structure");
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}", false);
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}/certs", false);
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}/crl", false);
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}/csr", false);
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}/newcerts", false);
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}/private", false);
            Bash.Execute($"mkdir -p {CaIntermediateDirectory}/params", false);
            Bash.Execute($"chmod 700 {CaIntermediateDirectory}/private", false);
            Bash.Execute($"touch {CaIntermediateDirectory}/index.txt", false);
            Bash.Execute($"echo 1000 > {CaIntermediateDirectory}/serial", false);
            Bash.Execute($"echo 1000 > {CaIntermediateDirectory}/crlnumber", false);
            Bash.Execute($"cp {Parameter.Resources}/openssl-intermediate.cnf {CaIntermediateConfFile}", false);
            Bash.Execute($"openssl genrsa -aes256 -out {CaIntermediatePrivateKey} -passout pass:{passphrase} 4096", false);
            Bash.Execute($"chmod 400 {CaIntermediatePrivateKey}", false);
            Bash.Execute($"openssl req -config {CaIntermediateConfFile} -key {CaIntermediatePrivateKey} -new -sha256 -out {CaIntermediateCertificateReq} -passin pass:{passphrase} -subj \"/C={_caCountry}/ST={_caProvince}/L={_caLocality}/O={_caOrganization}/OU={_caOrganizationalUnit}/CN={_caIntermediateCommonName}/emailAddress={_caEmail}\"", false);
            Bash.Execute($"openssl ca -batch -config {CaRootConfFile} -extensions v3_intermediate_ca -days 3650 -notext -md sha256 -passin pass:{passphrase} -in {CaIntermediateCertificateReq} -out {CaIntermediateCertificate}", false);
            Bash.Execute($"chmod 444 {CaIntermediateCertificate}", false);
            Bash.Execute($"openssl x509 -noout -text -in {CaIntermediateCertificate}", false);
            Bash.Execute($"openssl verify -CAfile {CaRootCertificate} {CaIntermediateCertificate}", false);
            Bash.Execute($"cat {CaIntermediateCertificate} {CaRootCertificate} > {CaIntermediateChain}", false);
            Bash.Execute($"chmod 444 {CaIntermediateChain}", false);

            ConsoleLogger.Log("setting up crl");
            Bash.Execute($"openssl ca -config {CaIntermediateCertificate} -gencrl -batch -passin pass:{passphrase} -out {CaIntermediateRevocationList}", false);
            ConsoleLogger.Log(Bash.Execute($"openssl crl -in {CaIntermediateRevocationList} -noout -text"));

            if(File.Exists(SambaCaCert)) {
                File.Delete(SambaCaCert);
            }
            Bash.Execute($"cp {CaIntermediateChain} {SambaCaCert}", false);

            if(File.Exists(SambaCaCrl)) {
                File.Delete(SambaCaCrl);
            }
            Bash.Execute($"cp {CaIntermediateRevocationList} {SambaCaCrl}", false);

            Bash.Execute("systemctl restart samba", false);

            //todo associa path e configurazione di NGINX al distribution point...
            //todo salva da qualche parte l'url della possibile crldtrpt
            //sambatool CNAME
            if(File.Exists(NginxCrl)) {
                File.Delete(NginxCrl);
            }
            Bash.Execute($"cp {CaIntermediateRevocationList} {NginxCrl}", false);
            Bash.Execute("systemctl restart nginx", false);

            ApplicationSetting.EnableCertificateAuthority();
        }

        public class DomainControllerCertificate {
            private static readonly string CertCnfTemplate = $"{Parameter.Resources}/openssl-dc-tmplate.cnf";
            private static string _certCurrentConfigurationFile;
            public static void Create(string crlDistPt, string domainGuid, string domainDnsName, string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string commonName, string emailAddress, string passphrase) {
                try {
                    const string replaceCrlDistPtd = "$crlDitributionPoint$";
                    const string replaceDomainControllerGuid = "$domainControllerGuid$";
                    const string replaceDomainDnsname = "$domainDnsName$";
                    var cnfText = File.ReadAllText(CertCnfTemplate)
                        .Replace(replaceCrlDistPtd, crlDistPt)
                        .Replace(replaceDomainControllerGuid, domainGuid)
                        .Replace(replaceDomainDnsname, domainDnsName);
                    _certCurrentConfigurationFile = $"{CaIntermediateDirectory}/openssl-dc-{domainGuid}.cnf";
                    if(File.Exists(_certCurrentConfigurationFile)) {
                        File.Delete(_certCurrentConfigurationFile);
                    }
                    File.WriteAllText(_certCurrentConfigurationFile, cnfText);
                    ConsoleLogger.Log($"certificate configuration file set for {domainGuid}");
                    const int days = 740;
                    var certificateKeyPath = $"{CaIntermediateDirectory}/private/dc-{domainGuid}.key.pem";
                    var certificateRequestPath = $"{CaIntermediateDirectory}/csr/dc-{domainGuid}.csr.pem";
                    var certificatePath = $"{CaIntermediateDirectory}/certs/dc-{domainGuid}.cert.pem";
                    Bash.Execute($"openssl req -new -newkey rsa:2048 -keyout {certificateKeyPath} -out {certificateRequestPath} -config {_certCurrentConfigurationFile} -passout pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={commonName}/emailAddress={emailAddress}\"", false);
                    Bash.Execute($"openssl ca -batch -config {_certCurrentConfigurationFile} -days {days} -in {certificateRequestPath} -out {certificatePath} -passin pass:{ApplicationSetting.X509()}", false);
                    var privDcKey = $"{CaIntermediateDirectory}/private/dc-privkey.pem";
                    Bash.Execute($"openssl rsa -in {certificateKeyPath} -inform PEM -out {privDcKey} -outform PEM -passin pass:{ApplicationSetting.X509()}", false);
                    var paramFile = $"{CaIntermediateDirectory}/params/dc-dhparams.pem";
                    Bash.Execute($"openssl dhparam 2048 -outform PEM -out {paramFile}", false);

                    if(File.Exists(SambaDcCert)) {
                        File.Delete(SambaDcCert);
                    }
                    Bash.Execute($"cp {certificatePath} {SambaDcCert}", false);

                    if(File.Exists(SambaDcParams)) {
                        File.Delete(SambaDcParams);
                    }
                    Bash.Execute($"cp {paramFile} {SambaDcParams}", false);

                    if(File.Exists(SambaDcKey)) {
                        File.Delete(SambaDcKey);
                    }
                    Bash.Execute($"cp {privDcKey} {SambaDcKey}", false);

                    Bash.Execute("systemctl restart samba", false);

                    var dt = DateTime.Now;
                    var model = new CertificateModel {
                        IsPresent = true,
                        IsRevoked = false,
                        CertificateGuid = Guid.NewGuid().ToString(),
                        CertificatePath = certificatePath,
                        CertificateCountryName = countryName,
                        CertificateStateProvinceNameh = stateProvinceName,
                        CertificateLocalityName = localityName,
                        CertificateOrganizationName = organizationName,
                        CertificateOrganizationalUnitName = organizationalUnitName,
                        CertificateCommonName = commonName,
                        CertificateEmailAddress = emailAddress,
                        CertificatePassphrase = passphrase,
                        CertificateAuthorityLevel = CertificateAuthorityLevel.Common,
                        CertificateAssignment = CertificateAssignment.DomainController,
                        AssignmentGuid = domainGuid,
                        ReleaseDateTime = dt,
                        ExpirationDateTime = dt.AddDays(days)
                    };
                    //DeNSo.Session.New.Set(model);
                }
                catch(Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
        }

        public class SmartCardCertificate {
            private static readonly string CertCnfTemplate = $"{Parameter.Resources}/openssl-sc-tmplate.cnf";
            private static string _certCurrentConfigurationFile;
            public static void Create(string crlDistPt, string userPrincipalName, string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string passphrase) {
                try {
                    const string replaceCrlDistPtd = "$crlDitributionPoint$";
                    const string replaceUserPrincipalName = "$userPrincipalName$";
                    var cnfText = File.ReadAllText(CertCnfTemplate)
                        .Replace(replaceCrlDistPtd, crlDistPt)
                        .Replace(replaceUserPrincipalName, userPrincipalName);
                    _certCurrentConfigurationFile = $"{CaIntermediateDirectory}/openssl-dc-{userPrincipalName}.cnf";
                    if(File.Exists(_certCurrentConfigurationFile)) {
                        File.Delete(_certCurrentConfigurationFile);
                    }
                    File.WriteAllText(_certCurrentConfigurationFile, cnfText);
                    ConsoleLogger.Log($"certificate configuration file set for {userPrincipalName}");
                    const int days = 740;
                    var certificateKeyPath = $"{CaIntermediateDirectory}/private/dc-{userPrincipalName}.key.pem";
                    var certificateRequestPath = $"{CaIntermediateDirectory}/csr/dc-{userPrincipalName}.csr.pem";
                    var certificatePath = $"{CaIntermediateDirectory}/certs/dc-{userPrincipalName}.cert.pem";
                    Bash.Execute($"openssl req -new -newkey rsa:2048 -keyout {certificateKeyPath} -out {certificateRequestPath} -config {_certCurrentConfigurationFile} -passout pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={userPrincipalName}/emailAddress={userPrincipalName}\"", false);
                    Bash.Execute($"openssl ca -batch -config {_certCurrentConfigurationFile} -days {days} -in {certificateRequestPath} -out {certificatePath} -passin pass:{ApplicationSetting.X509()}", false);
                    var certificateDerPath = $"{CaIntermediateDirectory}/certs/{userPrincipalName}.cert.cer";
                    Bash.Execute($"openssl x509 -in {certificatePath} -inform PEM -out {certificateDerPath} -outform DER", false);
                    Bash.Execute($"chmod 444 {certificateDerPath}", false);
                    var certificatePfxPath = $"{CaIntermediateDirectory}/certs/{userPrincipalName}.cert.pfx";
                    Bash.Execute($"openssl pkcs12 -export -in {certificatePath} -inkey {certificateKeyPath} -out {certificatePfxPath} -passin pass:{passphrase} -passout pass:{passphrase} -nodes", false);
                    Bash.Execute($"chmod 444 {certificatePfxPath}", false);
                    var dt = DateTime.Now;
                    var model = new CertificateModel {
                        IsPresent = true,
                        IsRevoked = false,
                        CertificateGuid = Guid.NewGuid().ToString(),
                        CertificatePath = certificatePath,
                        CertificateDerPath = certificateDerPath,
                        CertificatePfxPath = certificatePfxPath,
                        CertificateCountryName = countryName,
                        CertificateStateProvinceNameh = stateProvinceName,
                        CertificateLocalityName = localityName,
                        CertificateOrganizationName = organizationName,
                        CertificateOrganizationalUnitName = organizationalUnitName,
                        CertificateCommonName = userPrincipalName,
                        CertificateEmailAddress = userPrincipalName,
                        CertificatePassphrase = passphrase,
                        CertificateAuthorityLevel = CertificateAuthorityLevel.Common,
                        CertificateAssignment = CertificateAssignment.SmartCard,
                        AssignmentGuid = userPrincipalName,
                        ReleaseDateTime = dt,
                        ExpirationDateTime = dt.AddDays(days)
                    };
                    //DeNSo.Session.New.Set(model);
                }
                catch(Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
        }

        public class Certificate {
            public static void Create(string countryName, string stateProvinceName, string localityName, string organizationName, string organizationalUnitName, string commonName, string emailAddress, string passphrase, CertificateAssignment assignment, string bytesLength, string userGuid, string serviceGuid, string serviceAlias) {
                try {
                    var certName = commonName;
                    var usePassphraseForPrivateKey = passphrase.Length > 0;
                    var certificateKeyPath = $"{CaIntermediateDirectory}/private/{certName}.key.pem";
                    var certificateRequestPath = $"{CaIntermediateDirectory}/csr/{certName}.csr.pem";
                    var certificatePath = $"{CaIntermediateDirectory}/certs/{certName}.cert.pem";
                    if(usePassphraseForPrivateKey == false) {
                        Bash.Execute($"openssl genrsa -out {certificateKeyPath} {bytesLength}", false);
                        Bash.Execute($"chmod 400 {certificateKeyPath}", false);
                        Bash.Execute($"openssl req -config {CaIntermediateConfFile} -key {certificateKeyPath} -new -sha256 -out {certificateRequestPath} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={certName}/emailAddress={emailAddress}\"", false);
                    }
                    else {
                        Bash.Execute($"openssl genrsa -aes256 -passout pass:{passphrase} -out {certificateKeyPath} {bytesLength}", false);
                        Bash.Execute($"chmod 400 {certificateKeyPath}", false);
                        Bash.Execute($"openssl req -config {CaIntermediateConfFile} -key {certificateKeyPath} -new -sha256 -out {certificateRequestPath} -passin pass:{passphrase} -subj \"/C={countryName}/ST={stateProvinceName}/L={localityName}/O={organizationName}/OU={organizationalUnitName}/CN={certName}/emailAddress={emailAddress}\"", false);
                    }
                    var certExtension = "usr_cert";
                    if(assignment == CertificateAssignment.Service) {
                        certExtension = "server_cert";
                    }
                    const int days = 375;
                    Bash.Execute($"openssl ca -batch -config {CaIntermediateConfFile} -extensions {certExtension} -days {days} -notext -md sha256 -passin pass:{ApplicationSetting.X509()} -in {certificateRequestPath} -out {certificatePath}", false);
                    Bash.Execute($"chmod 444 {certificatePath}", false);
                    var certificateDerPath = $"{CaIntermediateDirectory}/certs/{certName}.cert.cer";
                    Bash.Execute($"openssl x509 -in {certificatePath} -inform PEM -out {certificateDerPath} -outform DER", false);
                    Bash.Execute($"chmod 444 {certificateDerPath}", false);
                    var certificatePfxPath = $"{CaIntermediateDirectory}/certs/{certName}.cert.pfx";
                    Bash.Execute($"openssl pkcs12 -export -in {certificatePath} -inkey {certificateKeyPath} -out {certificatePfxPath} -passin pass:{passphrase} -passout pass:{passphrase} -nodes", false);
                    Bash.Execute($"chmod 444 {certificatePfxPath}", false);
                    var dt = DateTime.Now;
                    var model = new CertificateModel {
                        IsPresent = true,
                        IsRevoked = false,
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
                    //DeNSo.Session.New.Set(model);
                }
                catch(Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
        }
    }
}
