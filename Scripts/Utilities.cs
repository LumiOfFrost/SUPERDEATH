namespace SUPERDEATH.Scripts
{

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Runtime;

    

    public class Transform
    {

        public Vector3 position;
        public Vector3 scale;
        public Vector3 rotation;

        public Transform(Vector3 pos, Vector3 scl, Vector3 rot)
        {

            position = pos;
            scale = scl;
            rotation = rot;

        }

        public static Transform Zero()
        {

            return new Transform(Vector3.Zero, Vector3.Zero, Vector3.Zero);

        }
    
    }

    public class MathUtils
    {

        public static Vector2 RoundVector2(Vector2 vector)
        {

            return new Vector2((float)Math.Round(vector.X), (float)Math.Round(vector.Y));

        }
        public static Vector3 RoundVector3(Vector3 vector)
        {

            return new Vector3((float)Math.Round(vector.X), (float)Math.Round(vector.Y), (float)Math.Round(vector.Z));

        }

    }

    public static class ColliderUtils
    {

        public static Vector3 GetIntersectionDepth(this BoundingBox rectA, BoundingBox rectB)
        {
            // Calculate half sizes.
            float halfWidthA = (rectA.Max.X - rectA.Min.X) / 2.0f;
            float halfHeightA = (rectA.Max.Y - rectA.Min.Y) / 2.0f;
            float halfDepthA = (rectA.Max.Z - rectA.Min.Z) / 2.0f;
            float halfWidthB = (rectB.Max.X - rectB.Min.X) / 2.0f;
            float halfHeightB = (rectB.Max.Y - rectB.Min.Y) / 2.0f;
            float halfDepthB = (rectB.Max.Z - rectB.Min.Z) / 2.0f;

            // Calculate centers.
            Vector3 centerA = new Vector3(rectA.Min.X + halfWidthA, rectA.Min.Y + halfHeightA, rectA.Min.Z + halfWidthA);
            Vector3 centerB = new Vector3(rectB.Min.X + halfWidthB, rectB.Min.Y + halfHeightB, rectB.Min.Z + halfWidthB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float distanceZ = centerA.Z - centerB.Z;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;
            float minDistanceZ = halfDepthA + halfDepthB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY || Math.Abs(distanceZ) >= minDistanceZ)
                return Vector3.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            float depthZ = distanceZ> 0 ? minDistanceZ - distanceZ : -minDistanceZ - distanceZ;
            return new Vector3(depthX, depthY, depthZ);
        }

    }

   

}
