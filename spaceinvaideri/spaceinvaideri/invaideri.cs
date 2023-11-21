using Raylib_CsLo;
using System.Numerics;

namespace Spaceinvaideri
{
    public class invaideri
    {
        public enum GameState { mainmenu, optionsmenu, play, Lose, Win, Pause }
        public int screenWidth = 800;
        public int screenHeight = 1000;
        public static int numEnemies = 25;
        Player player;
        List<Bullet> bullets;
        List<Enemy> enemies;
        private Vector2 position;
        public bool moveRight = true;
        public static bool shouldChangeDirection = false;
        public bool moveDown = false;
        public float speed = 0.1f;
        private Texture enemy1;
        private Texture player1;
        public Sound shootsound;
        public Sound dmgsound;
        public Font menufont;
        private bool showDevMenu = false;
        private float soundVolume = 1.0f;
        public double startTime;
        public double elapsedTime;
        public double pauseTime = 0.0;
        public static int destroyedEnemies = 0;

        StartScreen startScreen;
        PauseOptions optionsMenu;

        Stack<GameState> statestack = new Stack<GameState>();

        void init()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "invaideri");
            Raylib.InitAudioDevice();
            
            Raylib.SetExitKey(KeyboardKey.KEY_BACKSPACE);
            Raylib.SetTargetFPS(60);
            destroyedEnemies = 0;

            player1 = Raylib.LoadTexture("kuvat/player.png");
            enemy1 = Raylib.LoadTexture("kuvat/enemy.png");
            shootsound = Raylib.LoadSound("aanet/Shoot.wav");
            dmgsound = Raylib.LoadSound("aanet/Dmg.wav");
            menufont = Raylib.LoadFont("font/Starjedi.ttf");

            startScreen = new StartScreen(menufont);
            startScreen.optionpressed += optionpressed;
            optionsMenu = new PauseOptions();
            optionsMenu.pOptions += pauseback;
            int playerSize = 1;
            Vector2 playerStart = new Vector2(screenWidth / 2, screenHeight - playerSize * 2);

            bullets = new List<Bullet>();
            enemies = new List<Enemy>();
            player = new Player(playerStart, new Vector2(0, 0), speed * 100, playerSize, player1, dmgsound);
            
            optionsMenu.IncreaseSound += IncreaseSoundVolume;
            optionsMenu.DecreaseSound += DecreaseSoundVolume;

            for (int i = 0; i < numEnemies; i++)
            {
                int row = i / 5;
                int col = i % 5;
                Vector2 position = new Vector2(col * 100 + 100, row * 100 + 100);
                Enemy enemy = new Enemy(position, new Vector2(1, 0), 50, enemy1, new List<Player> { player }, dmgsound);
                enemies.Add(enemy);
            }
            statestack.Push(GameState.mainmenu);
        }

        private void OptionsMenu_OptionBackPressed(object? sender, EventArgs e)
        {
            statestack.Pop();
        }
        private void pauseback(object? sender, EventArgs e)
        {
            statestack.Pop();
        }
        public void optionpressed(object sender, EventArgs e) { statestack.Push(GameState.optionsmenu); }

        public void GameLoop()
        {
            init();
            PauseMenu pauseMenu = new PauseMenu();
            pauseMenu.pOptions += optionpressed;
            pauseMenu.pBack += pauseback;
            pauseMenu.RestartGame += restartGame;

            while (!Raylib.WindowShouldClose())
            {
                Raylib.SetSoundVolume(dmgsound, soundVolume);
                Raylib.SetSoundVolume(shootsound, soundVolume);
                elapsedTime = Raylib.GetTime() - startTime;
                optionsMenu.ChangeNumEnemies += SetNumEnemies;
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_F1))
                {
                    showDevMenu = !showDevMenu;
                }

                #if DEBUG
                if (showDevMenu)
                {
                    Raylib.DrawRectangle(635, 0, 165, 155, Raylib.BLACK);
                    Raylib.DrawText("restart game", 640, 0 + 35, 25, Raylib.WHITE);
                    Raylib.DrawText("kill player", 640, 0 + 70, 25, Raylib.WHITE);
                    Raylib.DrawText("kill enemies", 640, 0 + 105, 25, Raylib.WHITE);
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(640, 0 + 30, 100, 25)) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        pauseMenu.RestartGame += restartGame;
                    }
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(640, 0 + 70, 100, 25)) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        player.health = 0;
                    }
                    else if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(640, 0 + 105, 100, 25)) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        enemies.Clear();
                    }
                }
                #endif

                Raylib.BeginDrawing();
                switch (statestack.Peek())
                {
                    case GameState.mainmenu:
                        startScreen.Draw();
                        if (startScreen.Update())
                        {
                            statestack.Push(GameState.play);
                        }
                        break;
                    case GameState.optionsmenu:
                        optionsMenu.Draw();
                        break;

                    case GameState.play:
                        UpdateEnemies();
                        player.Update(enemies);
                        foreach (Enemy enemy in enemies.ToList())
                        {
                            enemy.Update(player);
                        }
                        drawGame();
                        if (player.health <= 0)
                        {
                            statestack.Push(GameState.Lose);
                        }
                        else if (enemies.Count == 0)
                        {
                            statestack.Push(GameState.Win);
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                        {
                            statestack.Push(GameState.Pause);
                        }
                        break;
                    case GameState.Pause:
                        pauseMenu.Draw(elapsedTime, destroyedEnemies);
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                        {
                            statestack.Push(GameState.play);
                        }
                        break;
                    case GameState.Win:
                        drawGameOver();
                        break;
                    case GameState.Lose:
                        drawGameOver();
                        break;
                }
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
            Raylib.UnloadTexture(enemy1);
            Raylib.UnloadTexture(player1);
        }

        public void SetNumEnemies(int newNumEnemies)
        {
            numEnemies = newNumEnemies;

            bullets.Clear();
            enemies.Clear();
            for (int i = 0; i < numEnemies; i++)
            {
                int row = i / 5;
                int col = i % 5;
                Vector2 position = new Vector2(col * 100 + 100, row * 100 + 100);
                Enemy enemy = new Enemy(position, new Vector2(1, 0), 50, enemy1, new List<Player> { player }, dmgsound);
                enemies.Add(enemy);
            }
        }

        private void IncreaseSoundVolume()
        {
            soundVolume += 0.1f;
            if (soundVolume > 1.0f)
            {
                soundVolume = 1.0f;
            }
        }
        private void DecreaseSoundVolume()
        {
            soundVolume -= 0.1f;
            if (soundVolume < 0.0f)
            {
                soundVolume = 0.0f;
            }
        }

        void UpdateEnemies()
        {
            bool wallHit = false;
            float enemySpeed = speed;

            foreach (Enemy enemy in enemies.ToList())
            {
                enemy.position.X += enemySpeed * enemy.direction.X;

                if (enemy.position.X - enemy.size / 2 <= 0 || enemy.position.X + enemy.size / 2 >= screenWidth)
                {
                    wallHit = true;
                }
            }

            if (wallHit)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.direction.X *= -1.0f;
                    enemy.position.Y += 10;
                }
            }

            if (enemies.Count > 0 && moveRight && enemies.Last().position.X >= screenWidth - 20)
            {
                moveRight = false;
                shouldChangeDirection = true;
                position.Y += 10;
            }
            else if (enemies.Count > 0 && !moveRight && enemies.First().position.X <= 20)
            {
                moveRight = true;
                shouldChangeDirection = true;
                position.Y += 10;
            }

            if (shouldChangeDirection)
            {
                speed *= -1;
                shouldChangeDirection = false;
                moveDown = true;
                foreach (Enemy enemy in enemies)
                {
                    enemy.direction.X *= -1.0f;
                    enemy.position.Y += 10;
                }
            }

            foreach (Enemy enemy in enemies.ToList())
            {
                if (!enemy.IsDestroyed)
                {
                    foreach (Bullet bullet in bullets.ToList())
                    {
                        if (Raylib.CheckCollisionCircles(bullet.position, 5, enemy.position, 20))
                        {
                            enemy.InflictDamage(1);
                            bullets.Remove(bullet);
                            enemy.IsDestroyed = true;
                            destroyedEnemies++;
                            break;
                        }
                    }
                }

                if (enemy.IsDestroyed)
                {
                    enemies.Remove(enemy);
                }
            }
            enemies.RemoveAll(enemy => !enemy.isAlive);
        }

        void drawGameOver()
        {
            if (statestack.Peek() == GameState.Lose)
            {
                Raylib.ClearBackground(Raylib.RED);
                Raylib.DrawText("Game Over!", 250, 400, 50, Raylib.BLACK);
                Raylib.DrawText("You got:" + player.score + " score", 225, 500, 40, Raylib.BLACK);
            }
            else if (statestack.Peek() == GameState.Win)
            {
                Raylib.ClearBackground(Raylib.WHITE);
                Raylib.DrawText("You Win!", 250, 400, 50, Raylib.BLACK);
                Raylib.DrawText("You got:" + player.score + " score", 225, 500, 40, Raylib.BLACK);
            }
            Raylib.DrawText("Press ENTER to start again", 175, 600, 30, Raylib.BLACK);

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                statestack.Push(GameState.play);
                player.health = 5;
                player.score = 0;
                bullets.Clear();
                enemies.Clear();
                shouldChangeDirection = false;
                moveRight = true;
                moveDown = false;
                startTime = Raylib.GetTime();

                int playerSize = 1;
                Vector2 playerStart = new Vector2(screenWidth / 2, screenHeight - playerSize * 2);
                player = new Player(playerStart, new Vector2(0, 0), speed * 100, playerSize, player1, dmgsound);
                for (int i = 0; i < numEnemies; i++)
                {
                    int row = i / 5;
                    int col = i % 5;
                    Vector2 position = new Vector2(col * 100 + 100, row * 100 + 100);
                    Enemy enemy = new Enemy(position, new Vector2(1, 0), 50, enemy1, new List<Player> { player }, dmgsound);
                    enemies.Add(enemy);
                }
            }
        }
        void restartGame()
        {
            statestack.Push(GameState.play);
            player.health = 5;
            player.score = 0;
            bullets.Clear();
            enemies.Clear();
            shouldChangeDirection = false;
            moveRight = true;
            moveDown = false;
            startTime = Raylib.GetTime();
            destroyedEnemies = 0;

            int playerSize = 1;
            Vector2 playerStart = new Vector2(screenWidth / 2, screenHeight - playerSize * 2);
            player = new Player(playerStart, new Vector2(0, 0), speed * 100, playerSize, player1, dmgsound);

            for (int i = 0; i < numEnemies; i++)
            {
                int row = i / 5;
                int col = i % 5;
                Vector2 position = new Vector2(col * 100 + 100, row * 100 + 100);
                Enemy enemy = new Enemy(position, new Vector2(1, 0), 50, enemy1, new List<Player> { player }, dmgsound);
                enemies.Add(enemy);
            }
        }

        void drawGame()
        {
            Raylib.ClearBackground(Raylib.GREEN);
            player.Draw();
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw();
            }
            player.DrawScore();

            string healthText = "Health: " + player.health.ToString();
            Raylib.DrawText(healthText, 10, 10, 20, Raylib.BLACK);
        }

        public int GetDestroyedEnemiesCount()
        {
            return destroyedEnemies;
        }
    }
}
