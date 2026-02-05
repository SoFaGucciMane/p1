using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Cell : MonoBehaviour
{


    public RectTransform rect; // »нформци€ о каждой €чейки хранитьс€ тут.

    [SerializeField] private UnityEngine.UI.Image _image;

    private CellData _cellData;

    public Points Points => _cellData.point; // ƒл€ почени€ данных координат €чеек с CellData
    public CellData.CellType CellType => _cellData.cellType; // ƒл€ почени€ данных €чеек с CellData
    public void Initialize(CellData cellData,  Sprite sprite) // »нициализуем и передаем данные о каждой €чейки
    {
        _cellData = cellData;

        _image.sprite = sprite;
        UpdateName();
    }

    private void UpdateName() // —оздаем функцию, дл€ иницилизации в какой €чейке наход€тьс€ спрайты
    {
        transform.name = $"Cell[{Points.x}, {Points.y}]";
    }
}
