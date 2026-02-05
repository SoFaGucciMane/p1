using UnityEngine;

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

    public void Multiply(int value)
    {
        x *= value;
        y *= value;
    }

    public void Add(Points point)
    {
        x += point.x;
        y += point.y;
    }

    public bool Equals(Points point)
    {
        return x == point.x && y == point.y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public static Points FromVector(Vector2 vector)
        => new Points((int)vector.x, (int)vector.y);

    public static Points FromVector(Vector3 vector)
        => new Points((int)vector.x, (int)vector.y);

    public static Points Multiply(Points point, int value)
        => new Points(point.x * value, point.y * value);

    public static Points Add(Points point1, Points point2)
        => new Points(point1.x + point2.x, point1.y + point2.y);

    public static Points Clone(Points point)
        => new Points(point.x, point.y);

    public static Points zero = new Points(0, 0);
    public static Points up = new Points(0, -1);
    public static Points down = new Points(0, 1);
    public static Points left = new Points(-1, 0);
    public static Points right = new Points(1, 0);
}