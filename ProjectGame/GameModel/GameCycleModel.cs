using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public partial class GameCycleModel : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    public event EventHandler<GameEventArgs> NewMapCreated;

    public static int PlayerId { get; set; }
    private int CurrentId { get; set; }
    private Dictionary<int, IEntity> Entities { get; set; }
    private Dictionary<int, IEntity> Buttons { get; set; }
    private int ScreenWidth { get; } 
    private int ScreenHeight { get; }
    private EntityTypes[,] Map { get; set; }
    private QuadTree QuadTree { get; set; }
    private GameTime CurrentGameTime { get; set; }
    private GameState GameState { get; set; }
    private static Level CurrentLevel { get; set; }

    public const int TileSize = 50;

    public GameCycleModel()
    {
        ScreenWidth = 1024;
        ScreenHeight = 768;
        
        CurrentLevel = new Level(ScreenWidth, ScreenHeight, TileSize);
        Map = CurrentLevel.CurrentMap;
        GameState = new GameState();
        
        Initialize();
    }

    public enum ButtonTypes : byte
    {
        StartGame,
        Exit
    }
    
    public enum EntityTypes : byte
    {
        Player = 2,
        Wall,
        Empty,
        Chest,
        Heart,
        Ratsbane
    }
    
    public enum ScreenEntityTypes : byte
    {
        HealthBar = 8,
        MainMenuBackground
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
        switch (GameState.State)
        {
            case State.Game:
                InitializeGame();
                break;
            
            case State.Menu:
                InitializeMenu();
                break;
        }
    }
    
    private void InitializeGame()
    {
        GameState = new GameState(CurrentLevel.ChestsTotalCount, CurrentLevel.LevelNumber);
        
        Entities = new Dictionary<int, IEntity>();
        QuadTree = new QuadTree(new Rectangle(0, 0, 
            CurrentLevel.MapWidth * TileSize, 
            CurrentLevel.MapHeight * TileSize)
        );
        
        CurrentId = 0;
        var isPlacedPlayer = false;
        for (var x = 0; x < Map.GetLength(0); x++)
        for (var y = 0; y < Map.GetLength(1); y++)
        {
            if (Map[x, y] == EntityTypes.Empty) continue;

            var generatedEntity = GenerateObject(Map[x, y], x, y);
            
            if (Map[x, y] == EntityTypes.Player && !isPlacedPlayer)
            {
                PlayerId = CurrentId;
                isPlacedPlayer = true;
            }

            generatedEntity.Id = CurrentId;
            Entities.Add(CurrentId, generatedEntity);
            CurrentId++;
            
            if (generatedEntity is ISolid generatedSolid)
                QuadTree.Insert(generatedSolid);
        }

        var player = Entities[PlayerId] as Player;
        GameState.Update(player.ChestBar, player.HealthBar);

        NewMapCreated?.Invoke(this, new GameEventArgs()
        {
            Entities = Entities,
            VisualShift = new Vector2(
                player.Position.X,
                player.Position.Y),
            GameState = GameState
        });
    }

    public void UpdateLogic(GameTime gameTime)
    {
        switch (GameState.State)
        {
            case State.Game:
                UpdateGame(gameTime);
                if (GameState.State == State.Menu)
                    UpdateMenu();
                break;
            
            case State.Menu:
                UpdateMenu();
                break;
        }
    }

    private void UpdateGame(GameTime gameTime)
    {
        CurrentGameTime = gameTime;
        
        var player = (Player)Entities[PlayerId];
        var playerInitialPosition = Entities[PlayerId].Position;

        foreach (var entity in Entities.Values)
        {
            UpdateGravity(entity);

            if (entity.Id == PlayerId) continue;
            entity.Update();
        }
        
        HandleCollisions(player);
        player.Update();

        GameState.Update(player.ChestBar, player.HealthBar);

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
                VisualShift = player.Position - playerInitialPosition,
                GameState = GameState
            });
        }
    }
    
    public void ChangeGameState()
    { 
        GameState.State = GameState.State == State.Menu ? State.Game : State.Menu;
    }
    
    public void StartNewGame()
    {
        GameState.State = State.Game;
        RebuildMapThisLevel();
        Initialize();
    }

    private void CreateNewLevel()
    {
        CurrentLevel.NewLevel();
        Map = CurrentLevel.CurrentMap;
    }
    
    private void RebuildMapThisLevel()
    {
        CurrentLevel.RebuildLevel();
        Map = CurrentLevel.CurrentMap;
    }
}