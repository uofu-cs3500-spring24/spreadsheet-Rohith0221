using System;
using System.Text.RegularExpressions;
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

        public Spreadsheet()
        {
            cellDependency = new();
            nonEmptyCells = new();
        }

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

        /// <summary>
        /// Returns an Enumerable of all the names of cells that are having non-Empty cell Contents
        /// </summary>
        /// <returns></returns> Enumerable of all the names of cells that are having non-Empty cell Contents
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            HashSet<string> nonEmptyCellNames = new();
            // checks if dictionary to store non-empty cells is non empty to ensure spreadsheet has non-empty cells
            if (nonEmptyCells.Count != 0)
                /// gets all the nonEmpty cellNames which are keys in this dictionary and checks if their
                /// content is empty and if so stores the cellNames in the HashSet otherwise returns an empty Set
                foreach (string cellName in nonEmptyCells.Keys)
                    if (!nonEmptyCells[cellName].getCellContent().Equals(""))
                        nonEmptyCellNames.Add(cellName);
            return nonEmptyCellNames;
        }

        /// <summary>
        ///  Set the contents of a given cellName to the double number provided
        /// </summary>
        /// <param name="name"></param> Name of the cell for which contents if exisiting is to be overwritten
        /// <param name="number"></param> The number to which content of the given cell name is to be changed
        /// <returns></returns> A Set consisting of all the cells dependent on the given cellName
        /// <exception cref="InvalidNameException"></exception> Throws an exception if cellName is invalid or null
        public override ISet<string> SetCellContents(string name, double number)
        {
            // if CellName is null or Invalid throws an exception
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            cellDependency.ReplaceDependees(name, new HashSet<string>());
            // Gets all the dependents by calling GetCellsToRecalculate method defined in Abstract class
            HashSet<string> dependents = GetCellsToRecalculate(name).ToHashSet();

            // if dictionary already stores the cell then overwrites the cell contents to the given number
            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(number);
            }
            // If dictionary doesn't contain the cellName ,Creates a new entry of the cellName and the cell
            else
            {
                Cell cell = new(name, number);
                nonEmptyCells.Add(name, cell);
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

        public override ISet<string> SetCellContents(string name, string text)
        {
            // if CellName is null or Invalid throws an exception
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
            // if given text is null throws an exception
            else if (text == null)
                throw new ArgumentNullException();

            cellDependency.ReplaceDependees(name, new HashSet<string>());
            // Gets all the dependents by calling GetCellsToRecalculate method defined in Abstract class
            HashSet<string> dependents = GetCellsToRecalculate(name).Reverse().ToHashSet();

            // if dictionary already stores the cell then overwrites the cell contents to the given number
            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(text);
            }
            // If dictionary doesn't contain the cellName ,Creates a new entry of the cellName and the cell
            else
            {
                Cell cell = new(name, text);
                nonEmptyCells.Add(name, cell);
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

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (formula is null)
                throw new ArgumentNullException();

            // if CellName is null or Invalid throws an exception
            else if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            // gets All the dependents in the formula and adds a connection with them for the cellName
            cellDependency.ReplaceDependees(name, formula.GetVariables());

            // Recalculates cells if needed and returns dependents of the cell
            HashSet<string> dependents = GetCellsToRecalculate(name).ToHashSet();

            // if dictionary already stores the cell then overwrites the cell contents to the given number
            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(formula);
            }
            // If dictionary doesn't contain the cellName ,Creates a new entry of the cellName and the cell
            else
            {
                Cell cell = new(name, formula);
                nonEmptyCells.Add(name, cell);
            }
            return dependents;
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
        ///  Checks if cellName is valid or not
        /// </summary>
        /// <param name="cellName"></param> CellName that is to be validated
        /// <returns></returns> true if valid name or false
        private bool validateCellName(string cellName)
        {
            return Regex.IsMatch(cellName, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
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
            private double cellValue;// stores cellValue

            /// <summary>
            ///  Constructor for taking in string type cellContent
            /// </summary>
            /// <param name="cellName"></param> Name of the cell 
            /// <param name="cellContent"></param> Contents in the cell
            public Cell(string cellName, string cellContent)
            {
                this.cellName = cellName;
                this.cellContent = cellContent;
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

            private double computeCellValue()
            {
                if (cellContent.GetType() == typeof(Double))
                    this.cellValue = (Double)cellContent;
                return cellValue;
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
        }

    }

}