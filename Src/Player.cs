﻿namespace BoardGame;

public abstract class Player
{
  public required string ID { get; set; }
  public virtual PlayToken Play()
  {
    return new PlayToken();
  }
  public virtual PlayToken Play(int col, int row = 1)
  {
    return new PlayToken { Column = col, Row = row, PlayerID = ID };
  }
}

public class PlayerHuman : Player
{
}

public class PlayerBot : Player
{
  Random random = new Random();
  public override PlayToken Play(int maxCol, int maxRow = 1)
  {
    int randCol = random.Next(1, maxCol + 1);
    int randRow = maxRow > 1 ? random.Next(1, maxRow + 1) : 1;
    // instead of asking for input, generate random value
    return new PlayToken { Column = randCol, Row = randRow, PlayerID = ID };
  }
}