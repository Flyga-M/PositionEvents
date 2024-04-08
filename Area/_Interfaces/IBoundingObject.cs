using Microsoft.Xna.Framework;

namespace PositionEvents.Area
{
    public interface IBoundingObject
    {   
        /// <summary>
        /// A <see cref="BoundingBox"/> that fully contains this <see cref="IBoundingObject"/>. Might not be the 
        /// smallest possible Bounding Box, depending on implementation or complexity. The closer the 
        /// Bounding Box is to the actual dimensions of the <see cref="IBoundingObject"/>, the more accurately it 
        /// can be placed in the <see cref="OcTree"/>.
        /// </summary>
        BoundingBox Bounds { get; }

        /// <summary>
        /// A measure of how resource intensive the <see cref="Contains(Vector3)"/> function is. If true, 
        /// the <see cref="Bounds"/> should be checked first, before using <see cref="Contains(Vector3)"/>.
        /// </summary>
        bool IsComplex { get; }

        /// <summary>
        /// Checks, whether the given <paramref name="position"/> is contained within the <see cref="IBoundingObject"/>. 
        /// Instead of calling this, you probably should use <see cref="IBoundingObjectExtensions.ContainsEfficent(IBoundingObject, Vector3)"/> 
        /// instead.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>True, if the <paramref name="position"/> is fully inside, or intersecting with this 
        /// <see cref="IBoundingObject"/>. Otherwise false.</returns>
        bool Contains(Vector3 position);
    }
}
