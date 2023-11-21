using System.Numerics;
using Raylib_CsLo;

namespace Spaceinvaideri
{
    class StartScreen
    {
        public Font menufont;
        bool startpressed = false;
        public event EventHandler optionpressed;

        public StartScreen(Font menufont)
        {
            this.menufont = menufont;
        }

        public void Draw()
        {
            Raylib.ClearBackground(Raylib.BLACK);

            int titleWidth = Raylib.MeasureText("Space Invaders", 80);
            Vector2 titlePosition = new Vector2((Raylib.GetScreenWidth() - titleWidth) / 2, 350);
            Raylib.DrawTextEx(menufont, "Space Invaders", titlePosition, 115, 1, Raylib.YELLOW);

            int startWidth = Raylib.MeasureText("Press SPACE to start", 40);
            Vector2 startPosition = new Vector2((Raylib.GetScreenWidth() - startWidth) / 2, 550);

            if (RayGui.GuiButton(new Rectangle((int)startPosition.X, (int)startPosition.Y, 450, 100), "Start"))
            {
                startpressed = true;
            }
            else
            {
                startpressed = false;
            }

            if (RayGui.GuiButton(new Rectangle((int)startPosition.X, (int)startPosition.Y + 150, 450, 100), "Options"))
            {
                optionpressed.Invoke(this, new EventArgs());
            }
        }

        public bool Update()
        {
            return startpressed;
        }
    }
}
