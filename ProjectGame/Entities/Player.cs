using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ProjectGame;

public class Player : ISolid
{
    public Player(Vector2 position, int chestCount)
    {
        Position = position;
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 39, 50);
        HasGravity = true;
        Mass = 2f;
        Speed = 0.7f;
        JumpMaxTime = 0.2f;
        JumpTime = 0f;
        JumpSpeed = 2f;
        _healthBar = 3;
        Win = false;
        Died = false;
        MaxChestCount = chestCount;
    }

    public int ImageId { get; set; }
    public Vector2 Position { get; private set; }
    public Vector2 Moving { get; set; }
    public float Friction { get; set; }
    public RectangleCollider Collider { get; set; }
    public int Id { get; set; }
    public bool HasGravity { get; }
    public float Mass { get; }
    public float JumpTime { get; set; }
    public float JumpMaxTime { get; set; }
    public float Speed { get; set; }
    public float JumpSpeed { get; set; }
    private int _healthBar { get; set; }
    private int _chestBar { get; set; }
    public bool Win { get; private set; }
    public bool Died { get; private set; }
    
    private readonly int MaxChestCount;

    public void IncreaseHealthBar()
    { 
        if (_healthBar < 3) _healthBar++;
    }
    
    public void DecreaseHealthBar()
    { 
        _healthBar--;
        if (_healthBar == 0) Died = true;
    }
    
    public void IncreaseChestBar()
    { 
        _chestBar++;
        if (_chestBar == MaxChestCount) Win = true;
    }

    public void AddJumpTime(float deltaTime)
    {
        if (JumpTime <= JumpMaxTime) JumpTime += deltaTime;
    }
    
    public void MoveCollider(Vector2 newPosition)
    {
        Collider = new RectangleCollider((int)newPosition.X, (int)newPosition.Y, 39, 50);
    }

    public void Update()
    {
        Position += Moving;
        Moving *= Friction;

        Move(Position + Moving);
    }

    public void TryUpdate(Dictionary<int, IEntity> CurrentEntities)
    {
        GameCycleModel.UpdateGravity(this);
        GameCycleModel.HandleCollisions(CurrentEntities[Id] as ISolid);
        Update();
    }

    public void Move(Vector2 newPosition)
    {
        Position = newPosition;
        MoveCollider(Position);
    }
}