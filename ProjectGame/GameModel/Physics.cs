using Microsoft.Xna.Framework;

namespace ProjectGame;

public partial class GameCycleModel
{
    private const float GravityForce = 9.8f;

    private void UpdateGravity(IEntity currentEntity)
    {
        if (currentEntity.HasGravity)
        {
            var deltaTime = (float)CurrentGameTime.ElapsedGameTime.TotalSeconds;
            currentEntity.Moving += new Vector2(0, GravityForce) * deltaTime * currentEntity.Mass;
        }
    }

    public void HandleCollisions(ISolid solid1)
    {
        var collidingObjects = QuadTree.FindNearbyObjects(solid1);
        
        QuadTree.Remove(solid1);
        
        foreach (var solid2 in collidingObjects)
        {
            if (solid2 is Player) continue;

            var isCollided = true;
            if (solid1.Moving.Y < 0 & IsTouchingBottom(solid1, solid2))
            {
                solid1.Moving = new Vector2(solid1.Moving.X, 0);
            }
            else if (solid1.Moving.Y > 0 && IsTouchingTop(solid1, solid2))
            {
                if (solid1 is Player player) player.JumpTime = 0f;
                
                solid1.Moving = new Vector2(solid1.Moving.X, 0);
            }
            else if ((solid1.Moving.X > 0 && IsTouchingLeft(solid1, solid2)) ||
                     solid1.Moving.X < 0 & IsTouchingRight(solid1, solid2))
            {
                if (solid1 is Player player) player.JumpTime = 0f;
                
                solid1.Moving = new Vector2(0, solid1.Moving.Y);
            }
            else
            {
                isCollided = false;
            }

            if (isCollided)
            {
                if (solid2 is Chest) ChestCollided(solid1, solid2);
                if (solid2 is Heart) HeartCollided(solid1, solid2);
                if (solid2 is Ratsbane) RatsbaneCollided(solid1, solid2);
            }
        }
        
        QuadTree.Insert(solid1);
    }

    private void HeartCollided(ISolid solid1, ISolid heart)
    {
        Entities.Remove(heart.Id);
        QuadTree.Remove(heart);
        
        if (solid1 is Player player)
        {
            player.IncreaseHealthBar();
        }
    }
    
    private void ChestCollided(ISolid solid1, ISolid chest)
    {
        Entities.Remove(chest.Id);
        QuadTree.Remove(chest);
        
        if (solid1 is Player player)
        {
            player.IncreaseChestBar();
        }
    }
    
    private void RatsbaneCollided(ISolid solid1, ISolid ratsbane)
    {
        Entities.Remove(ratsbane.Id);
        QuadTree.Remove(ratsbane);
        
        if (solid1 is Player player)
        {
            player.DecreaseHealthBar();
        }
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