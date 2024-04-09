namespace BoardGame;

public class Utils
{
  public static string GetScreen(GameProgramStatus state)
  {
    switch (state)
    {
      case GameProgramStatus.MainMenu:
        return "MAIN MENU";
      case GameProgramStatus.Playing:
        return "GAME";
      case GameProgramStatus.SavedFiles:
        return "SAVED FILES";
      case GameProgramStatus.SetupGame:
        return "NEW GAME";
      default:
        return "X";
    }
  }
}
