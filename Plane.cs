// Inside your Asteroid class/struct
using System.Numerics;

namespace Asteroids
{
    public class Plane: Polygon
    {
        private SolidBrush PlaneBrush;
        private SolidBrush ThrustBrush;

        private const int PlanePoints = 12;
        private const int ThrustPoints = 3;

        private PointF[]  ThrustShapeVertices;
        private PointF[]  ThrustPositionVertices;

        public double SpeedX;
        public double SpeedY;

        // Call this when spawning the plane
        public Plane(): base(PlanePoints)
        {
            PlaneBrush = new SolidBrush(Color.Gray);
            ThrustBrush = new SolidBrush(Color.Orange);

            ShapeVertices = new PointF[VertexCount];
            PositionVertices = new PointF[VertexCount];
            
            ThrustShapeVertices = new PointF[ThrustPoints];
            ThrustPositionVertices = new PointF[ThrustPoints];
                    
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

            ThrustShapeVertices[0] = new Point(0, 18);
            ThrustShapeVertices[1] = new Point(2, 10);
            ThrustShapeVertices[2] = new Point(-2, 10);
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

            // Calculate thrust points
            for (int i = 0; i < ThrustPoints; i++)
            {
                ThrustPositionVertices[i].X = (float)(Position.X + ThrustShapeVertices[i].X * Math.Cos(Heading * (Math.PI / 180.0)) - 
                        ThrustShapeVertices[i].Y * Math.Sin(Heading * (Math.PI / 180.0)));
                ThrustPositionVertices[i].Y = (float)(Position.Y + ThrustShapeVertices[i].X * Math.Sin(Heading * (Math.PI / 180.0)) + 
                        ThrustShapeVertices[i].Y * Math.Cos(Heading * (Math.PI / 180.0)));
            }
        }

        public override void Draw(Graphics gc)
        {
            // Draw just the plane
            gc.FillPolygon(this.PlaneBrush, this.PositionVertices);
        }

        public void Draw(Graphics gc, bool isAccelerating)
        {
            // Draw plane
            Draw(gc);

            // Draw flame if accelerating
            if (isAccelerating)
            {
                // Draw thrust
                gc.FillPolygon(this.ThrustBrush, this.ThrustPositionVertices);
            }
        }
    }
}

