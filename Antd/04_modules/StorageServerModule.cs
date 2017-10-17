using Antd.cmds;
using anthilla.core;
using KvpbaseSDK;
using Nancy;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Antd.Modules {
    /// <summary>
    /// Questo modulo sarà il punto di arrivo delle chiamate GET, POST, DELETE, PUT (?)
    /// dove passeranno le informazioni, in entrata e in uscita, verso e da altri nodi del cluster
    /// Quindi la classe VfsClient verrà comunque utilizzata da antd (server) (per esempio nel sync dei servizi)
    /// per ottenere informazioni o apportare modifiche su un file system remoto ( -> su un nodo gestito da antd)
    /// </summary>
    public class StorageServerModule : NancyModule {

        public StorageServerModule() : base("/storageserver") {

            Post["/{userguid}/{server}/{port}/0/0"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                string fileType = Request.Form.FileType;
                string fileName = Request.Form.FileName;
                var file = Request.Files.FirstOrDefault();
                if(file == null) {
                    return HttpStatusCode.BadRequest;
                }
                var data = file.Value.ReadAllBytes();
                var result = VfsClient.CreateObject(client, containerPath, fileType, data, fileName);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };
        }
    }
}