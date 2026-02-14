public class LevelData
{
    public int Level;
    public int TargetScore; // Сколько очков нужно набрать
    public int MaxMoves;

    public LevelData(int level)
    {
        Level = level;
        TargetScore = level * 1000;
        MaxMoves = 12 + (level * 2);
    }
}