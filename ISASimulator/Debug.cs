

namespace ISASimulator {
    public class Debug 
    {
        public const string Breakpoint = "BREAKPOINT";
        public const string Next = "NEXT";
        public const string Continue = "CONTINUE";
        public static void StartDebug()
        {
            Console.WriteLine();
            foreach (var reg in ISASimulator.GetRegisters())
            {
                Console.WriteLine(reg.Key + " " + reg.Value);
            }

            Console.WriteLine("Enter a number to get its value or enter " + Next + " to continue or enter " + Continue + " to continue without debugging");
            string input = "";
            do
            {
                input = Console.ReadLine();
                if (Operations.IsNumber(input))
                {
                    if (input.StartsWith("0x") || input.StartsWith("0X"))
                    {
                        long address = long.Parse(input.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        Console.WriteLine(input + ": " + (ISASimulator.GetAddresses().ContainsKey(address) ? ISASimulator.GetAddresses()[address] : 0));
                    }
                    else
                    {
                        long address = long.Parse(input);
                        Console.WriteLine(input + ": " + (ISASimulator.GetAddresses().ContainsKey(address) ? ISASimulator.GetAddresses()[address] : 0));
                    }
                }
                else if (Next.Equals(input.ToUpper()))
                {
                    return;
                }
                else if (Continue.Equals(input.ToUpper()))
                {
                    ISASimulator.SetDebuggingMode(false);
                    return;
                }
                else
                {
                    Console.Error.WriteLine("Invalid input!");
                }
            } while (true);
        }
    }
}