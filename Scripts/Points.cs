

using System.Drawing;
using System.Numerics;
using JetBrains.Annotations;

[System.Serializable]
public class Points
{
    public int x;
    public int y;

    public Points(int newX, int newY)
    {
        x = newX;
        y = newY;


    }
    public void Myltiply(int value) // Нужен для иницилизации координатов в сетку с блоками.
    {
        x *= value;
        y *= value;
    }

    public void Add(Points point)
    {
        x += point.x;
        y += point.y;
    }

    public bool Equals(Points point) // Поверка совпадения координат и точек.    
    {
        return x == point.x && y == point.y; 
    }

    public Vector2 ToVector() 
    {
        return new Vector2(x,y);
    }
    public static Points FromVector(Vector2 vector)
    => new((int)vector.X, (int)vector.Y);

    public static Points FromVector(Vector3 vector)
    => new((int)vector.X, (int)vector.Y);

    public static Points Myltiply(Points point, int value) // Умножение поинтов
    => new(point.x * value, point.y * value);

    public static Points Add(Points point1, Points point2) // Сложение поинтов
    => new(point1.x + point2.x, point1.y + point2.y);

    public static Points Clone(Points point)
        => new(point.x, point.y);

    public static Points zero = new(0,0);

    public static Points up = new(0, 1);

    public static Points down = new(1, 0);

    public static Points left = new(-1, 0);

    public static Points right = new(1, 0);
}
