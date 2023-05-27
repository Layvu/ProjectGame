using System;

namespace ProjectGame;

public class Level
{
    public Level(int screenWidth, int screenHeight, int tileSize)
    {
        LevelNumber = 1;
        
        ChestsTotalCount = 3;
        HeartsCount = 3;
        RatsbaneCount = 15;

        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _tileSize = tileSize;
        
        MapWidth = screenWidth * 2 / tileSize;
        MapHeight = screenHeight * 2 / tileSize;

        CurrentMap = new MapGenerator(MapWidth, MapHeight, 4)
            .GenerateMap(ChestsTotalCount, HeartsCount, RatsbaneCount);
    }

    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly int _tileSize;
    
    public int ChestsTotalCount { get; set; }
    public int HeartsCount { get; set; }
    public int RatsbaneCount { get; set; }
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public int LevelNumber { get; set; }
    public GameCycleModel.EntityTypes[,] CurrentMap { get; set; }

    public void NewLevel()
    {
        LevelNumber++;
        
        MapWidth += _screenWidth / _tileSize;
        MapHeight += _screenHeight / _tileSize;

        ChestsTotalCount = (int)Math.Ceiling(ChestsTotalCount * 1.5);
        HeartsCount = (int)Math.Ceiling(HeartsCount * 1.5);
        RatsbaneCount = (int)Math.Ceiling(RatsbaneCount * 1.5);
        
        CurrentMap = new MapGenerator(MapWidth, MapHeight, 4)
            .GenerateMap(ChestsTotalCount, HeartsCount, RatsbaneCount);
    }
}