using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ISASimulator
{
    public class ISASimulator
    {
        private static int interpretationIndex = 0;

        private static readonly Dictionary<string, long?> registers = new();
        private static readonly Dictionary<long, byte> addresses = new();
        private static readonly HashSet<string> keywords = new();
        private static readonly Dictionary<string, int> labels = new();
        private static List<string> code;
        private static readonly List<string> errorList = new();

        private static bool isDebuggingMode = false;
        private static bool isValid = true;

        public const string SwitchToMachineCodeExecution = "SWITCH";

        public static int GetInterpretationIndex()
        {
            return interpretationIndex;
        }

        public static void SetInterpretationIndex(int index)
        {
            interpretationIndex = index;
        }

        public static Dictionary<string, long?> GetRegisters()
        {
            return registers;
        }

        public static Dictionary<long, byte> GetAddresses()
        {
            return addresses;
        }

        public static Dictionary<string, int> GetLabels()
        {
            return labels;
        }

        public static HashSet<string> GetKeywords()
        {
            return keywords;
        }

        public static List<String> GetCode()
        {
            return code;
        }

        public static bool GetDebuggingMode()
        {
            return isDebuggingMode;
        }

        public static void SetDebuggingMode(bool mode)
        {
            isDebuggingMode = mode;
        }

        public static bool IsValid()
        {
            return isValid;
        }

        public static void SetValid(bool v) 
        {
            isValid = v;
        }

        public static List<string> GetErrorList() 
        {
            return errorList;
        }

        public static void InitRegisters()
        {
            registers.Add("RAX", (long)0);
            registers.Add("RBX", (long)0);
            registers.Add("RCX", (long)0);
            registers.Add("RDX", (long)0);
            registers.Add("RSI", (long)0);
            registers.Add("RDI", (long)0);
            registers.Add("RIP", (long)0);
        }

        public static void InitKeywordsAndOperations()
        {
            Operations.GetUnaryOperations().Add("NOT", Operations.Not);
            Operations.GetUnaryOperations().Add("JMP", Operations.Jump);
            Operations.GetUnaryOperations().Add("JE", Operations.JumpEqual);
            Operations.GetUnaryOperations().Add("JNE", Operations.JumpNotEqual);
            Operations.GetUnaryOperations().Add("JGE", Operations.JumpGreaterEqual);
            Operations.GetUnaryOperations().Add("JL", Operations.JumpLess);
            Operations.GetUnaryOperations().Add("PRINT", Operations.Print);
            Operations.GetUnaryOperations().Add("SCAN", Operations.Scan);
            Operations.GetBinaryOperations().Add("ADD", Operations.Add);
            Operations.GetBinaryOperations().Add("SUB", Operations.Sub);
            Operations.GetBinaryOperations().Add("MUL", Operations.Mul);
            Operations.GetBinaryOperations().Add("DIV", Operations.Div);
            Operations.GetBinaryOperations().Add("AND", Operations.And);
            Operations.GetBinaryOperations().Add("OR", Operations.Or);
            Operations.GetBinaryOperations().Add("XOR", Operations.Xor);
            Operations.GetBinaryOperations().Add("MOV", Operations.Mov);
            Operations.GetBinaryOperations().Add("CMP", Operations.Cmp);
            keywords.UnionWith(Operations.GetUnaryOperations().Keys);
            keywords.UnionWith(Operations.GetBinaryOperations().Keys);
            keywords.Add(Debug.Breakpoint);
            keywords.Add(SwitchToMachineCodeExecution);
        }

        public static void InitOpCodes()
        {
            byte i = 0;
            foreach (string keyword in keywords)
            {
                Bytecode.GetOpCodes().Add(i++, keyword);
            }
        }

        public static void InterpretCode()
        {
            for (int i = 0; i < code.Count; i++)
            {
                string line = code[i];
                line = line.Trim();
                int index = line.IndexOf(' ');
                if (index == -1)
                {
                    if (Debug.Breakpoint.Equals(line.ToUpper()))
                    {
                        isDebuggingMode = true;
                        Debug.StartDebug();
                        continue;
                    }
                    else if (SwitchToMachineCodeExecution.Equals(line.ToUpper()))
                    {
                        i++;
                        registers["RIP"] = Bytecode.GetMachineCodeAddresses()[i];
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

                if (isDebuggingMode)
                {
                    Debug.StartDebug();
                }
            }
        }        

        public static void Execute(string[] codeToRun)
        {
            InitRegisters();
            InitKeywordsAndOperations();
            InitOpCodes();

            code = new List<string>(codeToRun);

            Validator.Validate();

            if (!isValid)
            {
                Console.Error.WriteLine("Invalid code!");
                Console.Error.WriteLine("Errors:");
                errorList.ForEach(s => Console.Error.WriteLine(s));
                return;
            }

            Bytecode.TranslateToMachineCode(0x100);
            InterpretCode();
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("No input file specified!");
                return;
            }
            try
            {
                Execute(File.ReadAllLines(args[0]));
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }
        }

    }
}