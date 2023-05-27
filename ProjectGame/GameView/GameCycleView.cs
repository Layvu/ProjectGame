using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Microsoft.Xna.Framework.Graphics.GraphicsAdapter;

namespace ProjectGame;

public class GameCycleView : Game, IGameView
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public event EventHandler<AllMovesEventArgs> PlayerMovesChanged;
    public event EventHandler<AllMovesEventArgs> CycleFinished;

    private Vector2 _visualShift;
    
    private int _playerId;

    private Dictionary<int, IEntity> Entities  { get; set; }
    private Dictionary<int, Texture2D> Textures { get; }
    private Dictionary<int, Animation> Animations { get; }
    private GameState CurrentGameState { get; set; }
    private SpriteFont SpriteFont { get; set; }
    private Texture2D SpriteHealthBar { get; set; }
    
    public void LoadRenderingParameters(
        Dictionary<int, IEntity> entities, Vector2 visualShift, GameState currentGameState)
    {
        _playerId = GameCycleModel.PlayerId;
        Entities = entities;
        _visualShift += visualShift;
        CurrentGameState = currentGameState;
    }

    public void LoadNewMap(Dictionary<int, IEntity> entities, Vector2 visualShift, GameState gameState)
    {
        _visualShift = Vector2.Zero;
        
        _playerId = GameCycleModel.PlayerId;
        Entities = entities;
        _visualShift += visualShift;
        CurrentGameState = gameState;
        
        LoadAnimations();
        
        _visualShift.X -= DefaultAdapter.CurrentDisplayMode.Width / 2;
        _visualShift.Y -= 3 * DefaultAdapter.CurrentDisplayMode.Height / 4;
    }

    public GameCycleView()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Textures = new Dictionary<int, Texture2D>();
        Animations = new Dictionary<int, Animation>();
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        var adapter = DefaultAdapter;
        _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        Textures.Add((byte)GameCycleModel.EntityTypes.Player, Content.Load<Texture2D>("spritesPlayer"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Wall, Content.Load<Texture2D>("wall"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Chest, Content.Load<Texture2D>("chest"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Heart, Content.Load<Texture2D>("heart"));
        Textures.Add((byte)GameCycleModel.EntityTypes.Ratsbane, Content.Load<Texture2D>("ratsbane"));
                
        SpriteHealthBar = Content.Load<Texture2D>("heartForBar");
        SpriteFont = Content.Load<SpriteFont>("font");
    }

    protected override void Update(GameTime gameTime)
    {
        foreach (var animation in Animations.Values) 
            animation.Update(gameTime);
        
        UpdatePlayerActions();

        base.Update(gameTime);
        CycleFinished?.Invoke(this, new AllMovesEventArgs { gameTime = gameTime });
    }

    private void UpdatePlayerActions()
    {
        var keysState = Keyboard.GetState();
        var keysPressed = keysState.GetPressedKeys();

        var directions = new List<IGameModel.Direction>();

        var playerAnimation = Animations[_playerId];
        foreach (var key in keysPressed)
        {
            switch (key)
            {
                case Keys.Escape:
                    Exit();
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

    private void LoadAnimations()
    {
        Animations.Clear();
        
        var playerAnimation = new Animation(new Point(8, 10), 100, 46, 50);
        playerAnimation.SetLastFrameY(7);
        Animations.Add(_playerId, playerAnimation);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(64, 39, 13));
        _spriteBatch.Begin();
        
        foreach (var entity in Entities.Values)
        {
            var texture = Textures[entity.ImageId];
            if (Animations.ContainsKey(entity.Id)) AnimationDraw(texture, entity);
            else CenterDraw(texture, entity);
        }
        ChestsStateDraw();
        HeartsStateDraw();

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void ChestsStateDraw()
    {
        var collected = CurrentGameState.ChestsCollectedCount;
        var total = CurrentGameState.ChestsTotalCount;
        var stateChestsLine = $"Собрано сундуков: {collected} из {total}";
        var positionOnScreen = new Vector2(10f, 55f);
        _spriteBatch.DrawString(SpriteFont, stateChestsLine, positionOnScreen, Color.Wheat);
    }

    private void HeartsStateDraw()
    {
        for (var i = 0; i < CurrentGameState.PlayerHeartsCount; i++)
        {
            var position = new Vector2(10f + i * 50, 0.2f);
            _spriteBatch.Draw(SpriteHealthBar, position, Color.White);
        }
    }

    private void AnimationDraw(Texture2D texture, IEntity entity)
    {
        var animation = Animations[entity.Id];
                
        var centerPosition = entity.Position 
                             - new Vector2(animation.FrameWidth, animation.FrameHeight) / 2;
                
        _spriteBatch.Draw(texture, centerPosition - _visualShift, new Rectangle(
            animation.CurrentFrame.X * animation.FrameWidth, 
            animation.CurrentFrame.Y * animation.FrameHeight, 
            animation.FrameWidth, animation.FrameHeight), Color.White);
    }

    private void CenterDraw(Texture2D texture, IEntity entity)
    {
        var centerPosition = entity.Position - new Vector2(
            texture.Width / 2 - 5, texture.Height / 2 - 5);
        _spriteBatch.Draw(texture, centerPosition - _visualShift, Color.White);
    }
}