using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public static class CollisionDetection
    {
        public static bool IsThereCollision(GameObject a, GameObject b)
        {
            bool collision = false;

            double rotation = a.Rotation;

            List<Vector2> a_axes = GetTestableAxes(a);
            List<Vector2> b_axes = GetTestableAxes(b);

            List<Vector2> a_edges = GetEdges(a);
            List<Vector2> b_edges = GetEdges(b);

            List<Point> a_vertices = GetVertices(a);
            List<Point> b_vertices = GetVertices(b);

            foreach(Vector2 axle in a_axes)
            {
                
            }

            return collision;
        }

        private static List<Point> GetVertices(GameObject a)
        {
            double rotation = a.Rotation;

            Point a_corner1_not_rotated = new Point((int)(a.Position.X + a.Width / 2), (int)(a.Position.Y + a.Height / 2));
            Point a_corner2_not_rotated = new Point((int)(a.Position.X + a.Width / 2), (int)(a.Position.Y - a.Height / 2));
            Point a_corner3_not_rotated = new Point((int)(a.Position.X - a.Width / 2), (int)(a.Position.Y - a.Height / 2));
            Point a_corner4_not_rotated = new Point((int)(a.Position.X - a.Width / 2), (int)(a.Position.Y + a.Height / 2));
            Point a_corner1_rotated = RotatePoint(a_corner1_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner2_rotated = RotatePoint(a_corner2_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner3_rotated = RotatePoint(a_corner3_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner4_rotated = RotatePoint(a_corner4_not_rotated, a.Position.ToPoint(), rotation);

            List<Point> test = new List<Point>();
            test.Add(a_corner1_rotated);
            test.Add(a_corner2_rotated);
            test.Add(a_corner3_rotated);
            test.Add(a_corner4_rotated);

            return test;
        }

        private static List<Vector2> GetEdges(GameObject a)
        {
            double rotation = a.Rotation;

            Point a_corner1_not_rotated = new Point((int)(a.Position.X + a.Width / 2), (int)(a.Position.Y + a.Height / 2));
            Point a_corner2_not_rotated = new Point((int)(a.Position.X + a.Width / 2), (int)(a.Position.Y - a.Height / 2));
            Point a_corner3_not_rotated = new Point((int)(a.Position.X - a.Width / 2), (int)(a.Position.Y - a.Height / 2));
            Point a_corner4_not_rotated = new Point((int)(a.Position.X - a.Width / 2), (int)(a.Position.Y + a.Height / 2));
            Point a_corner1_rotated = RotatePoint(a_corner1_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner2_rotated = RotatePoint(a_corner2_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner3_rotated = RotatePoint(a_corner3_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner4_rotated = RotatePoint(a_corner4_not_rotated, a.Position.ToPoint(), rotation);

            List<Vector2> test = new List<Vector2>();
            test.Add(GetVectorBetweenPoints(a_corner1_rotated, a_corner2_rotated));
            test.Add(GetVectorBetweenPoints(a_corner2_rotated, a_corner3_rotated));
            test.Add(GetVectorBetweenPoints(a_corner3_rotated, a_corner4_rotated));
            test.Add(GetVectorBetweenPoints(a_corner4_rotated, a_corner1_rotated));

            return test;
        }

        private static List<Vector2> GetTestableAxes(GameObject a)
        {
            double rotation = a.Rotation;

            Point a_corner1_not_rotated = new Point((int)(a.Position.X + a.Width / 2), (int)(a.Position.Y + a.Height / 2));
            Point a_corner2_not_rotated = new Point((int)(a.Position.X + a.Width / 2), (int)(a.Position.Y - a.Height / 2));
            Point a_corner3_not_rotated = new Point((int)(a.Position.X - a.Width / 2), (int)(a.Position.Y - a.Height / 2));
            Point a_corner4_not_rotated = new Point((int)(a.Position.X - a.Width / 2), (int)(a.Position.Y + a.Height / 2));
            Point a_corner1_rotated = RotatePoint(a_corner1_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner2_rotated = RotatePoint(a_corner2_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner3_rotated = RotatePoint(a_corner3_not_rotated, a.Position.ToPoint(), rotation);
            Point a_corner4_rotated = RotatePoint(a_corner4_not_rotated, a.Position.ToPoint(), rotation);

            Vector2 a_axis1 = GetPerpendicularVector(GetVectorBetweenPoints(a_corner1_rotated, a_corner2_rotated));
            Vector2 a_axis2 = GetPerpendicularVector(GetVectorBetweenPoints(a_corner2_rotated, a_corner3_rotated));

            a_axis1.Normalize();
            a_axis2.Normalize();

            return new List<Vector2>() { a_axis1, a_axis2 };
        }

        private static Point RotatePoint(Point p, Point center, double rotation)
        {
            float tempX = p.X - center.X;
            float tempY = p.Y - center.Y;

            float rotatedX = (float)(tempX * Math.Cos(rotation) - tempY * Math.Sin(rotation));
            float rotatedY = (float)(tempX * Math.Sin(rotation) + tempY * Math.Cos(rotation));

            return new Point((int)(rotatedX + center.X), (int)(rotatedY + center.Y));
        }

        private static Vector2 GetPerpendicularVector(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        private static Vector2 GetVectorBetweenPoints(Point a, Point b)
        {
            return new Vector2((float)a.X - b.X, (float)a.Y - b.Y);
        }
    }
}
