using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectGame;

public partial class GameCycleModel : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    public event EventHandler<GameEventArgs> NewMapCreated;

    public static int PlayerId { get; set; }
    private int _currentId { get; set; }
    private Dictionary<int, IEntity> Entities { get; set; }
    
    private EntityTypes[,] _map;
    private readonly int _tileSize = 50;

    private readonly int _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
    private readonly int _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
    
    private QuadTree _quadTree;

    private GameTime _currentGameTime;
    private GameState _gameState;

    private static int _chestsCount = 3; // del
    private static int _heartsCount = 3; // del
    private static int _ratsbaneCount = 10; // del

    private int MapWidth { get; set; }
    private int MapHeight { get; set; }

    public GameCycleModel()
    {
        // в инициализацию
        MapWidth = _screenWidth * 2 / _tileSize;
        MapHeight = _screenHeight * 2 / _tileSize;
        
        _map = new MapGenerator(MapWidth, MapHeight, 4)
            .GenerateMap(_chestsCount, _heartsCount, _ratsbaneCount);
        //
        
        Initialize();
    }

    public enum EntityTypes : byte
    {
        Player = 1,
        Wall,
        Empty,
        Chest,
        Heart,
        Ratsbane
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
                        var deltaTime = (float)_currentGameTime.ElapsedGameTime.TotalSeconds;
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
        // CreateNewLevel(); // ниже 
        // _gameState = new GameState(_currentLevel.ChestsTotalCount);
        
        _gameState = new GameState(3);
        
        //
        
        Entities = new Dictionary<int, IEntity>();
        _quadTree = new QuadTree(new Rectangle(0, 0, 
            MapWidth * _tileSize, 
            MapHeight * _tileSize)
        );
        
        _currentId = 0;
        var isPlacedPlayer = false;
        for (var x = 0; x < _map.GetLength(0); x++)
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            if (_map[x, y] == EntityTypes.Empty) continue;

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

        var player = Entities[PlayerId] as Player;
        _gameState.Update(player.ChestBar, player.HealthBar); // + levelNumber

        NewMapCreated?.Invoke(this, new GameEventArgs()
        {
            Entities = Entities,
            VisualShift = new Vector2(
                Entities[PlayerId].Position.X,
                Entities[PlayerId].Position.Y),
            GameState = _gameState
        });
    }

    public void UpdateLogic(GameTime gameTime)
    {
        _currentGameTime = gameTime;
        
        var player = (Player)Entities[PlayerId];
        var playerInitialPosition = Entities[PlayerId].Position;

        foreach (var entity in Entities.Values)
        {
            UpdateGravity(entity);
            if (entity is Player)
            {
                HandleCollisions(player);
            }
            entity.Update();
        }
        
        _gameState.Update(player.ChestBar, player.HealthBar); // + levelNumber

        if (player.Died)
        {
            Initialize();
        }
        else if (player.Win)
        {
            CreateNewLevel();
            Initialize();
        }
        else
        {
            Updated?.Invoke(this, new GameEventArgs
            {
                Entities = Entities,
                VisualShift = Entities[PlayerId].Position - playerInitialPosition,
                GameState = _gameState
            });
        }
    }

    private void CreateNewLevel()
    {
        /*
        _currentLevel.NewLevel()
        _currentLevel.MapWidth;
        _currentLevel.MapHeight;
        _currentLevel.ChestsCount;
        _currentLevel.HeartsCount;
        _currentLevel.RatsbaneCount;
        */
             
        MapWidth += MapWidth / 2;
        MapHeight += MapHeight / 2;
        _heartsCount += _heartsCount / 2;
        _heartsCount += _heartsCount / 2; 
        _ratsbaneCount += _ratsbaneCount / 2;

        _map = new MapGenerator(MapWidth, MapHeight, 4)
            .GenerateMap(_chestsCount, _heartsCount, _ratsbaneCount);
    }
}