using System;
using System.Numerics;

namespace ProjectGame;

public class GameCycleModel : IGameModel
{
    public event EventHandler<AllGameEventArgs> Updated;
    private Vector2 _playerPosition = new(100, 100);
    
    public void MovePlayer(IGameModel.Direction dir)
    {
    }
    
    public void UpdateLogic()
    {
        _playerPosition += new Vector2(1, 0);
        Updated.Invoke(this, new AllGameEventArgs { PlayerPosition = _playerPosition });
    }
}