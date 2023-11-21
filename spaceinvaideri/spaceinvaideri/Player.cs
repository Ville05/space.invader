using Raylib_CsLo;
using System.Numerics;

namespace Spaceinvaideri
{
    internal class Player
    {
        public static int numEnemies = 5;
        public int score = 0;
        public Vector2 position;
        public float speed;
        public int health = 50;
        public float radius;
        public List<Bullet> bullets;
        private Texture texture;
        public Sound GiveDamage;
        private bool useMouse = false;
        private bool useKeyboardImage = false;
        private int lastRecordedScore = 0;

        public Player(Vector2 position, Vector2 vector2, float speed, int health, Texture playerImage, Sound GiveDamage)
        {
            float delta = 1;
            this.position = position;
            this.speed = speed * delta;
            this.health = health = 50;
            this.bullets = new List<Bullet>();
            this.GiveDamage = GiveDamage;
            this.texture = playerImage;
            useMouse = false;
            useKeyboardImage = true;
        }

        public void DrawScore()
        {
            Raylib.DrawText($"Points: {score}", Raylib.GetScreenWidth() - 150, 20, 20, Raylib.BLACK);
        }

        public void Update(List<Enemy> enemies)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
            {
                useMouse = !useMouse;
                useKeyboardImage = !useKeyboardImage;
            }

            if (!useMouse)
            {
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D) && position.X < Raylib.GetScreenWidth() - 20)
                    position.X += speed;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A) && position.X > 20)
                    position.X -= speed;
            }
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

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                bullets.Add(new Bullet(new Vector2(position.X, position.Y - 20), new Vector2(0, -5), 0.1f, 1, Raylib.RED));
                Raylib.PlaySound(GiveDamage);
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                bool hitEnemy = false;
                int defeatedEnemyIndex = -1;

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    if (Raylib.CheckCollisionCircles(bullets[i].position, 5, enemies[j].position, 20))
                    {
                        bullets.RemoveAt(i);
                        defeatedEnemyIndex = j;
                        enemies.RemoveAt(j);
                        numEnemies--;

                        score += 10;

                        if (score - lastRecordedScore >= 10)
                        {
                            invaideri.destroyedEnemies += (score - lastRecordedScore) / 10;
                            lastRecordedScore = score;
                        }
                        Raylib.PlaySound(GiveDamage);
                        hitEnemy = true;
                        break;
                    }
                }

                if (hitEnemy)
                {
                    hitEnemy = false;
                    break;
                }

                if (bullets[i].position.Y < 0 || bullets[i].position.Y > Raylib.GetScreenHeight())
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        public void Draw()
        {
            Raylib.DrawTexture(texture, (int)position.X - texture.width / 3, (int)position.Y - texture.height / 3, Raylib.WHITE);

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

            foreach (Bullet bullet in bullets)
            {
                bullet.Update();
                bullet.Draw();
            }
        }
    }
}
