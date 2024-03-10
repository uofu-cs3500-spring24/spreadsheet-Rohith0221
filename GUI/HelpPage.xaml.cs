namespace GUI;

public partial class HelpPage : ContentPage
{
	public HelpPage()
	{
		InitializeComponent();
        HelpMenu(this,EventArgs.Empty);
        ReturnToMainPageButton.Clicked += ReturnToMainPage;
    }

    /// <summary>
    ///  HelpMenu method is an event handler method that is invoked when user clicks on Help section and on About section in the Help menu
    ///  that generates a display alert giving information about the spreadsheet about how to enter inputs into a cell in the spreadsheet as well as
    ///  editing the information entered
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpMenu(object sender, EventArgs e)
    {
        DisplayAlert("HELP", "This is a spreadsheet program. User can enter information into each cell, " +"\n"+"\n"+
            "If a formula or a math operation is to be computed,enter the text with '=' sign infront.To compute the value from the entered contents,press enter and result will be displayed in the cell if it's a valid formula otherwise returns FormulaError" +
            "\n" + "\n" + "Click on the new option in the File menu to create a new blank spreadsheet"+"\n" + "\n" + "To save the spreadsheet click on Save option under File menu and type in the desired fileName with which file is to be saved.Files are saved by default in GUI/bin/debug" +
            "\n" + "\n" + "To load an existing spreadsheet file click on Load option under File menu and type in the fileName to be opened. If it exists it opens and prompt appears saying File is opened successfully otherwise if specified File doesn't exist or if entered path is wrong it gives an error message" +
           "\n" + "\n" + "To view the contents of a cell,click on cell to see the entered contents."+"\n"+"\n"+" Click File menu and click Exit to close the app and ensure to save the file before closing to not lose contents ", "OK");
    }

    async void ReturnToMainPage(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // This will return to the main page
    }


}
