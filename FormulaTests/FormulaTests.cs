namespace FormulaTests;
using SpreadsheetUtilities;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Formula formula = new("x+y");
    }
}
