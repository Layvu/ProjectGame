namespace ProjectGame;

public enum State : byte
{
    Game, 
    Menu
}

public class GameState
{
    public GameState()
    {
        State = State.Menu;
    }

    public GameState(int chestsTotalCount, int currentLevel)
    {
        ChestsTotalCount = chestsTotalCount;
        LevelNumber = currentLevel;
    }

    public int PlayerHeartsCount { get; private set; } 
    public int ChestsCollectedCount { get; private set; } 
    public int ChestsTotalCount { get; }
    public int LevelNumber { get; }
    public State State { get; set; }

    public void Update(int playerChestBar, int playerHealthBar)
    {
        PlayerHeartsCount = playerHealthBar;
        ChestsCollectedCount = playerChestBar;
    }
}