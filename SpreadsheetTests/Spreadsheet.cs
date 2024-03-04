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
    [TestMethod]
    public void recalculate1()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "2.0");
        s.SetContentsOfCell("A2", "=A1+20.0");
        s.SetContentsOfCell("A3", "=A2-A1+1.0");

        Assert.AreEqual(2.0, s.GetCellValue("A1"));
        Assert.AreEqual(22.0, s.GetCellValue("A2"));
        Assert.AreEqual(21.0, s.GetCellValue("A3"));

        s.SetContentsOfCell("A1", "0.0");

        Assert.AreEqual(0.0, s.GetCellValue("A1"));
        Assert.AreEqual(20.0, s.GetCellValue("A2"));
        Assert.AreEqual(21.0, s.GetCellValue("A3"));

    }

    // Recalculation Tests
    [TestMethod]
    public void recalculate2()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=2/0.0");
        s.SetContentsOfCell("A2", "=A1+20.0");
        s.SetContentsOfCell("A3", "=A2-A1+1.0");

        Assert.IsInstanceOfType(s.GetCellValue("A1"), new FormulaError().GetType());
        Assert.IsInstanceOfType(s.GetCellValue("A2"),new FormulaError().GetType());
        Assert.IsInstanceOfType(s.GetCellValue("A3"), new FormulaError().GetType());

        /// fixing recalculation
        /// 
        s.SetContentsOfCell("A1", "0.0");

        Assert.AreEqual(0.0, s.GetCellValue("A1"));
        Assert.AreEqual(20.0, s.GetCellValue("A2"));
        Assert.AreEqual(21.0, s.GetCellValue("A3"));
    }

    // Exception cases
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void exception_SetContentsOfCell()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1", "1.0");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void exception_Circular_SetContentsOfCell()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1","1.0");
        s.SetContentsOfCell("A2", "=A2");
    }

    [TestMethod]
    public void differenceBetween_FormulasAndString()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "A1");
        s.SetContentsOfCell("A2", "=A1");

        Assert.IsFalse(s.GetCellContents("A1").Equals(s.GetCellContents("A2")));
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SpreadsheetReadWriteException_versionError()
    {
        Spreadsheet s = new("testSave.xml", s => true, s => s.ToUpper(), "default");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SpreadsheetReadWriteException_incorrectFile()
    {
        Spreadsheet s = new("testSaveeee.xml", s => true, s => s.ToUpper(), "1.0");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void constructor4_readingFileError()
    {
        Spreadsheet s = new(".xml", s => true, s => s, "1.0");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void getCellValue_invalidCellName()
    {
        Spreadsheet s = new();
        s.GetCellContents("AA");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void getCellValue_emptyCellName()
    {
        Spreadsheet s = new();
        s.GetCellContents("");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValue_invalidCellNameByDelegate()
    {
        Spreadsheet s = new(s => { if (s.StartsWith("A")) return false; return true; },s=>s,"");
        s.GetCellValue("A1");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void getCellContent_invalidCellNameByDelegate()
    {
        Spreadsheet s = new(s => { if (s.StartsWith("A")) return false; return true; }, s => s, "");
        s.GetCellContents("A1");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void readNonExistentFile()
    {
        Spreadsheet s = new(".xml", s => true, s => s, "1.0");

    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void getSavedVersion_fileWithoutSpreadsheetTag()
    {
        Spreadsheet s = new();
        s.GetSavedVersion("testSave_WithoutSpreadsheetTag.xml");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void getSavedVersion_fileWithoutVersionAttribute()
    {
        Spreadsheet s = new();
        s.GetSavedVersion("testSave1_withoutVersionAtribute.xml");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void nullINputContent()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("a1", null);
    }


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
    public void save_constructor3()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "1.0");
        s.SetContentsOfCell("A1", "20.0");
        s.SetContentsOfCell("A2", "=10.5+A1");
        s.SetContentsOfCell("A3", "=A2-A1+11.2");
        s.Save("testSave1.xml");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void save_constructor4()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "1.0");
        s.SetContentsOfCell("A@1", "20.0");
        s.SetContentsOfCell("A2", "=10.5+A1");
        s.SetContentsOfCell("A3", "=A2-A1+11.2");
        s.Save("testSave1.xml");
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

    [TestMethod]
    public void save_emptySpreadsheet()
    {
        Spreadsheet s = new();
        s.Save("empty.xml");
    }

    [TestMethod]
    public void loadingEmptyCellsFile()
    {
        save_emptySpreadsheet();
        Spreadsheet s = new("empty.xml", s => true, s => s.ToUpper(), "default");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void exception_GetSavedFile()
    {
        Spreadsheet s = new();
        s.GetSavedVersion("1.xml");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void exception_Save()
    {
        Spreadsheet s = new();
        s.Save("");
    }

    [TestMethod]
    public void testConstructor4()
    {
        Spreadsheet s = new("testSave.xml", s => true, s => s.ToUpper(), "1.0");
        foreach(string cell in s.GetNamesOfAllNonemptyCells())
        {
            Console.WriteLine(s.GetCellContents(cell) + " "+s.GetCellValue(cell));
        }
    }

    [TestMethod]
    public void testConstructor4_doubleValues()
    {
        save_constructor3();
        Spreadsheet s = new("testSave1.xml", s => true, s => s.ToUpper(), "1.0");
        Assert.AreEqual(3, s.GetNamesOfAllNonemptyCells().Count());

        Assert.AreEqual(20.0, s.GetCellContents("A1"));
        Assert.AreEqual(new Formula("=10.5+A1"), s.GetCellContents("A2"));
        Assert.AreEqual(new Formula("=A2-A1+11.2"), s.GetCellContents("A3"));

        Assert.AreEqual(20.0, s.GetCellValue("A1"));
        Assert.AreEqual(30.5, s.GetCellValue("A2"));
        Assert.AreEqual(21.7, s.GetCellValue("A3"));
    }


    [TestMethod]
    public void normaliseCheck()
    {
        Spreadsheet s = new(s => true, s => s.ToUpper(), "");
        s.SetContentsOfCell("a1", "20.0");
        Assert.AreEqual(1, s.GetNamesOfAllNonemptyCells().Count());
        IEnumerator<string> enumerator = s.GetNamesOfAllNonemptyCells().GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual("A1", enumerator.Current.ToString());
    }

    [TestMethod]
    public void getXML_stringContent()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "20.0");
        s.SetContentsOfCell("A2", "A2");

        Console.WriteLine(s.GetXML());
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void loadingFromFile_circularDependency()
    {
        Spreadsheet s = new("testSave1_circular.xml", s => true, s => s, "1.0");

    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void loadingFromFile_incorrectCellName()
    {
        Spreadsheet s = new("testSave_incorrectCellName.xml", s => true, s => s, "1.0");

    }
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void loadingFromFile_incorrectFormula()
    {
        Spreadsheet s = new("testSave_incorrectFormula.xml", s => true, s => s, "1.0");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void enteringIncorrectFormula()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=A2$1.0");
    }

    
    [TestMethod]
    public void stressTest1_Doubles()
    {
        Spreadsheet s = new();
        for(int i=0;i<100;i++)
        {
            s.SetContentsOfCell("A" + i,(i+1*2).ToString());
        }

        for(int i=0;i<100;i++)
        {
            Assert.AreEqual((Double)i + 1 * 2, s.GetCellContents("A" + i));
            Assert.AreEqual((Double)i + 1 * 2, s.GetCellValue("A" + i));
        }
    }

    [TestMethod]
    public void stressTest1_Strings()
    {
        Spreadsheet s = new();
        for (int i = 0; i < 100; i++)
        {
            s.SetContentsOfCell("A" + i, "A"+i + 1 * 2);
        }

        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual("A" + i + 1 * 2, s.GetCellContents("A" + i));
            Assert.AreEqual("A" + i + 1 * 2, s.GetCellValue("A" + i));
        }
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void testFormualError()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=$20");
    }
}
