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
            registers["RAX"] = 0;
            registers["RBX"] = 0;
            registers["RCX"] = 0;
            registers["RDX"] = 0;
            registers["RSI"] = 0;
            registers["RDI"] = 0;
            registers["RIP"] = 0;
        }

        public static void InitKeywordsAndOperations()
        {
            Operations.GetUnaryOperations()["NOT"] = Operations.Not;
            Operations.GetUnaryOperations()["JMP"] = Operations.Jump;
            Operations.GetUnaryOperations()["JE"] = Operations.JumpEqual;
            Operations.GetUnaryOperations()["JNE"] = Operations.JumpNotEqual;
            Operations.GetUnaryOperations()["JGE"] = Operations.JumpGreaterEqual;
            Operations.GetUnaryOperations()["JL"] = Operations.JumpLess;
            Operations.GetUnaryOperations()["PRINT"] = Operations.Print;
            Operations.GetUnaryOperations()["SCAN"] = Operations.Scan;
            Operations.GetBinaryOperations()["ADD"] = Operations.Add;
            Operations.GetBinaryOperations()["SUB"] = Operations.Sub;
            Operations.GetBinaryOperations()["MUL"] = Operations.Mul;
            Operations.GetBinaryOperations()["DIV"] = Operations.Div;
            Operations.GetBinaryOperations()["AND"] = Operations.And;
            Operations.GetBinaryOperations()["OR"] = Operations.Or;
            Operations.GetBinaryOperations()["XOR"] = Operations.Xor;
            Operations.GetBinaryOperations()["MOV"] = Operations.Mov;
            Operations.GetBinaryOperations()["CMP"] = Operations.Cmp;
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
                Bytecode.GetOpCodes()[i++] = keyword;
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
            Interpreter.Interpret();
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