using System;
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
    ///
    ///
    ///
    ///  Spreadsheet class is a concrete implementation of the Abstract class
    ///  AbstractSpreadsheet alongwith some helper methods that help facilitate the creation
    ///  of a spreadsheet that can store the relation between two cells and can also
    ///  let the user know number of nonEmpty cells in this spreadsheet
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Dependency Graph to store connections between cells
        private DependencyGraph cellDependency;
        private Dictionary<string, Cell> nonEmptyCells;
        private bool changed;

        public Spreadsheet() : base(s => true,s=>s,"default")
        {

            cellDependency = new();
            this.nonEmptyCells = new();
            Changed = false;
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cellDependency = new();
            nonEmptyCells = new();
            Changed = false;
        }

        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            string path = filePath;
            cellDependency = new();
            nonEmptyCells = new();
            Version = version;
        }

        public override bool Changed { get =>changed ; protected set => value=changed=value; }

        /// <summary>
        ///  Given a cell name returns the contents in it
        /// </summary>
        /// <param name="name"></param> Cell name in the current spreadsheet
        /// <returns></returns> Contents in it maybe either Formula object,String or double
        /// <exception cref="InvalidNameException"></exception> Throws this exception if given cell name is invalid according to the rules
        /// or if name is nukk
        public override object GetCellContents(string name)
        {
            // If name is invalid or null throws an exception
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            // If spreadsheet contains this cell already filled with content returns the content in it otherwise returns an empty string
            if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].getCellContent();
            return "";
        }

        public override object GetCellValue(string name)
        {

            string normalisedCellName = Normalize(name);
            if (!validateCellName(normalisedCellName) || validateCellName(normalisedCellName) && !IsValid(normalisedCellName))
                throw new InvalidNameException();
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

        public override string GetSavedVersion(string filename)
        {
            try
            {

            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException(" Unable to open the given file ");
            }
            throw new NotImplementedException();
        }

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
                    writer.WriteAttributeString("spreadsheet"+"version", Version);

                    foreach (string cellName in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteStartElement("cell");
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteStartElement("name");
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteString(cellName);
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteEndElement();
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");

                        writer.WriteStartElement("contents");
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t ");

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
                        writer.WriteString("\t \t \t ");
                        writer.WriteEndElement();
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteEndElement(); // Close cell element
                        writer.WriteString("\n");
                    }

                    writer.WriteEndElement(); // Close spreadsheet element
                    writer.WriteEndDocument();
                }

                // Return the XML content as a string
                return xmlBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Error generating XML representation: " + ex.Message);
            }
        }

        public override void Save(string filename)
        {
            try
            {
                Changed = false;
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("spreadsheet"+"version",Version);

                    foreach (var cellName in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");

                        writer.WriteStartElement("cell");
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteStartElement("name");
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteString(cellName);
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteEndElement();
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");

                        writer.WriteStartElement("contents");
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");

                        if (GetCellContents(cellName).GetType()==typeof(string))
                        
                            writer.WriteString((string)GetCellContents(cellName));
                        else if(GetCellContents(cellName).GetType() == typeof(double))
                        {
                            double d = (double)GetCellContents(cellName);
                            writer.WriteString(d.ToString());
                        }
                        else if(GetCellContents(cellName).GetType() == typeof(Formula))
                        {
                            Formula f = (Formula)GetCellContents(cellName);
                            writer.WriteString(f.ToString());
                        }
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");

                        writer.WriteEndElement();
                        writer.WriteString("\n");
                        writer.WriteString("\t \t \t");
                        writer.WriteEndElement();
                        writer.WriteString("\n");
                    }
                    writer.WriteEndElement();
                   
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Error writing to the file: " + ex.Message);
            }
        }


        //}
        /// <summary>
        ///  Set the contents of a given cellName to the double number provided
        /// </summary>
        /// <param name="name"></param> Name of the cell for which contents if exisiting is to be overwritten
        /// <param name="number"></param> The number to which content of the given cell name is to be changed
        /// <returns></returns> A Set consisting of all the cells dependent on the given cellName
        /// <exception cref="InvalidNameException"></exception> Throws an exception if cellName is invalid or null
        protected override IList<string> SetCellContents(string name, double number)
        {
            string normalisedCellName = Normalize(name);
            // if CellName is null or Invalid throws an exception
            if (normalisedCellName == null || !validateCellName(normalisedCellName) || validateCellName(normalisedCellName) && !IsValid(normalisedCellName))
                throw new InvalidNameException();

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
        /// <exception cref="InvalidNameException"></exception> Throws an exception if cellName is invalid or null
        /// <exception cref="ArgumentNullException"></exception> Throws an exception if text is null

        protected override IList<string> SetCellContents(string name, string text)
        {
            string normalisedCellName = Normalize(name);
            // if CellName is null or Invalid throws an exception
            if (normalisedCellName == null || !validateCellName(normalisedCellName) || validateCellName(normalisedCellName) && !IsValid(normalisedCellName))
                throw new InvalidNameException();
            // if given text is null throws an exception
            else if (text == null)
                throw new ArgumentNullException();

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
        /// <exception cref="InvalidNameException"></exception> Throws an exception if cellName is invalid or null
        /// <exception cref="ArgumentNullException"></exception> Throws an exception if text is null
        /// <exception cref="CircularException"></exception> Throws an exception if formula is having the cellName either directly or indirectly

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            List<string> dependents;
            Cell previousCell = null ;
            if (formula is null)
                throw new ArgumentNullException();

            // if CellName is null or Invalid throws an exception
            else if (name == null || !validateCellName(name) || validateCellName(name) && !IsValid(name))
                throw new InvalidNameException();

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
            catch(CircularException)
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
                Changed=false;
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

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content.Equals(""))
                return new List<string>();
            string normalisedCellName = Normalize(name);
            object previousCellContents=null;
            if (nonEmptyCells.ContainsKey(normalisedCellName))
                previousCellContents = nonEmptyCells[normalisedCellName].getCellContent();
            if (!validateCellName(normalisedCellName) ||!IsValid(normalisedCellName)|| (validateCellName(normalisedCellName) && !IsValid(normalisedCellName)))
                throw new InvalidNameException();
            if (Double.TryParse(content, out double result))
            {
                Changed = true;
                return SetCellContents(name, result); ;
            }
            else if (content.StartsWith("="))
            {
                    Changed = true;
                    return SetCellContents(normalisedCellName,new Formula(content, s => s, s => { return Regex.IsMatch(s, "^[a-zA-Z]+\\d+$"); }));
            }
            if (!content.StartsWith("=") && content.GetType() == typeof(string) && !Double.TryParse(content, out double parsedValue))
            {
                return SetCellContents(normalisedCellName, (string)content);
            }
            return null; // stub value
        }

        /// <summary>
        /// Helper method for GetCellsToRecalculate method that retrieves all the dependents of the given cell
        /// using cellName
        /// </summary>
        /// <param name="name"></param> Name of the cell for which dependents has to be returned
        /// <returns></returns> List of the dependents of the given cell
        /// <exception cref="InvalidNameException"></exception> If cellNAme is invalid or null throws an exception
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
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

        private object computeCellValue(Cell cell)
        {
            if (cell.getCellContent().GetType() == typeof(Formula))
            {
                Formula f = (Formula)cell.getCellContent();
                return f.Evaluate(s =>(double) GetCellValue(s));
            }
            return cell.getValue();
        }

        


        /// <summary>
        ///
        /// Author      : Rohith Veeramachaneni
        /// Partner     : None
        /// Date Created: Feb 9,2023

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
            /// 
            /// Intentionally Commented :
            /// Leaving it blank as this assignment does not need the value of the cell
            /// </summary>
            public object getValue()
            {
                return this.cellValue;
            }

            public void setCellValue(object value)
            {
                cellValue = value;
            }
        }
        
    }

}