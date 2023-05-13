using Microsoft.Xna.Framework;

namespace ProjectGame;

public partial class GameCycleModel
{
    private IEntity GenerateObject (EntityTypes name, int xTile, int yTile)
    {
        float currentX = xTile * _tileSize;
        float currentY = yTile * _tileSize;
        
        switch (name)
        {
            case EntityTypes.Player:
                return CreatePlayer(currentX, currentY, EntityTypes.Player);
            case EntityTypes.Wall:
                return CreateWall(currentX, currentY, EntityTypes.Wall);
            default:
                return null;
        }
    }

    private static Player CreatePlayer (float x, float y, EntityTypes spriteId)
    {
        var player = new Player(new Vector2(x, y)) //
        {
            ImageId = (byte)spriteId,
            Moving = new Vector2(0, 0),
            Friction = 0.9f
        };
        return player;
    }

    private static Wall CreateWall(float x, float y, EntityTypes spriteId)
    {
        var wall = new Wall(new Vector2(x, y)) //
        {
            ImageId = (byte)spriteId,
            Moving = new Vector2(0, 0)
        };
        return wall;
    }
}