// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
  /// <summary>
  /// Represents formulas written in standard infix notation using standard precedence
  /// rules.  The allowed symbols are non-negative numbers written using double-precision 
  /// floating-point syntax (without unary preceeding '-' or '+'); 
  /// variables that consist of a letter or underscore followed by 
  /// zero or more letters, underscores, or digits; parentheses; and the four operator 
  /// symbols +, -, *, and /.  
  /// 
  /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
  /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
  /// and "x 23" consists of a variable "x" and a number "23".
  /// 
  /// Associated with every formula are two delegates:  a normalizer and a validator.  The
  /// normalizer is used to convert variables into a canonical form, and the validator is used
  /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
  /// that it consist of a letter or underscore followed by zero or more letters, underscores,
  /// or digits.)  Their use is described in detail in the constructor and method comments.
  /// </summary>
  public class Formula
  {
        private string normalisedAndValidatedString;
        private List<string> normalisedTokens = new();

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    /// 
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.
    /// </summary>
    public Formula(String formula) :
        this(formula, s => s, s => true)
    {
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    /// 
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.  
    /// 
    /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
    /// throws a FormulaFormatException with an explanatory message. 
    /// 
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    /// 
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    /// 
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
            List<string> tokens=GetTokens(formula).ToList();
            normalisedTokens=normalise(normalize, tokens);
            Boolean validation=validateTokens(normalisedTokens,isValid);
            normalisedAndValidatedString = string.Join("",normalisedTokens);
    }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
    {
            try
            {
                Stack<double> valueStack = new();
                Stack<string> operatorStack = new();

                for (int index= 0;index<normalisedTokens.Count();index++)
                {

                        normalisedTokens[index] = normalisedTokens[index].Trim();

                        // If theres a unary operator,throws an exception as it's considered bad formula

                        // If after splitting into tokens, first token is an empty space then looks one spot additional to check for
                        // improper unary operator 
                        //if (index == 0 && (!((index + 1) >= normalisedTokens.Count())) && ((normalisedTokens[index].Equals("")) && (normalisedTokens[index + 1].Equals("-")
                        //    || (normalisedTokens[index + 1].Equals("+"))) && (normalisedTokens[index + 2].Equals("(")
                        //    || validateIsVariable(normalisedTokens[index + 2]) || (Double.TryParse(normalisedTokens[index + 2], out Double convertedIntValue))
                        //    && convertedIntValue >= 0)))
                        //{
                        //    throw new ArgumentException(" Bad formula with unary operator found ! ");
                        //}

                        //else if (index == 0 && (!((index + 1) >= normalisedTokens.Count())) && (normalisedTokens[index].Equals("-")
                        //        || (normalisedTokens[index].Equals("+"))) && (normalisedTokens[index + 1].Equals("(") ||
                        //        validateIsVariable(normalisedTokens[index + 1]) || Double.TryParse(normalisedTokens[index + 1], out Double convertedValue)
                        //        && convertedValue >= 0))
                        //{
                        //    throw new ArgumentException(" Bad formula with unary operator found ! ");
                        //}

                        if (normalisedTokens[index].Equals(""))
                            continue;

                            /// If token is an integer
                        if (Double.TryParse(normalisedTokens[index], out double parsedValue))
                            {
                                /// if operatorStack has either multiplication or division operator, performs right operation
                                /// and pushes result onto valueStack

                                if (operatorStack != null && (operatorStack.Count != 0) && ((operatorStack.Peek().Equals("/"))
                                     || (operatorStack.Peek().Equals("*"))))
                                {
                                    if (valueStack != null && valueStack.Count != 0)
                                    {
                                        double poppedInt = valueStack.Pop();
                                        string poppedOperator = operatorStack.Pop();
                                        if (poppedOperator.Equals("*"))
                                            valueStack.Push(poppedInt * parsedValue);
                                        else if (poppedOperator.Equals("/"))
                                        {
                                            if (parsedValue == 0.0)
                                                throw new ArgumentException("Divide by Zero error");
                                            valueStack.Push(poppedInt / parsedValue);
                                        }
                                    }
                                }
                                else
                                    valueStack.Push(parsedValue);
                            }
                            // If token is a variable , delegate is used to find value of that
                            else if (validateIsVariable(normalisedTokens[index]))
                            {
                                try
                                {
                                    // If operatorStack has '/' or '*'
                                    if ((operatorStack != null && operatorStack.Count != 0)
                                       && ((operatorStack.Peek().Equals("/"))
                                       || (operatorStack.Peek().Equals("*"))))
                                    {
                                        if (valueStack != null && valueStack.Count != 0)
                                        {
                                            double poppedInt = valueStack.Pop();
                                            string poppedOperator = operatorStack.Pop();
                                            if (poppedOperator.Equals("*"))
                                                valueStack.Push(poppedInt * lookup(normalisedTokens[index]));
                                            else if (poppedOperator.Equals("/"))
                                            {
                                                try
                                                {
                                                    valueStack.Push(poppedInt / lookup(normalisedTokens[index]));
                                                }
                                                catch (Exception)
                                                {
                                                    throw new ArgumentException(" Cannot divide by zero! ");
                                                }
                                            }
                                        }
                                    }
                                    else
                                        valueStack.Push(lookup(normalisedTokens[index]));
                                }
                                catch (Exception e)
                                {
                                    throw new ArgumentException("No value found for given variable !");
                                }

                            }

                            // checks if parsed token is either '+' or '-' operator and if so pushes operator onto the operator stack
                            else if (normalisedTokens[index].Equals("+") || normalisedTokens[index].Equals("-"))
                            {
                                if (operatorStack != null && operatorStack.Count != 0)

                                {
                                    if (operatorStack.Peek().Equals("+") || operatorStack.Peek().Equals("-"))
                                    {
                                        if (valueStack.Count >= 2)
                                        {
                                            double value1 = valueStack.Pop();
                                            double value2 = valueStack.Pop();
                                            string poppedOperator = operatorStack.Pop();
                                            if (poppedOperator.Equals("+"))
                                            {
                                                valueStack.Push(value1 + value2);
                                                operatorStack.Push(normalisedTokens[index]);
                                            }
                                            else if (poppedOperator.Equals("-"))
                                            {
                                                valueStack.Push(value2 - value1);
                                                operatorStack.Push(normalisedTokens[index]);
                                            }
                                        }
                                        else
                                            operatorStack.Push(normalisedTokens[index]);
                                    }
                                    else
                                        operatorStack.Push(normalisedTokens[index]);
                                }
                                else
                                    operatorStack.Push(normalisedTokens[index]);
                            }

                            // checks if parsed token is either '*' or '/' operator and if so pushes operator onto the operator stack
                            else if (normalisedTokens[index].Equals("*") || normalisedTokens[index].Equals("/"))
                                operatorStack.Push(normalisedTokens[index]);

                            else if (normalisedTokens[index].Equals("("))
                                operatorStack.Push(normalisedTokens[index]);

                            // If top of the opeatorStack is ")" 
                            else if (normalisedTokens[index].Equals(")"))
                            {
                                // performs the operation by checking for the errors

                                /// If top of the operatorStack is either "+" or "-"
                                /// then applies operator to the popped values
                                if (operatorStack.Count != 0 && valueStack.Count >= 2 &&
                                    operatorStack.Peek().Equals("+") ||
                                    operatorStack.Peek().Equals("-"))
                                {
                                    double value1 = valueStack.Pop();
                                    double value2 = valueStack.Pop();
                                    string poppedOperator = operatorStack.Pop();
                                    if (poppedOperator.Equals("+"))
                                        valueStack.Push(value1 + value2);
                                    else if (poppedOperator.Equals("-"))
                                        valueStack.Push(value2 - value1);
                                }

                                // Step 2 if "(" is at top of operator stack, pops it
                                if (operatorStack != null && operatorStack.Count != 0
                                    && operatorStack.Peek().Equals("("))
                                    operatorStack.Pop();
                                else
                                    throw new ArgumentException(" ')' operator not found ");

                                /// Step 3
                                /// If top of the operatorStack is either  "*" or "/"
                                /// applies the poppedOperator to the values
                                if (operatorStack != null && operatorStack.Count != 0
                                    && (operatorStack.Peek().Equals("*") ||
                                    operatorStack.Peek().Equals("/")))
                                {
                                    if (valueStack.Count >= 2)
                                    {
                                        double value1 = valueStack.Pop();
                                        double value2 = valueStack.Pop();

                                        string poppedOperator = operatorStack.Pop();

                                        if (poppedOperator.Equals("*"))
                                            valueStack.Push(value1 * value2);
                                        else if (poppedOperator.Equals("/"))
                                            if (value1 == 0)
                                                throw new ArgumentException(" Cannot divide by zero ");
                                            else
                                                valueStack.Push(value2 / value1);
                                    }
                                }
                        }
                        else
                            throw new ArgumentException(" Invalid operator " + normalisedTokens[index] + " found !");
                    }

                    /*
                     * These operations happen when the last token has been parsed by looking at both of the stacks and performs 
                     * needed operation according to the algorithm provided
                     */

                    if (operatorStack.Count == 0 && valueStack.Count == 1)
                        return valueStack.Pop();

                    else if (operatorStack.Count == 1 && valueStack.Count == 2 &&
                        (operatorStack.Peek().Equals("+") || operatorStack.Peek().Equals("-")))
                    {
                        String poppedOperator = operatorStack.Pop();
                        double value1 = valueStack.Pop();
                        double value2 = valueStack.Pop();

                        if (poppedOperator.Equals("+"))
                            return value1 + value2;
                        else if (poppedOperator.Equals("-"))
                            return value2 - value1;
                    }
                }
            catch(ArgumentException e)
            {
                if (e.Message.Equals("Divide by Zero error"))
                    return new FormulaError(" Cannot evaluate as division by zero is not possible!").Reason;
                return new FormulaError("Formula cannot be evaluated!").Reason;
            }

      return null;
    }

    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in this 
    /// formula.  No normalization may appear more than once in the enumeration, even 
    /// if it appears more than once in this Formula.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    /// 
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<String> GetVariables()
    {
      List<string> normalisedVariableTokens = new();
      foreach(string token in normalisedTokens)
            {
                if (token.Equals("+") || token.Equals("-") || token.Equals("*")
                || token.Equals("/") || token.Equals("(") || token.Equals(")")
                || Double.TryParse(token, out double castValue))
                    continue;
                normalisedVariableTokens.Add(token);
            }
      return normalisedVariableTokens;
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    /// 
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
      string trimmedFormula = normalisedAndValidatedString.Trim();
      return trimmedFormula;
    }

    /// <summary>
    ///  <change> make object nullable </change>
    ///
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    /// 
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings 
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" 
    /// by C#'s standard conversion from string to double, then back to string. This 
    /// eliminates any inconsistencies due to limited floating point precision.
    /// Variable tokens are considered equal if their normalized forms are equal, as 
    /// defined by the provided normalizer.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///  
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
       bool equalityCheck = true;
       if (obj == null && !(obj is Formula))
                return false;
       Formula castedFormula = (Formula)obj;
            if (castedFormula.normalisedTokens.Count() == this.normalisedTokens.Count())
            {
                for (int i = 0; i < normalisedTokens.Count(); i++)
                {
                    if (validateIsVariable(castedFormula.normalisedTokens[i]) && validateIsVariable(normalisedTokens[i]))
                    {
                        if (validateIsVariable(castedFormula.normalisedTokens[i]).Equals(validateIsVariable(normalisedTokens[i])))
                            continue;
                        equalityCheck = false;
                        return equalityCheck;
                    }
                     else if (!(Double.TryParse(castedFormula.normalisedTokens[i],out double value1)
                        && Double.TryParse(normalisedTokens[i], out double value2))
                       || (validateIsVariable(castedFormula.normalisedTokens[i]) && validateIsVariable(normalisedTokens[i])))
                    {
                        if (castedFormula.normalisedTokens[i].Equals(normalisedTokens[i]))
                            continue;
                        else
                            equalityCheck = false;
                            return equalityCheck;
                    }
                    else 
                    {
                        if (Double.TryParse(castedFormula.normalisedTokens[i], out value1)
                         && Double.TryParse(normalisedTokens[i], out value2))
                        {
                            if (value1 == value2)
                                continue;
                            equalityCheck = false;
                        }
                        return equalityCheck;
                    }
                }
            }
            else
            {
                equalityCheck = false;
                return equalityCheck;
            }
       return equalityCheck;
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// 
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
       if (f1.ToString().Equals(f2.ToString()))
            return true;
      return false;
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
    ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
      return false;
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
      return 0;
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
    /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
    /// match one of those patterns.  There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(String formula)
    {
      // Patterns for individual tokens
      String lpPattern = @"\(";
      String rpPattern = @"\)";
      String opPattern = @"[\+\-*/]";
      String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
      String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
      String spacePattern = @"\s+";

      // Overall pattern
      String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                      lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

      // Enumerate matching tokens that don't consist solely of white space.
      foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
      {
        if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
        {
          yield return s;
        }
      }

    }

    /// <summary>
    ///  Helper method to validate the given string expression according to the
    ///  given rules 
    /// </summary>
    /// <param name="tokensToBeValidated"></param>
    /// <returns></returns> true if expression is valid otherwise false
    private Boolean validateTokens(List<string>tokensToBeValidated,
                                          Func<string,bool>isValid)
        {
            int closing_bracesCount = 0;
            int opening_bracesCount = 0;

            // Rule 5 Starting token rule violated
            if (!(tokensToBeValidated[0].Equals("(") || validateIsVariable(tokensToBeValidated[0])
                || Double.TryParse(tokensToBeValidated[0], out double doubleValue)))
                throw new FormatException($"Starting token rule violated : {tokensToBeValidated[0]} found");
            // Rule 6 Ending token rule violated
            else if(!(tokensToBeValidated[tokensToBeValidated.Count-1].Equals(")") || validateIsVariable(tokensToBeValidated[tokensToBeValidated.Count - 1])
                || Double.TryParse(tokensToBeValidated[tokensToBeValidated.Count - 1],out double doubleCastValue)))
                throw new FormatException($"Ending token rule violated : {tokensToBeValidated[tokensToBeValidated.Count - 1]} found");

            // Rule 2 : Empty formula with no tokens
            if (tokensToBeValidated.Count == 0)
                throw new FormatException(" No token provided ! \n Provide a new non-empty token");

            for (int i=0;i<tokensToBeValidated.Count();i++)
            {
                int startTokenIndex = 0;
                // Error from delegate with given token
                if (!isValid(tokensToBeValidated[i]))
                    throw new FormatException($" isValid delegate found an error with $ {tokensToBeValidated[i]} ");
                if (tokensToBeValidated[i].Equals("("))
                    opening_bracesCount += 1;
                if (tokensToBeValidated[i].Equals(")"))
                    closing_bracesCount += 1;

                // Rule 1 :checks if the parsed token is any of these,if not throws exception
                if (!checkIfAnyValidToken(tokensToBeValidated[i]))
                    throw new FormatException(" Unaccepted token found. \n Provide a valid token");

                // Rule 3 Right Parenthesis rule
                if (closing_bracesCount > opening_bracesCount)
                    throw new FormatException(") parenthesis found more than number of ( brackets ");
                // Rule 4 Balanced Parenthesis rule
                else if (!(closing_bracesCount == opening_bracesCount))
                    throw new FormatException("Unequal number of parenthesis found ");
                // Rule 7 Parenthesi/Operator following rule
                else if (tokensToBeValidated[i].Equals("+") || tokensToBeValidated[i].Equals("-")
                    || tokensToBeValidated[i].Equals("*") || tokensToBeValidated[i].Equals("/")
                    || tokensToBeValidated[i].Equals("("))
                {
                    if (i + 1 > tokensToBeValidated.Count())
                        throw new FormatException(" No further tokens found !");
                    if (!checkTokenForFollowingOperator(tokensToBeValidated, i + 1))
                        throw new FormatException(" Token following " + tokensToBeValidated[i]
                            + " doesn't have valid token");
                }

                // Rule 8 Extra following rule
                else if (i!=tokensToBeValidated.Count()-1
                    &&!checkTokenForExtraRule(tokensToBeValidated, i + 1))
                    throw new FormatException("Invalid token found after current token " + tokensToBeValidated[i]);
            }
            return true;
        }

    private List<string> normalise(Func<string,string>normalize,IEnumerable<string> tokenEnumerator)
        {
           foreach(string token in tokenEnumerator)
            {
                if (token.Equals("+") || token.Equals("-") || token.Equals("*")
                   || token.Equals("/") || token.Equals(""))
                {
                    normalisedTokens.Add(token);
                    continue;
                }
                normalisedTokens.Add(normalize(token));
            }
            return normalisedTokens;
        }

    private Boolean validateIsVariable(string variableToBeChecked)
        {
            return Regex.IsMatch(variableToBeChecked, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

    private Boolean checkTokenForFollowingOperator(List<string> tokensToBeValidated,int followingTokenIndex)
        {
            if (!(tokensToBeValidated[followingTokenIndex].Equals("(")
                || validateIsVariable(tokensToBeValidated[followingTokenIndex])
                || Double.TryParse(tokensToBeValidated[followingTokenIndex], out double castedValue)))
                return false;
            return true;
        }
    private Boolean checkTokenForExtraRule(List<string> tokensToBeValidated, int followingTokenIndex)
        {
            // If current tokens are closing parenthesis or a Number or a variable
            if (tokensToBeValidated[followingTokenIndex - 1].Equals(")")
               || Double.TryParse(tokensToBeValidated[followingTokenIndex-1],out double castValue)
               || validateIsVariable(tokensToBeValidated[followingTokenIndex-1]))
            {
                // checks for following token if its a operator or closing parenthesis
                if (tokensToBeValidated[followingTokenIndex].Equals("+")
                   || tokensToBeValidated[followingTokenIndex].Equals("-")
                   || tokensToBeValidated[followingTokenIndex].Equals("*")
                   || tokensToBeValidated[followingTokenIndex].Equals("/")
                   || tokensToBeValidated[followingTokenIndex].Equals(")"))
                    return true;
                return false;
            }
            return false;
        }

        private Boolean checkIfAnyValidToken(string token)
        {
            if (token.Equals("(") || token.Equals(")")
               || token.Equals("+") || token.Equals("-") || token.Equals("*")
               || token.Equals("/")
               || Double.TryParse(token, out double castedValue)
               || validateIsVariable(token))
                return true;
            return false;       
        }

  }

  /// <summary>
  /// Used to report syntactic errors in the argument to the Formula constructor.
  /// </summary>
  public class FormulaFormatException : Exception
  {
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(String message)
        : base(message)
    {
    }
  }

  /// <summary>
  /// Used as a possible return value of the Formula.Evaluate method.
  /// </summary>
  public struct FormulaError
  {
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(String reason)
        : this()
    {
      Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
  }
}

// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
