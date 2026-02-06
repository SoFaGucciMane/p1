using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform rect;
    [SerializeField] private Image _image;
    [SerializeField]private float _moveSpeed = 10;

    private CellData _cellData;

    public Points Point => _cellData.point;
    public CellData.CellType CellType => _cellData.cellType;

    CellMover _cellMover; // Сылка на скрипт, который будет отвечать за события движения ячейки



    public void Initialize(CellData cellData, Sprite sprite, CellMover cellMover)
    {
        _cellData = cellData;
        _image.sprite = sprite;
        UpdateName();
        _cellMover = cellMover;
    }

    public void OnPointerDown(PointerEventData eventData ) // При нажатии запуститься
    {
        _cellMover.MoveCell(this);
    }

    public void OnPointerUp(PointerEventData eventData) // Когда отпуститят запустиься
    {
        _cellMover.MoveDorp();
    }

    private void UpdateName()
    {
        transform.name = $"Cell[{Point.x}, {Point.y}]";
    }
    public void MovePosition(Vector2 position) 
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, position, Time.deltaTime + _moveSpeed); // Плавное передвиженик к новой ячейки, от стартовой     позиции к новой позиции
    }
}