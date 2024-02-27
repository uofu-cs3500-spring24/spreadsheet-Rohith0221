using System;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SS
{

    /// <summary>
    /// Author      : Rohith Veeramachaneni
    /// Partner     : None
    /// Date Created: Feb 9,2023
    /// Date Modified : Feb 19,2023 (Assignment 5)
    ///
    ///
    ///  Spreadsheet class is a concrete implementation of the Abstract class
    ///  AbstractSpreadsheet alongwith some helper methods that help facilitate the creation
    ///  of a spreadsheet that can store the relation between two cells and can also
    ///  let the user know number of nonEmpty cells in this spreadsheet
    ///
    ///  Spreadsheet class now implements three differenet constructors allowing the user to
    ///  create a spreadsheet based on an existing xml file and also create a new Spreadsheet
    ///  by giving a normalise and validation delegate to be passed in to allow input
    ///  restrictions by the user
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Dependency Graph to store connections between cells
        private DependencyGraph cellDependency;
        private Dictionary<string, Cell> nonEmptyCells;
        private bool changed; // keeps track of changed status of spreadsheet


        /// <summary>
        ///  Default constructor
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {

            cellDependency = new();
            this.nonEmptyCells = new();
            Changed = false;
        }

        /// <summary>
        ///  Constructor to build a spreadsheet with normaliser as well as validator
        /// </summary>
        /// <param name="isValid"></param> Delegate to have additional restrictions on inputs
        /// <param name="normalize"></param> Normaliser to normalise before passing input
        /// <param name="version"></param> Version of the current spreadsheet
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cellDependency = new();
            nonEmptyCells = new();
            Changed = false;
        }

        /// <summary>
        ///  Cosntructor to build a spreadsheet from the given xml file as well as
        ///  validator and normaliser delegate
        /// </summary>
        /// <param name="filePath"></param> Name of the file or filepath
        /// <param name="isValid"></param> Delegate to have additional restrictions on inputs
        /// <param name="normalize"></param> Normaliser to normalise before passing input
        /// <param name="version"></param> Version of the current spreadsheet
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            string path = filePath;
            cellDependency = new();
            nonEmptyCells = new();
            Version = version; // sets up the version given
            LoadFromFile(path); // Loads and sets up spreadsheet by creating new cells based on the given file
        }

        public override bool Changed { get => changed; protected set => value = changed = value; }

        /// <summary>
        ///  Given a cell name returns the contents in it
        /// </summary>
        /// <param name="name"></param> Cell name in the current spreadsheet
        /// <returns></returns> Contents in it maybe either Formula object,String or double
        /// <exception cref="InvalidNameException"></exception> Throws this exception if given cell name is invalid according to the rules
        /// or if name is null
        public override object GetCellContents(string name)
        {
            string normalisedCellName = Normalize(name);
            // If name is invalid or null throws an exception
            if (normalisedCellName == null || !validateCellName(normalisedCellName) || normalisedCellName.Equals("") ||!IsValid(normalisedCellName))
                throw new InvalidNameException();

            // If spreadsheet contains this cell already filled with content returns the content in it otherwise returns an empty string
            if (nonEmptyCells.ContainsKey(normalisedCellName))
                return nonEmptyCells[normalisedCellName].getCellContent();
            return "";
        }

        /// <summary>
        ///  Returns the cellvalue for the given cell name
        /// </summary>
        /// <param name="name"></param> Cellname
        /// <returns></returns> Value in that cell could be FormulaError,Double or String
        /// <exception cref="InvalidNameException"></exception> Throws this exception if name is invalid according to the rules
        public override object GetCellValue(string name)
        {

            string normalisedCellName = Normalize(name);
            if (!validateCellName(normalisedCellName) || !IsValid(normalisedCellName) || (!validateCellName(normalisedCellName) && IsValid(normalisedCellName)))
                throw new InvalidNameException();
            if (GetCellContents(name).Equals(""))
                return "";
            return nonEmptyCells[normalisedCellName].getValue();
        }

        /// <summary>
        /// Returns an Enumerable of all the names of cells that are having non-Empty cell Contents
        /// </summary>
        /// <returns></returns> Enumerable of all the names of cells that are having non-Empty cell Contents
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return nonEmptyCells.Keys;
        }

        /// <summary>
        ///  Retrieves the version number of the fileName provided 
        /// </summary>
        /// <param name="filename"></param> Filename for which version number is to be looked up
        /// <returns></returns>
        /// <exception cref="SpreadsheetReadWriteException"></exception Throws this exception if any errors are thrown while trying to read/open the file
        public override string GetSavedVersion(string filename)
        {
            try
            {
                // Load the XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);

                // Select the 'spreadsheet' element
                XmlNode spreadsheetNode = xmlDoc.SelectSingleNode("/spreadsheet");

                // Check if the 'spreadsheet' element exists
                    // Get the value of the 'version' attribute
                XmlAttribute versionAttribute = spreadsheetNode.Attributes["version"];
                if (versionAttribute != null)
                    {
                       return versionAttribute.Value;
                    }
                throw new SpreadsheetReadWriteException(" Version info not found ");
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException(" Unable to read the given file ");
            }
        }

        /// <summary>
        ///   Return an XML representation of the spreadsheet's contents
        /// </summary>
        /// <returns> contents in XML form </returns>

        public override string GetXML()
        {
            try
            {
                // Create a StringBuilder to store the XML content
                StringBuilder xmlBuilder = new StringBuilder();

                // Create an XmlWriterSettings to specify indentation and formatting
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "     "; // Use four spaces for indentation

                // Create an XmlWriter with the StringBuilder and settings
                using (XmlWriter writer = XmlWriter.Create(xmlBuilder, settings))
                {
                    writer.WriteStartDocument();

                    // Write the <spreadsheet> element with the version attribute
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    writingFile(writer);

                    //// Return the XML content as a string
                    return xmlBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Error generating XML representation: " + ex.Message);
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>

        public override void Save(string filename)
        {
            try
            {
                Changed = false;
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    writingFile(writer); // method writes a new file
                }
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Error writing to the file: " + ex.Message);
            }
        }

        /// <summary>
        ///  Loads the file given as an argument and this method helps create a new spreadsheet out of
        ///  a given file
        /// </summary>
        /// <param name="filePath"></param> Name of the file to be parsed 
        /// <exception cref="SpreadsheetReadWriteException"></exception> Throws exceptions if was unable to open/read the file
        private void LoadFromFile(string filePath)
        {
            string versionNumberReadFromFile=null;
            try
            {
                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found.", filePath);
                }

                // reads the version number and saves it
                versionNumberReadFromFile = GetSavedVersion(filePath);

                if (!Version.Equals(versionNumberReadFromFile))
                {
                    throw new SpreadsheetReadWriteException("Version Mismatch");
                }

                // creates a new XMLdocument and reads it
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                XmlNodeList cellNodes = xmlDoc.SelectNodes("//cell");
                foreach (XmlNode cellNode in cellNodes)
                {
                    XmlNode nameNode = cellNode.SelectSingleNode("name");
                    XmlNode contentsNode = cellNode.SelectSingleNode("contents");

                    if (nameNode != null && contentsNode != null)
                    {
                        string cellName = nameNode.InnerText.Trim();
                        string cellContents = contentsNode.InnerText.Trim();

                        SetContentsOfCell(cellName, cellContents);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(FileNotFoundException))
                    throw new SpreadsheetReadWriteException($"Unable to find the file {filePath} ! Please check the path and fileName once again ");
                else if (e.GetType() == typeof(SpreadsheetReadWriteException) && e.Message.Equals("Version Mismatch"))
                    throw new SpreadsheetReadWriteException($" Version mismatch found, Given version is {Version} but version in file is {versionNumberReadFromFile} ");
                else if (e.GetType() == typeof(InvalidNameException))
                    throw new SpreadsheetReadWriteException(" Invalid cellName found ");
                else if (e.GetType() == typeof(FormulaFormatException))
                    throw new SpreadsheetReadWriteException(" Incorrect fomrula found ");
                else if (e.GetType() == typeof(CircularException))
                    throw new SpreadsheetReadWriteException(" File is corrupted, not able to open the file ! ");
                else
                    throw new SpreadsheetReadWriteException(" File is corrupted, not able to open the file ! ");
            }
        }

        /// <summary>
        ///  Helper method to write a file which aids GetXML method and Save method
        /// </summary>
        /// <param name="writer"></param> writer passed on from the method that has a writer started to read the file
        private void writingFile(XmlWriter writer)
        {
            foreach (var cellName in GetNamesOfAllNonemptyCells())
            {
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");

                writer.WriteStartElement("cell"); // writes a cell element with the all attributes of a cell
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");
                writer.WriteStartElement("name"); // writes a name element with the all name of the cell
                writer.WriteString("\n"); 
                writer.WriteString("\t \t \t");
                writer.WriteString(cellName);
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");
                writer.WriteEndElement(); // closes the cellName element
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");

                writer.WriteStartElement("contents");// writes a cell content with the contents of a cell
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");

                if (GetCellContents(cellName).GetType() == typeof(string))

                    writer.WriteString((string)GetCellContents(cellName));
                else if (GetCellContents(cellName).GetType() == typeof(double))
                {
                    double d = (double)GetCellContents(cellName);
                    writer.WriteString(d.ToString());
                }
                else if (GetCellContents(cellName).GetType() == typeof(Formula))
                {
                    Formula f = (Formula)GetCellContents(cellName);
                    writer.WriteString(f.ToString());
                }
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");

                writer.WriteEndElement(); // closes the content element
                writer.WriteString("\n");
                writer.WriteString("\t \t \t");
                writer.WriteEndElement();// closes the cell element
                writer.WriteString("\n");
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();// closes the spreadsheet element
            writer.Dispose();

        }

/// <summary>
///  Set the contents of a given cellName to the double number provided
/// </summary>
/// <param name="name"></param> Name of the cell for which contents if exisiting is to be overwritten
/// <param name="number"></param> The number to which content of the given cell name is to be changed
/// <returns></returns> A Set consisting of all the cells dependent on the given cellName
protected override IList<string> SetCellContents(string name, double number)
        {
            string normalisedCellName = Normalize(name);

            cellDependency.ReplaceDependees(normalisedCellName, new HashSet<string>());
            // Gets all the dependents by calling GetCellsToRecalculate method defined in Abstract class
            List<string> dependents = GetCellsToRecalculate(normalisedCellName).ToList();

            // if dictionary already stores the cell then overwrites the cell contents to the given number
            if (nonEmptyCells.ContainsKey(normalisedCellName))
            {
                nonEmptyCells[normalisedCellName].setCellContent(number);
                nonEmptyCells[normalisedCellName].setCellValue(number);
            }
            // If dictionary doesn't contain the cellName ,Creates a new entry of the cellName and the cell
            else
            {
                Cell cell = new(normalisedCellName, number);
                cell.setCellValue(number);
                nonEmptyCells.Add(normalisedCellName, cell);

            }

            foreach (string dependent in dependents)
            {
                if (dependent.Equals(name))
                    continue;
                nonEmptyCells[dependent].setCellValue(computeCellValue(nonEmptyCells[dependent]));
            }

            return dependents;// list to store the cells dependent on the current cell
        }

        /// <summary>
        ///  Set the contents of a given cellName to the string provided
        /// </summary>
        /// <param name="name"></param> Name of the cell for which contents if exisiting is to be overwritten
        /// <param name="text"></param> The text to which content of the given cell name is to be changed
        /// <returns></returns> A Set consisting of all the cells dependent on the given cellName

        protected override IList<string> SetCellContents(string name, string text)
        {
            string normalisedCellName = Normalize(name);
            cellDependency.ReplaceDependees(normalisedCellName, new HashSet<string>());

            // Gets all the dependents by calling GetCellsToRecalculate method defined in Abstract class
            List<string> dependents = GetCellsToRecalculate(normalisedCellName).ToList();

            // if dictionary already stores the cell then overwrites the cell contents to the given number
            if (nonEmptyCells.ContainsKey(normalisedCellName))
            {
                nonEmptyCells[normalisedCellName].setCellContent(text);
                nonEmptyCells[normalisedCellName].setCellValue(text);
            }
            // If dictionary doesn't contain the cellName ,Creates a new entry of the cellName and the cell
            else
            {
                Cell cell = new(normalisedCellName, text);
                nonEmptyCells.Add(normalisedCellName, cell);
            }

            foreach (string dependent in dependents)
            {
                if (dependent.Equals(name))
                    continue;
                nonEmptyCells[dependent].setCellValue(computeCellValue(nonEmptyCells[dependent]));
            }

            return dependents;// list to store the cells dependent on the current cell
        }

        /// <summary>
        ///  Set the contents of a given cellName to the string provided
        /// </summary>
        /// <param name="name"></param> Name of the cell for which contents if exisiting is to be overwritten
        /// <param name="text"></param> The text to which content of the given cell name is to be changed
        /// <returns></returns> A Set consisting of all the cells dependent on the given cellName
        /// <exception cref="CircularException"></exception> Throws an exception if formula is having the cellName either directly or indirectly

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            List<string> dependents;
            Cell previousCell = null;

            if (nonEmptyCells.ContainsKey(name))
                previousCell = nonEmptyCells[name];

            List<string> oldDependees = cellDependency.GetDependents(name).ToList();
            // gets All the dependents in the formula and adds a connection with them for the cellName
            try
            {
                cellDependency.ReplaceDependees(name, formula.GetVariables());

                // Recalculates cells if needed and returns dependents of the cell
                dependents = GetCellsToRecalculate(name).ToList();
            }
            catch (CircularException)
            {
                if (previousCell == null)
                {
                    nonEmptyCells.Remove(name);
                    Changed = false;
                    cellDependency.ReplaceDependees(name, new HashSet<string>());
                    throw new CircularException();
                }
                cellDependency.ReplaceDependees(name, oldDependees);
                nonEmptyCells[name] = previousCell;
                Changed = false;
                throw new CircularException();
            }

            // if dictionary already stores the cell then overwrites the cell contents to the given number
            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(formula);
                nonEmptyCells[name].setCellValue(computeCellValue(nonEmptyCells[name]));
            }
            // If dictionary doesn't contain the cellName ,Creates a new entry of the cellName and the cell
            else
            {
                Cell cell = new(name, formula);
                cell.setCellValue(computeCellValue(cell));
                nonEmptyCells.Add(name, cell);
            }

            foreach (string dependent in dependents)
            {
                if (dependent.Equals(name))
                    continue;
                nonEmptyCells[dependent].setCellValue(computeCellValue(nonEmptyCells[dependent]));
            }

            return dependents;
        }

        /// <summary>
        ///  Sets the contents of the named cell to the appropriate value.
        /// </summary>
        /// <param name="name"></param> CellName 
        /// <param name="content"></param> Content to be set for a string
        /// <returns></returns> All dependents of the current cellName
        /// <exception cref="InvalidNameException"></exception> Throws this exception If name is invalid
        /// <exception cref="ArgumentNullException"></exception>Throws this exception If content is null
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
                throw new ArgumentNullException();

            if (content.Equals(""))
                return new List<string>();
            string normalisedCellName = Normalize(name);
            object previousCellContents = null;

            if (nonEmptyCells.ContainsKey(normalisedCellName))
                previousCellContents = nonEmptyCells[normalisedCellName].getCellContent();
            if (!validateCellName(normalisedCellName) || !IsValid(normalisedCellName) || (validateCellName(normalisedCellName) && !IsValid(normalisedCellName)))
                throw new InvalidNameException();


            if (Double.TryParse(content, out double result))
            {
                Changed = true;
                return SetCellContents(normalisedCellName, result); ;
            }
            // checks if content is a formula by looking if string starts with a '=' sign infront
            else if (content.StartsWith("="))
            {
                Changed = true;
                return SetCellContents(normalisedCellName, new Formula(content, s => s, s => { return Regex.IsMatch(s, "^[a-zA-Z]+\\d+$"); }));
            }
            if (!content.StartsWith("=") && content.GetType() == typeof(string) && !Double.TryParse(content, out double parsedValue))
            {
                return SetCellContents(normalisedCellName, (string)content);
            }
            return null; // stub value
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <required>
        ///    The name must be valid upon entry to the function.
        /// </required>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return cellDependency.GetDependents(name);// GetsDependents for cellName in the cellDependency Graph
        }

        /// <summary>
        ///  Checks if cellName is valid or not according to the new convention
        /// </summary>
        /// <param name="cellName"></param> CellName that is to be validated
        /// <returns></returns> true if valid name or false
        private bool validateCellName(string cellName)
        {
            return Regex.IsMatch(cellName, "^[a-zA-Z]+\\d+$");
        }

        /// <summary>
        ///  Method to compute the value of a cell if the given cell cotents are Formulas
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns> Value computed if successful othrwsie returns a Formula error
        private object computeCellValue(Cell cell)
        {
            Formula f = (Formula)cell.getCellContent();
            return f.Evaluate(s => (double)GetCellValue(s));
        }




        /// <summary>
        ///
        /// Author      : Rohith Veeramachaneni
        /// Partner     : None
        /// Date Created: Feb 9,2023
        /// Date modified : Feb 19,2023

        ///  Cell class helps create new cell objects that can be used as individual cells
        ///  in the Spreadsheet to represent values,contents and names
        ///
        ///  Contains few helper methods to retrieve the needed values or cellName or to change contents
        ///  
        /// </summary>
        private class Cell
        {
            private string cellName;// stores cellName
            private object cellContent;// stores CellCOntent
            private object cellValue;// stores cellValue

            /// <summary>
            ///  Constructor for taking in string type cellContent
            /// </summary>
            /// <param name="cellName"></param> Name of the cell 
            /// <param name="cellContent"></param> Contents in the cell
            public Cell(string cellName, string cellContent)
            {
                this.cellName = cellName;
                this.cellContent = cellContent;
                this.cellValue = cellContent;
            }

            /// <summary>
            ///  Constructor for taking in double type cellContent
            /// </summary>
            /// <param name="cellName"></param> Name of the cell 
            /// <param name="cellContent"></param> Contents in the cell 

            public Cell(string cellName, double cellContent)
            {
                this.cellName = cellName;
                this.cellContent = cellContent;
                this.cellValue = cellContent;
            }

            /// <summary>
            ///  Constructor for taking in Formula type cellContent
            /// </summary>
            /// <param name="cellName"></param> Name of the cell 
            /// <param name="cellContent"></param> Contents in the cell

            public Cell(string cellName, Formula cellContent)
            {
                this.cellName = cellName;
                this.cellContent = cellContent;
            }

            /// <summary>
            ///  Setter method to set cellCOntent to desired content value
            /// </summary>
            /// <param name="cellContent"></param> CellContent to be updated for current cell
            public void setCellContent(object cellContent)
            {
                this.cellContent = cellContent;
            }

            /// <summary>
            ///  Getter method to get the content of the current cell
            /// </summary>
            /// <returns></returns>
            public object getCellContent()
            {
                return this.cellContent;
            }

            /// <summary>
            /// Getter method to get Value of current cell
            /// </summary>
            public object getValue()
            {
                return this.cellValue;
            }

            /// <summary>
            ///  Helper method to set vlue of a cell
            /// </summary>
            /// <param name="value"></param>
            public void setCellValue(object value)
            {
                cellValue = value;
            }
        }

    }

}