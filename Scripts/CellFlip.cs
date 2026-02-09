public class CellFlip // Скрипт будет хранить в себе две ячейки и обрабатывать их смену
{
    private readonly Cell _firstCell;
    private readonly Cell _secondCell;

    public CellFlip(Cell firstCell, Cell secondCell)
    {
        _firstCell = firstCell;
        _secondCell = secondCell;
    }

    public Cell GetOtherCell(Cell cell)
    {
        if (cell == _firstCell)
            return _secondCell;
        if (cell == _secondCell)
            return _firstCell;
        return null;
    }
}