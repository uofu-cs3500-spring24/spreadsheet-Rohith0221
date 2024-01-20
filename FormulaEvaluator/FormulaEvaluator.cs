using System.Text.RegularExpressions;

namespace FormulaEvaluator;

public static class Evaluator
{
    public delegate int Lookup(String variable_name);

    public static int Evaluate(String expression,
                           Lookup variableEvaluator)
    {
        Stack<int> valueStack=null;
        Stack<string> operatorStack=null;

        expression = expression.Trim();
        string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

        for(int i=0;i<substrings.Length;i++)
        {
            /// If token is an integer
            if (int.TryParse(substrings[i], out int parsedValue))
            {
                /// if operatorStack has either multiplication or division operator, performs right operation
                /// and pushes result onto valueStack

                if ((operatorStack.Count != 0) && ((operatorStack.Peek().Equals("/")) || (operatorStack.Peek().Equals("*"))))
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
                valueStack.Push(parsedValue);
            }
            else if (!int.TryParse(substrings[i], out int castedValue))
            {
                try
                {
                    if ((operatorStack.Count != 0) && ((operatorStack.Peek().Equals("/")) || (operatorStack.Peek().Equals("*"))))
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
                    valueStack.Push(parsedValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine("No value found for given variable !");
                }

            }
            else if (valueStack.Count >= 2 && substrings[i].Equals("+") || substrings[i].Equals("-"))
            {
                int value1 = valueStack.Pop();
                int value2 = valueStack.Pop();
                if (operatorStack.Count != 0)
                {
                    string poppedOperator = operatorStack.Pop();
                    if(poppedOperator.Equals("+"))
                        valueStack.Push(value1 + value2);
                    else if (poppedOperator.Equals("-"))
                        valueStack.Push(value2 - value1);
                } 
                else
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
                if(operatorStack.Count!=0 && valueStack.Count >= 2 &&
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
                    if(operatorStack.Peek().Equals("*") ||
                        operatorStack.Peek().Equals("/"))
                    {
                        if(operatorStack.Count != 0 && valueStack.Count >= 2)
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

            if (operatorStack.Count == 0 && valueStack.Count == 1)
                return valueStack.Pop();

            else if(operatorStack.Count==1 && valueStack.Count==2 &&
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

        }
        
        return 0;
    }


}

