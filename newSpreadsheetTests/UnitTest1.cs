namespace newSpreadsheetTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        //AbstractSpreadsheet spreadsheet = new Spreadsheet();
        AbstractSpreadsheet spreadsheet1 = new Spreadsheet(s => true, s => s, "1.0");
        
    }
}
