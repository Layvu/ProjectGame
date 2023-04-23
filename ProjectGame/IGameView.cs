using System;
using System.Data.Common;
using System.Numerics;

namespace ProjectGame;

public class AllMovesEventArgs : EventArgs //
{
    public IGameModel.Direction[] Direction { get; set; }
}

public interface IGameView
{
    event EventHandler<AllMovesEventArgs> PlayerMoved;
    event EventHandler CycleFinished;
    
    void LoadRenderingParameters(Vector2 position);
    void Run();
}