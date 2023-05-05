

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
            while (true)
            {
                string input = Console.ReadLine().Trim();
                if (Operations.IsNumber(input))
                {
                    long address = long.Parse(input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? input.Substring(2) : input);
                    Console.WriteLine($"{input}: {(ISASimulator.GetAddresses().ContainsKey(address) ? ISASimulator.GetAddresses()[address] : "0")}");
                }
                else if (input.Equals(Next, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                else if (input.Equals(Continue, StringComparison.OrdinalIgnoreCase))
                {
                    ISASimulator.SetDebuggingMode(false);
                    return;
                }
                else
                {
                    Console.Error.WriteLine("Invalid input!");
                }
            }
        }
    }
}