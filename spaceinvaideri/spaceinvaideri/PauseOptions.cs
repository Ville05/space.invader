using Raylib_CsLo;

namespace Spaceinvaideri
{
    public class PauseOptions
    {
        public event EventHandler<EventArgs> pOptions;
        public event Action IncreaseSound;
        public event Action DecreaseSound;
        public event Action<int> ChangeNumEnemies;

        public void Draw()
        {
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.DrawText("Options Menu", 275, 300, 40, Raylib.WHITE);
            Raylib.DrawText("Press ESC to Return", 250, 800, 30, Raylib.WHITE);

            if (RayGui.GuiButton(new Rectangle(300, 370, 200, 100), "Back"))
            {
                pOptions.Invoke(this, EventArgs.Empty);
            }
            if (RayGui.GuiButton(new Rectangle(300, 480, 200, 100), "Increase Sound"))
            {
                IncreaseSound?.Invoke();
            }
            if (RayGui.GuiButton(new Rectangle(300, 590, 200, 100), "Decrease Sound"))
            {
                DecreaseSound?.Invoke();
            }

            if (RayGui.GuiButton(new Rectangle(100, 700, 100, 75), "20 Enemies"))
            {
                ChangeNumEnemies?.Invoke(20);
            }
            if (RayGui.GuiButton(new Rectangle(200, 700, 100, 75), "25 Enemies"))
            {
                ChangeNumEnemies?.Invoke(25);
            }
            if (RayGui.GuiButton(new Rectangle(300, 700, 100, 75), "30 Enemies"))
            {
                ChangeNumEnemies?.Invoke(30);
            }
            if (RayGui.GuiButton(new Rectangle(400, 700, 100, 75), "35 Enemies"))
            {
                ChangeNumEnemies?.Invoke(35);
            }
            if (RayGui.GuiButton(new Rectangle(500, 700, 100, 75), "40 Enemies"))
            {
                ChangeNumEnemies?.Invoke(40);
            }
        }

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
            {
                pOptions.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
