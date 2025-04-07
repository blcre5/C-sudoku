
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
class Program
{
    static Random random = new Random();

    static void Main(string[] args)
    {
        Program program = new Program();
        //var SudokuPuzzle = program.GenerateLists();
        var SudokuPuzzle = program.MakeTable();

        //var SudokuPuzzle2 = program.MakeTable(); // this is to show the original table before we start solving it, if needed


        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(SudokuPuzzle[i, j]);
            }
            Console.WriteLine();
        }
    }

    int[,] largeArray = new int[9, 9];


    public static bool DetectZeroes(int[,] largeArray)
    {
        foreach (int number in largeArray)
        {
            if (number == 0)
            {
                return true;
            }
        }
        return false;
    }

    private (int, int) GetGridCoordinates(int x, int y, int[] gridcoord)
    {
        int gridx = gridcoord[x / 3]; // uses int division to find the correct index
        int gridy = gridcoord[y / 3];
        return (gridx, gridy);
    }
/*
    public bool ValidNumber(int[,] largeArray, int currentx, int currenty, int randomNum)
    {
        List<int> currentRow = new List<int>();
        List<int> currentColumn = new List<int>();
        List<int> currentGrid = new List<int>();
        int[] gridcoord = new int[3] { 0, 3, 6 };
        int[] numbers = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int gridx = 0;
        int gridy = 0;
        bool leftRight = false;
        bool upDown = false;
        bool subgrid = false;


        for (int index = 0; index < 9; index++)
        {
            currentColumn.Add(largeArray[currentx, index]);
            currentRow.Add(largeArray[index, currenty]);
        }
        //generate the list to capture the current grid
        (gridx, gridy) = GetGridCoordinates(currentx, currenty, gridcoord); // works because a tuple(for which the individual variables were define earlier) is returned 
        for (int i = gridx; i < gridx + 3; i++)
        {
            for (int j = gridy; j < gridy + 3; j++)
            {
                currentGrid.Add(largeArray[i, j]);
            }
        }
        if (currentGrid.Contains(randomNum))
        {
            subgrid = false;
        }
        else
        {
            subgrid = true;
        }

        if (currentColumn.Contains(randomNum))
        {
            upDown = false;
        }
        else
        {
            upDown = true;
        }

        if (currentRow.Contains(randomNum))
        {
            leftRight = false;
        }
        else
        {
            leftRight = true;
        }

        return leftRight && upDown && subgrid;

    } */





    public bool errorCheck(int[,] largeArray, List<int> newRow, int setRow)
    {
        
        List<int> currentColumn = new List<int>();
        List<int> currentGrid = new List<int>();

        HashSet<int> HashColumn = new HashSet<int>();
        HashSet<int> HashGrid = new HashSet<int>();

        List<int> duplicatesColumn = new List<int>();
        List<int> duplicatesGrid = new List<int>();

        int[] gridcoord = new int[3] { 0, 3, 6 };
        
        List<bool> upside = new List<bool>();
        List<bool> subgrid = new List<bool>();

        for (int number = 0; number < 9; number++)
        {
            largeArray[setRow, number] = newRow[number];
        }



        foreach (int gridLR in gridcoord) //should check all 9 grids
        {
            foreach (int gridUD in gridcoord)
            {
                if (largeArray[gridLR, gridUD] == 0)
                {
                    break;
                }
                currentGrid.Clear();
                HashGrid.Clear();
                duplicatesGrid.Clear();

                for (int i = gridLR; i < gridLR + 3; i++)// generates the current 3X3 cell to check
                {
                    for (int j = gridUD; j < gridUD + 3; j++)
                    {
                        currentGrid.Add(largeArray[i, j]);
                    }
                }
                foreach (int number in currentGrid) //starts the check for duplicates
                {
                    if (HashGrid.Contains(number) && number != 0)
                    {
                        duplicatesGrid.Add(number);
                    }
                    else
                    {
                        HashGrid.Add(number);
                    }
                }
                if (duplicatesGrid.Any())//duplicates has any values, 
                {
                    return false;
                }
            }
        }

        for (int row = 0; row < 9; row++)
        {
            currentColumn.Clear();
            HashColumn.Clear();
            duplicatesColumn.Clear();
            for (int index = 0; index < 9; index++)// captures the current column for the current integer
            {
                currentColumn.Add(largeArray[index, row]);
            }

            foreach (int number in currentColumn) // checks the column
            {
                if (HashColumn.Contains(number) && number != 0)
                {
                    duplicatesColumn.Add(number);
                }
                else
                {
                    HashColumn.Add(number);
                }
            }
            if (duplicatesColumn.Any())//duplicates has any values, 
            {
                return false;
            }
        }
        return true;
    }

    public int[,] MakeTable()
    {
        int[,] Table = new int[9, 9];
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> origingalNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        do
        {
            ScrambleList(numbers);
            for (int i = 0; i < 9; i++)
            {
                Table[0, i] = numbers[i];
            }
            
            for (int row = 1; row < 9; row++)
            {
                ScrambleList(numbers);
                
                for (int col = 0; col < 9; col++)
                {
                    if (errorCheck(Table, numbers, row))
                    {
                        Table[row, col] = numbers[col];
                    }
                    else
                    {
                        row--;
                        break;
                    }
                }
            }
        } while (DetectZeroes(Table));
        return Table;
    }
    public static void ScrambleList<T>(List<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    static void ClearPreviousLine()
    {
        // Move the cursor to the beginning of the previous line
        Console.SetCursorPosition(0, Console.CursorTop - 1);

        // Clear the line by writing spaces over it
        Console.Write(new string(' ', Console.WindowWidth));

        // Move the cursor back to the beginning of the previous line
        Console.SetCursorPosition(0, Console.CursorTop - 1);
    }
}




