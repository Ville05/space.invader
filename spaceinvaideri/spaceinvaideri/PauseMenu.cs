using Raylib_CsLo;

namespace Spaceinvaideri
{
    class PauseMenu
    {
        public Font menufont;
        public event EventHandler pOptions;
        public event EventHandler pBack;
        public Action RestartGame;
        public bool RestartButtonPressed { get; set; }

        public void Draw(double elapsedTime, int destroyedEnemies)
        {
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.DrawText("Game Paused", 275, 300, 40, Raylib.WHITE);
            Raylib.DrawText("Elapsed Time: " + elapsedTime.ToString("0.00") + " seconds", 200, 475, 30, Raylib.WHITE);
            Raylib.DrawText("Enemies Destroyed: " + destroyedEnemies, 200, 525, 30, Raylib.WHITE);
            Raylib.DrawText("Press ESC to Resume", 225, 700, 30, Raylib.WHITE);

            if (RayGui.GuiButton(new Rectangle(300, 350, 200, 100), "Options"))
            {
                pOptions.Invoke(this, EventArgs.Empty);
            }
            if (RayGui.GuiButton(new Rectangle(300, 575, 200, 100), "Restart"))
            {
                RestartButtonPressed = true;
                RestartGame?.Invoke();
            }
        }

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
            {
                pBack.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
