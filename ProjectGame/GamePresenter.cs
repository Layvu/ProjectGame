namespace ProjectGame;

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
        _gameModel.NewMapCreated += NewMapCreated;
        
        _gameView.PlayerMovesChanged += ViewModelMovePlayer;
        _gameView.CycleFinished += ViewModelUpdate;

        _gameModel.Initialize();
    }

    private void ViewModelMovePlayer(object sender, AllMovesEventArgs e)
    {
        _gameModel.ChangesPlayerMoves(e.Direction);
    }

    private void ViewModelUpdate(object sender, AllMovesEventArgs e)
    {
        _gameModel.UpdateLogic(e.gameTime);
    }
    
    private void ModelViewUpdate(object sender, GameEventArgs e)
    {
        _gameView.LoadRenderingParameters(e.Entities, e.VisualShift, e.GameState);
    }
    
    private void NewMapCreated(object sender, GameEventArgs e)
    {
        _gameView.LoadNewMap(e.Entities, e.VisualShift, e.GameState);
    }
}