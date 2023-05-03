using Microsoft.Xna.Framework;

namespace ProjectGame;

public interface ISolid
{
    RectangleCollider Collider { get; set; }
    void MoveCollider(Vector2 newPosition);
}