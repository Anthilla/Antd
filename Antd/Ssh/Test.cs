using Renci.SshNet;
using System;

namespace Antd.Ssh {
    public class Test {

        public static void Start(string server, string user, string password, int serverport = 22) {
            try {
                ConnectionInfo ConnNfo = new ConnectionInfo(server, serverport, user,
                    new AuthenticationMethod[] { new PasswordAuthenticationMethod(user, password) });

                using (var sshclient = new SshClient(ConnNfo)) {
                    sshclient.Connect();
                    if (sshclient.IsConnected) {
                        Console.WriteLine("Client connected!");
                    }
                    using (var cmd = sshclient.CreateCommand("netstat")) {
                        cmd.Execute();
                        Console.WriteLine("Command>" + cmd.CommandText);
                        Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                    }
                    sshclient.Disconnect();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void Mainz() {
            // Setup Credentials and Server Information
            ConnectionInfo ConnNfo = new ConnectionInfo("hostOrIP", 22, "username",
                new AuthenticationMethod[]{
 
                // Pasword based Authentication
                new PasswordAuthenticationMethod("username","password"),
 
                // Key Based Authentication (using keys in OpenSSH Format)
                new PrivateKeyAuthenticationMethod("username",new PrivateKeyFile[]{ 
                    new PrivateKeyFile(@"..\openssh.key","passphrase")
                }),
            }
            );

            // Execute a (SHELL) Command - prepare upload directory
            using (var sshclient = new SshClient(ConnNfo)) {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand("mkdir -p /tmp/uploadtest && chmod +rw /tmp/uploadtest")) {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                }
                sshclient.Disconnect();
            }

            // Upload A File
            using (var sftp = new SftpClient(ConnNfo)) {
                string uploadfn = "Renci.SshNet.dll";

                sftp.Connect();
                sftp.ChangeDirectory("/tmp/uploadtest");
                using (var uplfileStream = System.IO.File.OpenRead(uploadfn)) {
                    sftp.UploadFile(uplfileStream, uploadfn, true);
                }
                sftp.Disconnect();
            }

            // Execute (SHELL) Commands
            using (var sshclient = new SshClient(ConnNfo)) {
                sshclient.Connect();

                // quick way to use ist, but not best practice - SshCommand is not Disposed, ExitStatus not checked...
                Console.WriteLine(sshclient.CreateCommand("cd /tmp && ls -lah").Execute());
                Console.WriteLine(sshclient.CreateCommand("pwd").Execute());
                Console.WriteLine(sshclient.CreateCommand("cd /tmp/uploadtest && ls -lah").Execute());
                sshclient.Disconnect();
            }
            Console.ReadKey();

        }
    }
}
