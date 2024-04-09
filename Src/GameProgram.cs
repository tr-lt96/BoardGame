using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Win32.SafeHandles;
namespace BoardGame;

public class GameProgram // 
{
  public GameController? GameController;
  //public string[] GameRecord;

  public GameProgramStatus GameState { get; set; }
  private static readonly Regex CommandPlayPattern = new(pattern: @"^play ([0-9]+)", options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

   History gameHistory = new History();

  Record Record;
  public GameProgram()
  {
    GameState = GameProgramStatus.MainMenu;
  }

  void HandleMenuCommand(string command)
  {
    switch (command)
    {
      case "start":
        //start new game
        SetupGame();
        break;

      case "load":
        //GameState = GameProgramStatus.SavedFiles;
        //string screen = GetScreen(GameState);
        //display list of current game(s)
        LoadGame();
        break;

      case "exit":
        GameState = GameProgramStatus.Exit;
        break;

      case "help":
        Help.PrintHelp(GameProgramStatus.MainMenu);
        break;

      default:
        throw new Exception("INVALID");
    }
  }

  void HandlePlayCommand(string command)
  {
    if (GameController == null)
    {
      throw new Exception("INVALID_STATE");
    }

    switch (command)
    {
      case "save":
        GameController.SaveGame();
        break;

      case "undo":
        //undo a move
        GameController.HandleUndo();
        break;

      case "redo":
        //redo a move
        GameController.HandleRedo();
        break;

      case "quit":
        // quit game to main menu - assume no auto save needed
        GameState = GameProgramStatus.MainMenu;
        break;

      case "help":
        Help.PrintHelp(GameProgramStatus.Playing);
        break;

      default:
        if (command.Contains("play"))
        {

          if (Regex.IsMatch(command, @"^play ([0-9]+)"))
          {
            string tokenIndex = command.Split(" ")[1];
            if (int.TryParse(tokenIndex, out int position))
            {
              GameState = GameController.HandlePlayerMove(position);
            }
            else
            {
              Console.WriteLine($"Cannot parse command position `{tokenIndex}`.");
              throw new Exception("INVALID_MOVE");
            }
            break;
          }

          throw new Exception("INVALID_MOVE");
        }

        throw new Exception("INVALID");
    }
  }

  void SetupGame()
  {
    try
    {
      GameState = GameProgramStatus.SetupGame;
      //skip game select

      //select mode
      Prompt.Setup("Select mode (1 - default) vs computer or (2) vs human - enter to set default mode 1?");
      string gameMode = Console.ReadLine()?.Trim() ?? "";
      if (gameMode == "")
      {
        gameMode = "1";
      }
      else if (gameMode != "1" && gameMode != "2")
      {
        throw new Exception($"Invalid game mode `{gameMode}`.");
      }

      //set board size
      Prompt.Setup("Board size - enter to set default size 10?");
      string? boardSizeInput = Console.ReadLine() ?? "";
      if (boardSizeInput == "")
      {
        boardSizeInput = "10";
      }

      if (!int.TryParse(boardSizeInput, out int boardSize))
      {
        throw new Exception($"Cannot parse board size `{boardSizeInput}`.");
      }

      GameController = new TrebleCrossController(gameMode == "1", boardSize);
      GameState = GameProgramStatus.Playing;
    }
    catch (Exception error)
    {
      Console.WriteLine($"{error.Message}");
      Console.WriteLine("Back to main menu.");
      GameState = GameProgramStatus.MainMenu;
    }
  }

  void LoadGame()
  {
    string filePath = "data.json";
    
    if (File.Exists(filePath))
    {
      try {
            string jsonReadData = File.ReadAllText(filePath);
            Console.WriteLine(jsonReadData);
            GameSnapshot snapshot = JsonSerializer.Deserialize<GameSnapshot>(jsonReadData);

            GameController gc = new();

            gc.Players[0] = new PlayerHuman { ID = snapshot.PlayerIDs[0] };
            gc.Players[1] = snapshot.PlayerIDs[1] == "bot_0" || snapshot.PlayerIDs[1] == null ? new PlayerBot() { ID = "bot_0" } : new PlayerHuman() { ID = snapshot.PlayerIDs[1]};
                gc.Board = new()
                {
                    CurrentBoard = snapshot.Board
                };
                gc.PlayerTurn = snapshot.PlayerTurn;
            gc.CurrentPlayer = gc.Players[gc.PlayerTurn];
            gc.OpponentIsBot = snapshot.GameMode == 1;
            gc.Record = new()
            {
                Actions = snapshot.Actions,
                Iterator = 0
            };
            
            GameController = gc;

            Board board = new(snapshot.Board[0].Length, snapshot.Board.Length);

            foreach (var action in snapshot.Actions)
            {
                Console.WriteLine(action.PlayerID);
                PlayToken token = new(action.Row, action.Column, action.PlayerID);
                board.PlaceToken(token);
                board.PrintBoard();

                if (gc.CheckWin())
        {
          GameState = GameProgramStatus.EndGame;
          Prompt.Playing($"{gc.CurrentPlayer.ID} won!", true); // Announce winner
          break; // Exit the loop if someone wins
        }

            }

            if (GameState != GameProgramStatus.EndGame && gc.CheckWin())
      {
        GameState = GameProgramStatus.EndGame;
        Prompt.Playing("It's a Draw!", true); // Announce draw
      }
      else
      {
        GameState = GameProgramStatus.Playing;  // Set Playing state if no winner/draw
      }

            GameState = GameProgramStatus.Playing;
      }
            catch (Exception ex)
          {
            Console.WriteLine($"Error loading game data: {ex.Message}");
            Console.WriteLine("Back to main menu.");
            GameState = GameProgramStatus.MainMenu;
          }
        }
        else
        {
            Console.WriteLine("File not found: data.json");
            Console.WriteLine("Back to main menu.");
            GameState = GameProgramStatus.MainMenu;
        }
  }

    public void RunApp()
  {
    while (true)
    {
      string? command = null;
      try
      {
        if (GameState == GameProgramStatus.Exit)
        {
          break;
        }

        else if (GameState == GameProgramStatus.MainMenu)
        {
          Prompt.MainMenu("Type `help` to get a list of command");

          command = Console.ReadLine() ?? "";
          HandleMenuCommand(command.Trim());
          continue;
        }

        else if (GameState == GameProgramStatus.Playing)
        {
          if (GameController == null)
          {
            throw new Exception("INVALID_STATE");
          }

          string currentPlayerID = GameController.CurrentPlayer.ID;
          Prompt.Playing($"- {currentPlayerID} turn");

          command = Console.ReadLine() ?? "";
          HandlePlayCommand(command.Trim());
          continue;
        }
        else if (GameState == GameProgramStatus.EndGame)
        {
          if (GameController == null)
          {
            throw new Exception("INVALID_STATE");
          }

          Prompt.Playing($"{GameController.CurrentPlayer.ID} won!", true);

          GameController = null;
          GameState = GameProgramStatus.MainMenu;
        }
        else
        {
          throw new Exception("INVALID_STATE");
        }

      }
      catch (Exception error)
      {
        switch (error.Message)
        {
          case "INVALID_MOVE":
            Console.WriteLine($"`{command}` is not a valid move. To place a token on the board, type \"play [position]\". Alternatively, type \"help\" to see a list of command.");
            GameState = GameProgramStatus.Playing;
            break;
          case "INVALID_STATE":
            Console.WriteLine("Invalid state of app. Return to Main Menu.");
            break;
          case "INVALID":
            Console.WriteLine($"`{command}` is not a valid command. Type \"help\" to see a list of valid command.");
            break;
          default:
            Console.WriteLine(error.Message);
            Console.WriteLine($"Unable to process `{command}`. Type \"help\" to see a list of valid command.");
            break;
        }
      }
    }
  }
}