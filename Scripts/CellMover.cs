using System;
using System.Drawing;
using StaticData;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CellMover
{

    private Points _newPoint;
    private Cell _movingCell; // Ячейка которую мы передвигаем
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
        var absouteDirection = new Vector2(Mathf.Abs(mousePosition.x), Mathf.Abs(mousePosition.y)); // Для проверки, куда больше двигается наша мышь, влево или вправо

        _newPoint = Points.Clone(_movingCell.Point); // Кланируем позицию мувинг села, тот целл который мы двмгаем
        var addPoint = Points.zero; // На сколько должен сдвинуаться поинт.

        if (mousePosition.magnitude > Config.PinceSize / 4)
        {
            if (absouteDirection.x > absouteDirection.y) // Проекра, движимся мы по горизонтали или вертикали
                addPoint = new Points(mousePosition.x > 0 ? 1 : -1, 0); // Проверяем на сколько мы двигаем поинт либо на одну позицию справа либо на одну позицию слева
            else
                addPoint = new Points(0, mousePosition.y > 0 ? 1 : -1);
        }

        _newPoint.Add(addPoint);

        var position = BoardService.GetBoardPositionFromPoint(_movingCell.Point);

        if (!_newPoint.Equals(_movingCell.Point))
            position += Points.Multiply(addPoint, Config.PinceSize / 2).ToVector();

        _movingCell.MovePosition(position);
    }





    public void MoveCell(Cell cell) // Когда мы зажимаем на ячейку, то запускается данный метод и становиться равной мувСел
    {
        if (_movingCell != null) // Проверка, если мы уже двигаем какую то ячейку, то повторяем запрос
            return;
        _movingCell = cell; // Передаем данные с ячейки в мовинг селл
        _startMousePosition = Input.mousePosition;
    }

    public void MoveDorp()
    {
        if (_movingCell == null) // Проверка
            return ;
        _boardService.ResetCell(_movingCell);
        _movingCell = null;
    }
}


