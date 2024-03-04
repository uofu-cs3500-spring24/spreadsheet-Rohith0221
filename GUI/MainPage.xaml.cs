using SS;
using System.Xml.Linq;

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        //warning for user when closing the app
        //one more row and one more column, cellInfo should stay at the top


        Spreadsheet sp;


        public MainPage()
        {
            InitializeComponent();
            CreateTextBoxes();

            sp = new Spreadsheet(s => true, s => s.ToUpper(), "six");
            
            //Closing += warnOnExit;
        }

        private void CreateTextBoxes()
        {

            for (int i = 1; i <= 99; i++)
            {
                Label label = new Label { Text = i.ToString(), HorizontalTextAlignment = TextAlignment.Center };
                grid.Children.Add(label);
                Grid.SetRow(label, 1);
                Grid.SetColumn(label, i);
            }

            for (int row = 1; row <= 26; row++)
            {
                int charIndex = row - 1;
                char labelChar = (char)(charIndex + 65);

                Label label = new Label { Text = labelChar.ToString(), HorizontalTextAlignment = TextAlignment.Center };
                grid.Children.Add(label);
                Grid.SetRow(label, row + 1); // shift row
                Grid.SetColumn(label, 0);
            }

            for (int row = 1; row < 27; row++)
            {
                for (int column = 1; column < 100; column++)
                {
                    // Create a text box
                    Entry textBox = new Entry
                    {
                        Placeholder = $"{(char)('A' + row - 1)}{column}",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    textBox.Focused += TextBox_TextFocused;
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.Completed += TextBox_TextCompleted;

                    grid.Children.Add(textBox);
                    // Add the text box to the grid
                    Grid.SetRow(textBox, row + 1); // shift row
                    Grid.SetColumn(textBox, column);

                }
            }
        }


        private void TextBox_TextCompleted(object? sender, EventArgs e)
        {
            if (sender is Entry textBox)
            {
                // Get the cell name based on the position of the Entry in the grid
                string cellName = textBox.Placeholder;
                try
                {
                    textBox.Text = sp.GetCellValue(cellName).ToString();
                }
                catch (Exception ex)
                {
                    textBox.Text = ex.Message;
                }
            }
        }

        private void TextBox_TextFocused(object? sender, FocusEventArgs e)
        {
            if (sender is Entry textBox)
            {
                string cellName = textBox.Placeholder;
                try
                {
                    infoLabel.Text = sp.GetCellContents(cellName).ToString();
                }
                catch (Exception)
                {
                }
            }
        }

        private void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is Entry textBox)
            {
                string cellName = textBox.Placeholder;
                try
                {
                    sp.SetContentsOfCell(cellName, textBox.Text);
                }
                catch (Exception)
                {
                }
            }
        }

        void FileMenuNew(object sender, EventArgs e)
        {
            foreach (View view in grid.Children)
            {
                if (view is Entry textBox)
                {
                    textBox.Text = "";
                }
            }
        }
        async void FileMenuLoad(object sender, EventArgs e)
        {
            // Display a prompt dialog for the user to enter the filename
            string filename = await DisplayPromptAsync("Load File", "Enter the filename:");

            if (filename != null) // If the user entered a filename
            {
                try
                {
                    // Read the contents of the file
                    string fileContents = File.ReadAllText(filename);

                    sp = new Spreadsheet(filename, s => true, s => s.ToUpper(), "six");

                    UpdateGridWithSpreadsheetData();

                    await DisplayAlert("File Loaded", "The file has been loaded successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred loading the file: {ex.Message}", "OK");
                }
            }
        }
        private void UpdateGridWithSpreadsheetData()
        {
            // Clear existing text boxes from the grid
            grid.Children.Clear();

            // Create text boxes for each cell in the new spreadsheet
            for (int row = 1; row < 27; row++)
            {
                for (int column = 1; column < 100; column++)
                {
                    // Get the cell name based on the current row and column
                    string cellName = $"{(char)('A' + row - 1)}{column}";

                    // Create a text box
                    Entry textBox = new Entry
                    {
                        Placeholder = cellName,
                        Text = sp.GetCellValue(cellName).ToString(),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    textBox.Focused += TextBox_TextFocused;
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.Completed += TextBox_TextCompleted;

                    grid.Children.Add(textBox);
                    // Add the text box to the grid
                    Grid.SetRow(textBox, row);
                    Grid.SetColumn(textBox, column);

                }
            }
            grid.Children.Add(infoLabel);
            Grid.SetRow(infoLabel, 0);
            Grid.SetColumn(infoLabel, 0);
        }

        async void FileMenuSave(object sender, EventArgs e)
        {
            string filename = await DisplayPromptAsync("Save File", "Enter the filename:");

            if (filename != null)
            {
                try
                {
                    // Call the Save method of your Spreadsheet class, passing the entered filename
                    sp.Save(filename);
                    await DisplayAlert("File Saved", "The file has been saved successfully.", "OK");
                }
                catch (SpreadsheetReadWriteException ex)
                {
                    await DisplayAlert("Error", $"An error occurred saving the file: {ex.Message}", "OK");
                }
            }
        }

        async void warnOnExit(Object sender, EventArgs e)
        {
            bool userResponse=await DisplayAlert(" Warning ", " Contents in the spreadsheet may be erased after exiting. Would you like to save it ? ", "Save","Do Not Save");
            if (userResponse)
            {
                FileMenuSave(sender, e);
               
            }
        }
    }
}