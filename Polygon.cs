// Inside your Asteroid class/struct
using System.Numerics;

namespace Asteroids
{

    public abstract class Polygon
    {
        public int VertexCount;
        public Vector2 Position;
        public Vector2 Velocity;
        public int Heading;
        public double Speed;

        public PointF[] ShapeVertices; // Shape centered at (0,0)

        public PointF[] PositionVertices; // PositionVertices; // Shape in world space

        public abstract void Draw(Graphics gc);

        public Polygon(int vertexCount)
        {
            VertexCount = vertexCount;
            ShapeVertices = new PointF[vertexCount];
            PositionVertices = new PointF[vertexCount];
        }

        public double  HeadingRadians
        {
            get
            {
                return Heading * (Math.PI / 180.0);
            }
        }

        public Vector2 ClipPosition(Vector2 position, Form parentWindow)
        {
            if (position.X < 0)
                position.X += parentWindow.Width;
            else if (position.X > parentWindow.Width)
                position.X -= parentWindow.Width;
            if (position.Y < 0)
                position.Y += parentWindow.Height;
            else if (position.Y > parentWindow.Height)
                position.Y -= parentWindow.Height;

            return position;
        }

        public bool CheckCollision(Vector2 localPoint)
        {
            bool inside = false;
            int j = PositionVertices.Length - 1;

            for (int i = 0; i < PositionVertices.Length; i++)
            {
                if ((PositionVertices[i].Y > localPoint.Y) != (PositionVertices[j].Y > localPoint.Y) &&
                    (localPoint.X < (PositionVertices[j].X - PositionVertices[i].X) * (localPoint.Y - PositionVertices[i].Y) / (PositionVertices[j].Y - PositionVertices[i].Y) + PositionVertices[i].X))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }
    }
}
