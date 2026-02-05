using System;

[Serializable]
public class ArrayLayout
{
    [Serializable]
    public struct RowData
    {
        public bool[] row;
    }

    public RowData[] rows = new RowData[8];

    // Конструктор для инициализации
    public ArrayLayout()
    {
        for (int i = 0; i < 8; i++)
        {
            rows[i].row = new bool[8];
        }
    }
}
