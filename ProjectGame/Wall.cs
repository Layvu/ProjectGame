using Microsoft.Xna.Framework;

namespace ProjectGame;

public class Wall : IEntity, ISolid
{
    public Wall(Vector2 position)
    {
        Position = position;
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }
    
    public int ImageId { get; set; }
    public Vector2 Position { get; private set; }
    public Vector2 Moving { get; set; }
    public RectangleCollider Collider { get; set; }

    public int Id { get; set; } //

    public void Update()
    {
    }

    public void Move(Vector2 pos)
    {
    }

    public void MoveCollider(Vector2 newPosition)
    {
        Collider = new RectangleCollider((int)newPosition.X, (int)newPosition.Y, 50, 50);
    }
}
