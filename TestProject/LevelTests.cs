namespace ProjectGame;

public class LevelTests
{
    private Level _level;

    [SetUp]
    public void Setup()
    {
        _level = new Level(1000, 1000, 32);
    }

    [Test]
    public void NewLevel_IncreasesLevelNumber()
    {
        var initialLevelNumber = _level.LevelNumber;

        _level.NewLevel();

        Assert.AreEqual(initialLevelNumber + 1, _level.LevelNumber);
    }

    [Test]
    public void NewLevel_IncreasesMapSize()
    {
        var initialMapWidth = _level.MapWidth;
        var initialMapHeight = _level.MapHeight;

        _level.NewLevel();
        
        Assert.That(_level.MapWidth, Is.GreaterThan(initialMapWidth));
        Assert.That(_level.MapHeight, Is.GreaterThan(initialMapHeight));
    }

    [Test]
    public void NewLevel_IncreasesEntityCounts()
    {
        var initialChestsCount = _level.ChestsTotalCount;
        var initialHeartsCount = _level.HeartsCount;
        var initialRatsbaneCount = _level.RatsbaneCount;

        _level.NewLevel();
        
        Assert.That(_level.ChestsTotalCount, Is.GreaterThan(initialChestsCount));
        Assert.That(_level.HeartsCount, Is.GreaterThan(initialHeartsCount));
        Assert.That(_level.RatsbaneCount, Is.GreaterThan(initialRatsbaneCount));
    }

    [Test]
    public void RebuildLevel_GeneratesNewMap()
    {
        var initialMap = _level.CurrentMap;
        _level.RebuildLevel();
        Assert.That(_level.CurrentMap, Is.Not.SameAs(initialMap));
    }
}