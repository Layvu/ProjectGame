using Microsoft.Xna.Framework;

namespace ProjectGame;

public class RectangleCollider
{
    public Rectangle Boundary { get; set; }

    public RectangleCollider(int x, int y, int width, int height)
    {
        Boundary = new Rectangle(x, y, width, height);
    }
    
    public static bool IsCollided(RectangleCollider firstRectangle, RectangleCollider secondRectangle)
    {
        return firstRectangle.Boundary.Intersects(secondRectangle.Boundary);
    }
}