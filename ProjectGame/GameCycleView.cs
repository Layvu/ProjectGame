using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectGame;

public class GameCycleView : Game, IGameView
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public event EventHandler<AllMovesEventArgs> PlayerMovesChanged; 
    public event EventHandler<AllMovesEventArgs> CycleFinished;
    
    private Vector2 _visualShift = Vector2.Zero;
    
    private Dictionary<int, IEntity> _entities = new();
    private readonly Dictionary<int, Texture2D> _textures = new();
    private readonly Dictionary<int, Animation> _animations = new();

    private int _playerId;
    
    public void LoadRenderingParameters(Dictionary<int, IEntity> entities, Vector2 visualShift)
    {
        _playerId = GameCycleModel.PlayerId;
        _entities = entities;
        _visualShift += visualShift;
    }

    public GameCycleView()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        var adapter = GraphicsAdapter.DefaultAdapter;
        _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        //начало координат 
        _visualShift.X -= adapter.CurrentDisplayMode.Width / 2;
        _visualShift.Y -= adapter.CurrentDisplayMode.Height - 100;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _textures.Add((byte)GameCycleModel.EntityTypes.Player, Content.Load<Texture2D>("spritesPlayer"));
        _textures.Add((byte)GameCycleModel.EntityTypes.Wall, Content.Load<Texture2D>("wall"));
        _textures.Add((byte)GameCycleModel.EntityTypes.Chest, Content.Load<Texture2D>("chest"));
        _textures.Add((byte)GameCycleModel.EntityTypes.Heart, Content.Load<Texture2D>("heart"));
        _textures.Add((byte)GameCycleModel.EntityTypes.Ratsbane, Content.Load<Texture2D>("ratsbane"));
        
        LoadAnimations();
    }


    protected override void Update(GameTime gameTime)
    {
        foreach (var animation in _animations.Values) animation.Update(gameTime);
        
        var keysState = Keyboard.GetState();
        var keysPressed = keysState.GetPressedKeys();

        var directions = new List<IGameModel.Direction>();

        var playerAnimation = _animations[_playerId];
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
        
        base.Update(gameTime);
        CycleFinished?.Invoke(this, new AllMovesEventArgs { gameTime = gameTime });
    }

    private void LoadAnimations()
    {
        var playerAnimation = new Animation(new Point(8, 10), 100, 46, 50);
        playerAnimation.SetLastFrameY(7);
        _animations.Add(_playerId, playerAnimation);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(64, 39, 13)); //CornflowerBlue

        _spriteBatch.Begin();
        
        foreach (var entity in _entities.Values)
        {
            var texture = _textures[entity.ImageId];
            if (_animations.ContainsKey(entity.Id))
            {
                var animation = _animations[entity.Id];
                
                var centerPosition = entity.Position 
                                     - new Vector2(animation.FrameWidth, animation.FrameHeight) / 2;
                
                _spriteBatch.Draw(texture, centerPosition - _visualShift, new Rectangle(
                        animation.CurrentFrame.X * animation.FrameWidth, 
                        animation.CurrentFrame.Y * animation.FrameHeight, 
                        animation.FrameWidth, animation.FrameHeight), Color.White);
            }
            else
            {
                var centerPosition = entity.Position - new Vector2(texture.Width, texture.Height) / 2;
                _spriteBatch.Draw(texture, centerPosition - _visualShift, Color.White);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}