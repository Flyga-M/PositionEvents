using Microsoft.Xna.Framework;
using PositionEvents.Area;

namespace PositionEvents.Implementation.Handlers
{
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// Calculates a <see cref="BoundingBox"/> with the added <paramref name="margin"/> in each direction.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="margin"></param>
        /// <returns>A <see cref="BoundingBox"/> with the added <paramref name="margin"/>.</returns>
        public static BoundingBox GetGrown(this BoundingBox box, float margin)
        {
            BoundingBox orientedBox = BoundingBox.CreateFromPoints(new Vector3[] { box.Min, box.Max });

            return new BoundingBox(orientedBox.Min.AddSingle(-margin, Axis3.All), orientedBox.Max.AddSingle(margin, Axis3.All));
        }
    }
}
