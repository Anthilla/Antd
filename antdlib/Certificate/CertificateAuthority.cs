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

        private static readonly string CaDirectory = Folder.CertificateAuthority;

        public static void Setup() {
            try {
                ConsoleLogger.Log("Setup Certificate Authority Directory");
            Directory.CreateDirectory(CaDirectory);
                ConsoleLogger.Log("Setup Certificate Authority Root");
                SetupRootCa();
                ConsoleLogger.Log("Setup Certificate Authority Intermediate");
                SetupIntermediateCa();
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
            }
        }

        private static readonly string CaRootConfFile = $"{CaDirectory}/openssl.cnf";
        private static readonly string CaRootPrivateKey = $"{CaDirectory}/private/ca.key.pem";
        private static readonly string CaRootCertificate = $"{CaDirectory}/certs/ca.cert.pem";
        private const string CaRootPass = "antdca";

        private static void SetupRootCa() {
            Directory.CreateDirectory(CaDirectory);
            Directory.CreateDirectory($"{CaDirectory}/certs");
            Directory.CreateDirectory($"{CaDirectory}/crl");
            Directory.CreateDirectory($"{CaDirectory}/newcerts");
            Directory.CreateDirectory($"{CaDirectory}/private");
            Terminal.Terminal.Execute($"chmod 700 {CaDirectory}/private");
            File.WriteAllText($"{CaDirectory}/index.txt", "");
            File.WriteAllText($"{CaDirectory}/serial", "1000");
            File.Copy($"{Folder.Resources}/openssl.cnf", CaRootConfFile, true);
            //Debug
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaRootPrivateKey} -passout pass:{CaRootPass} 4096"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 400 {CaRootPrivateKey}"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl req -config {CaRootConfFile} -key {CaRootPrivateKey}" +
                $"-new -x509 -days 10950 -sha256 -extensions v3_ca -out {CaRootCertificate} -passin pass:{CaRootPass}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU =./CN =Antd Root CA/emailAddress=.\""));
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
            Directory.CreateDirectory(CaIntermediateDirectory);
            Directory.CreateDirectory($"{CaIntermediateDirectory}/certs");
            Directory.CreateDirectory($"{CaIntermediateDirectory}/crl");
            Directory.CreateDirectory($"{CaIntermediateDirectory}/newcerts");
            Directory.CreateDirectory($"{CaIntermediateDirectory}/private");
            Terminal.Terminal.Execute($"chmod 700 {CaIntermediateDirectory}/private");
            File.WriteAllText($"{CaIntermediateDirectory}/index.txt", "");
            File.WriteAllText($"{CaIntermediateDirectory}/serial", "1000");
            File.WriteAllText($"{CaIntermediateDirectory}/crlnumber", "1000");
            File.Copy($"{Folder.Resources}/openssl-intermediate.cnf", CaIntermediateConfFile, true);
            //Debug
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaIntermediatePrivateKey} -passout pass:{CaIntermediatePass} 4096"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 400 {CaIntermediatePrivateKey}"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {CaIntermediatePrivateKey}" +
                $"-new -sha256 -out {CaIntermediateCertificateReq} -passin pass:{CaIntermediatePass}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU=./CN=Antd Intermediate CA/emailAddress=.\""));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl ca -batch -config {CaRootConfFile} -extensions v3_intermediate_ca" +
                 $"-days 3650 -notext -md sha256 -passin pass:{CaIntermediatePass}" +
                 $"-in {CaIntermediateCertificateReq}" +
                 $"-out {CaIntermediateCertificate}"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"chmod 444 {CaIntermediateCertificate}"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaIntermediateCertificate}"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"openssl verify -CAfile {CaRootCertificate} {CaIntermediateCertificate}"));
            ConsoleLogger.Point(Terminal.Terminal.Execute($"cat {CaIntermediateCertificate} {CaRootCertificate} > {CaIntermediateChain}"));
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
