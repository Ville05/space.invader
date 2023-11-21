using Raylib_CsLo;
using System.Numerics;

namespace Spaceinvaideri
{
    internal class Bullet
    {
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public float radius;

        public Vector2 velocity;

        public Bullet(Vector2 position, Vector2 direction, float speed, float radius, Color color)
        {
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.radius = radius;
            this.velocity = direction * speed * 2;
        }

        public void Update()
        {
            position += velocity;
        }

        public void Draw()
        {
            Raylib.DrawCircle((int)position.X, (int)position.Y, 5, Raylib.RED);
        }
    }
}
