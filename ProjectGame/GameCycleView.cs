using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace ProjectGame;

public class GameCycleView : Game, IGameView
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public event EventHandler<AllMovesEventArgs> PlayerMoved; 
    public event EventHandler CycleFinished;
    
    private Vector2 _playerPosition = Vector2.Zero;
    private Texture2D _playerImage;
    
    public void LoadRenderingParameters(Vector2 position)
    {
        _playerPosition = position;
    }

    public GameCycleView()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // TODO: use this.Content to load your game content here
        
        _playerImage = Content.Load<Texture2D>("image");
    }

    protected override void Update(GameTime gameTime)
    {
        var keysState = Keyboard.GetState();
        
        var keysPressed = keysState.GetPressedKeys();
        if (keysPressed.Length > 0)
        {
            var key = keysPressed.First();
            
            //var isKeyDown = keysState.IsKeyDown(key);

            switch (key)
            {
                case Keys.Escape:
                    Exit();
                    break;
            }
        }

        base.Update(gameTime);
        CycleFinished.Invoke(this, new EventArgs()); // UpdateLogic
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        base.Draw(gameTime);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.Draw(_playerImage, _playerPosition, Color.White);
        _spriteBatch.End();
    }
}