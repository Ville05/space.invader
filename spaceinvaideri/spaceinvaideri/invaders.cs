using System;
using Raylib_CsLo;
using System.Numerics;

namespace Spaceinvaders
{
    internal class invaders
    {
        public static int numEnemies = 25;

        public int screenWidth = 800;
        public int screenHeight = 1000;

        Player player;
        List<Bullet> bullets;
        List<Enemy> enemies;
        private Vector2 position;

        public bool moveRight = true;
        public static bool shouldChangeDirection = false;
        public bool moveDown = false;
        public float speed = 0.02f;

        bool isGamePaused = false;

        private Texture enemyImage;
        private Texture playerImage;
        public Sound playerhit;
        public Sound shot;
        public Font menufont;

        bool gameOver = false;
        bool gameStarted = false;

        StartScreen startScreen;

        enum GameState { Playing, Win, Lose };
        GameState gameState = GameState.Playing;

        void init()
        {
            Raylib.InitAudioDevice();

            Raylib.InitWindow(screenWidth, screenHeight, "Space Invaders");
            Raylib.SetExitKey(KeyboardKey.KEY_BACKSPACE);

            enemyImage = Raylib.LoadTexture("kuvat/enemy.png");
            playerImage = Raylib.LoadTexture("kuvat/player.png");
            playerhit = Raylib.LoadSound("aanet/Dmg.wav");
            shot = Raylib.LoadSound("aanet/Shoot.wav");
            menufont = Raylib.LoadFont("font/Starjedi.ttf");

            startScreen = new StartScreen(menufont);

            float playerSpeed = 120;
            int playerSize = 40;
            Vector2 playerStart = new Vector2(screenWidth / 2, screenHeight - playerSize * 2);

            bullets = new List<Bullet>();

            enemies = new List<Enemy>();

            player = new Player(playerStart, new Vector2(0, 0), playerSpeed, playerSize, playerImage, playerhit);

            for (int i = 0; i < numEnemies; i++)
            {
                int row = i / 5;
                int col = i % 5;  
                Vector2 position = new Vector2(col * 100 + 100, row * 100 + 100);
                Enemy enemy = new Enemy(position, new Vector2(1, 0), 50, enemyImage, new List<Player> { player }, playerhit);

                enemies.Add(enemy);
            }
            Raylib.SetTargetFPS(2500);
        }

        public void GameLoop()
        {
            init();
            PauseMenu pauseMenu = new PauseMenu(menufont);

            while (!Raylib.WindowShouldClose())
            {
                if (!gameStarted)
                {
                    startScreen.Draw();
                    if (startScreen.Update())
                    {
                        gameStarted = true;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE) && gameStarted && !gameOver)
                {
                    isGamePaused = !isGamePaused;
                }

                Raylib.BeginDrawing();

                if (!gameStarted)
                {
                    startScreen.Draw();
                }
                else if (gameOver)
                {
                    drawGameOver();
                }
                else
                {
                    if (isGamePaused)
                    {
                        pauseMenu.Draw();
                    }
                    else
                    {
                        drawGame();
                    }
                }

                if (!gameOver && gameStarted && !isGamePaused)
                {
                    UpdateEnemies();
                    player.Update(enemies);
                    foreach (Enemy enemy in enemies.ToList())
                    {
                        enemy.Update(player);
                    }
                }

                if (player.health <= 0)
                {
                    gameState = GameState.Lose;
                    gameOver = true;
                }
                else if (enemies.Count == 0)
                {
                    gameState = GameState.Win;
                    gameOver = true;
                }
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
            Raylib.UnloadTexture(enemyImage);
            Raylib.UnloadTexture(playerImage);
        }

        void drawGameOver()
        {
            if (gameState == GameState.Lose)
            {
                Raylib.ClearBackground(Raylib.RED);
                Raylib.DrawText("Game Over!", 250, 400, 50, Raylib.BLACK);
                Raylib.DrawText("You got:" + player.score + " score", 225, 500, 40, Raylib.BLACK);
            }
            else if (gameState == GameState.Win)
            {
                Raylib.ClearBackground(Raylib.LIME);
                Raylib.DrawText("You Win!", 250, 400, 50, Raylib.BLACK);
                Raylib.DrawText("You got:" + player.score + " score", 225, 500, 40, Raylib.BLACK);
            }
            Raylib.DrawText("Press ENTER to start again", 175, 600, 30, Raylib.BLACK);

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && gameOver)
            {
                gameState = GameState.Playing;
                player.health = 100;
                player.score = 0;
                bullets.Clear();
                enemies.Clear();
                shouldChangeDirection = false;
                moveRight = true;
                moveDown = false;

                float playerSpeed = 120;
                int playerSize = 40;
                Vector2 playerStart = new Vector2(screenWidth / 2, screenHeight - playerSize * 2);
                player = new Player(playerStart, new Vector2(0, 0), playerSpeed, playerSize, playerImage, playerhit);
                for (int i = 0; i < numEnemies; i++)
                {
                    int row = i / 5; 
                    int col = i % 5; 
                    Vector2 position = new Vector2(col * 100 + 100, row * 100 + 100);
                    Enemy enemy = new Enemy(position, new Vector2(1, 0), 50, enemyImage, new List<Player> { player }, playerhit);
                    enemies.Add(enemy);
                }
                gameStarted = true;
                gameOver = false;
            }
        }

        void drawGame()
        {
            Raylib.ClearBackground(Raylib.WHITE);
            player.Draw();
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw();
            }
            player.DrawScore();

            // Draw player health
            string healthText = "Health: " + player.health.ToString();
            Raylib.DrawText(healthText, 10, 10, 20, Raylib.BLACK);
        }

        void UpdateEnemies()
        {
            bool wallHit = false;
            float enemySpeed = speed;

            foreach (Enemy enemy in enemies)
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

            // Check if the enemies should change direction 
            if (enemies.Count > 0 && moveRight && enemies.Last().position.X >= screenWidth - 20)
            {
                moveRight = false;
                shouldChangeDirection = true;
                position.Y += 10; // add a small vertical offset
            }

            else if (enemies.Count > 0 && !moveRight && enemies.First().position.X <= 20)
            {
                moveRight = true;
                shouldChangeDirection = true;
                position.Y += 10; // add a small vertical offset
            }

            // If the enemies should change direction, change their direction and move them down
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
        }
    }
}
