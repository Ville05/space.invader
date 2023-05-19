using System;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace SpaceInvaders
{
    class Program
    {
        public static List<Enemy> enemies = new List<Enemy>();

        
        public class Player
        {
            public Vector2 position;
            public float speed;
            public int health;
            public List<Bullet> bullets;

            public Texture2D texture;

            public Player(Vector2 position, float speed, int health)
            {
                this.position = position;
                this.speed = speed / 20; 
                this.health = health;
                this.bullets = new List<Bullet>();

                this.texture = Raylib.LoadTexture("player.png");
            }

            public void Update(List<Enemy> enemies)
            {
                
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT) && position.X < Raylib.GetScreenWidth() - 20)
                    position.X += speed;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT) && position.X > 20)
                    position.X -= speed;

                
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    bullets.Add(new Bullet(new Vector2(position.X, position.Y - 20), new Vector2(0, -10)));

               
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    bullets[i].Update();

                    bool hitEnemy = false;
                   
                    for (int j = enemies.Count - 1; j >= 0; j--)
                    {
                        if (Raylib.CheckCollisionCircles(bullets[i].position, 5, enemies[j].position, 20))
                        {
                            
                            enemies[j].health -= 20;

                            bullets.RemoveAt(i);
                            hitEnemy = true;

                            if (enemies[j].health <= 0)
                            {
                                enemies.RemoveAt(j);
                            }

                            break;
                        }
                    }

                    if (!hitEnemy && bullets[i].position.Y < 0)
                        bullets.RemoveAt(i);
                }
            }


            public void Draw()
            {
                Raylib.DrawTexture(texture, (int)position.X - texture.width / 3, (int)position.Y - texture.height / 3, Color.WHITE);

                foreach (Bullet bullet in bullets)
                    bullet.Draw();

                Raylib.DrawText("elämät: " + health, 10, 10, 20, Color.RED);
            }
        }

        //vihollisen säädöt
        public class Enemy
        {
            public static float speed = 0.3f;
            public static bool shouldChangeDirection = false;

            public Vector2 position;
            public int health;

            private Texture2D texture;

            private bool moveDown = false;
            private bool moveRight = true;


            public Enemy(Vector2 position, int health, string texturePath, object transform)
            {
                this.position = position;
                this.health = health;

                texture = Raylib.LoadTexture(texturePath);
            }

            public void Update()
            {
                if (moveRight)
                {
                    position.X += speed;

                    if (position.X >= 800 - 50)
                    {
                        moveRight = false;
                        shouldChangeDirection = true;
                        position.Y += 10; 
                    }
                }
                else
                {
                    position.X -= speed;

                    if (position.X <= 20)
                    {
                        moveRight = true;
                        shouldChangeDirection = true;
                        position.Y += 10; 
                    }
                }

                if (moveDown)
                {
                    position.Y += 20;
                    moveDown = false;
                }
            }

            public void Draw()
            {
                Raylib.DrawTextureEx(texture, position, 0f, 0.5f, Color.WHITE);
            }

        }

        // luoti hommat
        public class Bullet
        {
            public Vector2 position;
            public Vector2 velocity;

            public Bullet(Vector2 position, Vector2 velocity)
            {
                this.position = position;
                this.velocity = velocity / 5;
            }

            public void Update()
            {
                position += velocity;
            }

            public void Draw()
            {
                Raylib.DrawCircle((int)position.X, (int)position.Y, 5, Color.GOLD);
            }
        }

        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 1000;

            Raylib.InitWindow(screenWidth, screenHeight, "raylib [core] example - basic window");

            Player player = new Player(new Vector2(screenWidth / 2, screenHeight - 50), 10, 100);

            List<Enemy> enemies = new List<Enemy>();
            for (int i = 0; i < 5; i++)
                enemies.Add(new Enemy(new Vector2(100 + i * 100, 100), 4, "enemy.png", null));

            while (!Raylib.WindowShouldClose())
            {

                player.Update(enemies);
                foreach (Enemy enemy in enemies)
                    enemy.Update();

                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.BLACK);

                player.Draw();
                foreach (Enemy enemy in enemies)
                    enemy.Draw();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
