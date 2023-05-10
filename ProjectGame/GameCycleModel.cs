using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectGame;

public partial class GameCycleModel : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    
    public int PlayerId { get; set; }
    private int _currentId { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private char[,] _map;
    private readonly int _tileSize = 50;

    private readonly int _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
    private readonly int _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
    
    private readonly QuadTree _quadTree;

    private GameTime CurrentGameTime;
    
    public GameCycleModel()
    {
        Entities = new Dictionary<int, IEntity>();
        _map = GenerateMap();
        _quadTree = new QuadTree(new Rectangle(0, 0, 
                _map.GetLength(0) * _tileSize, 
                _map.GetLength(1) * _tileSize)
            );
    }

    public enum EntityTypes : byte
    {
        Player = 1,
        Wall
    }
    
    public void ChangesPlayerMoves(IGameModel.Direction[] directions)
    {
        var player = (Player)Entities[PlayerId];

        foreach (var dir in directions)
        {
            switch (dir)
            {
                case IGameModel.Direction.Up:
                    if (player.JumpTime <= player.JumpMaxTime)
                    {
                        var deltaTime = (float)CurrentGameTime.ElapsedGameTime.TotalSeconds;
                        player.AddJumpTime(deltaTime);
                        player.Moving += new Vector2(0, -player.JumpSpeed);
                    }
                    break;
                case IGameModel.Direction.Right:
                    player.Moving += new Vector2(player.Speed, 0);
                    break;
                case IGameModel.Direction.Left:
                    player.Moving += new Vector2(-player.Speed, 0);
                    break;
            }
        }
        player.Moving *= player.Friction;
    }
    
    private char[,] GenerateMap() // в отдельный класс-генератор
    {
        var mapWidth = _screenWidth / _tileSize;
        var mapHeight = _screenHeight / _tileSize;
        var map = new char[mapWidth, mapHeight];

        // границы
        map[mapWidth / 2, mapHeight - 2] = 'P';
        map[mapWidth / 2 + 5, mapHeight - 5] = 'W';
        for (var x = 0; x < mapWidth; x++)
        {
            map[x, 0] = 'W';
            map[x, mapHeight - 1] = 'W';
        }
        for (var y = 0; y < mapHeight; y++)
        {
            map[0, y] = 'W';
            map[mapWidth - 1, y] = 'W';
        }
        
        return map;
    }

    public void Initialize()
    {
        _currentId = 1;

        var isPlacedPlayer = false;
        for (var x = 0; x < _map.GetLength(0); x++)
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            if (_map[x, y] == '\0') continue;

            var generatedEntity = GenerateObject(_map[x, y], x, y);
            
            // игрок лишь один, запомним Id
            if (_map[x, y] == 'P' && !isPlacedPlayer)
            {
                PlayerId = _currentId;
                isPlacedPlayer = true;
            }

            generatedEntity.Id = _currentId;
            Entities.Add(_currentId, generatedEntity);
            _currentId++;
            
            if (generatedEntity is ISolid generatedSolid)
                _quadTree.Insert(generatedSolid);
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

    public void UpdateLogic(GameTime gameTime)
    {
        CurrentGameTime = gameTime;
        
        var playerInitialPosition = Entities[PlayerId].Position;
        var positionsBeforeUpdate = new Dictionary<int, Vector2>();

        foreach (var entityId in Entities.Keys)
        {
            var initialPosition = Entities[entityId].Position;
            positionsBeforeUpdate.Add(entityId, initialPosition);

            UpdateGravity(entityId);

            Entities[entityId].Update();
        }
        
        HandleCollisions(positionsBeforeUpdate);

        Updated.Invoke(this, new GameEventArgs
        {
            Entities = this.Entities,
            VisualShift = Entities[PlayerId].Position - playerInitialPosition
        });
    }
}