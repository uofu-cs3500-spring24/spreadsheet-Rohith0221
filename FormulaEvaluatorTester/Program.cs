// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FormulaEvaluator;

/// Author : Rohith Veeramachaneni
///     Date   : Jan 21, 2024
///     Partner: None
///
/// This console app is a tester file with all the tests that test out the functionality of FormulaEvaluator also considering
/// edge cases



Console.WriteLine(Evaluator.Evaluate("(2+35)*A7", var2 => 8));
Console.WriteLine(Evaluator.Evaluate(" (2 +X1)*5+2", var => 7));

try
{
    Console.WriteLine(Evaluator.Evaluate("+5", null));
}
catch (ArgumentException)
{
    Console.WriteLine(" Unary + found ");
}

Console.WriteLine(Evaluator.Evaluate("(2 +   X1)*   5 + 2", var => 7));

Console.WriteLine(Evaluator.Evaluate("5+5", null));

Console.WriteLine(Evaluator.Evaluate("5*2+5", null));

Console.WriteLine(Evaluator.Evaluate("4/2*5", null));

Console.WriteLine(Evaluator.Evaluate("5", null));


Console.WriteLine(Evaluator.Evaluate("0/2", null));


try
{
    Evaluator.Evaluate(" -A- ", null);
}
catch (ArgumentException)
{
    Console.WriteLine("Found error");
}

try
{
    Console.WriteLine(Evaluator.Evaluate("-5", null));

}
catch (ArgumentException)
{
    Console.WriteLine("Unary operator exception thrown ");
}
try
{
    Console.WriteLine(Evaluator.Evaluate("()/0", null));
}
catch (ArgumentException)
{
    Console.WriteLine("Error 1");
}
try
{
    Console.WriteLine(Evaluator.Evaluate("(0*2)/0", null));
}
catch (ArgumentException)
{
    Console.WriteLine("Error 2");

}

try
{
    Console.WriteLine(Evaluator.Evaluate("2*4/0", null));
}
catch (ArgumentException)
{
    Console.WriteLine("error 3");
}

try
{
    Console.WriteLine(Evaluator.Evaluate("(2*4)+/0", null));
}

catch (ArgumentException)
{
    Console.WriteLine("error 4");
}


Console.WriteLine(Evaluator.Evaluate("5", s => 0));
    Console.WriteLine();

    Console.WriteLine(Evaluator.Evaluate("X5", s => 13));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("5+3", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("18-10", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2*4", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("16/2", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2+X1", s => 4));
Console.WriteLine();

//try
//{
//    Console.WriteLine(Evaluator.Evaluate("2+X1", s => { throw new ArgumentException("Unknown variable"); }));
//}
//catch(ArgumentException)
//{
//    throw new ArgumentException("Error ");
//}
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2*6+3", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2+6*3", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("(2+6)*3", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2*(3+5)", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2+(3+5)", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2+(3+5*9)", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("(1*1)-2/2", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2+3*(3+5)", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2+3*5+(3+4*8)*5+2", s => 0));
Console.WriteLine();

try
{
    Console.WriteLine(Evaluator.Evaluate("5/0", s => 0));
}
catch(ArgumentException)
{
    Console.WriteLine("Error");
}
Console.WriteLine();

try
{
    Console.WriteLine(Evaluator.Evaluate("+", s => 0));
}
catch(ArgumentException)
{
    Console.WriteLine(" Error 1");
}
Console.WriteLine();

try
{
    Console.WriteLine(Evaluator.Evaluate("2+5+", s => 0));
}
catch (ArgumentException)
{
    Console.WriteLine(" Error 2");
}
Console.WriteLine();

try
{
    Console.WriteLine(Evaluator.Evaluate("2+5*7)", s => 0));
}
catch(ArgumentException)
{
    Console.WriteLine("Error 3");
}
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("xx", s => 0));

Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("5+xx", s => 0));

Console.WriteLine();

//Console.WriteLine(Evaluator.Evaluate("5+7+(5)8", s => 0));
//Console.WriteLine();

try
{
    Console.WriteLine(Evaluator.Evaluate("", s => 0));
}
catch (ArgumentException)
{
    Console.WriteLine(" Error 6");
}

Console.WriteLine();


Console.WriteLine(Evaluator.Evaluate("y1*3-8/2+4*(8-9*2)/14*x7", s => (s == "x7") ? 1 : 4));
Console.WriteLine();


Console.WriteLine(Evaluator.Evaluate("x1+(x2+(x3+(x4+(x5+x6))))", s => 1));
Console.WriteLine();


Console.WriteLine(Evaluator.Evaluate("((((x1+x2)+x3)+x4)+x5)+x6", s => 2));
Console.WriteLine();


Console.WriteLine(Evaluator.Evaluate("a4-a4*a4/a4", s => 3));
Console.WriteLine();


//Test if code doesn't clear stacks between evaluations
Console.WriteLine(Evaluator.Evaluate("2*6+3", s => 0));
Console.WriteLine();

Console.WriteLine(Evaluator.Evaluate("2*6+3", s => 0));



