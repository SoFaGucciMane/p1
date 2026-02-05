using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class CellData
    {
    public enum CellType // Перечесление ячеек.
    {
        Blank = 0,
        Blue = 1,
        Brown = 2,
        Green = 3,
        Lavender = 4,
        Yellow = 5,
    }

    public CellType cellType; // Хранит тип данной ячейки
    public Points point; // Хранит позицию

    private Cell _cell;
    public CellData(CellType cellType, Points point) 
    {
        this.cellType = cellType;
        this.point = point;

    }
}

