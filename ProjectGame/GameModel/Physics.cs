using Microsoft.Xna.Framework;

namespace ProjectGame;

public partial class GameCycleModel
{
    private const float gravity = 9.8f;
    
    public static void UpdateGravity(IEntity currentEntity)
    {
        if (currentEntity.HasGravity)
        {
            const float gravity = 9.8f;
            var deltaTime = (float)CurrentGameTime.ElapsedGameTime.TotalSeconds;
            currentEntity.Moving += new Vector2(0, gravity) * deltaTime * currentEntity.Mass;
        }
    }

    public static void HandleCollisions(ISolid solid1)
    {
        var collidingObjects = _quadTree.FindNearbyObjects(solid1);
        
        _quadTree.Remove(solid1);
        
        foreach (var solid2 in collidingObjects)
        {
            var player = solid1 as Player;
            if (solid2 is Player) continue;

            if (solid1.Moving.Y < 0 & IsTouchingBottom(solid1, solid2))
            {
                solid1.Moving = new Vector2(solid1.Moving.X, 0);
            }
            
            if (solid1.Moving.Y > 0 && IsTouchingTop(solid1, solid2))
            {
                player.JumpTime = 0f;
                solid1.Moving = new Vector2(solid1.Moving.X, 0);
            }
            
            else if ((solid1.Moving.X > 0 && IsTouchingLeft(solid1, solid2)) ||
                solid1.Moving.X < 0 & IsTouchingRight(solid1, solid2))
            {
                player.JumpTime = 0f;
                solid1.Moving = new Vector2(0, solid1.Moving.Y);
            }
        }
        
        _quadTree.Insert(solid1);
    }

    private static bool IsTouchingLeft(ISolid solid1, ISolid solid2)
    {
        var boundary1 = solid1.Collider.Boundary;
        var boundary2 = solid2.Collider.Boundary;

        return boundary1.Right + solid1.Moving.X > boundary2.Left
               && boundary1.Left < boundary2.Left
               && boundary1.Bottom > boundary2.Top 
               && boundary1.Top < boundary2.Bottom;
    }

    private static bool IsTouchingRight(ISolid solid1, ISolid solid2)
    {
        var boundary1 = solid1.Collider.Boundary;
        var boundary2 = solid2.Collider.Boundary;

        return boundary1.Left + solid1.Moving.X < boundary2.Right
               && boundary1.Right > boundary2.Right
               && boundary1.Bottom > boundary2.Top 
               && boundary1.Top < boundary2.Bottom;
    }

    private static bool IsTouchingTop(ISolid solid1, ISolid solid2)
    {
        var boundary1 = solid1.Collider.Boundary;
        var boundary2 = solid2.Collider.Boundary;

        return boundary1.Bottom + solid1.Moving.Y > boundary2.Top
               && boundary1.Top < boundary2.Top
               && boundary1.Right > boundary2.Left
               && boundary1.Left < boundary2.Right;
    }

    private static bool IsTouchingBottom(ISolid solid1, ISolid solid2)
    {
        var boundary1 = solid1.Collider.Boundary;
        var boundary2 = solid2.Collider.Boundary;

        return boundary1.Top + solid1.Moving.Y < boundary2.Bottom
               && boundary1.Bottom > boundary2.Bottom
               && boundary1.Right > boundary2.Left 
               && boundary1.Left < boundary2.Right;
    }
}