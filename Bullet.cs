// Inside your Asteroid class/struct
using System.Numerics;

namespace Asteroids
{
    public class Bullet: Polygon
    {
        private SolidBrush BulletBrush;

        private const double BulletSpeed = 7.0;
        private const double MaxDistance = 300;
        public int Distance { get; set; }


        public Bullet(Vector2 position, int heading, SolidBrush brush) : base(0)
        {
            Position = position;
            Heading = heading;
            Distance = 0;
            BulletBrush = brush;
        }

        public bool MoveBullet(Form parentWindow)
        {
            Position.X += (float)(BulletSpeed * Math.Sin(HeadingRadians));
            Position.Y -= (float)(BulletSpeed * Math.Cos(HeadingRadians));
            Position = ClipPosition(Position, parentWindow);

            // Check if maximum distance flown
            Distance += (int)BulletSpeed;
            return Distance > MaxDistance;
        }

        public override void Draw(Graphics gc)
        {
            gc.FillRectangle(this.BulletBrush, this.Position.X - 1, this.Position.Y - 1, 3, 3);
        }
    }
}
