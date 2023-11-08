using Raylib_CsLo;

namespace Spaceinvaders
{
    class PauseOptions
    {
        public enum OptionsState
        {
            Options,
            BackToPauseMenu
        }

        private OptionsState currentState = OptionsState.Options;

        public OptionsState Draw()
        {
            Raylib.ClearBackground(Raylib.BLACK);

            if (currentState == OptionsState.Options)
            {
                Raylib.DrawText("Options Menu", 275, 300, 40, Raylib.WHITE);
                Raylib.DrawText("Press ESC to Return", 250, 500, 30, Raylib.WHITE);

                if (RayGui.GuiButton(new Rectangle(300, 370, 200, 100), "Back"))
                {
                    currentState = OptionsState.BackToPauseMenu;
                }
            }
            return currentState;
        }

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
            {
                currentState = OptionsState.BackToPauseMenu;
            }
        }
    }
}
