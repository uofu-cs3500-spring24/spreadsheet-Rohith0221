namespace SpreadsheetTests;
using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

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
        spreadsheet.SetCellContents("A1", 20.0);
        spreadsheet.SetCellContents("A2", new Formula("A1+10"));
        spreadsheet.SetCellContents("A3", new Formula("A4*A2"));
        spreadsheet.SetCellContents("A5", new Formula("A1-A5"));
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void circularExceptionThrown2()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("A1", new Formula("A1"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_DoubleContent_NullName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents(null, 20.0);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_DoubleContent_InvalidName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("A@", 20.0);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_StringContent_NullName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents(null,"2.0");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_StringContent_InvalidName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("A@","2.0");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void setCellContent_Exception_StringContent_NullString()
    {
        Spreadsheet spreadsheet = new();
        String s = null;
        spreadsheet.SetCellContents("A1",s);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void setCellContent_Exception_FormulaContent_NullFormula()
    {
        Spreadsheet spreadsheet = new();
        Formula f = null;
        spreadsheet.SetCellContents("X1", f);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_FormulaContent_NullName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents(null, new Formula("2.0"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void setCellContent_Exception_FormulaContent_InvalidName()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("A@", new Formula("2.0"));
    }

    [TestMethod]
    public void test_CellConstructor1()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X", 2.0);
    }

    [TestMethod]
    public void testConstructor2()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X", "Value_X");
    }

    [TestMethod]
    public void testConstructor3()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X",new Formula("20+X3"));
    }

    [TestMethod]
    public void getCellContents()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X1", 20.0);

        Assert.AreEqual(20.0, spreadsheet.GetCellContents("X1"));
    }

    [TestMethod]
    public void testingByCreating_AbstractSpreadSheetObject()
    {
        AbstractSpreadsheet abstractSpreadsheet = new Spreadsheet() ;
        abstractSpreadsheet.SetCellContents("A12", 2);
        Assert.AreEqual(2.0,abstractSpreadsheet.GetCellContents("A12"));
    }

    [TestMethod]
    public void overridingCellContent_Double()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X1", 30);
        Assert.AreEqual(30.0, spreadsheet.GetCellContents("X1"));
        spreadsheet.SetCellContents("X1", 20);
        Assert.AreEqual(20.0, spreadsheet.GetCellContents("X1"));

        Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void overridingCellContent_String()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X1", "Original");
        Assert.AreEqual("Original", spreadsheet.GetCellContents("X1"));
        spreadsheet.SetCellContents("X1", "Overriden value");
        Assert.AreEqual("Overriden value", spreadsheet.GetCellContents("X1"));

        Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void overridingCellContent_Formula()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("X1",new Formula("X2"));
        Assert.AreEqual(new Formula("X2"), spreadsheet.GetCellContents("X1"));
        spreadsheet.SetCellContents("X1", new Formula("x2"));
        Assert.AreEqual(new Formula("x2"), spreadsheet.GetCellContents("X1"));
        
        Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void getNonEmptyCellNames()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetCellContents("A1", 20);
        spreadsheet.SetCellContents("A2", "A2+20");
        spreadsheet.SetCellContents("A3", "A2-A1+8");
        spreadsheet.SetCellContents("A4", "");

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
    public void dependents_SetCellContents_Double()
    {
        Spreadsheet spreadsheet = new();
        List<string> list1 = spreadsheet.SetCellContents("A1",20.0).ToList();

        List<string> list2 = spreadsheet.SetCellContents("A2", new Formula("A1+10")).ToList();

        List<string> list3 = spreadsheet.SetCellContents("A3", new Formula("2*A2+9-A1")).ToList();

        List<string> expectedList1 = new() { "A1" };
        List<string> expectedList2 = new() { "A1","A2" };
        List<string> expectedList3 = new() { "A2","A1","A3" };

        Assert.AreEqual(expectedList1.Count(), list1.Count());
        for (int i = 0; i < expectedList1.Count(); i++)
            Assert.AreEqual(list1[i], expectedList1[i]);

        Assert.AreEqual(expectedList2.Count(), list2.Count());
        for (int i = 0; i < expectedList2.Count(); i++)
            Assert.AreEqual(list2[i], expectedList2[i]);

        Assert.AreEqual(expectedList3.Count(), list3.Count());
        for (int i = 0; i < expectedList3.Count(); i++)
            Assert.AreEqual(list3[i], expectedList3[i]);
    }
}
