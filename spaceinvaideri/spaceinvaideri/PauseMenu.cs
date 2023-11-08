using Raylib_CsLo;

namespace Spaceinvaders
{
    class PauseMenu
    {
        public Font menufont;
        public bool isPaused = false;
        private PauseOptions optionsMenu = new PauseOptions();

        public PauseMenu(Font menufont)
        {
            this.menufont = menufont;
        }

        public void Draw()
        {
            Raylib.ClearBackground(Raylib.BLACK);

            if (!isPaused)
            {
                Raylib.DrawText("Game Paused", 275, 300, 40, Raylib.WHITE);
                Raylib.DrawText("Press ESC to Resume", 250, 500, 30, Raylib.WHITE);

                if (RayGui.GuiButton(new Rectangle(300, 370, 200, 100), "Options"))
                {
                    isPaused = true;
                }
            }
            else
            {
                PauseOptions.OptionsState optionsState = optionsMenu.Draw();

                if (optionsState == PauseOptions.OptionsState.BackToPauseMenu)
                {
                    isPaused = false;
                }
            }
        }

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
            {
                if (isPaused)
                {
                    isPaused = false;
                }
            }

            if (isPaused)
            {
                optionsMenu.Update();
            }
        }
    }
}
