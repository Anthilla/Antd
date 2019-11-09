using anthilla.core;
using Newtonsoft.Json;
using SharpInit.Ipc;
using System;
using System.Linq;
using System.Text;

namespace Antd2.Init {

    /// <summary>
    /// Singleton che gestisce la connessione del Gateway al nodo di AnthillaEngine di riferimento
    /// </summary>
    public class InitController : IInitController, IDisposable {

        private static IpcConnection Connection { get; set; }
        private static ClientIpcContext Context { get; set; }

        private InitController() {
            Console.WriteLine("Setup unit controller");
            Connection = new IpcConnection();
            Connection.Connect();
            Context = new ClientIpcContext(Connection, "sharpinitctl");
        }

        public void DescribeDependenciesFunc(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                var unit = args[i];
                var activation_plan = Context.GetActivationPlan(unit);
                var deactivation_plan = Context.GetDeactivationPlan(unit);

                Console.WriteLine($"Activation plan for {unit}:");
                activation_plan.SelectMany(t => t.Value).ToList().ForEach(Console.WriteLine);

                Console.WriteLine();

                Console.WriteLine($"Dectivation plan for {unit}:");
                deactivation_plan.SelectMany(t => t.Value).ToList().ForEach(Console.WriteLine);
            }
        }

        public void RescanUnits(string[] args) {
            Console.Write("Rescanning unit directories...");

            var loaded_units = Context.RescanUnits();

            if (loaded_units >= 0)
                Console.WriteLine("loaded {0} units in total", loaded_units);
            else
                Console.WriteLine("error");
        }

        public void GetUnitStatus(string[] args) {
            var unit = args.First();
            var status = Context.GetUnitInfo(unit);

            if (status == null) {
                Console.WriteLine($"Unknown unit {unit}");
                return;
            }

            Console.OutputEncoding = Encoding.UTF8;
            var default_color = Console.ForegroundColor;
            var status_color = ConsoleColor.Gray;

            switch (status.State) {
                case UnitState.Activating:
                case UnitState.Active:
                case UnitState.Reloading:
                    status_color = ConsoleColor.Green;
                    break;
                case UnitState.Deactivating:
                case UnitState.Inactive:
                    status_color = default_color;
                    break;
                case UnitState.Failed:
                    status_color = ConsoleColor.Red;
                    break;
            }

            Console.ForegroundColor = status_color;
            Console.Write("•");
            Console.ForegroundColor = default_color;

            Console.WriteLine($" {status.Name}{(!string.IsNullOrWhiteSpace(status.Description) ? " - " + status.Description : "")}");
            Console.WriteLine($"Loaded from: {status.Path} at {status.LoadTime.ToLocalTime()}");
            Console.Write("Status: ");

            Console.ForegroundColor = status_color;
            Console.Write(status.State.ToString().ToLower());
            Console.ForegroundColor = default_color;

            Console.Write($" (last activated {(status.ActivationTime == DateTime.MinValue ? "never" : status.ActivationTime.ToLocalTime().ToString())})");
            Console.WriteLine();
            Console.WriteLine();
        }

        public void StartUnits(string[] args) {
            foreach (var unit in args) {
                Console.Write($"Starting {unit}...");

                var result = Context.ActivateUnit(unit);

                if (result)
                    Console.WriteLine("done");
                else
                    Console.WriteLine("error");
            }
        }

        public void StopUnits(string[] args) {
            foreach (var unit in args) {
                Console.Write($"Stopping {unit}...");

                var result = Context.DeactivateUnit(unit);

                if (result)
                    Console.WriteLine("done");
                else
                    Console.WriteLine("error");
            }
        }

        public void RestartUnits(string[] args) {
            foreach (var unit in args) {
                Console.Write($"Stopping {unit}...");

                var result = Context.DeactivateUnit(unit);

                if (result)
                    Console.WriteLine("done");
                else
                    Console.WriteLine("error");

                Console.Write($"Starting {unit}...");

                result = Context.ActivateUnit(unit);

                if (result)
                    Console.WriteLine("done");
                else
                    Console.WriteLine("error");
            }
        }

        public void ReloadUnits(string[] args) {
            foreach (var unit in args) {
                Console.Write($"Reloading {unit}...");

                var result = Context.ReloadUnit(unit);

                if (result)
                    Console.WriteLine("done");
                else
                    Console.WriteLine("error");
            }
        }

        public void ListUnits(string[] args) {
            var list = Context.ListUnits();

            if (list == null)
                Console.WriteLine("Couldn't retrieve the list of loaded units.");
            else
                Console.WriteLine($"{list.Count} units loaded: [{string.Join(", ", list)}]");
        }

        #region Do not modify!

        /// <summary>
        /// The volatile keyword ensures that the instantiation is complete
        /// before it can be accessed further helping with thread safety.
        /// </summary>
        private static volatile InitController _instance = null;
        private static readonly object _syncLock = new object();
        private readonly bool _disposed = false;

        /// <summary>
        /// Pattern 'double check locking'
        /// </summary>
        public static InitController Instance {
            get {
                if (_instance != null)
                    return _instance;
                lock (_syncLock) {
                    if (_instance == null) {
                        _instance = new InitController();
                    }
                    return _instance;
                }
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;
            if (disposing) {
                lock (_syncLock) {
                    Connection.Disconnect();
                    Connection = null;
                    Context = null;
                    _instance = null;
                }
            }
        }
        #endregion
    }
}
