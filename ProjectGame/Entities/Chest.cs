using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class Chest : ISolid
{
    public Chest(Vector2 position)
    {
        Position = position;
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
        HasGravity = false;
    }
    
    public int ImageId { get; set; }
    public Vector2 Position { get; private set; }
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