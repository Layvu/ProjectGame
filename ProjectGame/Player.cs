using System.Numerics;

namespace ProjectGame;

public class Player : IEntity
{
    public int ImageId { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Moving { get; set; }
    public float Friction { get; set; }
    
    public void Update()
    {
        Position += Moving;
        Moving *= Friction;
    }
}