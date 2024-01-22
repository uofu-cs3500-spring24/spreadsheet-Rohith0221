using System.Text.RegularExpressions;

namespace FormulaEvaluator;


/// <summary>
///
///     Author : Rohith Veeramachaneni
///     Date   : Jan 21, 2024
///     Partner: None

/// 
/// Evaluator class consists few methods that evaluates a given math expression utilising some helper methods
/// by taking in a string expression as well as takes in a delegate variable to lookup the value of the variable found while
/// parsing tokens from given string.
/// 
/// </summary>

public static class Evaluator
{
    /// <summary>
    /// 
    ///  This delegate is used to find the value of the variable found while parsing token from
    ///  a string expression
    ///  
    /// </summary>
    /// 
    /// <param name="variable_name"></param> Name of the variable found as a token
    /// <returns></returns> v=Integer value associated with that variable
    public delegate int Lookup(String variable_name);


    /// <summary>
    ///
    ///  Evaluate method calculates final value from a math expression given as a String
    ///  
    /// </summary>
    /// 
    /// <param name="expression"></param> Math expression given as type string
    /// <param name="variableEvaluator"></param> Delegate variable to find value of a variable token
    /// <returns></returns> final result in integer type
    /// <exception cref="ArgumentException"></exception> When errors occur while parsing or calculating value of given
    ///                                                  expression
    ///                                                  
    public static int Evaluate(String expression,
                           Lookup variableEvaluator)
    {
        Stack<int> valueStack = new();
        Stack<string> operatorStack = new();
        // trims down leading and trailing whitespaces in given expression
        expression = expression.Trim();
        string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

        for (int i = 0; i < substrings.Length; i++)
        {
            // trims down whitespaced again in the parsed token
            substrings[i] = substrings[i].Trim();

            // If theres a unary operator,throws an exception as it's considered bad formula
            if ((substrings[i].Equals("-") || (substrings[i].Equals("+"))) && (substrings[i + 1].Equals("(") ||
                isVariable(substrings[i + 1]) || (int.TryParse(substrings[i + 1], out int convertedIntValue))))
            {
                throw new ArgumentException(" Bad formula with unary operator found ! ");
            }
            if (substrings[i].Equals("") || substrings[i].Equals("+")
                || substrings[i].Equals("-") || substrings[i].Equals("/")
                || substrings[i].Equals(")") || substrings[i].Equals("/")
                || substrings[i].Equals("(") || substrings[i].Equals("*")
                || isVariable(substrings[i])
                || (int.TryParse(substrings[i], out int checkConvertedInt) && checkConvertedInt >= 0))
            {

                if (substrings[i].Equals(""))
                    continue;

                /// If token is an integer
                if (int.TryParse(substrings[i], out int parsedValue))
                {
                    /// if operatorStack has either multiplication or division operator, performs right operation
                    /// and pushes result onto valueStack

                    if (operatorStack != null && (operatorStack.Count != 0) && ((operatorStack.Peek().Equals("/"))
                         || (operatorStack.Peek().Equals("*"))))
                    {
                        if (valueStack.Count != 0)
                        {
                            int poppedInt = valueStack.Pop();
                            string poppedOperator = operatorStack.Pop();
                            if (poppedOperator.Equals("*"))
                                valueStack.Push(poppedInt * parsedValue);
                            else if (poppedOperator.Equals("/"))
                            {
                                try
                                {
                                    valueStack.Push(poppedInt / parsedValue);
                                }
                                catch (Exception)
                                {
                                    throw new ArgumentException(" Cannot divide by zero! ");
                                }

                            }
                        }
                    }
                    else
                        valueStack.Push(parsedValue);
                }
                // If token is a variable , delegate is used to find value of that
                else if (isVariable(substrings[i]))
                {
                    try
                    {
                        // If operatorStack has '/' or '*'
                        if ((operatorStack != null && operatorStack.Count != 0)
                           && ((operatorStack.Peek().Equals("/"))
                           || (operatorStack.Peek().Equals("*"))))
                        {
                            if (valueStack.Count != 0)
                            {
                                int poppedInt = valueStack.Pop();
                                string poppedOperator = operatorStack.Pop();
                                if (poppedOperator.Equals("*"))
                                    valueStack.Push(poppedInt * variableEvaluator(substrings[i]));
                                else if (poppedOperator.Equals("/"))
                                {
                                    try
                                    {
                                        valueStack.Push(poppedInt / variableEvaluator(substrings[i]));
                                    }
                                    catch (Exception)
                                    {
                                        throw new ArgumentException(" Cannot divide by zero! ");
                                    }
                                }
                            }
                            else
                                valueStack.Push(variableEvaluator(substrings[i]));
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("No value found for given variable !");
                    }

                }

                // checks if parsed token is either '+' or '-' operator and if so pushes operator onto the operator stack
                else if (substrings[i].Equals("+") || substrings[i].Equals("-"))
                {
                    if (valueStack.Count >= 2)
                    {
                        int value1 = valueStack.Pop();
                        int value2 = valueStack.Pop();
                        if (operatorStack.Count != 0 && operatorStack.Peek().Equals("+")
                            || operatorStack.Peek().Equals("-"))
                        {
                            string poppedOperator = operatorStack.Pop();
                            if (poppedOperator.Equals("+"))
                                valueStack.Push(value1 + value2);
                            else if (poppedOperator.Equals("-"))
                                valueStack.Push(value2 - value1);
                        }
                    }
                    operatorStack.Push(substrings[i]);
                }

                // checks if parsed token is either '*' or '/' operator and if so pushes operator onto the operator stack
                else if (substrings[i].Equals("*") || substrings[i].Equals("/"))
                    operatorStack.Push(substrings[i]);

                else if (substrings[i].Equals("("))
                    operatorStack.Push(substrings[i]);

                // If top of the opeatorStack is ")" 
                else if (substrings[i].Equals(")"))
                {
                    // performs the operation by checking for the errors

                    /// If top of the operatorStack is either "+" or "-"
                    /// then applies operator to the popped values
                    if (operatorStack.Count != 0 && valueStack.Count >= 2 &&
                        operatorStack.Peek().Equals("+") ||
                        operatorStack.Peek().Equals("-"))
                    {
                        int value1 = valueStack.Pop();
                        int value2 = valueStack.Pop();
                        string poppedOperator = operatorStack.Pop();
                        if (poppedOperator.Equals("+"))
                            valueStack.Push(value1 + value2);
                        else if (poppedOperator.Equals("-"))
                            valueStack.Push(value2 - value1);

                        // Step 2 if "(" is at top of operator stack, pops it
                        if (operatorStack.Peek().Equals("("))
                            operatorStack.Pop();

                        /// Step 3
                        /// If top of the operatorStack is either  "*" or "/"
                        /// applies the poppedOperator to the values
                        if (operatorStack != null && operatorStack.Count != 0
                            && (operatorStack.Peek().Equals("*") ||
                            operatorStack.Peek().Equals("/")))
                        {
                            if (valueStack.Count >= 2)
                            {
                                value1 = valueStack.Pop();
                                value2 = valueStack.Pop();

                                poppedOperator = operatorStack.Pop();

                                if (poppedOperator.Equals("*"))
                                    valueStack.Push(value1 * value2);
                                else if (poppedOperator.Equals("/"))
                                    if (value1 == 0)
                                        throw new ArgumentException(" Cannot divide by zero ");
                                valueStack.Push(value2 / value1);
                            }
                        }
                    }

                }
            }
            else
                throw new ArgumentException(" Invalid operator " + substrings[i] + " found !");
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
            int value1 = valueStack.Pop();
            int value2 = valueStack.Pop();

            if (poppedOperator.Equals("+"))
                return value1 + value2;
            else if (poppedOperator.Equals("-"))
                return value2 - value1;
        }
        return 0; // placeholder return value

    }


    /// <summary>
    ///  isVariable method is a helper method that checks if a token passed as an argument is a variable
    ///  
    /// <param name="token"></param> Certain token parsed from a string expression
    /// 
    /// <returns></returns> true if given token is a variable otherwise false
    private static Boolean isVariable(String token)
    {
        return Regex.IsMatch(token, "^[a-zA-Z][a-zA-Z0-9]*$");
    }


}

