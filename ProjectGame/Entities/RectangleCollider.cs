using Microsoft.Xna.Framework;

namespace ProjectGame;

public class RectangleCollider
{
    public Rectangle Boundary { get; }

    public RectangleCollider(int x, int y, int width, int height)
    {
        Boundary = new Rectangle(x, y, width, height);
    }
}