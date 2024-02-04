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
        IEnumerator<string>enumerator=formula.GetVariables().GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual("X",enumerator.Current);
        enumerator.MoveNext();
        Assert.AreEqual("Y", enumerator.Current);
        enumerator.MoveNext();
        Assert.AreEqual("Z", enumerator.Current);

    }

    [TestMethod]
    public void testUnequalsUsingEqualOperator()
    {
        Formula nonSpaced = new("x+y", s => s.ToUpper(),s=>true);
        Formula spaced = new("x      +   Y");
        Assert.IsTrue(spaced.Equals(nonSpaced));
    }

    [TestMethod]
    public void testEqualityBetweenTwoFormulasUsingEqualsOperator()
    {
        Formula f1 = new("2.0+x7");
        Formula f2 = new("2.000+x7");
        Assert.IsTrue(f1.Equals(f2));
    }

    [TestMethod]
    public void testEvaluateUsingLookup()
    {
        Formula formula = new Formula("2.0+x7");
        Assert.AreEqual(9.0,formula.Evaluate(x7=>7.0));
    }

    [TestMethod]
    public void testEvaluateForNormalisedFormulaUsingLookup()
    {
        Formula formula = new Formula("2.0+x7",s=>s.ToUpper(),s=>true);
        Assert.AreEqual(9.0, formula.Evaluate(x7 => 7.0));
    }

    [TestMethod]
    public void testDivisionByZero()
    {
        Formula formula = new Formula("2.0/0.0");
        Console.WriteLine(formula.Evaluate(null));
    }

    // Starting operator rule and Unary errors check with all 4 operators
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void testUnaryError1()
    {
        Formula formula = new("+9");
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void testUnaryError2()
    {
        Formula formula = new("-9");
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void testUnaryError3()
    {
        Formula formula = new("*9");
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void testUnaryError4()
    {
        Formula formula = new("/9");
    }

    // Rule 7 error with invalid closing parenthesis after + operator

    [TestMethod]
    //[ExpectedException(typeof(FormatException))]
    public void testFormulaWithMoreClosingBrackets()
    {
        Formula formula = new("2*(3+)5)");
    }

    // Rule 4 error with more open brackets
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void testFormulaWithMoreOpenBrackets()
    {
        Formula formula = new("(2*(3+5)");
    }

    // Rule 3 right parenthesis rule
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void FormulaError()
    {
        Formula formula = new("(2)+3)");
    }

    // Rule 2 Empty Formula with no tokens
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void emptyFormula()
    {
        Formula formula = new("");
    }

    [TestMethod]
    //[ExpectedException(typeof(FormatException))]
    public void invalidOperator()
    {
        Formula formula = new("9%2");
    }
}
