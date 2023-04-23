using System;
using System.Numerics;

namespace ProjectGame;

public class GameCycleModel : IGameModel
{
    public event EventHandler<AllGameEventArgs> Updated;
    private Vector2 _playerPosition = new(100, 100);
    
    public void MovesPlayer(IGameModel.Direction[] directions)
    {
        var moveVector = Vector2.Zero;

        foreach (var dir in directions)
        {
            switch (dir)
            {
                case IGameModel.Direction.Up:
                    moveVector += new Vector2(0, -1);
                    break;
                case IGameModel.Direction.Down:
                    moveVector += new Vector2(0, 1);
                    break;
                case IGameModel.Direction.Right:
                    moveVector += new Vector2(1, 0);
                    break;
                case IGameModel.Direction.Left:
                    moveVector += new Vector2(-1, 0);
                    break;
            }
        }

        _playerPosition += moveVector;
    }
    
    public void UpdateLogic()
    {
        Updated.Invoke(this, new AllGameEventArgs { PlayerPosition = _playerPosition });
    }
}