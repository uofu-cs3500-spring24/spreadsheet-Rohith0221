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

    // Evaluate method divison error
    [TestMethod]
    public void testDivisionByZero()
    {
        Formula formula = new Formula("2.0/0.0");
        Assert.AreEqual(new FormulaError(" Cannot evaluate as division by zero is not possible!").Reason,
                         formula.Evaluate(null));
    }

    // Rule 5 Test:Starting operator rule and Unary errors check with all 4 operators
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
    [ExpectedException(typeof(FormatException))]
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
    public void IncorrectFormula()
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

    // Rule 6 Ending token rule test
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void invalidOperator()
    {
        Formula formula = new("x6$");
    }

    [TestMethod]
    public void delegateLookupError()
    {
        Formula formula = new("2+x7");
        Assert.AreEqual(new FormulaError(" Delegate cannot find any value for the variable found").Reason,
                         formula.Evaluate(null));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void formulaFormatError_ExtraFollowingRule_FollowingNumber()
    {
        Formula formula = new("2$");
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void formulaFormatError_ExtraFollowingRule_FollowingVariable()
    {
        Formula formula = new("x7%");
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void formulaFormatError_ExtraFollowingRule_FollowingClosingBracket()
    {
        Formula formula = new("(x7)%");
    }

    [TestMethod]
    public void formulaOfVariablesWithUnderscores()
    {
        Formula formula = new("20+x_10");
        Assert.AreEqual(30.0, formula.Evaluate(x10=>10));
    }

}
