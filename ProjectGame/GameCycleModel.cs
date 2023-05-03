using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
    
    private readonly QuadTree _quadTree;

    public GameCycleModel()
    {
        Entities = new Dictionary<int, IEntity>();
        _map = GenerateMap();
        _quadTree = new QuadTree(
            new Rectangle(0, 0, _map.GetLength(0), 
                _map.GetLength(1))
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
    
    private char[,] GenerateMap() // в отдельный класс-генератор
    {
        var mapWidth = _screenWidth / _tileSize;
        var mapHeight = _screenHeight / _tileSize;
        var map = new char[mapWidth, mapHeight];

        // границы
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
        
        map[mapWidth / 2 + 5, mapHeight - 5] = 'W';
        map[mapWidth / 2, mapHeight - 2] = 'P';

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

            generatedEntity.Id = _currentId; //
            Entities.Add(_currentId, generatedEntity);
            _currentId++;
            
            if (generatedEntity is ISolid) //
                _quadTree.Insert(generatedEntity); //
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

    public void UpdateLogic()
    {
        var playerInitialPosition = Entities[PlayerId].Position;
        var positionsBeforeUpdate = new Dictionary<int, Vector2>();

        // Обновляем все объекты и сохраняем их начальную позицию
        foreach (var entityId in Entities.Keys)
        {
            var initialPosition = Entities[entityId].Position;
            positionsBeforeUpdate.Add(entityId, initialPosition);

            Entities[entityId].Update();
            Entities[entityId].Update();
            //_quadTree.Update(Entities[entityId]); // Обновляем позицию объектов в квадранте
        }

        // Обрабатываем столкновения объектов в квадранте
        foreach (var entityId in Entities.Keys)
        {
            if (Entities[entityId] is ISolid solid1)
            {
                var objectsInSameQuad = _quadTree.Retrieve(Entities[entityId]);
                foreach (var entity2 in objectsInSameQuad)
                {
                    if (entityId == entity2.Id) continue;
                    if (entity2 is ISolid solid2)
                    {
                        if (RectangleCollider.IsCollided(solid1.Collider, solid2.Collider))
                        {
                            CalculateObstacleCollision3(
                                (positionsBeforeUpdate[entityId], entityId),
                                (positionsBeforeUpdate[entity2.Id], entity2.Id)
                            );
                        }
                    }
                }
            }
        }

        Updated.Invoke(this, new GameEventArgs
        {
            Entities = this.Entities,
            VisualShift = Entities[PlayerId].Position - playerInitialPosition
        });
    }

    private void CalculateObstacleCollision3( //
        (Vector2 initPos, int Id) entity1,
        (Vector2 initPos, int Id) entity2)
    {
        var isCollided = false;
        if (Entities[entity1.Id] is ISolid p1 && Entities[entity2.Id] is ISolid p2)
        {
            var oppositeDirection = new Vector2(0, 0);
            while (RectangleCollider.IsCollided(p1.Collider, p2.Collider))
            {
                isCollided = true;
                if (entity1.initPos != Entities[entity1.Id].Position)
                {
                    oppositeDirection = Entities[entity1.Id].Position - entity1.initPos;
                    oppositeDirection.Normalize();
                    Entities[entity1.Id].Move(Entities[entity1.Id].Position - oppositeDirection);
                }

                if (entity2.initPos != Entities[entity2.Id].Position)
                {
                    oppositeDirection = Entities[entity2.Id].Position - entity2.initPos;
                    oppositeDirection.Normalize();
                    Entities[entity2.Id].Move(Entities[entity2.Id].Position - oppositeDirection);
                }
            }
        }

        if (isCollided)
        {
            Entities[entity1.Id].Moving = Vector2.Zero;
            Entities[entity2.Id].Moving = Vector2.Zero;
        }
    }
}