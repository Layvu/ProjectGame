using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace ProjectGame;

public class GameCycleView : Game, IGameView
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public event EventHandler<AllMovesEventArgs> PlayerMovesChanged; 
    public event EventHandler CycleFinished;
    
    public Texture2D _playerImage;

    private Vector2 _visualShift = Vector2.Zero;
    
    private Dictionary<int, IEntity> _entities = new();
    private readonly Dictionary<int, Texture2D> _textures = new();

    public void LoadRenderingParameters(Dictionary<int, IEntity> entities, Vector2 visualShift)
    {
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
        
        // TODO: Add your initialization logic here
        var adapter = GraphicsAdapter.DefaultAdapter;
        _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        //начало координат 
        _visualShift.X -= _graphics.PreferredBackBufferWidth / 2;
        _visualShift.Y -= _graphics.PreferredBackBufferHeight * 0.87f;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here
        
        _textures.Add((byte)GameCycleModel.EntityTypes.Player, Content.Load<Texture2D>("player"));
        _textures.Add((byte)GameCycleModel.EntityTypes.Wall, Content.Load<Texture2D>("wall"));
    }

    protected override void Update(GameTime gameTime)
    {
        var keysState = Keyboard.GetState();
        var keysPressed = keysState.GetPressedKeys();
        
        var directions = new List<IGameModel.Direction>();

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
                case Keys.S:
                    directions.Add(IGameModel.Direction.Down);
                    break;
                case Keys.A:
                    directions.Add(IGameModel.Direction.Left);
                    break;
                case Keys.D:
                    directions.Add(IGameModel.Direction.Right);
                    break;
            }
        }
        
        if (directions.Count > 0)
        {
            var allDirections = directions.ToArray();

            PlayerMovesChanged.Invoke(this, new AllMovesEventArgs { Direction = allDirections });
        }

        base.Update(gameTime);
        CycleFinished.Invoke(this, new EventArgs()); // UpdateLogic
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        
        foreach (var entity in _entities.Values)
        {
            var texture = _textures[entity.ImageId];
            var centerPosition = entity.Position - new Vector2(texture.Width, texture.Height) / 2;
            _spriteBatch.Draw(texture, centerPosition - _visualShift, Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}