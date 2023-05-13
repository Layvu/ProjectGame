using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectGame;

public partial class GameCycleModel : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    
    public static int PlayerId { get; set; }
    private int _currentId { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private EntityTypes[,] _map;
    private readonly int _tileSize = 50;

    private readonly int _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
    private readonly int _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
    
    private readonly QuadTree _quadTree;

    private GameTime CurrentGameTime;
    
    public GameCycleModel()
    {
        Entities = new Dictionary<int, IEntity>();
        var mapWidth = _screenWidth * 5 / _tileSize;
        var mapHeight = _screenHeight * 5 / _tileSize;
        _map = new RecursiveBacktrackerMapGenerator(mapWidth, mapHeight, 4).GenerateMap();
        
        _quadTree = new QuadTree(new Rectangle(0, 0, 
                _map.GetLength(0) * _tileSize, 
                _map.GetLength(1) * _tileSize)
            );
    }

    public enum EntityTypes : byte
    {
        Player = 1,
        Wall,
        Empty
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
    
    public void Initialize()
    {
        _currentId = 1;

        var isPlacedPlayer = false;
        for (var x = 0; x < _map.GetLength(0); x++)
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            if (_map[x, y] == EntityTypes.Empty) continue; // \0

            var generatedEntity = GenerateObject(_map[x, y], x, y);
            
            if (_map[x, y] == EntityTypes.Player && !isPlacedPlayer)
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