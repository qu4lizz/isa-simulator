
using System.Collections.Generic;
using System.Text;

namespace ISASimulator {
    public class Bytecode {
        private static readonly List<long> machineCodeAddresses = new();
        private static readonly Dictionary<byte, string> opCodes = new();

        public static List<long> GetMachineCodeAddresses()
        {
            return machineCodeAddresses;
        }

        public static Dictionary<byte, string> GetOpCodes()
        {
            return opCodes;
        }

        public static void Translate(long address)
        {
            var code = ISASimulator.GetCode();
            
            for (int i = 0; i < code.Count; i++)
            {
                var trimmedLine = code[i].Trim();
                var index = trimmedLine.IndexOf(' ');

                if (index == -1)
                {
                    string instruction = trimmedLine.ToUpper();
                    bool contains = ISASimulator.GetKeywords().Contains(instruction);
                    if (contains)
                    {
                        ISASimulator.GetAddresses().Add(address, FindKey(instruction));
                        machineCodeAddresses.Add(address++);
                    }
                    continue;
                }

                ISASimulator.GetAddresses().Add(address++, FindKey(trimmedLine.Split(" ")[0].ToUpper()));

                var remainingLine = trimmedLine.Substring(index + 1);
                var arr = Encoding.ASCII.GetBytes(remainingLine.Replace(" ", ""));
                machineCodeAddresses.Add(address - 1);

                foreach (var b in arr)
                {
                    ISASimulator.GetAddresses().Add(address++, b);
                }

                ISASimulator.GetAddresses().Add(address++, (byte)0);
            }

            ISASimulator.GetRegisters()["RIP"] = machineCodeAddresses[0];
        }

        public static void Execute()
        {
            while (true)
            {
                long rip = ISASimulator.GetRegisters()["RIP"] ?? -1;
                if (!ISASimulator.GetAddresses().ContainsKey(rip))
                {
                    return;
                }

                byte opcode = ISASimulator.GetAddresses()[rip++];
                string instruction = opCodes[opcode];

                if (Debug.Breakpoint.Equals(instruction))
                {
                    ISASimulator.SetDebuggingMode(true);
                    Debug.StartDebug();
                    ISASimulator.GetRegisters()["RIP"] = rip - 1;
                    continue;
                }
                else if (ISASimulator.SwitchToMachineCodeExecution.Equals(instruction))
                {
                    return;
                }

                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    byte ch = ISASimulator.GetAddresses()[rip++];
                    if (ch == 0)
                    {
                        break;
                    }
                    sb.Append((char)ch);
                }
                string operands = sb.ToString();

                if (Operations.GetUnaryOperations().ContainsKey(instruction))
                {
                    Operations.GetUnaryOperations()[instruction].Invoke(operands);
                }
                else if (Operations.GetBinaryOperations().ContainsKey(instruction))
                {
                    string[] operandsArray = operands.Split(",");
                    Operations.GetBinaryOperations()[instruction].Invoke(
                        operandsArray[0].ToUpper(),
                        operandsArray[1].ToUpper()
                    );
                }

                ISASimulator.GetRegisters()["RIP"] = rip;
                if (ISASimulator.GetDebuggingMode())
                {
                    Debug.StartDebug();
                }
            }
        }

        private static byte FindKey(string value)
        {
            return opCodes.FirstOrDefault(x => x.Value.Equals(value)).Key;
        }
    }
}