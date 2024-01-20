using System.Text.RegularExpressions;

namespace FormulaEvaluator;

public static class Evaluator
{
    public delegate int Lookup(String variable_name);

    public static int Evaluate(String expression,
                           Lookup variableEvaluator)
    {
        Stack<int> valueStack=null;
        Stack<char> operatorStack=null;

        expression = expression.Trim();
        string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

        for(int i=0;i<substrings.Length;i++)
        {
            /// If token is an integer
            if (int.TryParse(substrings[i],out int parsedValue))
            {
                /// if operatorStack has either multiplication or division operator, performs right operation
                /// and pushes result onto valueStack
                
               if((operatorStack.Count!=0) && ((operatorStack.Peek().Equals("/")) || (operatorStack.Peek().Equals("*"))))
                {
                    if(valueStack.Count!=0)
                    {
                        int poppedInt = valueStack.Pop();
                        char poppedOperator = operatorStack.Pop();
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
            else if (!int.TryParse(substrings[i],out int castedValue))
            {
                if(variableEvaluator(substrings[i])==null)
                    throw new ArgumentException(" Invalid variable given ");

            }
        }
        
        return 0;
    }


}

