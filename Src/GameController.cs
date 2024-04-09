namespace BoardGame;

using System.Text.Json;
using System.Linq;

public class GameController
{
  public string GameID { get; set; }
  public int PlayerTurn = 0;
  public Board Board { get; set; }
  public Player[] Players = new Player[2];
  public Player CurrentPlayer { get; set; }
  public bool OpponentIsBot;

  History history = new History();

  public Record Record;

  public GameController(){}

  public GameController(bool opponentIsBot, int boardColumnSizeInput, int boardRowSizeInput = 1)
  {
    GameID = "s1";
    Prompt.Playing("Name of player 1 - enter to set as `player_1`?");
    string Player1IDInput = Console.ReadLine() ?? "";

    Players[0] = new PlayerHuman { ID = Player1IDInput == "" ? "player_1" : Player1IDInput };

    if (!opponentIsBot)
    {
      Prompt.Playing("Name of player 2 - enter to set as `player_2`?");
      string Player2IDInput = Console.ReadLine() ?? "";

      Players[1] = new PlayerHuman { ID = Player2IDInput == "" ? "player_2" : Player2IDInput };
    }
    else
    {
      Players[1] = new PlayerBot { ID = "bot_0" };
    }

    Board = new Board(boardColumnSizeInput, boardRowSizeInput);
    Board.PrintBoard();

    PlayerTurn = 0;
    CurrentPlayer = Players[PlayerTurn];
    OpponentIsBot = opponentIsBot;
    Record = new Record();

    Console.WriteLine("Game start!");
  }

  public GameProgramStatus HandlePlayerMove(int colPosition, int rowPosition = 1)
  {
    PlayToken token = CurrentPlayer.Play(colPosition);

    bool isValidMove = ValidateMove(token);

    if (!isValidMove)
    {
      throw new Exception("INVALID_MOVE");
    }
    else
    {
      Board.PlaceToken(token);
      Board.PrintBoard();
      Record.Update(token);

      bool hasWon = CheckWin();
      if (hasWon)
      {
        //save in record, destruct this controller
        return GameProgramStatus.EndGame;
      }

      //next move
      if (OpponentIsBot)
      {
        CurrentPlayer = Players[1]; //the bot player

        bool isBotValidMove = false;
        while (isBotValidMove == false)
        {
          PlayToken botToken = Players[1].Play(Board.CurrentBoard[0].Length, Board.CurrentBoard.Length);
          isBotValidMove = ValidateMove(botToken);

          if (isBotValidMove)
          {
            Prompt.Playing($"- {CurrentPlayer.ID} turn", true);
            Board.PlaceToken(botToken);
            Board.PrintBoard();
            Record.Update(botToken);
          }
        };

        //check win
        hasWon = CheckWin();
        if (hasWon)
        {
          //destruct this controller
          return GameProgramStatus.EndGame;
        }

        CurrentPlayer = Players[0];
        //put in record
      }
      else
      {
        PlayerTurn = PlayerTurn == 0 ? 1 : 0;
        CurrentPlayer = Players[PlayerTurn];
      }
    }

    return GameProgramStatus.Playing;
  }

  public bool ValidateMove(PlayToken token)
  {
    if (Board.IsOutOfBound(token))
    {
      return false;
    }

    // check if the requested position on the board is occupied
    if (Board.CurrentBoard[token.Row - 1][token.Column - 1] != 0)
    {
      return false;
    }

    return true;
  }

  public virtual bool CheckWin()
  {
    int countSequence = 0;

    for (int col = 0; col < Board.CurrentBoard[0].Length; col++)
    {
      if (Board.CurrentBoard[0][col] == 1)
      {
        countSequence++;
        if (countSequence == 3)
        {
          return true;
        }
      }
      else
      {
        countSequence = 0;
      }
    }
    return false;
  }

  public void HandleUndo()
  {
    int currentRecordIndex = Record.Iterator;
    int lastStateIndex = Record.GetPrevState(PlayerTurn);

    for (int head = currentRecordIndex - 1; head >= lastStateIndex; head--)
    {
      PlayToken tokenToRemove = Record.Actions[head];
      Board.RemoveToken(tokenToRemove);
    }
    Board.PrintBoard();
  }

  public void HandleRedo()
  {
    int currentRecordIndex = Record.Iterator;
    int nextStateIndex = Record.GetNextState();

    for (int head = currentRecordIndex; head < nextStateIndex; head++)
    {
      PlayToken tokenToPlace = Record.Actions[head];
      Board.PlaceToken(tokenToPlace);
    }
    Board.PrintBoard();
  }

  public void SaveGame()
  {
     string filePath = "data.json";

     GameSnapshot snapshot = new()
     {
      PlayerIDs = Players.Select((player) => player.ID).ToArray(),
      PlayerTurn = PlayerTurn,
      GameMode = OpponentIsBot ? 1 : 2,
      Board = Board.CurrentBoard,
      Actions = Record.Actions
     };

     history.SaveGame(snapshot);
  }

    internal GameProgramStatus HandlePlayerMove(Player currentPlayer, Board board)
    {
        throw new NotImplementedException();
    }
}

public class TrebleCrossController(bool isPlayWithBot, int boardNumColumns) : GameController(isPlayWithBot, boardNumColumns)
{
    public override bool CheckWin()
  {
    int countSequence = 0;

    for (int col = 0; col < Board.CurrentBoard[0].Length; col++)
    {
      if (Board.CurrentBoard[0][col] == 1)
      {
        countSequence++;
        if (countSequence == 3)
        {
          return true;
        }
      }
      else
      {
        countSequence = 0;
      }
    }
    return false;
  }
}
