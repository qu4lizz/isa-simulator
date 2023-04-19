

namespace ISASimulator {
    public class Interpreter {
        public static void Interpret()
        {
            for (int i = 0; i < ISASimulator.GetCode().Count; i++)
            {
                string line = ISASimulator.GetCode()[i];
                line = line.Trim();
                int index = line.IndexOf(' ');
                if (index == -1)
                {
                    if (Debug.Breakpoint.Equals(line.ToUpper()))
                    {
                        ISASimulator.SetDebuggingMode(true);
                        Debug.StartDebug();
                        continue;
                    }
                    else if (ISASimulator.SwitchToMachineCodeExecution.Equals(line.ToUpper()))
                    {
                        i++;
                        ISASimulator.GetRegisters()["RIP"] = Bytecode.GetMachineCodeAddresses()[i];
                        Bytecode.MachineCodeExec();
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                string op = line.Split(' ')[0];
                op = op.ToUpper();
                string args1 = line.Substring(line.IndexOf(' '));
                args1 = args1.Replace(" ", "");
                string args = args1.ToUpper();
                if (Operations.GetUnaryOperations().ContainsKey(op))
                {
                    Operations.GetUnaryOperations()[op].Invoke(args1);
                }
                else if (Operations.GetBinaryOperations().ContainsKey(op))
                {
                    string arg1 = args.Split(',')[0], arg2 = args.Split(',')[1];
                    Operations.GetBinaryOperations()[op].Invoke(arg1, arg2);
                }

                if (ISASimulator.GetDebuggingMode())
                {
                    Debug.StartDebug();
                }
            }
        }        
    }
}