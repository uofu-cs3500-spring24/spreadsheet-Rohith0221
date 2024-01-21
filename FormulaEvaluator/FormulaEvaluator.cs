using System.Text.RegularExpressions;

namespace FormulaEvaluator;


/// <summary>
/// 
/// </summary>

public static class Evaluator
{
    public delegate int Lookup(String variable_name);


    public static int Evaluate(String expression,
                           Lookup variableEvaluator)
    {
        Stack<int> valueStack = new();
        Stack<string> operatorStack = new();
        expression = expression.Trim();
        string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

        for (int i = 0; i < substrings.Length; i++)
        {
            substrings[i] = substrings[i].Trim();

            if (substrings[i].Equals("-") && (substrings[i+1].Equals("(") ||
                isVariable(substrings[i+1]) || (int.TryParse(substrings[i+1],out int convertedIntValue))))
            {
                throw new Exception(" Bad formula with unary operator found ! ");
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
                                if (parsedValue == 0)
                                    throw new Exception(" Cannot divide by zero! ");
                                valueStack.Push(poppedInt / parsedValue);
                            }
                        }
                    }
                    else
                        valueStack.Push(parsedValue);
                }
                else if (isVariable(substrings[i]))
                {
                    try
                    {
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
                                    if (variableEvaluator(substrings[i]) == 0)
                                        throw new Exception(" Cannot divide by zero! ");
                                    valueStack.Push(poppedInt / variableEvaluator(substrings[i]));
                                }
                            }
                        }
                        else
                            valueStack.Push(variableEvaluator(substrings[i]));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("No value found for given variable !");
                    }

                }
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

                        // Step 2 if "(" is at top of stack, pops it
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
                                        throw new Exception(" Cannot divide by zero ");
                                valueStack.Push(value2 / value1);
                            }
                        }
                    }

                }
            }
            else
                throw new ArgumentException(" Invalid operator " + substrings[i] + " found !");

        }

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
        return 0;

    }



    private static Boolean isVariable(String token)
    {
        // Used chat-GPT to get this Regex
        return Regex.IsMatch(token, "^[a-zA-Z][a-zA-Z0-9]*$");
    }


}

