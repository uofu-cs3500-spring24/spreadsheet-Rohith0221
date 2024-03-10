using SS;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Maui.Storage;

/// <summary>
/// Author:    Seongjin Hwang/ Rohith Veeramachaneni
/// Partner:   Seongjin Hwang/ Rohith Veeramachaneni
/// Date:      4-March-2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Seongjin Hwang / Rohith Veeramachaneni - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, [Seongjin Hwang] and [Rohith Veeramachaneni], certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
/// 
/// This file is the Xaml.cs file for the GUI program, contains method
/// that sets up the grid and manupulation of cells, and methods regarding
/// the spreadsheet project such as save, new and load.
/// 
/// 
/// </summary>
///
namespace GUI
{
    public partial class MainPage : ContentPage
    {
        //warning for user when closing the app
        //one more row and one more column, cellInfo should stay at the top
        /// <summary>
        ///  make sure we're reading and saving files only in format .sprd
        /// </summary>

        private Spreadsheet sp;
        Label blank = new Label();
        Label blank2 = new Label();


        /// <summary>
        ///  Default constructor for the app
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            CreateTextBoxes();

            sp = new Spreadsheet(s => true, s => s.ToUpper(), "six");// creates a new spreadsheet with version six
            //CloseWindowCommand=new Command(warnOnExit(this,EventArgs.Empty));
        }

        /// <summary>
        /// Private function which creates the cells for the spreadsheet class,
        /// and creates the label next to the cells.
        /// also keeps tracks of user's action on each cells
        /// </summary>
        private void CreateTextBoxes()
        {
            grid.Children.Add(blank);
            grid.Children.Add(blank2);
            Grid.SetRow(blank, 0);
            Grid.SetRow(blank2, 1);
            Grid.SetColumn(blank, 2);
            Grid.SetColumn(blank2, 2);

            // Creates a numerical label from 1-99 on the top of the grid
            for (int i = 1; i <= 99; i++)
            {
                Label label = new Label { Text = i.ToString(), Padding=10, HorizontalTextAlignment = TextAlignment.Center };
                grid.Children.Add(label);
                Grid.SetRow(label, 2);
                Grid.SetColumn(label, i);
            }

            // Creates an alphabetical label from A-Z on the side of the grid

            for (int row = 3; row < 29; row++)
            {
                int charIndex = row - 3;
                char labelChar = (char)(charIndex + 65);

                Label label = new Label { Text = labelChar.ToString(), Padding=10,VerticalTextAlignment=TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
                grid.Children.Add(label);
                Grid.SetRow(label, row); // shift row
                Grid.SetColumn(label, 0);
            }

            // Creates spreadsheet grid of size 26*99
            for (int row = 3; row < 29; row++)
            {
                for (int column = 1; column <= 99; column++)
                {
                    // Create a text box
                    Entry textBox = new Entry
                    {
                        Placeholder = $"{(char)('A' + row - 3)}{column}",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    textBox.Focus();
                    textBox.Focused += TextBox_TextFocused;
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.Completed += TextBox_TextCompleted;

                    grid.Children.Add(textBox);
                    // Add the text box to the grid
                    Grid.SetRow(textBox, row);
                    Grid.SetColumn(textBox, column);

                }
            }
        }

        /// <summary>
        /// Text completed function sensing if the user pressed enter
        /// to get the value of the current cell the user selected, the
        /// cell's text will show up as the value of cell's content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextCompleted(object? sender, EventArgs e)
        {
            // checks if sender object is a textBox
            if (sender is Entry textBox)
            {
                // Get the cell name based on the position of the Entry in the grid
                string cellName = textBox.Placeholder;
                try
                {
                    textBox.Text = sp.GetCellValue(cellName).ToString();// makes textbox text the cellvalue of current cellName
                    
                    content.Text = sp.GetCellValue(cellName).ToString();
                }
                catch (Exception ex)
                {
                    textBox.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// TextFocused function sensing if the user clicked certain cell,
        /// displays the content of the cell in the Cell information label on top
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextFocused(object? sender, FocusEventArgs e)
        {
            if (sender is Entry textBox)
            {
                // Get the cell name based on the position of the Entry in the grid
                string cellName = textBox.Placeholder;
                try
                {
                    content.Text = sp.GetCellContents(cellName).ToString();// makes CellInfo label at the top of the spreadsheet display contents of the current cell
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// TextChanged function sensing if a cell is changed everytime
        /// the user inputs some text, then it set the contents of the cell
        /// which user changed the text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// TextFocused function sensing if the user clicked certain cell,
        /// displays the content of the cell in the Cell information label on top
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is Entry textBox)
            {
                string cellName = textBox.Placeholder;
                try
                {
                    sp.SetContentsOfCell(cellName, textBox.Text);
                    content.Text = sp.GetCellContents(cellName).ToString();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Clears all the entry in the current spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FileMenuNew(object sender, EventArgs e)
        {
            foreach (View view in grid.Children)
            {
                if (view is Entry textBox)
                {
                    textBox.Text = ""; // resets every cell to default value ""
                }
            }
        }

        /// <summary>
        /// Load method to load the previously saved spreadsheet,
        /// informs the user if the file is successfully loaded or an error occured while loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void FileMenuLoad(object sender, EventArgs e)
        {
            // Display a prompt dialog for the user to enter the filename
            string filename = await DisplayPromptAsync("Load File", "Enter the filename:");

            if (filename.Contains('.'))
            {
                // Get the file extension
                string extension = filename.Substring(filename.LastIndexOf('.'));

                // Check if the extension is not already ".sprd"
                if (extension != ".sprd")
                {
                    // Replaces the extension in the entered filename with ".sprd"
                    filename = filename.Substring(0, filename.LastIndexOf('.')) + ".sprd";
                }
            }
            else
            {
                // If there is no extension, simply append ".sprd"
                filename += ".sprd";
            }

            if (filename != null) // If the user entered a filename
            {
                try
                {
                    // creates a new spreadsheet out of the filename entered by the user
                    sp = new Spreadsheet(filename, s => true, s => s.ToUpper(), "six");

                    UpdateGrid();// updates the grid with appropriate values

                    await DisplayAlert("File Loaded", "The file has been loaded successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred loading the file: {ex.Message}", "OK");
                }
            }
        }

        /// <summary>
        /// Private helper method to update the grid when the user
        /// tries to load a previously saved spreadsheet
        /// </summary>
        private void UpdateGrid()
        {
            // Clear existing text boxes from the grid
            grid.Children.Clear();

            // Create text boxes for each cell in the new spreadsheet
            for (int row = 3; row < 29; row++)
            {
                for (int column = 1; column <= 99; column++)
                {
                    // Get the cell name based on the current row and column
                    string cellName = $"{(char)('A' + row - 3)}{column}";

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
            grid.Children.Add(content);// add the cellInfo label to grid
            Grid.SetRow(content, 0);
            Grid.SetColumn(content, 0);
        }

        /// <summary>
        /// Save method to save the spreadsheet, notices the user if file is saved or an error occurred
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void FileMenuSave(object sender, EventArgs e)
        {
            await Permissions.RequestAsync<Permissions.StorageWrite>();

            string filename = await DisplayPromptAsync("Save File", "Enter the filename:");

            // Check if the input contains a file extension
            if (filename.Contains('.'))
            {
                // Get the file extension
                string extension = filename.Substring(filename.LastIndexOf('.'));

                // Check if the extension is not already ".sprd"
                if (extension != ".sprd")
                {
                    // Replaces the extension in the entered filename with ".sprd"
                    filename = filename.Substring(0, filename.LastIndexOf('.')) + ".sprd";
                }
            }
            else
            {
                // If there is no extension, simply append ".sprd"
                filename += ".sprd";
            }


            FileOptions options = new FileOptions();
            FileResult fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Save File",                
            });

            string directoryPath = null;
            if (fileResult != null)
            {
                directoryPath = Path.GetDirectoryName(fileResult.FullPath);
            }
            else
            {
                // Display an alert indicating that the file was not saved
                await DisplayAlert("File Not Saved", "The file was not saved.", "OK");
                return;
            }

            string filepath = Path.Combine(directoryPath, filename);

            if (!Directory.Exists(directoryPath))
            {
                await DisplayAlert("Error", $"The specified directory '{directoryPath}' does not exist.", "OK");
                return;
            }

            if (filepath != null)
            {
                try
                {
                    // Call the Save method of your Spreadsheet class, passing the entered filename
                    sp.Save(filepath);
                    await DisplayAlert("File Saved", "The file has been saved successfully.", "OK");
                }
                catch (SpreadsheetReadWriteException ex)
                {
                    await DisplayAlert("Error", $"An error occurred saving the file: {ex.Message}", "OK");
                }
            }
        }

        /// <summary>
        /// This method displays a warning message to the user when they try to exit without saving the current spreadsheet or load a new spreadsheet without saving current
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        async void warnOnExit(Object sender, EventArgs e)
        {
            bool userResponse=await DisplayAlert(" Warning ", " Contents in the spreadsheet may be erased after exiting. Would you like to save it ? ", "Save","Do Not Save");
            if (userResponse)
            {
                FileMenuSave(sender, e);
               
            }
        }


        /// <summary>
        ///   Shows how to "push" a new "window" on top of the old and then
        ///   revert.  This is one way to go about the help page implementation.
        ///   
        ///   <para>
        ///     Notice that this recreates a page in code every time... You should either
        ///     only do this once, or have another xaml widget to use....
        ///   </para>
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        void HelpInformation(object sender, EventArgs e)
        {
            var page = new HelpPage();
            Navigation.PushAsync(page, true);
        }
    }

}