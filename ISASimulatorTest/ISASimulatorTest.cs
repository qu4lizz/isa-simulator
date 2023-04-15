namespace ISASimulatorTest;

using System.Text;
using ISASimulator;

[TestClass]
public class ISASimulatorTest
{
    string currentDir;

    [TestInitialize]
    public void Init()
    {
        string projectDirectory = Directory.GetCurrentDirectory();

        for (int i = 0; i < 3; i++)
        {
            projectDirectory = Directory.GetParent(projectDirectory).FullName;
        }
        currentDir = projectDirectory + "/test_files";
    }

    [TestMethod]
    public void CodeAnalysis()
    {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/code_analysis.txt"));
            Assert.AreEqual(ISASimulator.IsValid(), false);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void JumpTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/jump.txt"));
            Assert.AreEqual(1, ISASimulator.GetRegisters()["RAX"]);
            Assert.AreEqual(3, ISASimulator.GetRegisters()["RBX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void JumpEqual() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/jump_equal.txt"));
            Assert.AreEqual(5, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void JumpNotEqual() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/jump_not_equal.txt"));
            Assert.AreEqual(11, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void AddTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/add.txt"));
            Assert.AreEqual(10, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void SubtractionTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/sub.txt"));
            Assert.AreEqual(10, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void MultiplicationTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/mul.txt"));
            Assert.AreEqual(10, ISASimulator.GetRegisters()["RAX"]);

        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void DivisionTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/div.txt"));
            Assert.AreEqual(10, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void BitwiseTest() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/bitwise.txt"));
            Assert.AreEqual(12, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void Bytecode() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/bytecode.txt"));
            Assert.AreEqual(17, ISASimulator.GetRegisters()["RAX"]);

        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }

    [TestMethod]
    public void SelfModifyingCode() {
        try{
            ISASimulator.Execute(File.ReadAllLines(currentDir + "/self_modifying_code.txt"));
            Assert.AreEqual(15, ISASimulator.GetRegisters()["RAX"]);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }

    }
}