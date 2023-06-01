using Microsoft.Xna.Framework;

namespace ProjectGame;

public class GenerateObjectsTests
{
    private GameCycleModel _gameCycleModel;

    [SetUp]
    public void Setup()
    {
        _gameCycleModel = new GameCycleModel();
    }

    [Test]
    public void GenerateObject_CreatesPlayer()
    {
        const int xTile = 2;
        const int yTile = 3;

        var result = _gameCycleModel.GenerateObject(GameCycleModel.EntityTypes.Player, xTile, yTile);
        
        Assert.That(result, Is.InstanceOf<Player>());
        Assert.That(result.Position, Is.EqualTo(new Vector2(xTile * GameCycleModel.TileSize, yTile * GameCycleModel.TileSize)));
        Assert.That(result.ImageId, Is.EqualTo((byte)GameCycleModel.EntityTypes.Player));
        Assert.That(result.Moving, Is.EqualTo(new Vector2(0, 0)));
    }

    [Test]
    public void GenerateObject_CreatesWall()
    {
        const int xTile = 2;
        const int yTile = 3;

        var result = _gameCycleModel.GenerateObject(GameCycleModel.EntityTypes.Wall, xTile, yTile);
        
        Assert.That(result, Is.InstanceOf<Wall>());
        Assert.That(result.Position, Is.EqualTo(new Vector2(xTile * GameCycleModel.TileSize, yTile * GameCycleModel.TileSize)));
        Assert.That(result.ImageId, Is.EqualTo((byte)GameCycleModel.EntityTypes.Wall));
    }

    [Test]
    public void GenerateObject_CreatesChest()
    {
        const int xTile = 2;
        const int yTile = 3;

        var result = _gameCycleModel.GenerateObject(GameCycleModel.EntityTypes.Chest, xTile, yTile);

        Assert.That(result, Is.InstanceOf<Chest>());
        Assert.That(result.Position, Is.EqualTo(new Vector2(xTile * GameCycleModel.TileSize, yTile * GameCycleModel.TileSize)));
        Assert.That(result.ImageId, Is.EqualTo((byte)GameCycleModel.EntityTypes.Chest));
    }

    [Test]
    public void GenerateObject_CreatesHeart()
    {
        const int xTile = 2;
        const int yTile = 3;
        
        var result = _gameCycleModel.GenerateObject(GameCycleModel.EntityTypes.Heart, xTile, yTile);

        Assert.IsInstanceOf<Heart>(result);
        Assert.That(result.Position, Is.EqualTo(new Vector2(xTile * GameCycleModel.TileSize, yTile * GameCycleModel.TileSize)));
        Assert.That(result.ImageId, Is.EqualTo((byte)GameCycleModel.EntityTypes.Heart));
    }

    [Test]
    public void GenerateObject_CreatesRatsbane()
    {
        const int xTile = 2;
        const int yTile = 3;

        var result = _gameCycleModel.GenerateObject(GameCycleModel.EntityTypes.Ratsbane, xTile, yTile);

        Assert.IsInstanceOf<Ratsbane>(result);
        Assert.That(result.Position, Is.EqualTo(new Vector2(xTile * GameCycleModel.TileSize, yTile * GameCycleModel.TileSize)));
        Assert.That(result.ImageId, Is.EqualTo((byte)GameCycleModel.EntityTypes.Ratsbane));
    }
}