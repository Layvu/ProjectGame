using System.Numerics;

namespace ProjectGame;

public interface IEntity
{
    int ImageId { get; set; }
    Vector2 Position { get; }
    void Update();  
}
