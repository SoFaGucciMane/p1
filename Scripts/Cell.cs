using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform rect;
    [SerializeField] private Image _image;
    [SerializeField] private float _moveSpeed = 10;

    private CellData _cellData;

    public Points Point => _cellData.point;
    public CellData.CellType CellType => _cellData.cellType;

    CellMover _cellMover;
    private Vector2 _position;
    private bool _isUpdating;

    public bool UpdateCell()
    {
        if (Vector3.Distance(rect.anchoredPosition, _position) > 1)
        {
            MovePosition(_position);
            _isUpdating = true;
        }
        else
        {
            rect.anchoredPosition = _position;
            _isUpdating = false;
        }
        return _isUpdating;
    }

    public void Initialize(CellData cellData, Sprite sprite, CellMover cellMover)
    {
        _cellData = cellData;
        _image.sprite = sprite;
        UpdateName();
        _cellMover = cellMover;
        ResetPosition();
    }

    public void SetPoint(Points point) // ќбновл€ет позицию €чейки в данных (при свапе)
    {
        _cellData.point = point;
        UpdateName();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _cellMover.MoveCell(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _cellMover.MoveDorp();
    }

    private void UpdateName()
        => transform.name = $"Cell[{Point.x}, {Point.y}]";

    public void MovePosition(Vector2 position)
        => rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, position, Time.deltaTime * _moveSpeed); // »справлено: * вместо +

    public void ResetPosition()
        => _position = BoardService.GetBoardPositionFromPoint(Point);
}