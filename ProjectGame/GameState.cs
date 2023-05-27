namespace ProjectGame;

public class GameState
{
    public GameState(int chestsTotalCount, int currentLevel)
    {
        ChestsTotalCount = chestsTotalCount;
        LevelNumber = currentLevel;
    }
    
    public int PlayerHeartsCount { get; private set; } 
    public int ChestsCollectedCount { get; private set; } 
    public int ChestsTotalCount { get; }
    public int LevelNumber { get; }
    
    public void Update(int playerChestBar, int playerHealthBar)
    {
        PlayerHeartsCount = playerHealthBar;
        ChestsCollectedCount = playerChestBar;
    }
}