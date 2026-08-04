// Inside your Asteroid class/struct
using System.Numerics;

namespace B2Asteroids
{
    public class Bullet: Polygon
    {
        private const double BulletSpeed = 7.0;
        private const double MaxDistance = 300;
        public int Distance { get; set; }


        public Bullet(Vector2 position, int heading): base(0)
        {
            Position = position;
            Heading = heading;
            Distance = 0;
        }

        public bool MoveBullet(Form parentWindow)
        {
            Position.X += (float)(BulletSpeed * Math.Sin(Heading * (Math.PI / 180.0)));
            Position.Y -= (float)(BulletSpeed * Math.Cos(Heading * (Math.PI / 180.0)));
            Position = ClipPosition(Position, parentWindow);

            // Check if maximum distance flown
            Distance += (int)BulletSpeed;
            return Distance > MaxDistance;
        }
    }
}
