using System;
using System.Data.Common;
using System.Numerics;

namespace ProjectGame;

public class AllGameEventArgs : EventArgs //
{
    public Vector2 PlayerPosition { get; set; }
}

public interface IGameModel
{
    event EventHandler<AllGameEventArgs> Updated;

    public enum Direction : byte
    {
        Up,
        Down,
        Right,
        Left
    }
    
    void MovePlayer(Direction dir);
    void UpdateLogic();
}