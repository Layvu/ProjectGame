using System;

namespace ProjectGame;

public static class Program
{
    [STAThread]
    static void Main()
    {
        var game = new GamePresenter(new GameCycleView(), new GameCycleModel());
        game.LaunchGame();
    }
}