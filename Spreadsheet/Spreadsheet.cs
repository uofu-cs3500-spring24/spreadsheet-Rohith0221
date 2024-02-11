using System;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SS
{
	public class Spreadsheet:AbstractSpreadsheet
	{
        private DependencyGraph cellDependency;
        private Dictionary<string, Cell> nonEmptyCells;

        public Spreadsheet()
		{
            cellDependency= new();
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
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            if(nonEmptyCells.Count!=0)
                return nonEmptyCells.Keys;
            return new HashSet<string>();
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

            cellDependency.AddDependency(name, "");
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

            cellDependency.AddDependency(name, "");

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
            if (formula == null)
                throw new ArgumentNullException();
            else if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            foreach (string dependent in formula.GetVariables())
                cellDependency.AddDependency(name, dependent);
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
            return cellDependency.GetDependents(name);
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

    }

}

