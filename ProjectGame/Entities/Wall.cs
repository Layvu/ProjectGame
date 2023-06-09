using Microsoft.Xna.Framework;

namespace ProjectGame;

public class Wall : ISolid
{
    public Wall(Vector2 position)
    {
        Position = position;
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
        HasGravity = false;
        Mass = 10f;
    }
    
    public int ImageId { get; set; }
    public Vector2 Position { get; }
    public Vector2 Moving { get; set; }
    public RectangleCollider Collider { get; set; }
    public int Id { get; set; } 
    public float Mass { get; }
    public bool HasGravity { get; }
    
    public void MoveCollider(Vector2 newPosition)
    {
        Collider = new RectangleCollider((int)newPosition.X, (int)newPosition.Y, 50, 50);
    }
    public void Move(Vector2 pos) {}
    public void Update() {}
}
