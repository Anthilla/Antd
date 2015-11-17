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
using antdlib.Common;
using antdlib.MountPoint;

namespace antdlib.Certificate {
    public class CertificateAuthority {
        public static void Setup() {
            try {
                SetupRootCa();
                ConsoleLogger.Log("___________________________________");
                SetupIntermediateCa();
                ConsoleLogger.Log("___________________________________");
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.ToString());
            }
        }

        private static readonly string CaDirectory = Folder.CertificateAuthority;
        private static readonly string CaRootConfFile = $"{CaDirectory}/openssl.cnf";
        private static readonly string CaRootPrivateKey = $"{CaDirectory}/private/ca.key.pem";
        private static readonly string CaRootCertificate = $"{CaDirectory}/certs/ca.cert.pem";
        private const string CaRootPass = "antdca";

        private static void SetupRootCa() {
            ConsoleLogger.Log("______ Setup Root CA ______");
            ConsoleLogger.Log("1) Create directories: /ca ");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}");
            ConsoleLogger.Log("1a) .certs");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/certs");
            ConsoleLogger.Log("1a) .crl");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/crl");
            ConsoleLogger.Log("1a) .newcerts ");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/newcerts");
            ConsoleLogger.Log("1a) .private");
            Terminal.Terminal.Execute($"mkdir -p {CaDirectory}/private");
            ConsoleLogger.Log("2) Change .private acl");
            Terminal.Terminal.Execute($"chmod 700 {CaDirectory}/private");
            ConsoleLogger.Log("3) Create index file");
            Terminal.Terminal.Execute($"touch {CaDirectory}/index.txt");
            ConsoleLogger.Log("4) Create serial file");
            Terminal.Terminal.Execute($"echo 1000 > {CaDirectory}/serial");
            ConsoleLogger.Log("5) Copy .conf file");
            Terminal.Terminal.Execute($"cp {Folder.Resources}/openssl.cnf {CaRootConfFile}");
            //Debug
            ConsoleLogger.Log("6) Generate root private key");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaRootPrivateKey} -passout pass:{CaRootPass} 4096"));
            ConsoleLogger.Log("7) Change private key acl");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 400 {CaRootPrivateKey}"));
            ConsoleLogger.Log("8) Generate root certificate");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl req -config {CaRootConfFile} -key {CaRootPrivateKey}" +
                $"-new -x509 -days 10950 -sha256 -extensions v3_ca -out {CaRootCertificate} -passin pass:{CaRootPass}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU =./CN =Antd Root CA/emailAddress=.\""));
            ConsoleLogger.Log("9) Check root certificate");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaRootCertificate}"));

            //Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaRootPrivateKey} -passout pass:{CaRootPass} 4096");
            //Terminal.Terminal.Execute($"chmod 400 {CaRootPrivateKey}");
            //Terminal.Terminal.Execute($"openssl req -config {CaRootConfFile} -key {CaRootPrivateKey}" +
            //    $"-new -x509 -days 10950 -sha256 -extensions v3_ca -out {CaRootCertificate} -passin pass:{CaRootPass}" +
            //     "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU =./CN =Antd Root CA/emailAddress=.\"");
            //Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaRootCertificate}");
        }

        private static readonly string CaIntermediateDirectory = $"{CaDirectory}/intermediate";
        private static readonly string CaIntermediateConfFile = $"{CaIntermediateDirectory}/openssl.cnf";
        private static readonly string CaIntermediatePrivateKey = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
        private static readonly string CaIntermediateCertificateReq = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
        private static readonly string CaIntermediateCertificate = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
        private static readonly string CaIntermediateChain = $"{CaIntermediateDirectory}/certs/ca-chain.cert.pem";
        private const string CaIntermediatePass = "antdca";

        private static void SetupIntermediateCa() {
            ConsoleLogger.Log("______ Setup Intermediate CA ______");
            ConsoleLogger.Log("1) Create directories: /ca/intermediate .certs .crl .newcerts .private");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/certs");
            Terminal.Terminal.Execute($"mkdir -p {CaIntermediateDirectory}/crl");
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
            Terminal.Terminal.Execute($"cp {Folder.Resources}/openssl-intermediate.cnf {CaIntermediateConfFile}");
            //Debug
            ConsoleLogger.Log("7) Generate intermediate private key");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaIntermediatePrivateKey} -passout pass:{CaIntermediatePass} 4096"));
            ConsoleLogger.Log("8) Change private key acl");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 400 {CaIntermediatePrivateKey}"));
            ConsoleLogger.Log("9) Generate intermediate cert request");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {CaIntermediatePrivateKey}" +
                $"-new -sha256 -out {CaIntermediateCertificateReq} -passin pass:{CaIntermediatePass}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU=./CN=Antd Intermediate CA/emailAddress=.\""));
            ConsoleLogger.Log("10) Generate intermediate certificate, signed with root certificate");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl ca -batch -config {CaRootConfFile} -extensions v3_intermediate_ca" +
                 $"-days 3650 -notext -md sha256 -passin pass:{CaIntermediatePass}" +
                 $"-in {CaIntermediateCertificateReq}" +
                 $"-out {CaIntermediateCertificate}"));
            ConsoleLogger.Log("11) Change intermediate certificate acl");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 444 {CaIntermediateCertificate}"));
            ConsoleLogger.Log("12) Check intermediate certificate");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaIntermediateCertificate}"));
            ConsoleLogger.Log("13) Verify intermediate certificate");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl verify -CAfile {CaRootCertificate} {CaIntermediateCertificate}"));
            ConsoleLogger.Log("14) Generate intermediate chain file");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"cat {CaIntermediateCertificate} {CaRootCertificate} > {CaIntermediateChain}"));
            ConsoleLogger.Log("15) Change intermediate chain file acl");
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 444 {CaIntermediateChain}"));

            //Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaIntermediatePrivateKey} -passout pass:{CaIntermediatePass} 4096");
            //Terminal.Terminal.Execute($"chmod 400 {CaIntermediatePrivateKey}");
            //Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {CaIntermediatePrivateKey}" +
            //    $"-new -sha256 -out {CaIntermediateCertificateReq} -passin pass:{CaIntermediatePass}" +
            //     "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU=./CN=Antd Intermediate CA/emailAddress=.\"");
            //Terminal.Terminal.Execute($"openssl ca -batch -config {CaRootConfFile} -extensions v3_intermediate_ca" +
            //     $"-days 3650 -notext -md sha256 -passin pass:{CaIntermediatePass}" +
            //     $"-in {CaIntermediateCertificateReq}" +
            //     $"-out {CaIntermediateCertificate}");
            //Terminal.Terminal.Execute($"chmod 444 {CaIntermediateCertificate}");
            //Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaIntermediateCertificate}");
            //Terminal.Terminal.Execute($"openssl verify -CAfile {CaRootCertificate} {CaIntermediateCertificate}");
            //Terminal.Terminal.Execute($"cat {CaIntermediateCertificate} {CaRootCertificate} > {CaIntermediateChain}");
            //Terminal.Terminal.Execute($"chmod 444 {CaIntermediateChain}");
        }

        private static void Convert() {
            throw new NotImplementedException();
        }

        public class Certificate {
            private static string _certificateName;
            public Certificate(string certificateName) {
                _certificateName = certificateName;
            }
            private static readonly string CertificateKeyPath = $"{CaIntermediateDirectory}/private/{_certificateName}.key.pem ";
            private static readonly string CertificateRequestPath = $"{CaIntermediateDirectory}/csr/{_certificateName}.csr.pem ";
            private static readonly string CertificatePath = $"{CaIntermediateDirectory}/certs/{_certificateName}.cert.pem ";
            private static readonly string CertificatePass = $"antdca_{_certificateName}";

            public void Create() {
                //add -aes256 for password
                Terminal.Terminal.Execute($"openssl genrsa -out {CertificateKeyPath} 2048");
                Terminal.Terminal.Execute($"chmod 400 {CertificateKeyPath}");
                //add -passin for password
                Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {CertificateKeyPath}" +
    $"-new -sha256 -out {CertificateRequestPath}" +
     $"-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU=./CN={_certificateName} CA/emailAddress=.\"");

                Terminal.Terminal.Execute($"openssl ca -batch -config {CaIntermediateConfFile} -extensions server_cert" +
     $"-days 375 -notext -md sha256 -passin pass:{CertificatePass}" +
     $"-in {CertificateRequestPath}" +
     $"-out {CertificatePath}");
                Terminal.Terminal.Execute($"chmod 444 {CertificatePath}");
            }

            public string Verify() {
                return Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CertificateKeyPath}");
            }

            public string VerifyChained() {
                return Terminal.Terminal.Execute($"openssl verify -CAfile {CaIntermediateChain} {CertificatePath}");
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
