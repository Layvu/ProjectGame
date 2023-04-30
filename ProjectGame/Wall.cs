using System.Numerics;

namespace ProjectGame;

class Wall : IEntity
{
    public int ImageId { get; set; }
    public Vector2 Position { get; set; }
  
    public void Update()
    {
    }
}
