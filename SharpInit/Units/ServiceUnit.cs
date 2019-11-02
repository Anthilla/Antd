﻿using NLog;
using SharpInit.Platform;
using SharpInit.Tasks;
using System;
using System.Linq;
using System.Threading;

namespace SharpInit.Units {
    public class ServiceUnit : Unit {
        Logger Log = LogManager.GetCurrentClassLogger();

        public new ServiceUnitFile File { get; set; }
        public override UnitFile GetUnitFile() => (UnitFile)File;

        public int COMMAND_TIMEOUT = 5000;

        public ServiceUnit(string path) : base(path) {
            ProcessStart += HandleProcessStart;
            ProcessExit += HandleProcessExit;
        }

        private void HandleProcessExit(Unit unit, ProcessInfo info, int code) {
            switch (CurrentState) {
                case UnitState.Deactivating:
                    SetState(UnitState.Inactive);
                    break;
                default:
                    // TODO: treat process exit differently based on service type
                    if (code != 0) {
                        SetState(UnitState.Failed);
                    }
                    else {
                        SetState(UnitState.Inactive);
                    }

                    var should_restart = false;

                    if (code == 0)
                        should_restart =
                            File.Restart == RestartBehavior.Always ||
                            File.Restart == RestartBehavior.OnSuccess;
                    else
                        should_restart =
                            File.Restart == RestartBehavior.Always ||
                            File.Restart == RestartBehavior.OnFailure ||
                            File.Restart == RestartBehavior.OnAbnormal;

                    if (should_restart) {
                        var restart_transaction = new Transaction(
                            new DelayTask(File.RestartSec),
                            UnitRegistry.CreateDeactivationTransaction(UnitName),
                            UnitRegistry.CreateActivationTransaction(UnitName));

                        // TODO: un-hack this
                        new Thread((ThreadStart)delegate { restart_transaction.Execute(); }).Start();
                    }
                    break;
            }
        }

        private void HandleProcessStart(Unit unit, ProcessInfo info) {
            // do nothing for now
        }

        public override void LoadUnitFile(string path) {
            File = UnitParser.Parse<ServiceUnitFile>(path);
            LoadTime = DateTime.UtcNow;
        }

        public override void LoadUnitFile(UnitFile file) {
            File = (ServiceUnitFile)file;
            LoadTime = DateTime.UtcNow;
        }

        internal override Transaction GetActivationTransaction() {
            var transaction = new Transaction($"Activation transaction for unit {UnitName}");
            transaction.Add(new SetUnitStateTask(this, UnitState.Activating, UnitState.Inactive | UnitState.Failed));

            var working_dir = File.WorkingDirectory;
            var user = (File.Group == null && File.User == null ? null : PlatformUtilities.GetImplementation<IUserIdentifier>(File.Group, File.User));

            switch (File.ServiceType) {
                case ServiceType.Simple:
                    if (File.ExecStart == null) {
                        Log.Error($"Unit {UnitName} has no ExecStart directives.");
                        SetState(UnitState.Failed);
                        return null;
                    }

                    if (File.ExecStart.Count != 1) {
                        Log.Error($"Service type \"simple\" only supports one ExecStart value, {UnitName} has {File.ExecStart.Count}");
                        SetState(UnitState.Failed);
                        return null;
                    }

                    if (File.ExecStartPre.Any()) {
                        foreach (var line in File.ExecStartPre)
                            transaction.Add(new RunUnregisteredProcessTask(ServiceManager.ProcessHandler, ProcessStartInfo.FromCommandLine(line, working_dir, user), 5000));
                    }

                    transaction.Add(new RunRegisteredProcessTask(ProcessStartInfo.FromCommandLine(File.ExecStart.Single(), working_dir, user), this));

                    if (File.ExecStartPost.Any()) {
                        foreach (var line in File.ExecStartPost)
                            transaction.Add(new RunUnregisteredProcessTask(ServiceManager.ProcessHandler, ProcessStartInfo.FromCommandLine(line, working_dir, user), 5000));
                    }
                    break;
                default:
                    Log.Error($"Only the \"simple\" service type is supported for now, {UnitName} has type {File.ServiceType}");
                    SetState(UnitState.Failed);
                    break;
            }

            transaction.Add(new SetUnitStateTask(this, UnitState.Active, UnitState.Activating));
            transaction.Add(new UpdateUnitActivationTimeTask(this));

            transaction.OnFailure = new SetUnitStateTask(this, UnitState.Failed);

            return transaction;
        }

        internal override Transaction GetDeactivationTransaction() {
            var transaction = new Transaction($"Deactivation transaction for unit {UnitName}");

            transaction.Add(new SetUnitStateTask(this, UnitState.Deactivating, UnitState.Active));
            transaction.Add(new StopUnitProcessesTask(this));
            transaction.Add(new SetUnitStateTask(this, UnitState.Inactive, UnitState.Deactivating));

            transaction.OnFailure = new SetUnitStateTask(this, UnitState.Inactive);

            return transaction;
        }

        public override Transaction GetReloadTransaction() {
            var transaction = new Transaction();
            transaction.Add(new SetUnitStateTask(this, UnitState.Reloading, UnitState.Active));

            var working_dir = File.WorkingDirectory;
            var user = (File.Group == null && File.User == null ? null : PlatformUtilities.GetImplementation<IUserIdentifier>(File.Group, File.User));

            if (!File.ExecReload.Any()) {
                throw new Exception($"Unit {UnitName} has no ExecReload directives.");
            }

            foreach (var reload_cmd in File.ExecReload) {
                transaction.Add(new RunUnregisteredProcessTask(ServiceManager.ProcessHandler, ProcessStartInfo.FromCommandLine(reload_cmd, working_dir, user), 5000));
            }

            transaction.Add(new SetUnitStateTask(this, UnitState.Active, UnitState.Reloading));

            return transaction;
        }
    }
}
