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
            Vector2 aP = a.Position;
            Vector2 aX = RotateVector(Vector2.UnitX, a.Rotation);
            Vector2 aY = RotateVector(Vector2.UnitY, a.Rotation);
            float aW = (float)a.Width / 2;
            float aH = (float)a.Height / 2;

            Vector2 bP = b.Position;
            Vector2 bX = RotateVector(Vector2.UnitX, b.Rotation);
            Vector2 bY = RotateVector(Vector2.UnitY, b.Rotation);
            float bW = (float)b.Width / 2;
            float bH = (float)b.Height / 2;

            Vector2 T = Vector2.Subtract(bP, aP);

            List<Vector2> testableAxes = new() { aX, aY, bX, bY };

            foreach(Vector2 L in testableAxes)
            {
                float TdotL = Math.Abs(Vector2.Dot(T, L));

                float proj1 = Math.Abs(Vector2.Dot(aW * aX, L));
                float proj2 = Math.Abs(Vector2.Dot(aH * aY, L));
                float proj3 = Math.Abs(Vector2.Dot(bW * bX, L));
                float proj4 = Math.Abs(Vector2.Dot(bH * bY, L));

                //Debug.WriteLine(TdotL);
                //Debug.WriteLine(proj1);
                //Debug.WriteLine(proj2);
                //Debug.WriteLine(proj3);
                //Debug.WriteLine(proj4);

                // If true, separating axis was found
                if (TdotL > proj1 + proj2 + proj3 + proj4)
                {
                    Debug.WriteLine("NO COLLISON!");
                    return false;
                }
            }

            Debug.WriteLine("COLLISON!");
            return true;
        }

        private static Vector2 RotateVector(Vector2 v, double rotation)
        {
            return new Vector2((float)(v.X * Math.Cos(rotation) - v.Y * Math.Sin(rotation)), (float)(v.X * Math.Sin(rotation) + v.Y * Math.Cos(rotation)));
        }
    }
}
