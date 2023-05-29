using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectGame;

public partial class GameCycleModel
{
    private void UpdateMenu()
    {
        var mouseState = Mouse.GetState();
        foreach (var button in Buttons.Values.Cast<Button>())
            button.Update(mouseState);
        
        Updated?.Invoke(this, new GameEventArgs
        {
            Entities = Buttons,
            VisualShift = Vector2.Zero,
            GameState = GameState
        });
    }

    private void InitializeMenu()
    {
        var startGameButtonPosition = new Vector2(30, 515);
        
        var startGameButton = new Button(7, startGameButtonPosition, 300, 
            40, "Start new game", 0, Color.White);
        
        var exitButton = new Button(
            8, startGameButtonPosition + new Vector2(0, 80), 
            80, 40, "Exit", 1, Color.White);
        
        Buttons = new Dictionary<int, IEntity>
        {
            {0, startGameButton},
            {1, exitButton}
        };
        
        UpdateMenu();
    }
}