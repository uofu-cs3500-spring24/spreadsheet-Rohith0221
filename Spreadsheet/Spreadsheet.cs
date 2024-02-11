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

        public override object GetCellContents(string name)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();

            if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].getCellContent();
            return null;
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            if(nonEmptyCells.Count!=0)
                return nonEmptyCells.Keys;
            return new HashSet<string>();
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
            HashSet<string> dependents = this.GetDirectDependents(name).ToHashSet();
            dependents.Add(name);
            Cell cell = new(name,number);
            nonEmptyCells.Add(name, cell);
            cellDependency.AddDependency(name, "");
            return dependents;
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
            else if (text == null)
                throw new ArgumentNullException();

            HashSet<string> dependents = this.GetDirectDependents(name).ToHashSet();
            dependents.Add(name);
            Cell cell = new(name, text);
            nonEmptyCells.Add(name, cell);
            cellDependency.AddDependency(name, "");
            return dependents; 
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (formula == null)
                throw new ArgumentNullException();
            else if (name == null || !validateCellName(name))
                throw new InvalidNameException();


            Cell cell = new(name, formula);
            nonEmptyCells.Add(name, cell);
            foreach(string dependent in formula.GetVariables())
                cellDependency.AddDependency(name,dependent);
            HashSet<string> dependents = this.GetDirectDependents(name).ToHashSet();
            dependents.Add(name);
            return dependents;
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null || !validateCellName(name))
                throw new InvalidNameException();
            return cellDependency.GetDependents(name);
        }

        private bool validateCellName(string cellName)
        {
            return Regex.IsMatch(cellName, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

    }

}

