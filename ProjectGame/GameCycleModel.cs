using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectGame;

public class GameCycleModel : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    
    public int PlayerId { get; set; }
    private int _currentId { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private char[,] _map;
    private readonly int _tileSize = 50;

    private readonly int _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
    private readonly int _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
    
    public enum EntityTypes : byte
    {
        Player = 1,
        Wall
    }

    public void Initialize()
    {
        Entities = new Dictionary<int, IEntity>();
        _currentId = 1;
        
        // Определяем размеры _map. Пока что на размер экрана
        var mapWidth = _screenWidth / _tileSize;
        var mapHeight = _screenHeight / _tileSize;
        _map = new char[mapWidth, mapHeight];

        //Задаём карту, будет вынесено
        for (var x = 0; x < mapWidth - 1; x++)
        {
            _map[x, 0] = 'W';
            _map[x, _map.GetLength(1) - 1] = 'W';
        }
        for (var y = 0; y < mapHeight; y++)
        {
            _map[0, y] = 'W';
            _map[_map.GetLength(0) - 1, y] = 'W';
        }
        _map[mapWidth / 2, mapHeight - 2] = 'P';
        //

        var isPlacedPlayer = false;
        for (var x = 0; x < mapWidth; x++)
        for (var y = 0; y < mapHeight; y++)
        {
            if (_map[x, y] == '\0') continue;

            var generatedEntity = GenerateObject(_map[x, y], x, y);
            
            // игрок лишь один
            if (_map[x, y] == 'P' && !isPlacedPlayer)
            {
                PlayerId = _currentId;
                isPlacedPlayer = true;
            }

            Entities.Add(_currentId, generatedEntity);
            _currentId++;
        }
        
        Updated.Invoke(this, new GameEventArgs()
        {
            Entities = this.Entities,
            VisualShift = new Vector2(
                Entities[PlayerId].Position.X,
                Entities[PlayerId].Position.Y)
        });

    }
    
    private IEntity GenerateObject (char name, int xTile, int yTile)
    {
        float currentX = xTile * _tileSize;
        float currentY = yTile * _tileSize;
        
        switch (name)
        {
            case 'P':
                return CreatePlayer(currentX, currentY, EntityTypes.Player);
            case 'W':
                return CreateWall(currentX, currentY, EntityTypes.Wall);
            default:
                return null;
        }
    }

    private static Player CreatePlayer (float x, float y, EntityTypes spriteId)
    {
        var player = new Player
        {
            Position = new Vector2(x, y),
            ImageId = (byte)spriteId,
            Moving = new Vector2(0, 0),
            Friction = 0.9f
        };
        return player;
    }

    private static Wall CreateWall(float x, float y, EntityTypes spriteId)
    {
        var wall = new Wall
        {
            Position = new Vector2(x, y),
            ImageId = (byte)spriteId
        };
        return wall;
    }

    
    public void ChangesPlayerMoves(IGameModel.Direction[] directions)
    {
        var player = (Player)Entities[PlayerId];
        
        player.Moving = Vector2.Zero;
        foreach (var dir in directions)
        {
            switch (dir)
            {
                case IGameModel.Direction.Up:
                    player.Moving += new Vector2(0, -1);
                    break;
                case IGameModel.Direction.Down:
                    player.Moving += new Vector2(0, 1);
                    break;
                case IGameModel.Direction.Right:
                    player.Moving += new Vector2(1, 0);
                    break;
                case IGameModel.Direction.Left:
                    player.Moving += new Vector2(-1, 0);
                    break;
            }
        }
        player.Moving *= player.Friction;
    }
    
    public void UpdateLogic()
    {
        var playerInitialPosition = Entities[PlayerId].Position;
        foreach (var entity in Entities.Values)
        {
            entity.Update();
        }
        var currentShift = Entities[PlayerId].Position - playerInitialPosition;
        Updated.Invoke(this, new GameEventArgs { Entities = this.Entities, VisualShift = currentShift } );
    }
}