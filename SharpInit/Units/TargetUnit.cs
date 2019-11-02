﻿using NLog;
using SharpInit.Tasks;
using System;

namespace SharpInit.Units {
    public class TargetUnit : Unit {
        Logger Log = LogManager.GetCurrentClassLogger();

        public new UnitFile File { get; set; }
        public override UnitFile GetUnitFile() => File;

        public TargetUnit(string path) : base(path) {

        }

        public override void LoadUnitFile(string path) {
            File = UnitParser.Parse<UnitFile>(path);
            LoadTime = DateTime.UtcNow;
        }

        public override void LoadUnitFile(UnitFile file) {
            File = file;
            LoadTime = DateTime.UtcNow;
        }

        internal override Transaction GetActivationTransaction() {
            var transaction = new Transaction($"Activation transaction for unit {UnitName}");

            transaction.Add(new SetUnitStateTask(this, UnitState.Activating, UnitState.Inactive | UnitState.Failed));
            transaction.Add(new SetUnitStateTask(this, UnitState.Active, UnitState.Activating));
            transaction.Add(new UpdateUnitActivationTimeTask(this));

            return transaction;
        }

        internal override Transaction GetDeactivationTransaction() {
            var transaction = new Transaction($"Deactivation transaction for unit {UnitName}");

            transaction.Add(new SetUnitStateTask(this, UnitState.Deactivating, UnitState.Active));
            transaction.Add(new SetUnitStateTask(this, UnitState.Inactive, UnitState.Deactivating));

            return transaction;
        }

        public override Transaction GetReloadTransaction() {
            return new Transaction();
        }
    }
}
