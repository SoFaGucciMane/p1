using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public RectTransform rect;
    [SerializeField] private Image _image;

    private CellData _cellData;

    public Points Points => _cellData.point;
    public CellData.CellType CellType => _cellData.cellType;

    public void Initialize(CellData cellData, Sprite sprite)
    {
        _cellData = cellData;
        _image.sprite = sprite;
        UpdateName();
    }

    private void UpdateName()
    {
        transform.name = $"Cell[{Points.x}, {Points.y}]";
    }
}