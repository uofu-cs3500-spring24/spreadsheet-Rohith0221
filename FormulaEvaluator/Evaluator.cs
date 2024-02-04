using System;
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
        if (expression == null)
            throw new ArgumentException(" No string expression found ! ");
        Stack<int> valueStack = new();
        Stack<string> operatorStack = new();
        // trims down leading and trailing whitespaces in given expression
        expression = expression.Trim();
        string[] normalisedTokens =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

        for (int i = 0; i < normalisedTokens.Length; i++)
        {
            // trims down whitespaced again in the parsed token
            normalisedTokens[i] = normalisedTokens[i].Trim();

            // If theres a unary operator,throws an exception as it's considered bad formula

            // If after splitting into tokens, first token is an empty space then looks one spot additional to check for
            // improper unary operator 
            if (i == 0 && (!((i + 1) >= normalisedTokens.Length)) && ((normalisedTokens[i].Equals("")) && (normalisedTokens[i + 1].Equals("-")
                || (normalisedTokens[i + 1].Equals("+"))) && (normalisedTokens[i + 2].Equals("(")
                || isVariable(normalisedTokens[i + 2]) || (int.TryParse(normalisedTokens[i + 2], out int convertedIntValue))
                && convertedIntValue >= 0)))
            {
                throw new ArgumentException(" Bad formula with unary operator found ! ");
            }

            else if (i == 0 && (!((i + 1) >= normalisedTokens.Length)) && (normalisedTokens[i].Equals("-")
                    || (normalisedTokens[i].Equals("+"))) && (normalisedTokens[i + 1].Equals("(") ||
                    isVariable(normalisedTokens[i + 1]) || int.TryParse(normalisedTokens[i + 1], out int convertedValue)
                    && convertedValue >= 0))
            {
                throw new ArgumentException(" Bad formula with unary operator found ! ");
            }

            if (normalisedTokens[i].Equals(""))
                continue;


            if ( normalisedTokens[i].Equals("+") || normalisedTokens[i].Equals("-") || normalisedTokens[i].Equals("/")
                || normalisedTokens[i].Equals(")") || normalisedTokens[i].Equals("/")
                || normalisedTokens[i].Equals("(") || normalisedTokens[i].Equals("*")
                || isVariable(normalisedTokens[i])
                || (int.TryParse(normalisedTokens[i], out int checkConvertedInt) && checkConvertedInt >= 0))
            {


                /// If token is an integer
                if (int.TryParse(normalisedTokens[i], out int parsedValue))
                {
                    /// if operatorStack has either multiplication or division operator, performs right operation
                    /// and pushes result onto valueStack

                    if (operatorStack != null && (operatorStack.Count != 0) && ((operatorStack.Peek().Equals("/"))
                         || (operatorStack.Peek().Equals("*"))))
                    {
                        if (valueStack != null && valueStack.Count != 0)
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
                else if (isVariable(normalisedTokens[i]))
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
                                int poppedInt = valueStack.Pop();
                                string poppedOperator = operatorStack.Pop();
                                if (poppedOperator.Equals("*"))
                                    valueStack.Push(poppedInt * variableEvaluator(normalisedTokens[i]));
                                else if (poppedOperator.Equals("/"))
                                {
                                    try
                                    {
                                        valueStack.Push(poppedInt / variableEvaluator(normalisedTokens[i]));
                                    }
                                    catch (Exception)
                                    {
                                        throw new ArgumentException(" Cannot divide by zero! ");
                                    }
                                }
                            }
                        }
                        else
                            valueStack.Push(variableEvaluator(normalisedTokens[i]));
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("No value found for given variable !");
                    }

                }

                // checks if parsed token is either '+' or '-' operator and if so pushes operator onto the operator stack
                else if (normalisedTokens[i].Equals("+") || normalisedTokens[i].Equals("-"))
                {
                    if (operatorStack != null && operatorStack.Count != 0)

                    {
                        if (operatorStack.Peek().Equals("+") || operatorStack.Peek().Equals("-"))
                        {
                            if (valueStack.Count >= 2)
                            {
                                int value1 = valueStack.Pop();
                                int value2 = valueStack.Pop();
                                string poppedOperator = operatorStack.Pop();
                                if (poppedOperator.Equals("+"))
                                {
                                    valueStack.Push(value1 + value2);
                                    operatorStack.Push(normalisedTokens[i]);
                                }
                                else if (poppedOperator.Equals("-"))
                                {
                                    valueStack.Push(value2 - value1);
                                    operatorStack.Push(normalisedTokens[i]);
                                }
                            }
                            else
                                operatorStack.Push(normalisedTokens[i]);
                        }
                        else
                            operatorStack.Push(normalisedTokens[i]);
                    }
                    else
                        operatorStack.Push(normalisedTokens[i]);
                }

                // checks if parsed token is either '*' or '/' operator and if so pushes operator onto the operator stack
                else if (normalisedTokens[i].Equals("*") || normalisedTokens[i].Equals("/"))
                    operatorStack.Push(normalisedTokens[i]);

                else if (normalisedTokens[i].Equals("("))
                    operatorStack.Push(normalisedTokens[i]);

                // If top of the opeatorStack is ")" 
                else if (normalisedTokens[i].Equals(")"))
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
                            int value1 = valueStack.Pop();
                            int value2 = valueStack.Pop();

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
            }
            else
                throw new ArgumentException(" Invalid operator " + normalisedTokens[i] + " found !");
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
        throw new ArgumentException(" Errors found ");

        
    }


    /// <summary>
    ///  isVariable method is a helper method that checks if a token passed as an argument is a variable
    ///  
    /// <param name="token"></param> Certain token parsed from a string expression
    /// 
    /// <returns></returns> true if given token is a variable otherwise false
    static Boolean isVariable(String token)
    {
        return Regex.IsMatch(token, "^[a-zA-Z][a-zA-Z0-9]*[0-9]$");
    }

   

}

