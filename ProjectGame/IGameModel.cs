using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class GameEventArgs : EventArgs //
{
    public Dictionary<int, IEntity> Entities { get; set; }    
    public Vector2 VisualShift { get; set; }
}

public interface IGameModel
{
    event EventHandler<GameEventArgs> Updated;
    
    int PlayerId { get; set; }
    Dictionary<int, IEntity> Entities { get; set; }

    public enum Direction : byte
    {
        Up,
        Down,
        Right,
        Left
    }
    
    void ChangesPlayerMoves(Direction[] dir);
    void UpdateLogic();
    void Initialize();
}