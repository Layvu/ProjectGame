using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class GameEventArgs : EventArgs
{
    public Dictionary<int, IEntity> Entities { get; set; }    
    public Vector2 VisualShift { get; set; }
    public GameState GameState { get; set; }
}

public interface IGameModel
{
    event EventHandler<GameEventArgs> NewMapCreated;
    event EventHandler<GameEventArgs> Updated;

    public enum Direction : byte
    {
        Up,
        Right,
        Left
    }
    
    void ChangesPlayerMoves(Direction[] dir);
    void UpdateLogic(GameTime gameTime);
    void Initialize();
    void ChangeGameState();
    void StartNewGame();
}