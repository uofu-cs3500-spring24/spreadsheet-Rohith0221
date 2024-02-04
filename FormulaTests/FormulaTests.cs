namespace FormulaTests;
using SpreadsheetUtilities;

/// <summary>
/// 
/// Author  :          Rohith V
/// Project Created :  Feb 3,2023
/// Partner :          None
/// 
///  FormulaTest class is a test suite containing all the tests that exploit the bugs in the
///  Formula class simultaneously testing out the functionality and its output
/// </summary>
/// 
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
        Assert.IsFalse(spaced==nonSpaced);
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

    // isValid delegate will throw exception if '/'operator is found
    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void passingFalseFor_isValidDelegate()
    {
        Formula formula = new("2987/30+1", s => s, s => { if (s.Equals("/")) { return false;} return true;});
    }

    [TestMethod]
    public void testingEqualSymbolOverloaded()
    {
        Formula formula = new("2+x7", s => s.ToUpper(), s => true);
        Formula spaced = new("2  + X7");
        Assert.IsTrue(formula.ToString() == spaced.ToString());
    }

    [TestMethod]
    public void testingNotEqualSymbolOverloaded()
    {
        Formula formula = new("2+x7", s => s.ToUpper(), s => true);
        Formula spaced = new("2  + x7");
        Assert.IsTrue(formula.ToString() != spaced.ToString());
    }

    [TestMethod]
    public void testGetHashCodeEquality()
    {
        Formula formula1 = new("2+x7", s => s.ToUpper(), s => true);
        Formula formula2 = new("2+  X7");
        Assert.IsTrue(formula1.Equals(formula2));
        formula1.Evaluate(X7 => 17);
        formula2.Evaluate(X7 => 17);
        Assert.IsTrue(formula1.GetHashCode() == formula2.GetHashCode());
    }

    [TestMethod]
    public void testGetHashCodeUnequality()
    {
        Formula formula1 = new("2+x7");
        Formula formula2 = new("2+  X7");
        Assert.IsFalse(formula1.Equals(formula2));
        formula1.Evaluate(X7 => 17);
        formula2.Evaluate(X7 => 17);
        Assert.IsTrue(formula1.GetHashCode() != formula2.GetHashCode());

    }

    [TestMethod]
    public void evaluateMultiplicationAndDivision()
    {
        Formula formula = new("(2*3)/6");
        Assert.AreEqual(1.0,formula.Evaluate(null));
    }

    [TestMethod]
    public void evaluateMultiplicationAndDivisionWithVariables()
    {
        Formula formula = new("(2*x1)/x1");
        Assert.AreEqual(2.0, formula.Evaluate(x1=>3));
    }

    [TestMethod]
    public void divideByZeroValueVariable()
    {
        Formula formula = new("2/x1");
        Assert.AreEqual(new FormulaError(" Cannot evaluate as division by zero is not possible!").Reason,formula.Evaluate(x1 => 0));
    }

    [TestMethod]
    public void threeOperatorsWithPlus()
    {
        Formula formula = new("2*2+3+2");
        Assert.AreEqual(9.0, formula.Evaluate(null));
    }

    [TestMethod]
    public void threeOperatorsWithMinus()
    {
        Formula formula = new("2*2-3+2");
        Assert.AreEqual(3.0, formula.Evaluate(null));
    }

    [TestMethod]
    public void equalsMethodForNonFormulaObject()
    {
        Formula f1 = new("2");
        String f2 = "2";
        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    public void complexMath()
    {
        Formula complex = new Formula("2*(3+x2)/(13)-2+90*120/(x2)-(x2*10)");
        Assert.AreEqual(980.0,complex.Evaluate(x2 => 10));
    }

    [TestMethod]
    public void testEqualityOn_UnequalNumberOfTokenBetweenTwoStrings()
    {
        Formula f1 = new("x1+x2");
        Formula f2 = new("x1");

        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void syntaxError()
    {
        Formula f = new("A 1");

    }

    [TestMethod]
    public void evaluationError()
    {
        Formula f = new("1/(A1-A1)");
        Assert.AreEqual(new FormulaError(" Cannot evaluate as division by zero is not possible!").Reason, f.Evaluate(A1 => 1));
    }

    [TestMethod]
    public void testDoubleAsStringEquality_usingToStringMethod()
    {
        Formula f1 = new("2.0");
        Formula f2 = new("2");
        String f2String = f2.ToString();
        Console.WriteLine(f2String);
        Assert.IsTrue(f1.Equals(f2));
        Assert.IsTrue(f1==f2);
        Console.WriteLine(f1.Equals(f2));
    }

    [TestMethod]
    public void testingEqualOnNullObject()
    {
        Formula f1 = new("1");
        String s = null;
        Assert.IsFalse(f1.Equals(s));
    }

    [TestMethod]
    public void equals_DiffersByDoubleValue()
    {
        Formula f1 = new("X+1.0");
        Formula f2 = new("X+2.0");
        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    public void equals_DiffersByOperator()
    {
        Formula f1 = new("X+1.0");
        Formula f2 = new("X-1.0");
        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    public void notEqualOperatorFalse()
    {
        Formula f1 = new("x+1", s => s.ToUpper(), s => true);
        Formula f2 = new("X+1");
        Assert.IsFalse(f1 != f2);
        Assert.IsTrue(f1 == f2);
    }

    [TestMethod]
    public void notEqualOperatorTrue()
    {
        Formula f1 = new("x+1");
        Formula f2 = new("X+1");
        Assert.IsTrue(f1 != f2);
        Assert.IsFalse(f1 == f2);
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void invalidToken()
    {
        Formula f = new("x+6$-(2)");

    }
}
