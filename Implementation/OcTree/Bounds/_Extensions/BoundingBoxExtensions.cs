using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using PositionEvents.Area;

namespace PositionEvents.Implementation.OcTree.Bounds
{
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// Test, if the <paramref name="boundingObject"/> is contained in the <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the <paramref name="boundingObject"/> is fully contained inside this 
        /// <see cref="BoundingBox"/>. Otherwise false.</returns>
        public static bool Contains(this BoundingBox box, IBoundingObject boundingObject)
        {
            return box.Contains(boundingObject.Bounds) == ContainmentType.Contains;
        }

        /// <summary>
        /// Calculates the center of the <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box"></param>
        /// <returns>The center.</returns>
        public static Vector3 GetCenter(this BoundingBox box)
        {
            return box.Min + ((box.Max - box.Min) / 2);
        }

        /// <summary>
        /// Splits the <see cref="BoundingBox"/> into 8 equal sized <see cref="BoundingBox">BoundingBoxes</see>.
        /// </summary>
        /// <returns>8 equal sized <see cref="BoundingBox">BoundingBoxes</see> that fit exactly inside this 
        /// <see cref="BoundingBox"/>.</returns>
        public static BoundingBox[] GetSubBoxes(this BoundingBox box)
        {
            float lengthX = box.Max.X - box.Min.X;
            float lengthY = box.Max.Y - box.Min.Y;
            float lengthZ = box.Max.Z - box.Min.Z;

            float halfX = lengthX / 2;
            float halfY = lengthY / 2;
            float halfZ = lengthZ / 2;

            Vector3 halfAll = new Vector3(halfX, halfY, halfZ);

            Vector3 min = box.Min;
            Vector3 max = box.Max;
            Vector3 midPoint = min + halfAll;

            List<BoundingBox> boxes = new List<BoundingBox>()
            {
                new BoundingBox(min, midPoint),
                new BoundingBox(min.AddSingle(halfX,Axis3.X), midPoint.AddSingle(halfX,Axis3.X)),
                new BoundingBox(min.AddSingle(halfY,Axis3.Y), midPoint.AddSingle(halfY,Axis3.Y)),
                new BoundingBox(min.AddSingle(halfZ,Axis3.Z), midPoint.AddSingle(halfZ, Axis3.Z)),

                new BoundingBox(midPoint, max),
                new BoundingBox(midPoint.AddSingle(-halfX,Axis3.X), max.AddSingle(-halfX,Axis3.X)),
                new BoundingBox(midPoint.AddSingle(-halfY,Axis3.Y), max.AddSingle(-halfY,Axis3.Y)),
                new BoundingBox(midPoint.AddSingle(-halfZ,Axis3.Z), max.AddSingle(-halfZ, Axis3.Z))
            };

            return boxes.ToArray();
        }

        /// <summary>
        /// Calculates the relative position of the given <paramref name="position"/> inside this 
        /// <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>A <see cref="Vector3"/>, that represents the relative position inside this <see cref="BoundingBox"/>. 
        /// Each value is between 0 and 1, if the position is contained in this <see cref="BoundingBox"/>.</returns>
        public static Vector3 GetRelativePosition(this BoundingBox box, Vector3 position)
        {
            Vector3 sideLengths = box.Max - box.Min;

            return (position - box.Min) / sideLengths;
        }

        /// <summary>
        /// Calculates the corresponding child positions for the given <paramref name="position"/>. Generates multiple 
        /// positions, if it's on the border of multiple child <see cref="Node">Nodes</see>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>An Array of child positions. A child position consists of 3 int values (either 0 or 1), 
        /// corresponding to the <see cref="_childrenByPosition"/> array.
        /// </returns>
        public static int[][] GetChildPositions(this BoundingBox box, Vector3 position)
        {
            if (box.Contains(position) == ContainmentType.Disjoint)
            {
                return Array.Empty<int[]>();
            }

            List<int[]> result = new List<int[]>();

            Vector3 relativePosition = box.GetRelativePosition(position);

            int directionX = ChildUtil.GetDirection(relativePosition.X);
            int directionY = ChildUtil.GetDirection(relativePosition.Y);
            int directionZ = ChildUtil.GetDirection(relativePosition.Z);

            List<int> xPositions = new List<int>();
            List<int> yPositions = new List<int>();
            List<int> zPositions = new List<int>();

            if (directionX == 0)
            {
                xPositions.Add(0);
                xPositions.Add(1);
            }
            else
            {
                xPositions.Add(directionX == -1 ? 0 : 1);
            }

            if (directionY == 0)
            {
                yPositions.Add(0);
                yPositions.Add(1);
            }
            else
            {
                yPositions.Add(directionY == -1 ? 0 : 1);
            }

            if (directionZ == 0)
            {
                zPositions.Add(0);
                zPositions.Add(1);
            }
            else
            {
                zPositions.Add(directionZ == -1 ? 0 : 1);
            }

            foreach (int positionX in xPositions)
            {
                foreach (int positionY in yPositions)
                {
                    foreach (int positionZ in zPositions)
                    {
                        result.Add(new int[] { positionX, positionY, positionZ });
                    }
                }
            }

            return result.ToArray();
        }
    }
}
