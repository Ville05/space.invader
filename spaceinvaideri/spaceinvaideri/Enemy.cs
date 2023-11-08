
using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Spaceinvaders
{
    internal class Enemy
    {
        public List<Bullet> bullets;
        public float bulletCooldown = 0;
        public float bulletCooldownMax = 12000;

        public Transform transform;

        public Vector2 position;
        public Vector2 direction;
        public int health;

        public bool isAlive = true;

        public Texture myImage;

        public List<Player> players;
        public float speed;
        public float size;
        public float scale;
        public Sound playerDmg;

        public Enemy(Vector2 position, Vector2 direction, int health,
            Texture enemyImage, List<Player> players,Sound playerdamage, float speed = 10f, float size = 1f, float scale = 0.5f)
        {
            this.position = position;
            this.health = health;
            this.direction = direction;

            this.bullets = new List<Bullet>();
            this.players = players;

            myImage = enemyImage;

            this.speed = speed;
            this.size = size;
            this.scale = scale;
            this.playerDmg = playerdamage;
          

            
            // Set the initial bullet cooldown time to a random value between 5 and 12 seconds
            bulletCooldownMax = Raylib.GetRandomValue(5000, 15000);
        }

        public void Update(Player player)
        {

            // Subtract time since last update from the bullet cooldown time
            bulletCooldown -= Raylib.GetFrameTime() * 1000;

            // Move enemy bullets and check for collisions
            foreach (Bullet bullet in bullets.ToList())
            {
                bullet.Update();

                foreach (Player otherPlayer in players)
                {
                    if (Raylib.CheckCollisionCircles(bullet.position, 5, player.position, 20))
                    {
                        // Reduce player health
                        player.health -= 10;

                        Raylib.PlaySound(playerDmg);

                        bullets.Remove(bullet);
                        break;
                    }
                }
            }

            // Remove bullets that leave the screen
            bullets.RemoveAll(bullet => bullet.position.Y < 0 || bullet.position.Y > Raylib.GetScreenHeight());

            // Check if it's time to shoot
            if (bulletCooldown <= 0)
            {
                // Reset bullet cooldown time to bulletCooldownMax
                bulletCooldown = bulletCooldownMax;

                // Create a new bullet and add it to the list of bullets
                Vector2 bulletPosition = new Vector2(position.X + myImage.width * scale / 2, position.Y + myImage.height * scale);
                Vector2 bulletDirection = new Vector2(0, 1);
                Bullet bullet = new Bullet(bulletPosition, bulletDirection, 0.3f, 1f, Raylib.RED);
                bullets.Add(bullet);
            }
        }

        public void Draw()
        {
            Raylib.DrawTextureEx(myImage, position, 0f, 0.5f, Raylib.WHITE);

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw();
            }
        }
    }
}
