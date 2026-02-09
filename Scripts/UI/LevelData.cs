public class LevelData
{
    public int Level;
    public int GoalCells;  // —колько €чеек нужно собрать
    public int MaxMoves;   // —колько ходов дано

    public LevelData(int level)
    {
        Level = level;
        GoalCells = 25 + (level * 5);
        MaxMoves = 12 + (level * 2);
    }
}