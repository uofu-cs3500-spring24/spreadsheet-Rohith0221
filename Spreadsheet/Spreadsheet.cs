using System;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SS
{
    public class Spreadsheet :AbstractSpreadsheet
    {
        private DependencyGraph cellDependency;
        private Dictionary<string, Cell> nonEmptyCells;

        public Spreadsheet()
        {
            cellDependency = new();
            nonEmptyCells = new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].getCellContent();
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            HashSet<string> nonEmptyCellNames = new();
            if (nonEmptyCells.Count != 0)
                foreach (string cellName in nonEmptyCells.Keys)
                    if (!nonEmptyCells[cellName].getCellContent().Equals(""))
                        nonEmptyCellNames.Add(cellName);
            return nonEmptyCellNames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            HashSet<string> dependents = this.GetDirectDependents(name).ToHashSet();
            dependents.Add(name);
            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(number);
            }
            else
            {
                Cell cell = new(name, number);
                nonEmptyCells.Add(name, cell);
            }
            return dependents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
            else if (text == null)
                throw new ArgumentNullException();


            HashSet<string> dependents = this.GetDirectDependents(name).ToHashSet();
            dependents.Add(name);
            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(text);
            }
            else
            {
                Cell cell = new(name, text);
                nonEmptyCells.Add(name, cell);
            }
            return dependents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (formula is null)
                throw new ArgumentNullException();
            else if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            foreach (string dependent in formula.GetVariables())
                cellDependency.AddDependency(dependent, name);

            foreach (string n in GetDirectDependents(name))
            {
                if (n.Equals(name))
                {
                    throw new CircularException();
                }
            }

            HashSet<string> dependents = this.GetDirectDependents(name).ToHashSet();
            dependents.Add(name);

            if (nonEmptyCells.ContainsKey(name))
            {
                nonEmptyCells[name].setCellContent(formula);
            }
            else
            {
                Cell cell = new(name, formula);
                nonEmptyCells.Add(name, cell);
            }
            return dependents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
            return cellDependency.GetDependees(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        private bool validateCellName(string cellName)
        {
            return Regex.IsMatch(cellName, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }


        /// <summary>
        /// 
        /// </summary>
        private class Cell
        {
            private string cellName;
            private object cellContent;
            private double cellValue;

            public Cell(string cellName, string cellContent)
            {
                this.cellName = cellName;
                this.cellContent = cellContent;
            }

            public Cell(string cellName, double cellContent)
            {
                this.cellName = cellName;
                this.cellContent = cellContent;
                this.cellValue = cellContent;
            }

            public Cell(string cellName, Formula cellContent)
            {
                this.cellName = cellName;
                this.cellName = cellName;
                this.cellContent = cellContent;
            }

            public string getCellName()
            {
                return this.cellName;
            }

            public void setCellContent(object cellContent)
            {
                this.cellContent = cellContent;
            }

            public object getCellContent()
            {
                return this.cellContent;
            }

            /// <summary>
            /// 
            /// Intentionally Commented :
            /// Leaving it blank as this assignment does not need the value of the cell
            /// </summary>

            //private double computeCellValue()
            //{
            //    if (cellContent.GetType() == typeof(Double))
            //        this.cellValue = (Double)cellContent;
            //    return cellValue;
            //}

            /// <summary>
            /// 
            /// Intentionally Commented :
            /// Leaving it blank as this assignment does not need the value of the cell
            /// </summary>
            //public object getValue()
            //{
            //    return this.cellValue;
            //}
        }

    }

}