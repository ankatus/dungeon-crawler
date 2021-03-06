using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.VisualBasic;

namespace DungeonCrawler
{
    public static class CollisionDetection
    {
        public static bool IsThereCollision(GameObject a, GameObject b)
        {
            Vector2 aP = a.Position;
            Vector2 aX = RotateVector(Vector2.UnitX, a.Rotation);
            Vector2 aY = RotateVector(Vector2.UnitY, a.Rotation);
            float aW = (float) a.Width / 2;
            float aH = (float) a.Height / 2;

            Vector2 bP = b.Position;
            Vector2 bX = RotateVector(Vector2.UnitX, b.Rotation);
            Vector2 bY = RotateVector(Vector2.UnitY, b.Rotation);
            float bW = (float) b.Width / 2;
            float bH = (float) b.Height / 2;

            Vector2 T = Vector2.Subtract(bP, aP);

            List<Vector2> testableAxes = new() {aX, aY, bX, bY};

            foreach (Vector2 L in testableAxes)
            {
                float TdotL = Math.Abs(Vector2.Dot(T, L));

                float proj1 = Math.Abs(Vector2.Dot(aW * aX, L));
                float proj2 = Math.Abs(Vector2.Dot(aH * aY, L));
                float proj3 = Math.Abs(Vector2.Dot(bW * bX, L));
                float proj4 = Math.Abs(Vector2.Dot(bH * bY, L));

                // If true, separating axis was found
                if (TdotL > proj1 + proj2 + proj3 + proj4)
                {
                    return false;
                }
            }

            return true;
        }

        public static List<GameObject> GetCollisions(GameObject collider, List<GameObject> gameObjects)
        {
            var found = new List<GameObject>();

            foreach (var current in gameObjects)
            {
                if (collider.Id != current.Id && IsThereCollision(collider, current))
                {
                    found.Add(current);
                }
            }

            return found;
        }

        public static bool IsThereOverlap(GameObject a, GameObject b)
        {
            Vector2 aP = a.Position;
            Vector2 aX = RotateVector(Vector2.UnitX, a.Rotation);
            Vector2 aY = RotateVector(Vector2.UnitY, a.Rotation);
            float aW = (float) a.Width / 2;
            float aH = (float) a.Height / 2;

            Vector2 bP = b.Position;
            Vector2 bX = RotateVector(Vector2.UnitX, b.Rotation);
            Vector2 bY = RotateVector(Vector2.UnitY, b.Rotation);
            float bW = (float) b.Width / 2;
            float bH = (float) b.Height / 2;

            Vector2 T = Vector2.Subtract(bP, aP);

            List<Vector2> testableAxes = new() {aX, aY, bX, bY};

            foreach (Vector2 L in testableAxes)
            {
                float TdotL = Math.Abs(Vector2.Dot(T, L));

                float proj1 = Math.Abs(Vector2.Dot(aW * aX, L));
                float proj2 = Math.Abs(Vector2.Dot(aH * aY, L));
                float proj3 = Math.Abs(Vector2.Dot(bW * bX, L));
                float proj4 = Math.Abs(Vector2.Dot(bH * bY, L));

                // If true, separating axis was found
                if (TdotL >= proj1 + proj2 + proj3 + proj4)
                {
                    return false;
                }
            }

            return true;
        }

        public static List<GameObject> GetOverlaps(GameObject collider, List<GameObject> gameObjects)
        {
            var found = new List<GameObject>();

            foreach (var current in gameObjects)
            {
                if (collider.Id != current.Id && IsThereOverlap(collider, current))
                {
                    found.Add(current);
                }
            }

            return found;
        }

        public static Vector2 RotateVector(Vector2 v, double rotation)
        {
            return new((float) (v.X * Math.Cos(rotation) - v.Y * Math.Sin(rotation)),
                (float) (v.X * Math.Sin(rotation) + v.Y * Math.Cos(rotation)));
        }
    }
}