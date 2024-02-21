
Author : Rohith Veeramachaneni
Partner : None
Start date : Janaury 19,2024
Course : CS 3500 - University of Utah,School of Computing
Github ID : Rohith0221
Repo : https://github.com/uofu-cs3500-spring24/spreadsheet-Rohith0221
Commit date : Feb 11 -2024
Commit time : 11:00 pm
Solution : Spreadsheet

Copyright:  CS 3500 and Rohith Veeramachaneni - This work may not be copied for use in Academic Coursework.


# Overview of the Spreadsheet functionality

The Spreadsheet program is currently capable of evaluating a formula and also validating if it's good or bad
Future extensions are using Graph to make it function as a cell in Spreadsheet program.Added a new GraphDependencyLibrary
to simulate cells like as in a spreadsheet establishing dependency between two cells.Changed the way validation is checked for formula
and now evaluates the result in doubles and accepts double type numbers.

# Time Expenditures:

    1. Assignment One:   Predicted Hours:          13        Actual Hours:   11

    2. Assignment Two:   Predicted Hours:          12        Actual Hours:   10

    3. Assignment Three : Predicted Hours:         15-20     Actual Hours:10-11  Comments: Took more time to understand the description as it's very vague

    4. Assignment Four  :  Predicted Hours:        16        Actual Hours:12-13

    5. Assignment Five  :  Predicted Hours:        16-18     Actual Hours:12-14

# Examples of Good Software Practices:

1. Had old tests from Assignment 4 rather than writing new tests that test out same functionality
   and fixed the code if any of tests broke

2. Abstraction: Already had a GetCellValue method to return the cell value that is in the cell class

3. DRY: For many methods,when a cell is to be validated against the rules given by the API,the helper method
   validateCellName validates it without repeating the same lines of code again and again throughout the Spreadsheet project





