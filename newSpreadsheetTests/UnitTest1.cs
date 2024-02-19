namespace newSpreadsheetTests;

using System.Text.RegularExpressions;
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
        spreadsheet1.SetContentsOfCell("A1", "10");
    }

    //[TestMethod]
    //public void regexCheck()
    //{
    //    string s = "A1";
    //    Assert.IsTrue(Regex.IsMatch(s, "^[a-zA-Z]+\\d+$"));
    //    //AbstractSpreadsheet spreadsheet = new Spreadsheet();
    //    //spreadsheet.SetContentsOfCell("A1")
    //}
}
