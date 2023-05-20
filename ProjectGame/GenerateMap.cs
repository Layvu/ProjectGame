using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectGame;

public class RecursiveBacktrackerMapGenerator
{
    private Random Random { get; }
    private int Width { get; }
    private int Height { get; }
    private int PassageWidth { get; }
    private GameCycleModel.EntityTypes[,] Map { get; }
    
    public RecursiveBacktrackerMapGenerator(int width, int height, int passageWidth)
    {
        Random = new Random();
        Width = width;
        Height = height;
        PassageWidth = passageWidth;
        Map = new GameCycleModel.EntityTypes[Width, Height];
    }

    public GameCycleModel.EntityTypes[,] GenerateMap()
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
                Map[neighborX, neighborY] = GameCycleModel.EntityTypes.Empty;

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
            if (IsWithinBounds(x, y)) Map[x, y] = GameCycleModel.EntityTypes.Empty;
    }


    private bool IsWithinBounds(int x, int y)
    {
        return x >= 3 * PassageWidth / 2
               && x < Width - 3 * PassageWidth / 2
               && y >= 3 * PassageWidth / 2
               && y < Height - 3 * PassageWidth / 2;
    }
}
