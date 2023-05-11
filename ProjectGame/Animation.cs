using Microsoft.Xna.Framework;

namespace ProjectGame;

public class Animation
{
    private Point _currentFrame;
    public Point CurrentFrame => _currentFrame;
    private Point SpriteSize { get; }
    private int CurrentTimeSpend { get; set; }
    private int UpdatePeriod { get; }
    public int LastFrameY { get; private set; }
    public int FrameWidth { get; }
    public int FrameHeight { get; }

    public Animation(Point spriteSize, int updatePeriod, int frameWidth, int frameHeight)
    {
        SpriteSize = spriteSize;
        UpdatePeriod = updatePeriod;
        _currentFrame = Point.Zero;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
    }

    public void Update(GameTime gameTime)
    {
        CurrentTimeSpend += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

        if (CurrentTimeSpend > UpdatePeriod)
        {
            CurrentTimeSpend -= UpdatePeriod;
            _currentFrame.X++;
            if (_currentFrame.X >= SpriteSize.X) _currentFrame.X = 0;
        }
    }

    public void SetCurrentFrameY(int frameY) { _currentFrame.Y = frameY; }
    public void SetLastFrameY(int lastFrameY) { LastFrameY = lastFrameY; }
    public void SetCurrentFrameX(int currentFrameX) { _currentFrame.X = currentFrameX; }
}