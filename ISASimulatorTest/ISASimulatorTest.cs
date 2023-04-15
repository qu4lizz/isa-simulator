namespace ISASimulatorTest;
using ISASimulator;

[TestClass]
public class ISASimulatorTest
{
    [TestMethod]
    public void CodeAnalysis()
    {
        try{
            ISASimulator.Execute(File.ReadAllLines("code_analysis.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
        Assert.AreEqual(ISASimulator.IsValid(), false);
    }

    [TestMethod]
    public void DebugTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines("debug_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
        // TODO: Add your test code here
    }

    [TestMethod]
    public void JumpTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines("jump_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
        // TODO: Add your test code here
    }

    [TestMethod]
    public void OperationsTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines("operations_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
        // TODO: Add your test code here
    }

    [TestMethod]
    public void BitwiseTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines("bitwise_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
        // TODO: Add your test code here
    }

    [TestMethod]
    public void Bytecode() {
        try{
            ISASimulator.Execute(File.ReadAllLines("bytecode_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
    }

    [TestMethod]
    public void ScanAndPrint() {
        try{
            ISASimulator.Execute(File.ReadAllLines("scan_and_print_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }

    }

    [TestMethod]
    public void SelfModifyingCode() {
        try{
            ISASimulator.Execute(File.ReadAllLines("self_modifying_code_test.txt"));
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }

    }
}