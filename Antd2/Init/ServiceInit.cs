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
            Console.WriteLine("SharpInit starting");

            PlatformUtilities.RegisterImplementations();
            PlatformUtilities.GetImplementation<IPlatformInitialization>().Initialize();

            Console.WriteLine("Platform initialization complete");

            UnitRegistry.InitializeTypes();
            UnitRegistry.ScanDefaultDirectories();

            Console.WriteLine($"Loaded {UnitRegistry.Units.Count} units");

            UnitRegistry.UnitStateChange += StateChangeHandler;

            Console.WriteLine("Registering IPC context...");

            var context = new SharpInit.ServerIpcContext();
            IpcFunctionRegistry.AddFunction(DynamicIpcFunction.FromContext(context));

            Console.WriteLine("Starting IPC listener...");

            IpcListener = new IpcListener();
            IpcListener.StartListening();

            Console.WriteLine($"Listening on {IpcListener.SocketEndPoint}");

            if (UnitRegistry.GetUnit("default.target") != null) {
                Console.WriteLine("Activating default.target...");
                var result = UnitRegistry.CreateActivationTransaction("default.target").Execute();

                if (result.Type == SharpInit.Tasks.ResultType.Success)
                    Console.WriteLine("Successfully activated default.target.");
                else
                    Console.WriteLine($"Error while activating default.target: {result.Type}, {result.Message}");
            }
        }

        private static void StateChangeHandler(Unit source, SharpInit.Units.UnitState next_state) {
            Console.WriteLine($"Unit {source.UnitName} is transitioning from {source.CurrentState} to {next_state}");
        }

        public void Stop() {
            if (IpcListener != null) {
                IpcListener.StopListening();
            }
        }
    }
}
