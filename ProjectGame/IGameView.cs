using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class AllMovesEventArgs : EventArgs //
{
    public IGameModel.Direction[] Direction { get; set; }
}

public interface IGameView
{
    event EventHandler<AllMovesEventArgs> PlayerMovesChanged;
    event EventHandler CycleFinished;
    
    void LoadRenderingParameters(Dictionary<int, IEntity> entities, Vector2 visualShift);
    void Run();
}