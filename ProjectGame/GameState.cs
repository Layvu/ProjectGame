namespace ProjectGame;

public class GameState
{
    public GameState(int chestsTotalCount)
    {
        ChestsTotalCount = chestsTotalCount;
    }
    
    public int PlayerHeartsCount { get; private set; } 
    public int ChestsCollectedCount { get; private set; } 
    public int ChestsTotalCount { get; }
    
    public void Update(int playerChestBar, int playerHealthBar)
    {
        PlayerHeartsCount = playerHealthBar;
        ChestsCollectedCount = playerChestBar;
    }
}