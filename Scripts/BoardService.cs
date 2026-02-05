

using StaticData;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using Random = UnityEngine.Random;


public class BoardService : MonoBehaviour // В BoardService мы будет регистрировать наши элементы по сетке.
{
    [SerializeField] RectTransform _boardRect;
    [SerializeField] Cell _cellPrefab;
    [SerializeField] private Sprite[] _cellSprites; // Массив спрайтов для ячеек
    private void Start()
    {
        for (int x = 0; x < Config.BoardWith; x++)  // Заполнение самих ячеек.
        {
            for (int y = 0; y < Config.BoardHeight ; y++)
            {
                var cell = InstantiateCell();
                var point = new Points(x, y);
                cell.rect.anchoredPosition = GetBoardPositionsFromPoint(point);
                var cellType = GetRandomCellType();
                cell.Initialize(new CellData(cellType, point ), _cellSprites[(int)(cellType - 1)]);
            }
        }
    }

    private CellData.CellType GetRandomCellType()
      =>(CellData.CellType)( Random.Range(0, _cellSprites.Length) + 1);
    

    private Cell InstantiateCell() => Instantiate(_cellPrefab,_boardRect);
    private Vector2 GetBoardPositionsFromPoint(Points point)
    {

        return new Vector2(
            Config.PinceSize / 2 + Config.PinceSize * point.x
            ,
            -Config.PinceSize / 2 - Config.PinceSize * point.y
            );
    }
}
