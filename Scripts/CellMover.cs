using System;
using System.Drawing;
using StaticData;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CellMover
{
    private Points _newPoint;
    private Cell _movingCell;
    private Vector2 _startMousePosition;
    BoardService _boardService;

    public CellMover(BoardService boardService)
    {
        _boardService = boardService;
    }

    public void Update()
    {
        if (_movingCell == null)
            return;

        var mousePosition = (Vector2)Input.mousePosition - _startMousePosition;
        var absouteDirection = new Vector2(Mathf.Abs(mousePosition.x), Mathf.Abs(mousePosition.y));

        _newPoint = Points.Clone(_movingCell.Point);
        var addPoint = Points.zero;

        if (mousePosition.magnitude > Config.PinceSize / 4)
        {
            if (absouteDirection.x > absouteDirection.y)
                addPoint = new Points(mousePosition.x > 0 ? 1 : -1, 0);
            else
                addPoint = new Points(0, mousePosition.y > 0 ? -1 : 1);
        }

        _newPoint.Add(addPoint);

        // Проверяем, что целевая позиция внутри доски и не пустая
        var targetCell = _boardService.GetCellAt(_newPoint);
        bool validTarget = targetCell != null && !_newPoint.Equals(_movingCell.Point);

        var position = BoardService.GetBoardPositionFromPoint(_movingCell.Point);
        if (validTarget)
        {
            var visualOffset = new Vector2(addPoint.x, -addPoint.y) * (Config.PinceSize / 2);
            position += visualOffset;
        }
        else
        {
            // Невалидная цель — сбрасываем newPoint обратно
            _newPoint = Points.Clone(_movingCell.Point);
        }

        _movingCell.MovePosition(position);
    }

    public void MoveCell(Cell cell)
    {
        if (_movingCell != null)
            return;
        _movingCell = cell;
        _startMousePosition = Input.mousePosition;
    }

    public void MoveDorp()
    {
        if (_movingCell == null)
            return;

        if (!_newPoint.Equals(_movingCell.Point))
        {
            _boardService.FlipCells(_movingCell.Point, _newPoint, true);
        }
        else
        {
            _boardService.ResetCell(_movingCell);
        }

        _movingCell = null;
    }
}