using System.Collections.Generic;

public class MatchMachine // Данная функция, будет проходить по всей доске и находить match готовые, чтобы предотвродить их совместое появлнеие заранее
{
    private readonly BoardService _boardService; // Сылка на скрипт, который отвечает за регенираюцию ячеек

    public MatchMachine(BoardService boardService) // Создаем конструтор, чтобы матч машин могла получать данные о сетке и ячейках, координатах.
    {
        _boardService = boardService;
    }

    public List<Points> GetMathedPoins(Points point, bool main) // Будет проверять схему с начальной точки и выписвать уже где есть матчи. 
    {
        return null;
    }
}

