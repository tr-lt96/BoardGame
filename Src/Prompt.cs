namespace BoardGame;

public class Prompt
{
  static void PromptGenerate(string message, GameProgramStatus state, bool nextLine = false)
  {
    string currentScreen = Utils.GetScreen(state);
    if (nextLine)
    {
      Console.WriteLine($"[{currentScreen}] {message} ");
    }
    else
    {
      Console.Write($"[{currentScreen}] {message} > ");
    }
  }
  public static void MainMenu(string message, bool nextLine = false)
  {
    PromptGenerate(message, GameProgramStatus.MainMenu, nextLine);

  }
  public static void Playing(string message, bool nextLine = false)
  {
    PromptGenerate(message, GameProgramStatus.Playing, nextLine);
  }

  public static void Setup(string message, bool nextLine = false)
  {
    PromptGenerate(message, GameProgramStatus.SetupGame, nextLine);
  }
}