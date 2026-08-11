// Inside your Asteroid class/struct
using System.Drawing;
using System.Numerics;

namespace Asteroids
{
    public class Asteroid: Polygon
    {
        private const int AsteroidPoints = 8;

        public static Random RandomNumberGenerator = new Random();

        public double Radius; // Used for broad-phase collision / bounding circle
        public Color Color { get; private set; }
        public SolidBrush Brush { get; private set; }

        private static Color MakeRandomColor()
        {
            return Color.FromArgb(
                255,
                RandomNumberGenerator.Next(80, 256),
                RandomNumberGenerator.Next(80, 256),
                RandomNumberGenerator.Next(80, 256));
        }

        // Call this when spawning the asteroid
        public Asteroid(double radius, Vector2 position, int heading, double speed, Form parentWindow): base(AsteroidPoints)
        {
            // Define initial shape
            ShapeVertices = new PointF[VertexCount];
            PositionVertices = new PointF[VertexCount];
            float angleStep = MathF.Tau / VertexCount;
            Radius = radius;
            Heading = heading;
            Speed = speed;
            Position = position;

            Color = MakeRandomColor();
            Brush = new SolidBrush(Color);

            for (int i = 0; i < VertexCount; i++)
            {
                double angle = i * angleStep;
                // Vary the radius slightly for an irregular jagged look
                double variance = 0.7f + 0.6f * RandomNumberGenerator.NextDouble();
                double r = Radius * variance;

                ShapeVertices[i] = new PointF((float)(Math.Cos(angle) * r), (float)(Math.Sin(angle) * r));
            }

        }
        public override void Draw(Graphics gc)
        {
            // Draw hard-edged shadow/highlight for cel-shading effect
            PointF[] shadowVertices = new PointF[PositionVertices.Length];
            for (int i = 0; i < PositionVertices.Length; i++)
            {
                shadowVertices[i] = new PointF(PositionVertices[i].X + 4, PositionVertices[i].Y + 4);
            }

            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(100, Color.Black)))
            {
                gc.FillPolygon(shadowBrush, shadowVertices);
            }

            // Draw the main asteroid body with its flat color
            gc.FillPolygon(this.Brush, this.PositionVertices);

            // Draw a thick, cartoon-style black outline
            using (Pen outlinePen = new Pen(Color.Black, 3))
            {
                gc.DrawPolygon(outlinePen, this.PositionVertices);
            }
        }

        public void MoveAsteroid(Form parentWindow)
        {
            // Compute new position
            Position.X += (float)(Speed * Math.Sin(Heading));
            Position.Y += (float)(Speed * Math.Cos(Heading));

            Position = ClipPosition(Position, parentWindow);

            for (int i = 0; i < VertexCount; i++)
            {
                PositionVertices[i].X = ShapeVertices[i].X + Position.X;
                PositionVertices[i].Y = ShapeVertices[i].Y + Position.Y;
            }
        }
    }
}

