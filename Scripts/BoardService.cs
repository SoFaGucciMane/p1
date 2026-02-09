using System.Collections;
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
    [SerializeField] private LevelManager _levelManager;

    private Cell[,] _board;

    private CellMover _cellMover;

    private readonly List<Cell> _updatingCells = new List<Cell>();
    private readonly List<CellFlip> _flippedCells = new List<CellFlip>();

    private bool _isProcessing; // Блокировка ввода во время каскада


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
        _cellMover.Update();

        var finishedUpdating = new List<Cell>();
        foreach (var cell in _updatingCells)
        {
            if (!cell.UpdateCell())
                finishedUpdating.Add(cell);
        }

        foreach (var cell in finishedUpdating)
        {
            var flip = GetFlip(cell);
            if (flip != null)
                _flippedCells.Remove(flip);
            _updatingCells.Remove(cell);
        }
    }

    public bool IsProcessing => _isProcessing; // Геттер для CellMover

    private CellFlip GetFlip(Cell cell)
    {
        foreach (var flip in _flippedCells)
        {
            if (flip.GetOtherCell(cell) != null)
                return flip;
        }
        return null;
    }

    public int GetCellTypeAtPoint(Points point)
    {
        var cell = GetCellAt(point.x, point.y);
        if (cell == null)
            return -1;
        return (int)cell.CellType;
    }

    public void FlipCells(Points firstPoint, Points secondPoint, bool main)
    {
        if (_isProcessing)
            return;

        if (_levelManager != null && _levelManager.IsLevelOver)
            return;

        if (GetCellTypeAtPoint(firstPoint) < 0)
            return;
        if (GetCellTypeAtPoint(secondPoint) < 0)
            return;

        // Предварительный свап — проверяем, будет ли матч
        SwapCells(firstPoint, secondPoint);

        var matches = FindAllMatches();

        if (matches.Count == 0)
        {
            // Матча нет — откатываем свап
            SwapCells(firstPoint, secondPoint);
            var cell = GetCellAt(firstPoint);
            ResetCell(cell);
            return;
        }

        // Матч есть — тратим ход
        if (_levelManager != null)
            _levelManager.UseMove();

        // Запускаем анимацию движения
        var firstCell = GetCellAt(firstPoint);
        var secondCell = GetCellAt(secondPoint);

        _flippedCells.Add(new CellFlip(firstCell, secondCell));

        ResetCell(firstCell);
        ResetCell(secondCell);

        // Запускаем каскад: удаление → падение → генерация → проверка
        StartCoroutine(ProcessCascade(matches));
    }

    private IEnumerator ProcessCascade(List<Cell> matches)
    {
        _isProcessing = true;

        while (matches.Count > 0)
        {
            // 1. Удаляем совпавшие ячейки с анимацией
            yield return StartCoroutine(DestroyMatchedCellsSmooth(matches));

            // 2. Падение ячеек вниз
            yield return StartCoroutine(DropCells());

            // 3. Генерация новых ячеек сверху
            yield return StartCoroutine(SpawnNewCells());

            // 4. Ждём пока все анимации закончатся
            yield return StartCoroutine(WaitForUpdatingCells());

            // 5. Проверяем каскадные матчи
            matches = FindAllMatches();
        }

        _isProcessing = false;
    }

    private IEnumerator DestroyMatchedCellsSmooth(List<Cell> matches)
    {
        yield return new WaitForSeconds(0.2f);

        // Сообщаем LevelManager о удалённых ячейках
        if (_levelManager != null)
            _levelManager.AddMatchedCells(matches.Count);

        // Запускаем анимацию уменьшения для всех ячеек одновременно
        var coroutines = new List<Coroutine>();
        foreach (var cell in matches)
        {
            int x = cell.Point.x;
            int y = cell.Point.y;
            _updatingCells.Remove(cell);
            _board[x, y] = null;

            if (cell != null && cell.gameObject != null)
                coroutines.Add(StartCoroutine(ShrinkAndDestroy(cell)));
        }

        // Ждём пока все уменьшатся
        foreach (var c in coroutines)
            yield return c;
    }

    private IEnumerator ShrinkAndDestroy(Cell cell)
    {
        var t = cell.transform;
        var startScale = t.localScale;
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (t == null) yield break;
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            t.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
            yield return null;
        }

        if (cell != null && cell.gameObject != null)
            Destroy(cell.gameObject);
    }

    private IEnumerator DropCells()
    {
        bool hasMoved = true;

        // Повторяем пока есть что двигать (ячейка может упасть на несколько позиций)
        while (hasMoved)
        {
            hasMoved = false;

            // Проходим снизу вверх по каждому столбцу
            for (int x = 0; x < Config.BoardWith; x++)
            {
                for (int y = Config.BoardHeight - 1; y >= 1; y--)
                {
                    if (_board[x, y] != null)
                        continue;

                    // Пустая ячейка — ищем ближайшую непустую сверху
                    for (int above = y - 1; above >= 0; above--)
                    {
                        if (_board[x, above] != null)
                        {
                            // Двигаем ячейку вниз
                            var cell = _board[x, above];
                            _board[x, y] = cell;
                            _board[x, above] = null;

                            cell.SetPoint(new Points(x, y));
                            ResetCell(cell);

                            hasMoved = true;
                            break;
                        }
                    }
                }
            }

            if (hasMoved)
                yield return new WaitForSeconds(0.05f); // Маленькая пауза между шагами падения
        }

        // Ждём пока все ячейки доедут
        yield return StartCoroutine(WaitForUpdatingCells());
    }

    private IEnumerator SpawnNewCells()
    {
        // Проходим по столбцам сверху вниз, заполняем пустоты
        for (int x = 0; x < Config.BoardWith; x++)
        {
            for (int y = 0; y < Config.BoardHeight; y++)
            {
                if (_board[x, y] == null)
                {
                    CreateCellAt(x, y, _cellMover);

                    var cell = _board[x, y];

                    // Стартовая позиция — выше доски, чтобы ячейка "упала" сверху
                    var spawnPoint = new Points(x, -1);
                    cell.rect.anchoredPosition = GetBoardPositionFromPoint(spawnPoint);
                    cell.transform.localScale = Vector3.one;

                    ResetCell(cell); // Запускаем анимацию движения к целевой позиции
                }
            }
        }

        // Ждём пока все новые ячейки доедут
        yield return StartCoroutine(WaitForUpdatingCells());
    }

    private IEnumerator WaitForUpdatingCells()
    {
        while (_updatingCells.Count > 0)
            yield return null;
    }

    private void SwapCells(Points firstPoint, Points secondPoint)
    {
        var firstCell = GetCellAt(firstPoint);
        var secondCell = GetCellAt(secondPoint);

        _board[firstPoint.x, firstPoint.y] = secondCell;
        _board[secondPoint.x, secondPoint.y] = firstCell;

        firstCell.SetPoint(secondPoint);
        secondCell.SetPoint(firstPoint);
    }

    public void ResetCell(Cell cell)
    {
        cell.ResetPosition();
        if (!_updatingCells.Contains(cell))
            _updatingCells.Add(cell);
    }

    // ===== ИНИЦИАЛИЗАЦИЯ =====

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

    // ===== СИСТЕМА МАТЧЕЙ =====

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