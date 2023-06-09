

namespace ISASimulator
{
    public class Validator 
    {
        public static void Validate()
        {
            LexicalAndSyntaxAnalysis();

            if (!ISASimulator.IsValid())
                return;

            SemanticAnalysis();
        }

        private static void LexicalAndSyntaxAnalysis() {
            for (int i = 0; i < ISASimulator.GetCode().Count; i++)
            {
                string line = ISASimulator.GetCode()[i].Trim();
                int index = line.IndexOf(' ');
                if (index == -1 && Debug.Breakpoint.Equals(line.ToUpper()))
                    continue;
                if (index == -1 && !ISASimulator.GetKeywords().Contains(line.ToUpper()) && !line.EndsWith(":"))
                {
                    ISASimulator.GetErrorList().Add(line);
                    ISASimulator.SetValid(false);
                }
                else if (index == -1 && line.EndsWith(":"))
                    ISASimulator.GetLabels()[line.Substring(0, line.Length - 1)] = i;
                else
                {
                    string keyword = line.Split(' ')[0];
                    if (!ISASimulator.GetKeywords().Contains(keyword.ToUpper()))
                    {
                        ISASimulator.GetErrorList().Add(keyword);
                        ISASimulator.SetValid(false);
                    }
                }
            }
        }

        private static void SemanticAnalysis() {
            foreach (string str in ISASimulator.GetCode())
            {
                string line = str.Trim();
                int index;
                if ((index = line.IndexOf(' ')) == -1)
                    return;
                string keyword = line.Split(' ')[0].ToUpper();
                string[] operands = line.Substring(index + 1).Replace(" ", "").Split(',');
                if ((Operations.GetUnaryOperations().ContainsKey(keyword) && operands.Length != 1) ||
                    (Operations.GetBinaryOperations().ContainsKey(keyword) && operands.Length != 2))
                {
                    ISASimulator.SetValid(false);
                    ISASimulator.GetErrorList().Add(line);
                    continue;
                }

                if (Operations.IsNumber(operands[0]) && !"PRINT".Equals(keyword) && !"CMP".Equals(keyword))
                {
                    ISASimulator.SetValid(false);
                    ISASimulator.GetErrorList().Add(operands[0]);
                    continue;
                }

                foreach (string o in operands)
                {
                    string oprnd = o.ToUpper();
                    if (!oprnd.StartsWith("[") && !ISASimulator.GetRegisters().ContainsKey(oprnd) && !Operations.IsNumber(oprnd) && !ISASimulator.GetLabels().ContainsKey(o))
                    {
                        ISASimulator.SetValid(false);
                        ISASimulator.GetErrorList().Add(o);
                    }
                    else if (Operations.IsNumber(oprnd))
                    {
                        if (!oprnd.StartsWith("0X"))
                            ISASimulator.GetAddresses()[long.Parse(oprnd)] = 0;
                        else
                            ISASimulator.GetAddresses()[long.Parse(oprnd.Substring(2), System.Globalization.NumberStyles.HexNumber)] = 0;
                    }
                    else if (oprnd.StartsWith("["))
                    {
                        if (!oprnd.EndsWith("]"))
                        {
                            ISASimulator.SetValid(false);
                            ISASimulator.GetErrorList().Add(o);
                            return;
                        }
                        string address = oprnd.Substring(1, oprnd.Length - 2);
                        if (ISASimulator.GetRegisters().ContainsKey(address))
                            return;
                        if (!Operations.IsNumber(address))
                        {
                            ISASimulator.SetValid(false);
                            ISASimulator.GetErrorList().Add(address);
                        }
                        else if (!address.StartsWith("0X"))
                            ISASimulator.GetAddresses()[long.Parse(address)] = 0;
                        else
                            ISASimulator.GetAddresses()[long.Parse(address.Substring(2), System.Globalization.NumberStyles.HexNumber)] = 0;
                    }
                };
            };
        }
    }
}