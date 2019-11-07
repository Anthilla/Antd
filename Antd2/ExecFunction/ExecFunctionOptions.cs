using System;
using System.Diagnostics;

namespace Antd.ExecFunction {
    public class ExecFunctionOptions {
        internal ExecFunctionOptions(ProcessStartInfo psi) {
            StartInfo = psi;
        }

        public ProcessStartInfo StartInfo { get; }

        public Action<Process> OnExit { get; set; }
    }
}