using Raylib_CsLo;
using System.Numerics;

namespace Spaceinvaideri
{
    internal class Enemy
    {
        public List<Bullet> bullets;
        public float bulletCooldown = 0;
        public float bulletCooldownMax = 75000;

        public Vector2 position;
        public Vector2 direction;
        public int health;

        public bool isAlive = true;

        private Texture texture;

        public List<Player> players;
        public float speed;
        public float size;
        public float scale;
        public Sound TakeDamage;
        private bool isDestroyed = false;

        public Enemy(Vector2 position, Vector2 direction, int health, Texture enemy1, List<Player> players, Sound TakeDamage, float speed = 100f, float size = 1f, float scale = 0.5f)
        {
            this.position = position;
            this.health = health;
            this.direction = direction;
            this.bullets = new List<Bullet>();
            this.players = players;
            texture = enemy1;
            this.speed = speed * Raylib.GetFrameTime();
            this.size = size;
            this.scale = scale;
            this.TakeDamage = TakeDamage;
            bulletCooldownMax = Raylib.GetRandomValue(1, 100000);
        }

        public void Update(Player player)
        {
            bulletCooldown -= Raylib.GetFrameTime() * 500;

            foreach (Bullet bullet in bullets.ToList())
            {
                bullet.Update();

                foreach (Player otherPlayer in players)
                {
                    if (Raylib.CheckCollisionCircles(bullet.position, 5, player.position, 20))
                    {
                        player.health -= 10;
                        Raylib.PlaySound(TakeDamage);
                        bullets.Remove(bullet);
                        break;
                    }
                }
            }

            bullets.RemoveAll(bullet => bullet.position.Y < 0 || bullet.position.Y > Raylib.GetScreenHeight());

            if (bulletCooldown <= 0)
            {
                bulletCooldown = bulletCooldownMax;

                Vector2 bulletPosition = new Vector2(position.X + texture.width * scale / 2, position.Y + texture.height * scale);
                Vector2 bulletDirection = new Vector2(0, 1);
                Bullet bullet = new Bullet(bulletPosition, bulletDirection, 2f, 1f, Raylib.RED);
                bullets.Add(bullet);
            }
        }

        public void InflictDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                isAlive = false;
            }
        }

        public bool IsDestroyed
        {
            get { return isDestroyed; }
            set { isDestroyed = value; }
        }

        public void Draw()
        {
            Raylib.DrawTextureEx(texture, position, 0f, 0.5f, Raylib.WHITE);

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw();
            }
        }
    }
}
