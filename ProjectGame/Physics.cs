using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public partial class GameCycleModel
{
    private const float gravity = 9.8f;
    private void UpdateGravity(int entityId)
    {
        var currentEntity = Entities[entityId];
        if (currentEntity.HasGravity)
        {
            var deltaTime = (float)CurrentGameTime.ElapsedGameTime.TotalSeconds;
            currentEntity.Moving += new Vector2(0, gravity) * deltaTime * currentEntity.Mass;
        }
    }


    private void HandleCollisions(Dictionary<int, Vector2> positionsBeforeUpdate)
    {
        var firstEntityBefore = positionsBeforeUpdate[PlayerId];
        var player = Entities[PlayerId] as Player;
        
        var collidingObjects = _quadTree.FindCollisions(player);
        foreach (var entity2 in collidingObjects)
        {
            if (PlayerId == entity2.Id) continue;
            
            var secondEntity = Entities[entity2.Id];
            if (secondEntity is ISolid solid2)
            {
                if (RectangleCollider.IsCollided(player.Collider, solid2.Collider))
                {
                    player.JumpTime = 0f;

                    _quadTree.Remove(player);

                    if (firstEntityBefore != player.Position)
                    {
                        var intersects = Rectangle.Intersect(player.Collider.Boundary, solid2.Collider.Boundary);
                        if (intersects.Width > intersects.Height)
                            player.Move(new Vector2(player.Position.X, firstEntityBefore.Y));
                        else if (intersects.Width < intersects.Height)
                            player.Move(new Vector2(firstEntityBefore.X, player.Position.Y)); 
                    }
                    
                    _quadTree.Insert(player);
                }
            }
        }
    }
}