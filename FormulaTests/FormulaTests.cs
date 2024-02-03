namespace FormulaTests;
using SpreadsheetUtilities;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void InvalidEndingOperator()
    {
        Formula formula = new("x+");
    }

    [TestMethod]
    public void testGetVariables()
    {
        Formula formula = new("x+y+z", s => s.ToUpper(), s=>true);
        IEnumerator<string> enumerator = formula.GetVariables().GetEnumerator();
        while(enumerator.MoveNext())
           Console.WriteLine(enumerator.Current);
    }
}
