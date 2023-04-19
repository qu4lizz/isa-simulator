
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
            foreach (string line in ISASimulator.GetCode())
            {
                string trimmedLine = line.Trim();
                int index = trimmedLine.IndexOf(' ');

                if (index == -1 && ISASimulator.GetKeywords().Contains(trimmedLine.ToUpper()))
                {
                    ISASimulator.GetAddresses().Add(address, GetKeyForValue(trimmedLine.ToUpper()));
                    machineCodeAddresses.Add(address++);
                    continue;
                }
                else if (index == -1 && !ISASimulator.GetKeywords().Contains(trimmedLine.ToUpper()))
                {
                    continue;
                }
                else if (index != -1)
                {
                    ISASimulator.GetAddresses().Add(address++, GetKeyForValue(trimmedLine.Split(" ")[0].ToUpper()));
                }

                string remainingLine = trimmedLine.Substring(index + 1);
                byte[] arr = Encoding.ASCII.GetBytes(remainingLine.Replace(" ", ""));
                machineCodeAddresses.Add(address - 1);

                foreach (byte b in arr)
                {
                    ISASimulator.GetAddresses().Add(address++, b);
                }

                ISASimulator.GetAddresses().Add(address++, (byte)0);
            }

            ISASimulator.GetRegisters()["RIP"] = machineCodeAddresses[0];
        }

        private static byte GetKeyForValue(string value)
        {
            foreach (KeyValuePair<byte, string> entry in opCodes)
            {
                if (entry.Value.Equals(value))
                {
                    return entry.Key;
                }
            }

            return default;
        }

        public static void Execute()
        {
            for (; ; ISASimulator.SetInterpretationIndex(ISASimulator.GetInterpretationIndex() + 1))
            {
                if (!ISASimulator.GetAddresses().ContainsKey(ISASimulator.GetRegisters()["RIP"] ?? -1))
                {
                    return;
                }

                long address = ISASimulator.GetRegisters()["RIP"] ?? -1;

                string instruction = opCodes[ISASimulator.GetAddresses()[address++]];

                StringBuilder sb = new StringBuilder();

                if (!Debug.Breakpoint.Equals(instruction))
                {
                    while (ISASimulator.GetAddresses()[address] != 0)
                    {
                        sb.Append((char)(int)ISASimulator.GetAddresses()[address++]);
                    }
                }

                ISASimulator.GetRegisters()["RIP"] = address + 1;
                string operands = sb.ToString();

                if (Debug.Breakpoint.Equals(instruction))
                {
                    ISASimulator.SetDebuggingMode(true);
                    Debug.StartDebug();
                    ISASimulator.GetRegisters()["RIP"] = address;
                    continue;
                }
                else if (ISASimulator.SwitchToMachineCodeExecution.Equals(instruction))
                {
                    return;
                }
                else if (Operations.GetUnaryOperations().ContainsKey(instruction))
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
                if (ISASimulator.GetDebuggingMode())
                {
                    Debug.StartDebug();
                }
            }
        }
    }
}