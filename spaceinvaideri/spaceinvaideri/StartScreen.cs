using System.Numerics;
using Raylib_CsLo;

namespace Spaceinvaders
{
    class StartScreen
    {
        public Font menufont;
        bool startpressed = false;
       
        public StartScreen(Font menufont)
        {
            this.menufont = menufont;
            Raylib.SetTargetFPS(60);
        }

        public void Draw()
        {
            Raylib.DrawText("Liikutus nuolinäppäimistsä", 200, 700, 30, Raylib.WHITE);
            Raylib.DrawText("Ammut space näppäimestä", 200, 750, 30, Raylib.WHITE);

            Raylib.ClearBackground(Raylib.BLACK);

            int titleWidth = Raylib.MeasureText("Space Invaders", 80);
            
            Vector2 titlePosition = new Vector2((Raylib.GetScreenWidth() - titleWidth) / 2, 350);

            Raylib.DrawTextEx(menufont, "Space Invaders", titlePosition, 110, 1, Raylib.YELLOW);

            int startWidth = Raylib.MeasureText("Press SPACE to start", 40);
            Vector2 startPosition = new Vector2((Raylib.GetScreenWidth() - startWidth) / 2, 550);

            if (RayGui.GuiButton(new Rectangle((int)startPosition.X, (int)startPosition.Y, 200, 100), "Start"))
            { startpressed = true; }
            else startpressed = false;
        }
        public bool Update()
        {
            return startpressed;
        }
    }
}
