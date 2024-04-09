namespace BoardGame;

public class Record
{
  public List<PlayToken> Actions { get; set; }
  public int Iterator;

  public Record()
  {
    Actions = [];
    Iterator = 0;
  }
  public int GetPrevState(int currentPlayerTurn)
  {
    if (Iterator == currentPlayerTurn)
    {
      throw new Exception("Cannot undo further.");
    }

    Iterator = Math.Max(Iterator - 2, 0);
    return Iterator;
  }

  public int GetNextState()
  {
    if (Iterator == Actions.Count)
    {
      throw new Exception("Cannot redo - this is the latest game state.");
    }

    Iterator = Math.Min(Iterator + 2, Actions.Count);
    return Iterator;
  }

  public void Update(PlayToken token)
  {
    if (Iterator < Actions.Count)
    {
      Actions = Actions[..Iterator];
    }

    Actions.Add(token);
    Iterator = Actions.Count;
  }
}
