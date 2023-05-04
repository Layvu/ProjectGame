using Microsoft.Xna.Framework;

namespace ProjectGame;

public class Player : IEntity, ISolid
{
    public Player(Vector2 position)
    {
        Position = position;
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 39, 50);
    }

    public int ImageId { get; set; }
    public Vector2 Position { get; private set; }
    public Vector2 Moving { get; set; }
    public float Friction { get; set; }
    public RectangleCollider Collider { get; set; }
    
    public int Id { get; set; } //

    public void Update()
    {
        Position += Moving;
        Moving *= Friction;

        Move(Position + Moving);
    }

    public void MoveCollider(Vector2 newPosition)
    {
        Collider = new RectangleCollider((int)newPosition.X, (int)newPosition.Y, 39, 50);
    }
    
    public void Move(Vector2 newPosition)
    {
        Position = newPosition;
        MoveCollider(Position);
    }
}