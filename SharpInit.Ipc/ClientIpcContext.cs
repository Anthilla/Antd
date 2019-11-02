﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SharpInit.Ipc {
    /// <summary>
    /// An implementation of IBaseIpcContext that dispatches serialized method calls over the wire.
    /// </summary>
    public class ClientIpcContext : IBaseIpcContext {
        public IpcConnection Connection { get; set; }
        public string SourceName { get; set; }

        public ClientIpcContext(IpcConnection connection, string name) {
            Connection = connection;
            SourceName = name;
        }

        public ClientIpcContext() :
            this(new IpcConnection(), "sharpinit-ipc-client") {

        }

        public bool ActivateUnit(string unit) {
            var result = InvokeIpcFunction("activate-unit", unit);
            return result.Success ? (bool)result.AdditionalData : false;
        }

        public bool DeactivateUnit(string unit) {
            var result = InvokeIpcFunction("deactivate-unit", unit);
            return result.Success ? (bool)result.AdditionalData : false;
        }

        public bool ReloadUnit(string unit) {
            var result = InvokeIpcFunction("reload-unit", unit);
            return result.Success ? (bool)result.AdditionalData : false;
        }

        public List<string> ListUnits() {
            var result = InvokeIpcFunction("list-units");
            return result.Success ? (List<string>)result.AdditionalData : null;
        }

        public bool LoadUnitFromFile(string path) {
            var result = InvokeIpcFunction("load-unit", path);
            return result.Success ? (bool)result.AdditionalData : false;
        }

        public bool ReloadUnitFile(string unit) {
            var result = InvokeIpcFunction("reload-unit", unit);
            return result.Success ? (bool)result.AdditionalData : false;
        }

        public int RescanUnits() {
            var result = InvokeIpcFunction("rescan-units");
            return result.Success ? Convert.ToInt32(result.AdditionalData) : -1;
        }

        public UnitInfo GetUnitInfo(string unit) {
            var result = InvokeIpcFunction("get-unit-info", unit);
            return result.Success ? (UnitInfo)result.AdditionalData : null;
        }

        public Dictionary<string, List<string>> GetActivationPlan(string unit) {
            var result = InvokeIpcFunction("get-activation-plan", unit);
            return result.Success ? (Dictionary<string, List<string>>)result.AdditionalData : null;
        }

        public Dictionary<string, List<string>> GetDeactivationPlan(string unit) {
            var result = InvokeIpcFunction("get-deactivation-plan", unit);
            return result.Success ? (Dictionary<string, List<string>>)result.AdditionalData : null;
        }

        public IpcResult InvokeIpcFunction(string name, params object[] args) {
            var ipc_message = new IpcMessage(SourceName, "sharpinit", name,
                JsonConvert.SerializeObject(args, IpcInterface.SerializerSettings));
            var result = Connection.SendMessageWaitForReply(ipc_message);

            return result;
        }
    }
}
