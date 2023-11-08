
using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Spaceinvaders
{
    internal class Player
    {
        public static int numEnemies = 5;

        public int score = 0;

        public Vector2 position;
        public float speed;
        public int health;
        public float radius;
        public List<Bullet> bullets;

        public Texture myImage;

        public Sound shootSound;
        public Sound enemyDeath;

        private bool useMouse = false;
        private bool useKeyboardImage = false;

        public Player(Vector2 position, Vector2 vector2, float speed, int health, Texture playerImage, Sound enemyDeath)
        {
            this.position = position;
            this.speed = speed / 100;
            this.health = health;
            this.bullets = new List<Bullet>();
            this.enemyDeath = enemyDeath;

            myImage = playerImage;

            // Start with keyboard image
            useMouse = false;
            useKeyboardImage = true;

      
        }
        
        public void DrawScore()
        {
            Raylib.DrawText($"Points: {score}", Raylib.GetScreenWidth() - 150, 20, 20, Raylib.BLACK);
        }

        public void Update(List<Enemy> enemies)
        {
            // Toggle between mouse and keyboard control
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
            {
                useMouse = !useMouse;
                useKeyboardImage = !useKeyboardImage;
            }

            // Move player left and right
            if (!useMouse)
            {
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT) && position.X < Raylib.GetScreenWidth() - 20)
                    position.X += speed;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT) && position.X > 20)
                    position.X -= speed;

            }

            // Move player left and right with mouse
            else
            {
                Vector2 mousePosition = Raylib.GetMousePosition();
                if (mousePosition.X < position.X && position.X > 20)
                {
                    position.X -= speed;
                }
                else if (mousePosition.X > position.X && position.X < Raylib.GetScreenWidth() - 20)
                {
                    position.X += speed;
                }
            }

            // Fire bullets
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                bullets.Add(new Bullet(new Vector2(position.X, position.Y - 20), new Vector2(0, -5), 0.1f, 1, Raylib.RED));

                Raylib.PlaySound(shootSound);
            }

            // Update bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();

                bool hitEnemy = false;
                int defeatedEnemyIndex = -1;

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    if (Raylib.CheckCollisionCircles(bullets[i].position, 5, enemies[j].position, 20))
                    {
                        // Remove the bullet and the enemy
                        bullets.RemoveAt(i);
                        defeatedEnemyIndex = j;
                        enemies.RemoveAt(j);
                        numEnemies--;

                        // Add points to the score
                        score += 10;

                        // Play a sound effect
                        Raylib.PlaySound(enemyDeath);

                        // Set hitEnemy flag to true
                        hitEnemy = true;

                        break;
                    }
                }

                if (hitEnemy)
                {
                    hitEnemy = false;
                    break;
                }

                // Remove bullets that leave the screen
                if (bullets[i].position.Y < 0 || bullets[i].position.Y > Raylib.GetScreenHeight())
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        public void Draw()
        {
            // Draw player image
            Raylib.DrawTexture(myImage, (int)position.X - myImage.width / 3, (int)position.Y - myImage.height / 3, Raylib.WHITE);

            // Draw control text
            string controlText = "";
            if (useMouse)
            {
                controlText = "Control: Mouse";
            }
            else if (useKeyboardImage)
            {
                controlText = "Control: Keyboard";
            }
            Raylib.DrawText(controlText, Raylib.GetScreenWidth() / 2 - 80, 10, 20, Raylib.BLACK);

            // Draw bullets
            foreach (Bullet bullet in bullets)
            {
                bullet.Update();
                bullet.Draw();
            }
        }
    }
}
