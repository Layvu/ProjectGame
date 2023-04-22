using System;

namespace ProjectGame;

/// <summary>
/// Обрабатывает взаимодействие между моделью и представлением,
/// перенаправляя данные и обновления с одного компонента на другой
/// </summary>
public class GamePresenter
{
    private IGameModel _gameModel;
    private IGameView _gameView;

    public void LaunchGame()
    {
        _gameView.Run();
    }

    public GamePresenter(IGameView gameView, IGameModel gameModel)
    {
        _gameModel = gameModel;
        _gameView = gameView;

        _gameModel.Updated += ModelViewUpdate;
        
        _gameView.PlayerMoved += ViewModelMovePlayer;
        _gameView.CycleFinished += ViewModelUpdate;
    }

    private void ViewModelMovePlayer(object sender, AllMovesEventArgs e)
    {
        _gameModel.MovePlayer(e.Direction);
    }

    private void ViewModelUpdate(object sender, EventArgs e)
    {
        _gameModel.UpdateLogic();
    }
    
    private void ModelViewUpdate(object sender, AllGameEventArgs e)
    {
        _gameView.LoadRenderingParameters(e.PlayerPosition);
    }
}