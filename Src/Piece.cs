namespace BoardGame;

public struct PlayToken(int row, int column, string playerID)
{
    public int Row { get; set; } = row;
    public int Column { get; set; } = column;
    public string PlayerID { get; set; } = playerID;
}