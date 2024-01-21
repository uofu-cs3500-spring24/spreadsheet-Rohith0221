// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;

Console.WriteLine("Hello, World!");

Console.WriteLine(Evaluator.Evaluate("5+5",null));
Console.WriteLine(Evaluator.Evaluate("2*5", null));
Console.WriteLine(Evaluator.Evaluate("5*2+5", null));
Console.WriteLine(Evaluator.Evaluate("4/2*5", null));

Console.WriteLine(Evaluator.Evaluate("5", null));


try
{
    Evaluator.Evaluate(" -A- ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("Found error");
    // write a message saying that your code detected the invalid syntax of the formula
}




if (Evaluator.Evaluate("5+5",null) == 10)
    Console.WriteLine("Correct");

