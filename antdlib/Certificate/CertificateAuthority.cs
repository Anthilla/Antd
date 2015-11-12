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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using antdlib.MountPoint;

namespace antdlib.Certificate {
    public class CertificateAuthority {

        private static readonly string CaDirectory = Folder.CertificateAuthority;
        private static readonly string CaMountDirectory = Mount.SetDirsPath(CaDirectory);

        public static void SetupCaDirectory() {
            Directory.CreateDirectory(CaDirectory);
            Directory.CreateDirectory(CaMountDirectory);
            Mount.Dir(CaDirectory);
        }

        private static readonly string CaRootConfFile = $"{CaDirectory}/openssl.cnf";
        private static readonly string CaRootPrivateKey = $"{CaDirectory}/private/ca.key.pem";
        private static readonly string CaRootCertificate = $"{CaDirectory}/certs/ca.cert.pem";
        private const string CaRootPass = "antdca";

        public static void SetupRootCa() {
            Directory.CreateDirectory(CaDirectory);
            Directory.CreateDirectory($"{CaDirectory}/certs");
            Directory.CreateDirectory($"{CaDirectory}/crl");
            Directory.CreateDirectory($"{CaDirectory}/newcerts");
            Directory.CreateDirectory($"{CaDirectory}/private");
            Terminal.Terminal.Execute($"chmod 700 {CaDirectory}/private");
            Terminal.Terminal.Execute($"touch {CaDirectory}/index.txt");
            Terminal.Terminal.Execute($"echo 1000 > {CaDirectory}/serial");
            File.Copy($"{Folder.Resources}/openssl.cnf", CaRootConfFile);
            Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaRootPrivateKey} -passout pass:{CaRootPass} 4096");
            Terminal.Terminal.Execute($"chmod 400 {CaRootPrivateKey}");
            Terminal.Terminal.Execute($"openssl req -config {CaRootConfFile} -key {CaRootPrivateKey}" +
                $"-new -x509 -days 10950 -sha256 -extensions v3_ca -out {CaRootCertificate} -passin pass:{CaRootPass}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU =./CN =Anthilla SRL Root CA/emailAddress=.\"");
            Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaRootCertificate}");
        }

        private static readonly string CaIntermediateDirectory = $"{CaDirectory}/intermediate";
        private static readonly string CaIntermediateConfFile = $"{CaIntermediateDirectory}/openssl-intermediate.cnf";
        private static readonly string CaIntermediatePrivateKey = $"{CaIntermediateDirectory}/private/intermediate.key.pem";
        private static readonly string CaIntermediateCertificateReq = $"{CaIntermediateDirectory}/csr/intermediate.csr.pem";
        private static readonly string CaIntermediateCertificate = $"{CaIntermediateDirectory}/certs/intermediate.cert.pem";
        private const string CaIntermediatePass = "antdca";

        public static void SetupIntermediateCa(string name = "intermediate") {
            Directory.CreateDirectory(CaIntermediateDirectory);
            Directory.CreateDirectory($"{CaIntermediateDirectory}/certs");
            Directory.CreateDirectory($"{CaIntermediateDirectory}/crl");
            Directory.CreateDirectory($"{CaIntermediateDirectory}/newcerts");
            Directory.CreateDirectory($"{CaIntermediateDirectory}/private");
            Terminal.Terminal.Execute($"chmod 700 {CaIntermediateDirectory}/private");
            Terminal.Terminal.Execute($"touch {CaIntermediateDirectory}/index.txt");
            Terminal.Terminal.Execute($"echo 1000 > {CaIntermediateDirectory}/serial");
            Terminal.Terminal.Execute($"echo 1000 > {CaIntermediateDirectory}/crlnumber");
            File.Copy($"{Folder.Resources}/openssl-intermediate.cnf", CaIntermediateConfFile);
            Terminal.Terminal.Execute($"openssl genrsa -aes256 -out {CaIntermediatePrivateKey} -passout pass:{CaIntermediatePass} 4096");
            Terminal.Terminal.Execute($"chmod 400 {CaIntermediatePrivateKey}");
            Terminal.Terminal.Execute($"openssl req -config {CaIntermediateConfFile} -key {CaIntermediatePrivateKey}" +
                $"-new -sha256 -out {CaIntermediateCertificateReq} -passin pass:{CaIntermediatePass}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU =./CN =Anthilla SRL Intermediate CA/emailAddress=.\"");

            Terminal.Terminal.Execute($"openssl ca -config {CaRootConfFile} -extensions v3_intermediate_ca" +
                $"-days 3650 - notext - md sha256 -passin pass:{CaIntermediatePass}" +
                $"-in {CaIntermediateCertificateReq} -out {CaIntermediateCertificate}" +
                 "-subj \"/C=IT/ST=Milan/L=./O=Anthilla SRL/OU =./CN =Anthilla SRL Intermediate CA/emailAddress=.\"");
            Terminal.Terminal.Execute($"openssl x509 -noout -text -in {CaRootCertificate}");

            //# cd /root/ca
            //# openssl ca -config openssl.cnf -extensions v3_intermediate_ca \
            //            -days 3650 - notext - md sha256 \
            //      -in intermediate / csr / intermediate.csr.pem \
            //      -out intermediate / certs / intermediate.cert.pem

            //Enter pass phrase for ca.key.pem: secretpassword
            //Sign the certificate?[y / n]: y

            //# chmod 444 intermediate/certs/intermediate.cert.pem
        }

        public static void SignCertificate() {

        }
    }
}
