// Inside your Asteroid class/struct
using System.Numerics;

namespace Asteroids
{
    public class Plane: Polygon
    {
        private SolidBrush PlaneBrush;

        private const int PlanePoints = 12;
        
        public double SpeedX;
        public double SpeedY;

        // Call this when spawning the plane
        public Plane(): base(PlanePoints)
        {
            PlaneBrush = new SolidBrush(Color.Gray);

            ShapeVertices = new PointF[VertexCount];
            PositionVertices = new PointF[VertexCount];
                    
            ShapeVertices[0] = new Point(0, 4);
            ShapeVertices[1] = new Point(2, 2);
            ShapeVertices[2] = new Point(3, 4);
            ShapeVertices[3] = new Point(5, 2);
            ShapeVertices[4] = new Point(10, 5);
            ShapeVertices[5] = new Point(13, 2);
            ShapeVertices[6] = new Point(0, -12);
            ShapeVertices[7] = new Point(-13, 2);
            ShapeVertices[8] = new Point(-10, 5);
            ShapeVertices[9] = new Point(-5, 2);
            ShapeVertices[10] = new Point(-3, 4);
            ShapeVertices[11] = new Point(-2, 2);
        }

        public void SetLocation(Vector2 position, int heading, double speedX, double speedY, Form parentWindow)
        {
            Position = position;
            Heading = heading;
            SpeedX = speedX;
            SpeedY = speedY;

            // Compute new position based on heading and speed
            Position.X += (float) speedX;
            Position.Y += (float) speedY;
            Position = ClipPosition(Position, parentWindow);

            // Compute drawing points
            for (int i = 0; i < PlanePoints; i++)
            {
                PositionVertices[i].X = (float)(Position.X + ShapeVertices[i].X * Math.Cos(Heading * (Math.PI / 180.0)) - 
                        ShapeVertices[i].Y * Math.Sin(Heading * (Math.PI / 180.0)));
                PositionVertices[i].Y = (float)(Position.Y + ShapeVertices[i].X * Math.Sin(Heading * (Math.PI / 180.0)) + 
                        ShapeVertices[i].Y * Math.Cos(Heading * (Math.PI / 180.0)));
            }
        }

        public override void Draw(Graphics gc)
        {
            gc.FillPolygon(this.PlaneBrush, this.PositionVertices);
        }
    }
}

