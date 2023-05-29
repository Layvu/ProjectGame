using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectGame;

public partial class GameCycleView : Game, IGameView
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public event EventHandler<AllMovesEventArgs> PlayerMovesChanged;
    public event EventHandler<AllMovesEventArgs> CycleFinished;
    public event EventHandler ChangedGameState;
    public event EventHandler StartNewGame;

    private Dictionary<int, IEntity> Entities  { get; set; }
    private Dictionary<int, Texture2D> Textures { get; }
    private Dictionary<int, Animation> Animations { get; }
    private GameState CurrentGameState { get; set; }
    private SpriteFont Font { get; set; }
    private int ScreenWidth { get; }
    private int ScreenHeight { get; }
    private int PlayerId { get; set; }
    private DateTime LastTimeExitButtonPressed { get; set; }
    
    private Vector2 _visualShift;
    
    public void LoadRenderingParameters(
        Dictionary<int, IEntity> entities, Vector2 visualShift, GameState currentGameState)
    {
        PlayerId = GameCycleModel.PlayerId;
        Entities = entities;
        _visualShift += visualShift;
        CurrentGameState = currentGameState;
    }

    public void LoadNewMap(Dictionary<int, IEntity> entities, Vector2 visualShift, GameState gameState)
    {
        LoadRenderingParameters(entities, visualShift, gameState);
        LoadAnimations();

        _visualShift = visualShift;
        _visualShift.X -= ScreenWidth / 2;
        _visualShift.Y -= 3 * ScreenHeight / 4;
    }

    public GameCycleView()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Textures = new Dictionary<int, Texture2D>();
        Animations = new Dictionary<int, Animation>();
        
        ScreenWidth = 1024;
        ScreenHeight = 768;

        LastTimeExitButtonPressed = DateTime.Now;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _graphics.PreferredBackBufferWidth = ScreenWidth;
        _graphics.PreferredBackBufferHeight = ScreenHeight;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        Textures.Add((byte)GameCycleModel.EntityTypes.Player, Content.Load<Texture2D>("spritesPlayer"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Wall, Content.Load<Texture2D>("ground"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Chest, Content.Load<Texture2D>("chest"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Heart, Content.Load<Texture2D>("heart"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Ratsbane, Content.Load<Texture2D>("ratsbane"));
        
        Textures.Add((byte)GameCycleModel.ScreenEntityTypes.MainMenuBackground, Content.Load<Texture2D>("mainMenuBackground"));
        Textures.Add((byte)GameCycleModel.ScreenEntityTypes.HealthBar, Content.Load<Texture2D>("heartForBar"));
                
        Font = Content.Load<SpriteFont>("font");
    }

    private void LoadAnimations()
    {
        Animations.Clear();
        
        var playerAnimation = new Animation(new Point(8, 10), 100, 46, 50);
        playerAnimation.SetLastFrameY(7);
        Animations.Add(PlayerId, playerAnimation);
    }
    
    private void CheckMenuClickableButtons()
    {
        if (CurrentGameState.State != State.Menu) return;

        var buttons = Entities.Values.Cast<Button>().ToArray();
        var buttonExit = buttons[(int)GameCycleModel.ButtonTypes.Exit];
        var buttonStartGame = buttons[(int)GameCycleModel.ButtonTypes.StartGame];
        
        if (buttonExit.IsClicked()) Environment.Exit(0);
        else if (buttonStartGame.IsClicked())
        {
            StartNewGame?.Invoke(this, EventArgs.Empty);
            buttonStartGame.Clicked();
        }
    }

    protected override void Update(GameTime gameTime)
    {
        foreach (var animation in Animations.Values) 
            animation.Update(gameTime);
        
        var keysState = Keyboard.GetState();
        var keysPressed = keysState.GetPressedKeys();
        
        CheckMenuClickableButtons();
        
        if(CurrentGameState.State == State.Game) UpdatePlayerActions(keysPressed);
        else
        {
            foreach (var key in keysPressed) CheckInMenuButtons(key);
        }

        base.Update(gameTime);
        CycleFinished?.Invoke(this, new AllMovesEventArgs { gameTime = gameTime });
    }

    private void UpdatePlayerActions(Keys[] keysPressed)
    {
        var directions = new List<IGameModel.Direction>();
        var playerAnimation = Animations[PlayerId];
        const int updateDelay = 200;
        
        foreach (var key in keysPressed)
        {
            switch (key)
            {
                case Keys.Escape:
                    if ((DateTime.Now - LastTimeExitButtonPressed).Milliseconds < updateDelay) break;
                    ChangedGameState?.Invoke(this, EventArgs.Empty);
                    LastTimeExitButtonPressed = DateTime.Now;
                    break;
                case Keys.W:
                    directions.Add(IGameModel.Direction.Up);
                    break;
                case Keys.A:
                    directions.Add(IGameModel.Direction.Left);
                    playerAnimation.SetCurrentFrameY(5);
                    break;
                case Keys.D:
                    directions.Add(IGameModel.Direction.Right);
                    playerAnimation.SetCurrentFrameY(7);
                    break;
            }
        }

        if (directions.Count > 0)
        {
            playerAnimation.SetLastFrameY(playerAnimation.CurrentFrame.Y);
            
            PlayerMovesChanged?.Invoke(this, new AllMovesEventArgs { Direction = directions.ToArray() });
        }
        else
        {
            playerAnimation.SetCurrentFrameX(0);

            if (playerAnimation.LastFrameY > 4) playerAnimation.SetCurrentFrameY(playerAnimation.LastFrameY - 4);
            else playerAnimation.SetCurrentFrameY(playerAnimation.LastFrameY);
        }
    }

    private void CheckInMenuButtons(Keys key)
    {
        const int updateDelay = 200;
        
        if ((DateTime.Now - LastTimeExitButtonPressed).Milliseconds < updateDelay || PlayerId == 0) 
            return;
        if (key == Keys.Escape) 
            ChangedGameState?.Invoke(this, EventArgs.Empty);
        
        LastTimeExitButtonPressed = DateTime.Now;
    }
}