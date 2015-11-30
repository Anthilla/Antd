using Renci.SshNet;
using System;

namespace antdlib.Ssh {
    public class Test {

        public static void Start(string server, string user, string password, int serverport = 22) {
            try {
                var connNfo = new ConnectionInfo(server, serverport, user, 
                    new PasswordAuthenticationMethod(user, password), 
                    new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(@"/chiave.ppk")));
                using (var sshclient = new SshClient(connNfo)) {
                    sshclient.Connect();
                    if (sshclient.IsConnected) {
                        Console.WriteLine("Client connected!");
                    }
                    using (var cmd = sshclient.CreateCommand("mkdir -p /mnt/cdrom/Apps/tmpssh")) {
                        cmd.Execute();
                        Console.WriteLine($"Command> {cmd.CommandText}");
                        Console.WriteLine($"Return Value = {cmd.ExitStatus}");
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
            var connNfo = new ConnectionInfo("hostOrIP", 22, "username", new PasswordAuthenticationMethod("username", "password"), new PrivateKeyAuthenticationMethod("username", new PrivateKeyFile(@"..\openssh.key", "passphrase")));
            // Execute a (SHELL) Command - Prepare upload directory
            using (var sshclient = new SshClient(connNfo)) {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand("mkdir -p /tmp/uploadtest && chmod +rw /tmp/uploadtest")) {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                }
                sshclient.Disconnect();
            }

            // Upload A File
            using (var sftp = new SftpClient(connNfo)) {
                const string uploadfn = "Renci.SshNet.dll";
                sftp.Connect();
                sftp.ChangeDirectory("/tmp/uploadtest");
                using (var uplfileStream = System.IO.File.OpenRead(uploadfn)) {
                    sftp.UploadFile(uplfileStream, uploadfn, true);
                }
                sftp.Disconnect();
            }

            // Execute (SHELL) Commands
            using (var sshclient = new SshClient(connNfo)) {
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
