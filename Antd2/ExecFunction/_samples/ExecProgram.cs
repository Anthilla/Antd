namespace Antd.ExecFunction {

    public class ExecProgram {
        static int Main(string[] args) {
            if (ExecFunction.IsExecFunctionCommand(args)) {
                return ExecFunction.Program.Main(args);
            }
            else {
                ExecFunction.Run(() => System.Console.WriteLine("Hello world!"));
                return 0;
            }
        }
    }
}
