using Microsoft.Xna.Framework;

namespace ProjectGame;

public partial class GameCycleModel
{
    public IEntity GenerateObject (EntityTypes name, int xTile, int yTile)
    {
        float currentX = xTile * TileSize;
        float currentY = yTile * TileSize;
        
        switch (name)
        {
            case EntityTypes.Player:
                return CreatePlayer(currentX, currentY, EntityTypes.Player);
            case EntityTypes.Wall:
                return CreateWall(currentX, currentY, EntityTypes.Wall);
            case EntityTypes.Chest:
                return CreateChest(currentX, currentY, EntityTypes.Chest);
            case EntityTypes.Heart:
                return CreateHeart(currentX, currentY, EntityTypes.Heart);
            case EntityTypes.Ratsbane:
                return CreateRatsbane(currentX, currentY, EntityTypes.Ratsbane);
            default:
                return null;
        }
    }

    private static Player CreatePlayer (float x, float y, EntityTypes spriteId)
    {
        var player = new Player(new Vector2(x, y), CurrentLevel.ChestsTotalCount) //
        {
            ImageId = (byte)spriteId,
            Moving = new Vector2(0, 0),
            Friction = 0.9f
        };
        return player;
    }

    private static Wall CreateWall(float x, float y, EntityTypes spriteId)
    {
        var wall = new Wall(new Vector2(x, y))
        {
            ImageId = (byte)spriteId,
        };
        return wall;
    }
    
    private static Chest CreateChest(float x, float y, EntityTypes spriteId)
    {
        var chest = new Chest(new Vector2(x, y))
        {
            ImageId = (byte)spriteId,
        };
        return chest;
    }
    
    private static Heart CreateHeart(float x, float y, EntityTypes spriteId)
    {
        var heart = new Heart(new Vector2(x, y))
        {
            ImageId = (byte)spriteId,
        };
        return heart;
    }
    
    private static Ratsbane CreateRatsbane(float x, float y, EntityTypes spriteId)
    {
        var ratsbane = new Ratsbane(new Vector2(x, y))
        {
            ImageId = (byte)spriteId,
        };
        return ratsbane;
    }
}