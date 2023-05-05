using System;
using System.Collections.Generic;

namespace ISASimulator
{
    public class Operations
    {
        private static readonly Dictionary<string, Action<string>> UnaryOperations = new();
        private static readonly Dictionary<string, Action<string, string>> BinaryOperations = new();

        private static int equal, less;

        public static Dictionary<string, Action<string, string>> GetBinaryOperations()
        {
            return BinaryOperations;
        }

        public static Dictionary<string, Action<string>> GetUnaryOperations()
        {
            return UnaryOperations;
        }
        
        public static void Jump(string label)
        {
            Process(label);
        }

        public static void JumpEqual(string label)
        {
            if (equal == 1)
            {
                Process(label);
            }
        }

        public static void JumpNotEqual(string label)
        {
            if (equal == 0)
            {
                Process(label);
            }
        }

        public static void JumpGreaterEqual(string label)
        {
            if (less == 0)
            {
                Process(label);
            }
        }

        public static void JumpLess(string label)
        {
            if (less == 1)
            {
                Process(label);
            }
        }

        private static void Process(string label)
        {
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

        public static void Not(string val)
        {
            SetRegisterValue(val.ToUpper(), ~GetRegisterValue(val.ToUpper()));
        }

        public static void Add(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) + GetRegisterValue(val2));
        }

        public static void Sub(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) - GetRegisterValue(val2));
        }

        public static void Mul(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) * GetRegisterValue(val2));
        }

        public static void Div(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) / GetRegisterValue(val2));
        }

        public static void And(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) & GetRegisterValue(val2));
        }

        public static void Or(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) | GetRegisterValue(val2));
        }

        public static void Xor(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val1) ^ GetRegisterValue(val2));
        }

        public static void Mov(string val1, string val2)
        {
            SetRegisterValue(val1, GetRegisterValue(val2));
        }

        public static void Scan(string val)
        {
            string input = Console.ReadLine();
            val = val.ToUpper();
            if (IsNumber(input))
            {
                SetRegisterValue(val, GetNumberFromString(input));
            }
            else
            {
                if (!val.StartsWith("[")) 
                {
                    SetRegisterValue(val, GetNumberFromInputString(input));
                }
                else
                {
                    int result = (int)input[0];
                    SetRegisterValue(val, result);
                }
            }
        }

        private static long GetNumberFromString(string input)
        {
            return input.StartsWith("0x") || input.StartsWith("0X") ? Convert.ToInt64(input.Substring(2), 16) : Convert.ToInt64(input);
        }

        private static long GetNumberFromInputString(string input)
        {
            long result = 0;
            int length = Math.Min(8, input.Length);
            for (int i = 0; i < length; i++)
            {
                result = result * 100 + (int)input[i];
            }
            return result;
        }

        public static void Print(string val)
        {
            Console.WriteLine(GetRegisterValue(val.ToUpper()));
        }

        public static void Cmp(string val1, string val2)
        {
            long num1 = GetRegisterValue(val1);
            long num2 = GetRegisterValue(val2);
            
            equal = num1 == num2 ? 1 : 0;
            less = num1 < num2 ? 1 : 0;
        }

        private static long GetRegisterValue(string val)
        {
            bool isAddress = val.StartsWith("[");
            val = isAddress ? val.Substring(1, val.Length - 2) : val;
            bool isHex = val.StartsWith("0X");
            val = isHex ? val.Substring(2) : val;
            long result = 0;
            
            if (isAddress)
            {
                if (IsNumber(val))
                {
                    result = long.Parse(val, isHex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.None);
                }
                else
                {
                    result = ISASimulator.GetRegisters()[val] ?? -1;
                }
            }
            else
            {
                if (ISASimulator.GetRegisters().ContainsKey(val))
                {
                    result = ISASimulator.GetRegisters()[val] ?? -1;
                }
                else
                {
                    result = long.Parse(val, isHex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.None);
                }
            }

            return result;
        }

        private static void SetRegisterValue(string val, long result)
        {
            long address = 0;
            bool isAddress = false;

            if (val.StartsWith("[") && val.EndsWith("]"))
            {
                val = val.Substring(1, val.Length - 2);
                isAddress = true;
            }

            if (ISASimulator.GetRegisters().ContainsKey(val))
            {
                ISASimulator.GetRegisters()[val] = result;
            }
            else
            {
                if (val.StartsWith("0X") || val.StartsWith("0x"))
                {
                    if (long.TryParse(val.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out address))
                    {
                        isAddress = true;
                    }
                }
                else
                {
                    if (long.TryParse(val, out address))
                    {
                        isAddress = true;
                    }
                }
                if (isAddress)
                {
                    ISASimulator.GetAddresses()[address] = (byte)result;
                }
            }
        }

        public static bool IsNumber(string s)
        {
            bool isHex = s.StartsWith("0x") || s.StartsWith("0X");
            if (isHex)
                s = s.Substring(2);
            if (long.TryParse(s, out long result))
                return true;
            if (isHex && long.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out result))
                return true;
            return false;
        }
    }
}