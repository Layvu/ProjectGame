namespace ProjectGame;

public class GenerationMapTests
{
    private MapGenerator _mapGenerator;

    [SetUp]
    public void Setup()
    {
        _mapGenerator = new MapGenerator(1000, 1000, 5);
    }

    [Test]
    public void GenerateMap_WithRandomEntities_CountMatches()
    {
        var testIterations = 10;
        for (var i = 0; i < testIterations; i++)
        {
            var maxCount = 20;
            var chestsCount = GetRandomCount(maxCount);
            var heartsCount = GetRandomCount(maxCount);
            var ratsbaneCount = GetRandomCount(maxCount);

            var generatedMap = _mapGenerator.GenerateMap(chestsCount, heartsCount, ratsbaneCount);

            var actualChestsCount = CountEntitiesOfType(generatedMap, GameCycleModel.EntityTypes.Chest);
            var actualHeartsCount = CountEntitiesOfType(generatedMap, GameCycleModel.EntityTypes.Heart);
            var actualRatsbaneCount = CountEntitiesOfType(generatedMap, GameCycleModel.EntityTypes.Ratsbane);

            Assert.That(actualChestsCount, Is.EqualTo(chestsCount));
            Assert.That(actualHeartsCount, Is.EqualTo(heartsCount));
            Assert.That(actualRatsbaneCount, Is.EqualTo(ratsbaneCount));
        }
    }

    private static int CountEntitiesOfType(GameCycleModel.EntityTypes[,] map, GameCycleModel.EntityTypes entityType)
    {
        var count = 0;
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++) 
            if (map[x, y] == entityType) count++;
        
        return count;
    }

    private static int GetRandomCount(int maxCount)
    {
        return new Random().Next(maxCount);
    }
}