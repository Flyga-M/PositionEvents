using Microsoft.Xna.Framework;
using System.Linq;

namespace PositionEvents.Area
{
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// Creates the <see cref="BoundingBox"/>, that represents the intersection (overlap) of this 
        /// <see cref="BoundingBox"/> and the <paramref name="other"/> one.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="other"></param>
        /// <returns>The <see cref="BoundingBox"/> representing the intersection of the 
        /// given <see cref="BoundingBox">BoundingBoxes</see>.</returns>
        public static BoundingBox GetIntersection(this BoundingBox box, BoundingBox other)
        {
            // TODO: check if works
            float[] sortedX = new float[] { box.Min.X, box.Max.X, other.Min.X, other.Max.X }.OrderBy(x => x).ToArray();
            float[] sortedY = new float[] { box.Min.Y, box.Max.Y, other.Min.Y, other.Max.Y }.OrderBy(y => y).ToArray();
            float[] sortedZ = new float[] { box.Min.Z, box.Max.Z, other.Min.Z, other.Max.Z }.OrderBy(z => z).ToArray();

            Vector3 min = new Vector3(sortedX[1], sortedZ[1], sortedZ[1]);
            Vector3 max = new Vector3(sortedX[2], sortedZ[2], sortedZ[2]);

            return new BoundingBox(min, max);
        }
    }
}
