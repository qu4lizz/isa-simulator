

using System;
using System.Collections.Generic;

namespace ISASimulator {
    public class Operations {
        // Map of unary operators
        private static readonly Dictionary<string, Action<string>> UnaryOperations = new();
        // Map of binary operators
        private static readonly Dictionary<string, Action<string, string>> BinaryOperations = new();
    
        // Variables for the result of CMP instruction
        private static bool _equalResult, _lessResult;

        public static Dictionary<string, Action<string>> GetUnaryOperations()
        {
            return UnaryOperations;
        }

        public static Dictionary<string, Action<string, string>> GetBinaryOperations()
        {
            return BinaryOperations;
        }

        private static void Process(string label) {
            ISASimulator.SetInterpretationIndex(ISASimulator.GetLabels()[label]);
            if (ISASimulator.GetInterpretationIndex() < Bytecode.GetMachineCodeAddresses().Count)
            {
                ISASimulator.GetRegisters()["RIP"] = Bytecode.GetMachineCodeAddresses()[ISASimulator.GetInterpretationIndex()];
            }
            else
            {
                ISASimulator.GetRegisters()["RIP"] = null;
            }
        }
        public static void Jump(string label)
        {
            Process(label);
        }

        public static void JumpEqual(string label)
        {
            if (_equalResult)
            {
                Process(label);
            }
        }

        public static void JumpNotEqual(string label)
        {
            if (!_equalResult)
            {
                Process(label);
            }
        }

        public static void JumpGreaterEqual(string label)
        {
            if (!_lessResult)
            {
                Process(label);
            }
        }

        public static void JumpLess(string label)
        {
            if (_lessResult)
            {
                Process(label);
            }
        }

        public static void Not(string arg)
        {
            arg = arg.ToUpper();
            PutValue(arg, ~GetValue(arg));
        }

        public static void Add(string arg1, string arg2)
        {
            long result = GetValue(arg1) + GetValue(arg2);
            PutValue(arg1, result);
        }

        public static void Sub(string arg1, string arg2) {
            long result = GetValue(arg1) - GetValue(arg2);
            PutValue(arg1, result);
        }

        public static void And(string arg1, string arg2) {
            long result = GetValue(arg1) & GetValue(arg2);
            PutValue(arg1, result);
        }

        public static void Or(string arg1, string arg2) {
            long result = GetValue(arg1) | GetValue(arg2);
            PutValue(arg1, result);
        }

        public static void Mov(string arg1, string arg2) {
            PutValue(arg1, GetValue(arg2));
        }

        //Check if input is a number, if true store it
        //if false, it is a string, store it as string
        public static void Scan(string arg) {
            arg = arg.ToUpper();
            string input = Console.ReadLine();
            if (IsNumber(input)) {
                long result = input.StartsWith("0x") || input.StartsWith("0X") ? Convert.ToInt64(input.Substring(2), 16) : Convert.ToInt64(input);
                PutValue(arg, result);
            }
            else {
                if (!arg.StartsWith("[")) {
                    long result = 0;
                    for (int i = 0; i < 8 && i < input.Length; i++) {
                        result = result * 100 + (int)input[i];
                    }
                    PutValue(arg, result);
                }
                else {
                    PutValue(arg, (int)input[0]);
                }
            }
        }

        public static void Print(string arg) {
            arg = arg.ToUpper();
            Console.WriteLine(GetValue(arg));
        }

        public static void Cmp(string arg1, string arg2) {
            long num1 = GetValue(arg1), num2 = GetValue(arg2);
            _equalResult = num1 == num2;
            _lessResult = num1 < num2;
        }

        private static long GetValue(string arg)
        {
            long result = 0;
            if (!arg.StartsWith("["))
            {
                if (ISASimulator.GetRegisters().ContainsKey(arg))
                    result = (long)ISASimulator.GetRegisters()[arg];
                else if (arg.StartsWith("0X"))
                    result = long.Parse(arg.Substring(2), System.Globalization.NumberStyles.HexNumber);
                else
                    result = long.Parse(arg);
            }
            else
            {
                arg = arg.Substring(1, arg.Length - 2);
                if (ISASimulator.GetRegisters().ContainsKey(arg))
                    result = ISASimulator.GetAddresses()[(long)ISASimulator.GetRegisters()[arg]];
                else if (arg.StartsWith("0X"))
                {
                    long address = long.Parse(arg.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    result = ISASimulator.GetAddresses()[address];
                }
                else
                    result = ISASimulator.GetAddresses()[long.Parse(arg)];
            }
            return result;
        }

        private static void PutValue(string arg, long result)
        {
            if (!arg.StartsWith("["))
                ISASimulator.GetRegisters()[arg] = result;
            else
            {
                arg = arg.Substring(1, arg.Length - 2);
                if (ISASimulator.GetRegisters().ContainsKey(arg))
                    ISASimulator.GetAddresses()[(long)ISASimulator.GetRegisters()[arg]] = (byte)result;
                else if (arg.StartsWith("0X"))
                {
                    long address = long.Parse(arg.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    ISASimulator.GetAddresses()[address] = (byte)result;
                }
                else
                    ISASimulator.GetAddresses()[long.Parse(arg)] = (byte)result;
            }
        }

        public static bool IsNumber(string s)
        {
            try
            {
                long.Parse(s);
                return true;
            }
            catch (FormatException)
            {
                try
                {
                    s = s.Substring(2);
                    long.Parse(s, System.Globalization.NumberStyles.HexNumber);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }
    }
}