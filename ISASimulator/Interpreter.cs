

namespace ISASimulator {
    public class Interpreter {
        public static void Interpret()
        {
            for (int i = 0; i < ISASimulator.GetCode().Count; i++)
            {
                string line = ISASimulator.GetCode()[i].Trim();
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
                        Bytecode.Execute();
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                string op = line.Split(' ')[0].ToUpper();
                string arg = line.Substring(line.IndexOf(' ')).Replace(" ", "");
                string args = arg.ToUpper();
                if (Operations.GetUnaryOperations().ContainsKey(op))
                {
                    Operations.GetUnaryOperations()[op].Invoke(arg);
                }
                else if (Operations.GetBinaryOperations().ContainsKey(op))
                {
                    string[] argsArray = args.Split(",");
                    Operations.GetBinaryOperations()[op].Invoke(argsArray[0], argsArray[1]);
                }

                if (ISASimulator.GetDebuggingMode())
                {
                    Debug.StartDebug();
                }
            }
        }        
    }
}