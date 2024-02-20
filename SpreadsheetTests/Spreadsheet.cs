namespace SpreadsheetTests;
using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

/// <summary>
///
///Author      : Rohith Veeramachaneni
/// Partner     : None
/// Date Created: Feb 10,2023

///  Tester class to test fucntionality of the Spreadsheet class by various edge case tests
///  including testing for null inputs
///  
/// </summary>
[TestClass]
public class UnitTest1
{
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void getCellContents_Exception_NameIsNull()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.GetCellContents(null);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void getCellContents_Exception_NameIsInvalid()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.GetCellContents(" ");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void circularExceptionThrown()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A1", "20.0");
        spreadsheet.SetContentsOfCell("A2", "=A1+10");
        spreadsheet.SetContentsOfCell("A3", "=A4*A2");
        spreadsheet.SetContentsOfCell("A5", "=A1-A5");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void circularExceptionThrown2()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A1", "=A1");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_DoubleContent_InvalidName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A@", "20.0");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_StringContent_InvalidName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A@","2.0");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_FormulaContent_InvalidName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A@","=2.0");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void testConstructor3()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("X","=20+X3");
    }

    [TestMethod]
    public void getCellContents()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("X1", "20.0");

        Assert.AreEqual(20.0, spreadsheet.GetCellContents("X1"));
    }

    [TestMethod]
    public void testingByCreating_AbstractSpreadSheetObject()
    {
        AbstractSpreadsheet abstractSpreadsheet = new Spreadsheet() ;
        abstractSpreadsheet.SetContentsOfCell("A12", "2");
        Assert.AreEqual(2.0,abstractSpreadsheet.GetCellContents("A12"));
    }

    [TestMethod]
    public void overridingCellContent_Double()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("X1", "30");
        Assert.AreEqual(30.0, spreadsheet.GetCellContents("X1"));
        spreadsheet.SetContentsOfCell("X1", "20");
        Assert.AreEqual(20.0, spreadsheet.GetCellContents("X1"));

        Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void overridingCellContent_String()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("X1", "Original");
        Assert.AreEqual("Original", spreadsheet.GetCellContents("X1"));
        spreadsheet.SetContentsOfCell("X1", "Overriden value");
        Assert.AreEqual("Overriden value", spreadsheet.GetCellContents("X1"));

        Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void overridingCellContent_Formula()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("X1","=X2");
        Assert.AreEqual("=X2", spreadsheet.GetCellContents("X1").ToString());
        spreadsheet.SetContentsOfCell("X1", "=x2");
        Assert.AreEqual("=x2", spreadsheet.GetCellContents("X1").ToString());
        
        Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void getNonEmptyCellNames()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A1", "20");
        spreadsheet.SetContentsOfCell("A2", "A2+20");
        spreadsheet.SetContentsOfCell("A3", "A2-A1+8");
        spreadsheet.SetContentsOfCell("A4", "");

        List<string> output = spreadsheet.GetNamesOfAllNonemptyCells().ToList();
        Assert.AreEqual(3, spreadsheet.GetNamesOfAllNonemptyCells().Count());

        List<string> list = new();
        list.Add("A1");
        list.Add("A2");
        list.Add("A3");

        for(int i=0;i<output.Count();i++)
        {

            Assert.AreEqual(list[i].ToString(), output[i].ToString());
        }
    }


    [TestMethod]
    public void getCellContentsForEmptyCell()
    {
        Spreadsheet spreadsheet = new();
        Assert.AreEqual("", spreadsheet.GetCellContents("A1"));
    }


    //[TestMethod()]
    //public void hugeData_StressTest()
    //{
    //    Spreadsheet s = new Spreadsheet();
    //    s.SetContentsOfCell("A1","=B1+B2");
    //    s.SetContentsOfCell("B1","=C1-C2");
    //    s.SetContentsOfCell("B2", "=C3*C4");
    //    s.SetContentsOfCell("C1", "=D1*D2");
    //    s.SetContentsOfCell("C2", "=D3*D4");
    //    s.SetContentsOfCell("C3", "=D5*D6");
    //    s.SetContentsOfCell("C4", "=D7*D8");
    //    s.SetContentsOfCell("D1", "=E10");
    //    s.SetContentsOfCell("D2", "=E10");
    //    s.SetContentsOfCell("D3", "=E10");
    //    s.SetContentsOfCell("D4", "=E10");
    //    s.SetContentsOfCell("D5", "=E10");
    //    s.SetContentsOfCell("D6", "=E10");
    //    s.SetContentsOfCell("D7", "=E10");
    //    s.SetContentsOfCell("D8", "=E10");
    //    IList<String> cells = s.SetContentsOfCell("E10", "0");
    //    Assert.IsTrue(new List<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E10" }.Equals(cells));
    //}

    [TestMethod]
    public void testDoubleValue()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "20.0");
        Assert.AreEqual(20.0, s.GetCellContents("A1"));
        Assert.AreEqual(20.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    public void testStringValue()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "20 is a string");
        Assert.AreEqual("20 is a string", s.GetCellContents("A1"));
        Assert.AreEqual("20 is a string", s.GetCellValue("A1"));
    }

    [TestMethod]
    public void testFormula_DoubleValue()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=20.0");
        Assert.AreEqual(new Formula("=20.0"), s.GetCellContents("A1"));
        Assert.AreEqual(20.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void testFormula_stringValue()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=string");
        Assert.AreEqual(new FormulaError(), s.GetCellContents("A1"));
        Assert.AreEqual("string", s.GetCellValue("A1"));
    }

    [TestMethod]
    public void testFormula_valueWithDependencyBetweenCells()
    {
        Spreadsheet s = new(s=>true,s=>s.ToUpper(),"1.0");
        s.SetContentsOfCell("A1", "20.0");
        s.SetContentsOfCell("A2", "=A1+30.0");
        Assert.AreEqual(new Formula("=A1+30.0"), s.GetCellContents("A2"));
        Assert.AreEqual(50.0, s.GetCellValue("A2"));
    }

    [TestMethod]
    public void testFormula_valueWithDependencyBetweenCells_whereDependentCellISUndefined()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "1.0");
        s.SetContentsOfCell("A2", "=A1");
        Assert.AreEqual(new Formula("=A1"), s.GetCellContents("A2"));
        Assert.IsInstanceOfType(s.GetCellValue("A2"),new FormulaError().GetType());
    }

    [TestMethod]
    public void testFormula_divideByZeroException()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "1.0");
        s.SetContentsOfCell("A2", "0.0");
        s.SetContentsOfCell("A1", "=5/A2");
        Assert.AreEqual(new Formula("=5/A2"), s.GetCellContents("A1"));
        Assert.IsInstanceOfType(s.GetCellValue("A1"), new FormulaError().GetType());
    }

    [TestMethod]
    public void multipleDependencies()
    {
        Spreadsheet s = new(s=>true,s=>s.ToUpper(),"10.0");
        s.SetContentsOfCell("A1", "20.0");
        s.SetContentsOfCell("A2", "=A1+10.0");
        s.SetContentsOfCell("A3", "=A2*A1");

        Assert.AreEqual(20.0, s.GetCellContents("A1"));
        Assert.IsTrue(new Formula("=A1+10.0").Equals(s.GetCellContents("A2")));
        Assert.IsTrue(new Formula("=A2*A1").Equals(s.GetCellContents("A3")));

        Assert.AreEqual(20.0, s.GetCellValue("A1"));
        Assert.AreEqual(30.0, s.GetCellValue("A2"));
        Assert.AreEqual(600.0, s.GetCellValue("A3"));
    }

    [TestMethod]
    public void getChanged_creatingNewCell_EmptyString()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "1.0");
        s.SetContentsOfCell("A1", "");
        Assert.IsFalse(s.Changed);
    }

    [TestMethod]
    public void getChanged_creatingNewCell_validString()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "1.0");
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void getChanged_creatingNewCell_validDouble()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "1.0");
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void getChanged_creatingNewCell_validFormula_doubleValue()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=1.0");
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void getChanged_creatingNewCell_validFormula_dependencyWithoutException()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "1.0");
        s.SetContentsOfCell("A2", "1.0+A1");
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void getChanged_creatingNewCell_validFormula_dependencyWithException()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "1.0");
        s.SetContentsOfCell("A2", "=1.0+A2");
        Assert.IsFalse(s.Changed);
    }

    // Recalculation Tests


    // Exception cases



    // Save methods
    [TestMethod]
    public void save_constructor2()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "1.0");
        s.SetContentsOfCell("A1", "20.0");
        s.SetContentsOfCell("A2", "=10.0+A1");
        s.SetContentsOfCell("A3", "=A2-A1+10.0");
        s.Save("testSave.xml");
    }

    [TestMethod]
    public void getXML()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "20.0");
        s.SetContentsOfCell("A2", "=A1-1.0");
        Console.WriteLine(s.GetXML());
    }

    [TestMethod]
    public void getSavedVersion()
    {
        Spreadsheet s = new();
        Assert.AreEqual("1.0", s.GetSavedVersion("testSave.xml"));
    }
}
