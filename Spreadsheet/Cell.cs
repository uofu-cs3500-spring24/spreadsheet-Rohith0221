using System;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
namespace SS
{
	/// <summary>
	///  
	/// </summary>
	public class Cell
	{
		//private HashSet<Tuple<string, Formula>> cells;
		//private Dictionary<string,Tuple<string,>> cell;
		private string cellName;
		private Object cellContent;
		private double cellValue;

		public Cell(string cellName,string cellContent)
		{
			this.cellName = cellName;
            if (!validateCellName())
                throw new CellNameFormatException("Invalid Cell Name");
            this.cellContent = cellContent;
		}

		public Cell(string cellName,double cellContent)
		{
            //cells = new();
            this.cellName = cellName;
            if (!validateCellName())
                throw new CellNameFormatException("Invalid Cell Name");
            this.cellContent = cellContent;
			this.cellValue = cellContent;
			//cells.Add(new Tuple<string, Formula>(cellName,new Formula("")));
		}

        public Cell(string cellName,Formula cellContent)
        {
            this.cellName = cellName;
			//cells = new();
			if (!validateCellName())
				throw new CellNameFormatException("Invalid Cell Name");
			this.cellName = cellName;
			this.cellContent = cellContent;
        }

		public string getCellName()
		{
			return this.cellName;
		}

		public void setCellContent(Object cellContent)
		{
			this.cellContent = cellContent;
		}

		public Object getCellContent()
		{
			return this.cellContent;
		}

		public IEnumerable<string> getDependents()
		{
			if (cellContent.GetType() == typeof(Formula))
			{
				Formula formula = (Formula)cellContent;
				return formula.GetVariables();
			}
			else
				return new HashSet<string>();
		}

		private Boolean validateCellName()
		{
            return Regex.IsMatch(this.cellName, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }


		public double computeCellValue()
		{
			if (cellContent == typeof(Double))
				this.cellValue = (Double)cellContent;
			//else if(cellContent)
			return cellValue;
		}

		public Object getValue()
		{
			return this.cellValue;
		}
	}

	public class CellNameFormatException : FormatException
    {
        private String Reason;
        public CellNameFormatException(String Reason)
        {
            this.Reason = Reason;
        }
    }

}

