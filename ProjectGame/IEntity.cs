using Microsoft.Xna.Framework;

namespace ProjectGame;

public interface IEntity // размеры и id для дерева
{
    int ImageId { get; set; }
    Vector2 Position { get; }
    public Vector2 Moving { get; set; }
    int Width { get; } //
    int Height { get; } //
    int Id { get; set; } //
    void Update();
    void Move(Vector2 pos);
}
