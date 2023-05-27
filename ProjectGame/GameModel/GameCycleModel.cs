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
    private int CurrentId { get; set; }
    private Dictionary<int, IEntity> Entities { get; set; }
    
    private EntityTypes[,] _map;
    private const int TileSize = 50;

    private readonly int _screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
    private readonly int _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
    
    private QuadTree _quadTree;

    private GameTime _currentGameTime;
    private GameState _gameState;
    private static Level CurrentLevel { get; set; }

    public GameCycleModel()
    {
        CurrentLevel = new Level(_screenWidth, _screenHeight, TileSize);
        _map = CurrentLevel.CurrentMap;
        
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
        _gameState = new GameState(CurrentLevel.ChestsTotalCount, CurrentLevel.LevelNumber);
        Entities = new Dictionary<int, IEntity>();
        _quadTree = new QuadTree(new Rectangle(0, 0, 
            CurrentLevel.MapWidth * TileSize, 
            CurrentLevel.MapHeight * TileSize)
        );
        
        CurrentId = 0;
        var isPlacedPlayer = false;
        for (var x = 0; x < _map.GetLength(0); x++)
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            if (_map[x, y] == EntityTypes.Empty) continue;

            var generatedEntity = GenerateObject(_map[x, y], x, y);
            
            if (_map[x, y] == EntityTypes.Player && !isPlacedPlayer)
            {
                PlayerId = CurrentId;
                isPlacedPlayer = true;
            }

            generatedEntity.Id = CurrentId;
            Entities.Add(CurrentId, generatedEntity);
            CurrentId++;
            
            if (generatedEntity is ISolid generatedSolid)
                _quadTree.Insert(generatedSolid);
        }

        var player = Entities[PlayerId] as Player;
        _gameState.Update(player.ChestBar, player.HealthBar);

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
        
        _gameState.Update(player.ChestBar, player.HealthBar);

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
        CurrentLevel.NewLevel();
        _map = CurrentLevel.CurrentMap;
    }
}