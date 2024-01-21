// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FormulaEvaluator;

Console.WriteLine("Hello, World!");

string var = "X1";
string var2 = "A7";
Console.WriteLine(Evaluator.Evaluate("(2+35)*A7", var2 => 8));
Console.WriteLine(Evaluator.Evaluate(" (2 +X1)*5+2", var => 7));



Console.WriteLine(Evaluator.Evaluate("(2 +   X1)*   5 + 2", var => 7));

Console.WriteLine(Evaluator.Evaluate("5+5", null));

Console.WriteLine(Evaluator.Evaluate("5*2+5", null));

Console.WriteLine(Evaluator.Evaluate("4/2*5", null));

Console.WriteLine(Evaluator.Evaluate("5", null));

try
{
    Evaluator.Evaluate(" -A- ",null);
}
catch (Exception)
{
    Console.WriteLine("Found error");
}

Console.WriteLine(Evaluator.Evaluate("-5", null));
try
{
    Console.WriteLine(Evaluator.Evaluate("()/0", null));
}
catch (Exception)
{
    Console.WriteLine("Error 1");
}
try
{
    Console.WriteLine(Evaluator.Evaluate("(0*2)/0", null));
}
catch(Exception)
{
        Console.WriteLine("Error 2");

}

try
{
    Console.WriteLine(Evaluator.Evaluate("2*4/0", null));
}
catch(Exception)
{
    Console.WriteLine("error 3");
}

try
{
    Console.WriteLine(Evaluator.Evaluate("(2*4)+6/0", null));
}

catch(Exception)
{
    Console.WriteLine("error 4");
}


