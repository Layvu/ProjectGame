using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectGame;

public class MapGenerator
{
    private Random Random { get; }
    private int Width { get; }
    private int Height { get; }
    private int PassageWidth { get; }
    private GameCycleModel.EntityTypes[,] Map { get; }
    private List<(int, int)> EmptyCells { get; }

    public MapGenerator(int width, int height, int passageWidth)
    {
        Random = new Random();
        Width = width;
        Height = height;
        PassageWidth = passageWidth;
        Map = new GameCycleModel.EntityTypes[Width, Height];
        EmptyCells = new List<(int, int)>();
    }

    public GameCycleModel.EntityTypes[,] GenerateMap(int chestsCount, int heartsCount, int ratsbaneCount)
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            Map[x, y] = GameCycleModel.EntityTypes.Wall;
        
        var startCell = new Point(
            Random.Next(3 * PassageWidth / 2, Width - 3 * PassageWidth / 2), 
            Random.Next(3 * PassageWidth / 2, Height - 3 * PassageWidth / 2)
            );
        
        Map[startCell.X, startCell.Y] = GameCycleModel.EntityTypes.Empty;

        VisitCell(startCell);
        
        GenerateEntity(chestsCount, GameCycleModel.EntityTypes.Chest, PlacementOnlyFloor);
        GenerateEntity(heartsCount, GameCycleModel.EntityTypes.Heart, PlacementOnlyFloor);
        GenerateEntity(ratsbaneCount, GameCycleModel.EntityTypes.Ratsbane, PlacementFloorAndCeil);
        
        Map[startCell.X, startCell.Y] = GameCycleModel.EntityTypes.Player;
        return Map;
    }

    private void VisitCell(Point cell)
    {
        var directions = GetRandomDirections();

        foreach (var direction in directions)
        {
            var neighborX = cell.X + direction.X;
            var neighborY = cell.Y + direction.Y;

            if (IsWithinBounds(neighborX, neighborY) 
                && Map[neighborX, neighborY] == GameCycleModel.EntityTypes.Wall)
            {
                var neighborCell = new Point(neighborX, neighborY);

                RemoveWallBetweenCells(cell, neighborCell);

                VisitCell(neighborCell);
            }
        }
    }

    private List<Point> GetRandomDirections()
    {
        var delta = 2 * (PassageWidth + (PassageWidth + 1) % 2);
        var directions = new List<Point>
        {
            new(delta, 0),
            new(-delta, 0),
            new(0, delta),
            new(0, -delta)
        };

        for (var index = 0; index < directions.Count; index++)
        {
            var temp = directions[index];
            var randomIndex = Random.Next(index, directions.Count);
            directions[index] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        return directions;
    }

    private void RemoveWallBetweenCells(Point cell1, Point cell2)
    {
        var xMin = Math.Min(cell1.X, cell2.X) - PassageWidth / 2;
        var yMin = Math.Min(cell1.Y, cell2.Y) - PassageWidth / 2;
        var xMax = Math.Max(cell1.X, cell2.X) + PassageWidth / 2;
        var yMax = Math.Max(cell1.Y, cell2.Y) + PassageWidth / 2;
        
        for (var x = xMin; x <= xMax; x++)
        for (var y = yMin; y <= yMax; y++)
        {
            if (IsWithinBounds(x, y))
            {
                Map[x, y] = GameCycleModel.EntityTypes.Empty;
                EmptyCells.Add((x, y));
            }
        }
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 3 * PassageWidth / 2
               && x < Width - 3 * PassageWidth / 2
               && y >= 3 * PassageWidth / 2
               && y < Height - 3 * PassageWidth / 2;
    }

    private void GenerateEntity(int count, GameCycleModel.EntityTypes entity, Func<int, int, bool> PlacementRules)
    { 
        var emptyCellsCopy = new List<(int, int)>(EmptyCells);
        
        for (var i = 0; i < count; i++)
        {
            var isEntitySet = false;
            while (!isEntitySet)
            {
                if (emptyCellsCopy.Count == 0) 
                    break;
            
                var index = Random.Next(emptyCellsCopy.Count);
                var (x, y) = emptyCellsCopy[index];

                if (PlacementRules(x, y))
                {
                    Map[x, y] = entity;
                    isEntitySet = true;
                    EmptyCells.RemoveAt(index);
                }
                emptyCellsCopy.RemoveAt(index);
            }
        }
    }

    private bool PlacementOnlyFloor(int x, int y)
    {
        if (y + 1 >= Map.GetLength(1) 
            || Map[x, y + 1] != GameCycleModel.EntityTypes.Wall)
            return false;
        return true;
    }

    private bool PlacementFloorAndCeil(int x, int y)
    {
        if (y + 1 < Map.GetLength(1) && Map[x, y + 1] == GameCycleModel.EntityTypes.Wall)
            return true;
        if (y - 1 >= 0 && Map[x, y - 1] == GameCycleModel.EntityTypes.Wall)
            return true;
        return false;
    }
}

