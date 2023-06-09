using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class AllMovesEventArgs : EventArgs
{
    public IGameModel.Direction[] Direction { get; set; }
    public GameTime gameTime { get; set; }
}

public interface IGameView
{
    event EventHandler<AllMovesEventArgs> PlayerMovesChanged;
    event EventHandler<AllMovesEventArgs> CycleFinished;
    event EventHandler ChangedGameState;
    event EventHandler StartNewGame;

    void LoadRenderingParameters(
        Dictionary<int, IEntity> entities, Vector2 visualShift, GameState currentGameState);
    void LoadNewMap(Dictionary<int, IEntity> entities, Vector2 visualShift, GameState gameState);
    void Run();
}