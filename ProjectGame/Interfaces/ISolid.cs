using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public interface ISolid : IEntity
{
    RectangleCollider Collider { get; set; }
    void MoveCollider(Vector2 newPosition);
    void TryUpdate(Dictionary<int, IEntity> currentEntities);
}