using anthilla.core;
using SharpInit.Ipc;
using SharpInit.Platform;
using SharpInit.Units;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Antd2.Init {
    public class ServiceInit {

        private IpcListener IpcListener { get;  set; }

        public void Start() {
            ConsoleLogger.Log("SharpInit starting");

            PlatformUtilities.RegisterImplementations();
            PlatformUtilities.GetImplementation<IPlatformInitialization>().Initialize();

            ConsoleLogger.Log("Platform initialization complete");

            UnitRegistry.InitializeTypes();
            UnitRegistry.ScanDefaultDirectories();

            ConsoleLogger.Log($"Loaded {UnitRegistry.Units.Count} units");

            UnitRegistry.UnitStateChange += StateChangeHandler;

            ConsoleLogger.Log("Registering IPC context...");

            var context = new SharpInit.ServerIpcContext();
            IpcFunctionRegistry.AddFunction(DynamicIpcFunction.FromContext(context));

            ConsoleLogger.Log("Starting IPC listener...");

            IpcListener = new IpcListener();
            IpcListener.StartListening();

            ConsoleLogger.Log($"Listening on {IpcListener.SocketEndPoint}");

            if (UnitRegistry.GetUnit("default.target") != null) {
                ConsoleLogger.Log("Activating default.target...");
                var result = UnitRegistry.CreateActivationTransaction("default.target").Execute();

                if (result.Type == SharpInit.Tasks.ResultType.Success)
                    ConsoleLogger.Log("Successfully activated default.target.");
                else
                    ConsoleLogger.Log($"Error while activating default.target: {result.Type}, {result.Message}");
            }
        }

        private static void StateChangeHandler(Unit source, SharpInit.Units.UnitState next_state) {
            ConsoleLogger.Log($"Unit {source.UnitName} is transitioning from {source.CurrentState} to {next_state}");
        }

        public void Stop() {
            if (IpcListener != null) {
                IpcListener.StopListening();
            }
        }
    }
}
