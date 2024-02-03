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
        Formula f = new("");
    }

    [TestMethod]
    public void testToStringOnSpacedFormula()
    {
        Formula nonSpaced = new("x+y", s => s.ToUpper(),s=>true);
        Formula spaced = new("x      +   Y");
        Assert.IsTrue(spaced.ToString().Equals(nonSpaced.ToString()));
        Assert.AreEqual(nonSpaced.ToString(),spaced.ToString());
    }

    [TestMethod]
    public void testEqualityBetweenTwoFormulasUsingEqualsOperator()
    {
        Formula f1 = new("2.0+x7");
        Formula f2 = new("2.000+x7");
        Assert.IsTrue(f1.Equals(f2));
        Assert.IsTrue(Formula.ReferenceEquals(f1,f2));
    }
}
