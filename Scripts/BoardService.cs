using System.Collections.Generic;
using System.Drawing;
using StaticData;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardService : MonoBehaviour
{
    [SerializeField] RectTransform _boardRect;
    [SerializeField] Cell _cellPrefab;
    [SerializeField] private Sprite[] _cellSprites;

    private Cell[,] _board;

    private CellMover _cellMover;

    private readonly List<Cell> _updatingCells = new List<Cell>(); // —писок €чеек, которых мы должны обновл€ть
    private readonly List<CellFlip> _flippedCells = new List<CellFlip>(); // —писок €чеек, которых мы должны обновл€ть


    private void Awake()
    {
        _cellMover = new CellMover(this);
    }

    private void Start()
    {
        InitializeBoard();
        
    }
        private void Update()
        {
            _cellMover.Update();// «апускаем update в скрипте, потому что у него есть 

            var finishedUpdating = new List<Cell>();
            foreach (var cell in _updatingCells) // ѕеребиаем из существующих €чеек обновленные €чейки
            {
                if(!cell.UpdateCell())
                finishedUpdating.Add(cell);
            }
            foreach (var cell in finishedUpdating) 
            {
                var flip = GetFlip(cell); // ѕолучаем флип от той €чейки, по который мы проходимс€ / —¬я«№ ƒ¬”’ я„≈≈ 
                _flippedCells.Remove(flip); // ”дал€ем их из активынх €чееек
                _updatingCells.Remove(cell);
            }
        }

    private CellFlip GetFlip(Cell cell) // »щет где участвует данна€ пара €чеек. Ќаходит всю пару целиком.
    {
        foreach (var flip in _flippedCells) 
        {
            if(flip.GetOtherCell(cell) != null)
                return flip;
            return null;
        }
    }

    public void FlipCells(Points firstPoint, Points secondPoint, bool main) // ћетод, который будет запускать процесс смену €чеек
    {
        if (GetCellAt(firstPoint) >= 0)
            return;
        return;
    }

    public void ResetCell(Cell cell) // –ессетит позицию €чейки
    {
        cell.ResetPosition(); // ћетод, который обновл€ет позициию
        _updatingCells.Add(cell); // ƒобавление в список €чеек, которых нужно обновить
    }
    private void InitializeBoard()
    {
        _board = new Cell[Config.BoardWith, Config.BoardHeight];

        for (int x = 0; x < Config.BoardWith; x++)
        {
            for (int y = 0; y < Config.BoardHeight; y++)
            {
                CreateCellAt(x, y, _cellMover);
            }
        }

        while (HasMatches())
        {
            RemoveMatchesInstant();
            RefillBoard();
        }
    }

    private void CreateCellAt(int x, int y, CellMover cellMover)
    {
        var cell = Instantiate(_cellPrefab, _boardRect);
        var point = new Points(x, y);
        cell.rect.anchoredPosition = GetBoardPositionFromPoint(point);
        var cellType = GetRandomCellType();
        cell.Initialize(new CellData(cellType, point), _cellSprites[(int)(cellType - 1)], cellMover);

        _board[x, y] = cell;
    }

    public static Vector2 GetBoardPositionFromPoint(Points point)
    {
        return new Vector2(
            Config.PinceSize / 2 + Config.PinceSize * point.x,
            -Config.PinceSize / 2 - Config.PinceSize * point.y
        );
    }

    private CellData.CellType GetRandomCellType()
        => (CellData.CellType)(Random.Range(0, _cellSprites.Length) + 1);

    public Cell GetCellAt(int x, int y)
    {
        if (x < 0 || x >= Config.BoardWith || y < 0 || y >= Config.BoardHeight)
            return null;
        return _board[x, y];
    }

    public Cell GetCellAt(Points point) => GetCellAt(point.x, point.y);

    // ===== —»—“≈ћј ћј“„≈… =====

    public List<Cell> FindAllMatches()
    {
        List<Cell> matchedCells = new List<Cell>();

        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWith - 2; x++)
            {
                var match = GetMatchHorizontal(x, y);
                if (match.Count >= 3)
                {
                    foreach (var cell in match)
                    {
                        if (!matchedCells.Contains(cell))
                            matchedCells.Add(cell);
                    }
                }
            }
        }

        for (int x = 0; x < Config.BoardWith; x++)
        {
            for (int y = 0; y < Config.BoardHeight - 2; y++)
            {
                var match = GetMatchVertical(x, y);
                if (match.Count >= 3)
                {
                    foreach (var cell in match)
                    {
                        if (!matchedCells.Contains(cell))
                            matchedCells.Add(cell);
                    }
                }
            }
        }

        return matchedCells;
    }

    private List<Cell> GetMatchHorizontal(int startX, int y)
    {
        List<Cell> match = new List<Cell>();
        Cell startCell = GetCellAt(startX, y);

        if (startCell == null) return match;

        var targetType = startCell.CellType;

        for (int x = startX; x < Config.BoardWith; x++)
        {
            Cell cell = GetCellAt(x, y);
            if (cell != null && cell.CellType == targetType)
                match.Add(cell);
            else
                break;
        }

        return match;
    }

    private List<Cell> GetMatchVertical(int x, int startY)
    {
        List<Cell> match = new List<Cell>();
        Cell startCell = GetCellAt(x, startY);

        if (startCell == null) return match;

        var targetType = startCell.CellType;

        for (int y = startY; y < Config.BoardHeight; y++)
        {
            Cell cell = GetCellAt(x, y);
            if (cell != null && cell.CellType == targetType)
                match.Add(cell);
            else
                break;
        }

        return match;
    }

    public bool HasMatches() => FindAllMatches().Count > 0;

    private void RemoveMatchesInstant()
    {
        var matches = FindAllMatches();
        foreach (var cell in matches)
        {
            int x = cell.Point.x;
            int y = cell.Point.y;
            Destroy(cell.gameObject);
            _board[x, y] = null;
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < Config.BoardWith; x++)
        {
            for (int y = 0; y < Config.BoardHeight; y++)
            {
                if (_board[x, y] == null)
                {
                    CreateCellAt(x, y, _cellMover);
                }
            }
        }
    }


}