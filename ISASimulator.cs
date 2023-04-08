using System;
using System.Collections.Generic;
using System.Linq;

namespace ISASimulator
{
    public class ISASimulator
    {
        //Index for the interpretation of the code
        private static int interpretationIndex = 0;


        //Registers
        private static readonly Dictionary<string, long?> registers = new();
        //Memory addresses in address space
        private static readonly Dictionary<long, byte> addresses = new();
        private static readonly HashSet<string> keywords = new();
        // Labels and their corresponding line number
        private static readonly Dictionary<string, int> labels = new();
        private static List<string> code;
        private static readonly List<string> errorList = new();

        public const string Breakpoint = "BREAKPOINT";
        public const string Next = "NEXT";
        public const string Continue = "CONTINUE";

        private static bool isDebuggingMode = false;
        private static bool isValid = true;

        //Instruction for switching to machine code execution
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
            Operations.GetBinaryOperations().Add("AND", Operations.And);
            Operations.GetBinaryOperations().Add("OR", Operations.Or);
            Operations.GetBinaryOperations().Add("MOV", Operations.Mov);
            Operations.GetBinaryOperations().Add("CMP", Operations.Cmp);
            keywords.UnionWith(Operations.GetUnaryOperations().Keys);
            keywords.UnionWith(Operations.GetBinaryOperations().Keys);
            keywords.Add(Breakpoint);
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



        public static void CodeValidation()
        {
            // Syntax analysis
            for (int i = 0; i < code.Count; i++)
            {
                string line = code[i].Trim();
                int index = line.IndexOf(' ');
                if (index == -1 && Breakpoint.Equals(line.ToUpper())) // Breakpoint
                    continue;
                if (index == -1 && !keywords.Contains(line.ToUpper()) && !line.EndsWith(":")) // Invalid keyword
                {
                    errorList.Add(line);
                    isValid = false;
                }
                else if (index == -1 && line.EndsWith(":")) // Label 
                    labels[line.Substring(0, line.Length - 1)] = i;
                else
                {
                    string keyword = line.Split(' ')[0];
                    if (!keywords.Contains(keyword.ToUpper()))
                    {
                        errorList.Add(keyword);
                        isValid = false;
                    }
                }
            }

            if (!isValid)
                return;

            // Semantic analysis
            // Check for number of operands
            // Check for validity of operands
            // Check if a number is the first operand
            code.ForEach(s =>
            {
                s = s.Trim();
                int index = s.IndexOf(' ');
                if (index == -1)
                    return;
                string keyword = s.Split(' ')[0].ToUpper();
                string[] operands = s.Substring(index + 1).Replace(" ", "").Split(',');
                if (Operations.GetUnaryOperations().ContainsKey(keyword) && operands.Length != 1)
                {
                    isValid = false;
                    errorList.Add(s);
                }
                else if (Operations.GetBinaryOperations().ContainsKey(keyword) && operands.Length != 2)
                {
                    isValid = false;
                    errorList.Add(s);
                }

                if (Operations.IsNumber(operands[0]) && !"PRINT".Equals(keyword) && !"CMP".Equals(keyword))
                {
                    isValid = false;
                    errorList.Add(operands[0]);
                }

                Array.ForEach(operands, o =>
                {
                    string oprnd = o.ToUpper();
                    if (!oprnd.StartsWith("[") && !registers.ContainsKey(oprnd) && !Operations.IsNumber(oprnd) && !labels.ContainsKey(o))
                    {
                        isValid = false;
                        errorList.Add(o);
                    }
                    else if (Operations.IsNumber(oprnd))
                    {
                        if (!oprnd.StartsWith("0X"))
                            addresses[long.Parse(oprnd)] = 0;
                        else
                            addresses[long.Parse(oprnd.Substring(2), System.Globalization.NumberStyles.HexNumber)] = 0;
                    }
                    else if (oprnd.StartsWith("["))
                    {
                        if (!oprnd.EndsWith("]"))
                        {
                            isValid = false;
                            errorList.Add(o);
                            return;
                        }
                        string address = oprnd.Substring(1, oprnd.Length - 2);
                        if (registers.ContainsKey(address))
                            return;
                        if (!Operations.IsNumber(address))
                        {
                            isValid = false;
                            errorList.Add(address);
                        }
                        else if (!address.StartsWith("0X"))
                            addresses[long.Parse(address)] = 0;
                        else
                            addresses[long.Parse(address.Substring(2), System.Globalization.NumberStyles.HexNumber)] = 0;
                    }
                });
            });
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
                    if (Breakpoint.Equals(line.ToUpper()))
                    {
                        isDebuggingMode = true;
                        Debug();
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
                    Debug();
                }
            }
        }

        public static void Debug()
        {
            Console.WriteLine();
            foreach (var reg in registers)
            {
                Console.WriteLine(reg.Key + " " + reg.Value);
            }

            Console.WriteLine("Enter memory address for examination or NEXT or CONTINUE:");
            string input = "";
            do
            {
                input = Console.ReadLine();
                if (Operations.IsNumber(input))
                {
                    if (input.StartsWith("0x") || input.StartsWith("0X"))
                    {
                        long address = long.Parse(input.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        Console.WriteLine(input + ": " + (addresses.ContainsKey(address) ? addresses[address] : 0));
                    }
                    else
                    {
                        long address = long.Parse(input);
                        Console.WriteLine(input + ": " + (addresses.ContainsKey(address) ? addresses[address] : 0));
                    }
                }
                else if (Next.Equals(input.ToUpper()))
                {
                    return;
                }
                else if (Continue.Equals(input.ToUpper()))
                {
                    isDebuggingMode = false;
                    return;
                }
                else
                {
                    Console.Error.WriteLine("Invalid command or memory address!");
                }
            } while (true);
        }

        public static void Execute(string[] codeToRun)
        {
            InitRegisters();
            InitKeywordsAndOperations();
            InitOpCodes();

            code = new List<string>(codeToRun);

            CodeValidation();

            if (!isValid)
            {
                Console.Error.WriteLine("Code not valid.");
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
                Console.Error.WriteLine("Missing an argument.");
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