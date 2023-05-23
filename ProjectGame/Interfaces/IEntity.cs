using Microsoft.Xna.Framework;

namespace ProjectGame;

public interface IEntity
{
    int ImageId { get; set; }
    Vector2 Position { get; }
    public Vector2 Moving { get; set; }
    int Id { get; set; }
    void Update();
    void Move(Vector2 position);
    bool HasGravity { get; }
    public float Mass { get; }
}
