﻿namespace BoardGame;

public class Board
{
  public int[][] CurrentBoard { get; set; }

public Board(){}
  public Board(int col, int row = 1)
  {
    // Initialize the board with the specified number of columns and rows
    CurrentBoard = new int[row][];
    for (int i = 0; i < row; i++)
    {
      CurrentBoard[i] = new int[col];
      
      for (int j = 0; j < col; j++)
      {
        CurrentBoard[i][j] = 0;
      }
    }
  }

  public bool IsOutOfBound(PlayToken token)
  {
    int row = token.Row - 1;
    int col = token.Column - 1;

    int numBoardRows = CurrentBoard.Length;
    int numBoardCols = CurrentBoard[0].Length;

    if (row < 0 || row >= numBoardRows)
    {
      return true;
    }

    if (col < 0 || col >= numBoardCols)
    {
      return true;
    }

    return false;
  }

  public int GetCellValue(PlayToken piece)
  {
    return CurrentBoard[piece.Row][piece.Column];
  }

  public void PlaceToken(PlayToken token)
  {
    CurrentBoard[token.Row - 1][token.Column - 1] = 1;
  }

  public void RemoveToken(PlayToken token)
  {
    CurrentBoard[token.Row - 1][token.Column - 1] = 0;
  }

  public void PrintBoard()
  {
    int maxCol = CurrentBoard[0].Length;
    int maxRow = CurrentBoard.Length;

    for (int row = 0; row < maxRow; row++)
    {
      for (int dash = 0; dash < maxCol * 4 + 1; dash++)
      {
        Console.Write("-");
      }
      Console.Write("\n");

      for (int col = 0; col < maxCol; col++)
      {
        if (CurrentBoard[row][col] == 1)
        {

          Console.Write("| x ");
        }
        else
        {
          Console.Write("|   ");
        }
      }
      Console.Write("|\n");
    }
    for (int dash = 0; dash < maxCol * 4 + 1; dash++)
    {
      Console.Write("-");
    }
    Console.Write("\n");
  }
}
