using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class Button : ISolid
{
    public Button(int imageId, Vector2 position, int buttonWidth, 
        int buttonHeight, string text, int id, Color textColor)
    {
        ImageId = imageId;
        Position = position;
        Text = text;
        TextColor = textColor;
        _height = buttonHeight;
        _width = buttonWidth;
        Id = id;

        Collider = new RectangleCollider((int)position.X, 
            (int)position.Y, buttonWidth, buttonHeight);
    }
    
    public readonly string Text;
    public readonly Color TextColor;
    private readonly int _height;
    private readonly int _width;
    
    public int ImageId { get; set; }
    public Vector2 Position { get; private set; }
    public Vector2 Moving { get; set; }
    public RectangleCollider Collider { get; set; }
    public int Id { get; set; } 
    public float Mass { get; }
    public bool HasGravity { get; }
    
    private bool _isClicked;
    public bool IsClicked() => _isClicked;
    public void Clicked() => _isClicked = false;
    
    public void MoveCollider(Vector2 newPosition)
    {
        Position = newPosition;
        Collider = new RectangleCollider((int)newPosition.X, (int)newPosition.Y, _width, _height);
    }
    
    public void Move(Vector2 pos) {}
    public void Update() {}
    
    public void Update(MouseState mouseState)
    {
        var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

        if (mouseRectangle.Intersects(Collider.Boundary))
            _isClicked = mouseState.LeftButton == ButtonState.Pressed;
        else
            _isClicked = false;
    }
}
