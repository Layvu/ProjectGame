using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectGame;

public partial class GameCycleView
{
    protected override void Draw(GameTime gameTime)
    {
        if (CurrentGameState.State == State.Menu) DrawMenu(gameTime);
        else DrawGame(gameTime);
    }
    
    private void DrawMenu(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(64, 39, 13));
        _spriteBatch.Begin();

        var backgroundTexture = Textures[(int)GameCycleModel.ScreenEntityTypes.MainMenuBackground];

        _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

        DrawButton(Entities[(int)GameCycleModel.ButtonTypes.StartGame] as Button);
        DrawButton(Entities[(int)GameCycleModel.ButtonTypes.Exit] as Button);
        
        _spriteBatch.DrawString(Font, "SuperKladMan",
            new Vector2(800, (float)_graphics.PreferredBackBufferHeight / 16), Color.White);
        
        _spriteBatch.DrawString(Font, "Собери все сундуки или умри \nУправление клавишами W, A, D",
            new Vector2(30, (float)_graphics.PreferredBackBufferHeight / 16), Color.Ivory);

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawGame(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(64, 39, 13));

        _spriteBatch.Begin();

        foreach (var entity in Entities.Values)
        {
            var texture = Textures[entity.ImageId];
            if (Animations.ContainsKey(entity.Id)) AnimationDraw(texture, entity);
            else CenterDraw(texture, entity.Position);
        }
        ChestsStateDraw();
        LevelStateDraw();
        HeartsStateDraw();

        _spriteBatch.End();
        base.Draw(gameTime);  
    }

    private void ChestsStateDraw()
    {
        var collected = CurrentGameState.ChestsCollectedCount;
        var total = CurrentGameState.ChestsTotalCount;
        var stateChestsLine = $"Собрано сундуков: {collected} из {total}";
        var positionOnScreen = new Vector2(10f, 55f);
        _spriteBatch.DrawString(Font, stateChestsLine, positionOnScreen, Color.Wheat);
    }
    
    private void LevelStateDraw()
    {
        var level = CurrentGameState.LevelNumber;
        var stateLevelLine = $"Уровень: {level}";
        var positionOnScreen = new Vector2(10f, 90f);
        _spriteBatch.DrawString(Font, stateLevelLine, positionOnScreen, Color.Wheat);
    }

    private void HeartsStateDraw()
    {
        for (var i = 0; i < CurrentGameState.PlayerHeartsCount; i++)
        {
            var position = new Vector2(10f + i * 50, 0.2f);
            var texture = Textures[(int)GameCycleModel.ScreenEntityTypes.HealthBar];
            _spriteBatch.Draw(texture, position, Color.White);
        }
    }

    private void AnimationDraw(Texture2D texture, IEntity entity)
    {
        var animation = Animations[entity.Id];
        var centerPosition = entity.Position 
                             - new Vector2(animation.FrameWidth, animation.FrameHeight) / 2;

        var texturePosition = centerPosition - _visualShift; //GetEntityPositionByWindowSize(centerPosition - _visualShift); //

        _spriteBatch.Draw(texture, texturePosition, new Rectangle(
            animation.CurrentFrame.X * animation.FrameWidth, 
            animation.CurrentFrame.Y * animation.FrameHeight, 
            animation.FrameWidth, animation.FrameHeight), Color.White);
    }

    private void CenterDraw(Texture2D texture, Vector2 position)
    {
        var centerPosition = position - new Vector2(
            texture.Width / 2 - 5, texture.Height / 2 - 5);
        
        _spriteBatch.Draw(texture, centerPosition - _visualShift, Color.White);
    }

    private void DrawButton(Button button)
    {
        _spriteBatch.DrawString(Font, button.Text, button.Position, button.TextColor);
    }
}