using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using PositionEvents.Area;

namespace PositionEvents
{
    public static class IBoundingObjectExtensions
    {
        // extensions method, because explicit interface implementations are not available in .net framework 4.8
        /// <summary>
        /// A wrapper for the <see cref="IBoundingObject.Contains(Vector3)"/> method. Will check the 
        /// <see cref="IBoundingObject.Bounds"/> before calling the method, if <see cref="IBoundingObject.IsComplex"/> 
        /// is set to true.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="position"></param>
        /// <returns>True, if the <paramref name="position"/> is fully inside, or intersecting with this 
        /// <see cref="IBoundingObject"/>. Otherwise false.</returns>
        public static bool ContainsEfficent(this IBoundingObject boundingObject, Vector3 position)
        {
            if (boundingObject.IsComplex && boundingObject.Bounds.Contains(position) == ContainmentType.Disjoint)
            {
                return false;
            }

            return boundingObject.Contains(position);
        }

        #region boolean operations

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupUnion"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one. If this <see cref="IBoundingObject"/> or <paramref name="other"/> 
        /// is already a <see cref="BoundingObjectGroupUnion"/>, their <see cref="BoundingObjectGroupUnion.Content"/> 
        /// will be used instead, to reduce nesting.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="other"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupUnion"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one.</returns>
        public static BoundingObjectGroupUnion Union(this IBoundingObject boundingObject, IBoundingObject other, bool isComplex = true)
        {
            List<IBoundingObject> boundingObjects = new List<IBoundingObject>();

            if (boundingObject is BoundingObjectGroupUnion thisUnion)
            {
                boundingObjects.AddRange(thisUnion.Content);
            }
            else
            {
                boundingObjects.Add(boundingObject);
            }

            if (other is BoundingObjectGroupUnion otherUnion)
            {
                boundingObjects.AddRange(otherUnion.Content);
            }
            else
            {
                boundingObjects.Add(other);
            }

            return new BoundingObjectGroupUnion(boundingObjects, isComplex);
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupUnion"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>. If this <see cref="IBoundingObject"/> or any of the 
        /// <paramref name="others"/> are already a <see cref="BoundingObjectGroupUnion"/>, their 
        /// <see cref="BoundingObjectGroupUnion.Content"/> will be used instead, to reduce nesting.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="others"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupUnion"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>.</returns>
        public static BoundingObjectGroupUnion Union(this IBoundingObject boundingObject, IEnumerable<IBoundingObject> others, bool isComplex = true)
        {
            if (!others.Any())
            {
                throw new ArgumentException("others must have at least one element.", nameof(others));
            }
            BoundingObjectGroupUnion result = boundingObject.Union(others.First(), isComplex);

            foreach (IBoundingObject other in others.Skip(1))
            {
                result = result.Union(other, isComplex);
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupIntersection"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one. If this <see cref="IBoundingObject"/> or <paramref name="other"/> 
        /// is already a <see cref="BoundingObjectGroupIntersection"/>, their <see cref="BoundingObjectGroupIntersection.Content"/> 
        /// will be used instead, to reduce nesting.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="other"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupIntersection"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one.</returns>
        public static BoundingObjectGroupIntersection Intersection(this IBoundingObject boundingObject, IBoundingObject other, bool isComplex = true)
        {
            List<IBoundingObject> boundingObjects = new List<IBoundingObject>();

            if (boundingObject is BoundingObjectGroupIntersection thisIntersection)
            {
                boundingObjects.AddRange(thisIntersection.Content);
            }
            else
            {
                boundingObjects.Add(boundingObject);
            }

            if (other is BoundingObjectGroupIntersection otherIntersection)
            {
                boundingObjects.AddRange(otherIntersection.Content);
            }
            else
            {
                boundingObjects.Add(other);
            }

            return new BoundingObjectGroupIntersection(boundingObjects, isComplex);
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupIntersection"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>. If this <see cref="IBoundingObject"/> or any of the 
        /// <paramref name="others"/> are already a <see cref="BoundingObjectGroupIntersection"/>, their 
        /// <see cref="BoundingObjectGroupIntersection.Content"/> will be used instead, to reduce nesting.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="others"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupIntersection"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>.</returns>
        public static BoundingObjectGroupIntersection Intersection(this IBoundingObject boundingObject, IEnumerable<IBoundingObject> others, bool isComplex = true)
        {
            if (!others.Any())
            {
                throw new ArgumentException("others must have at least one element.", nameof(others));
            }
            BoundingObjectGroupIntersection result = boundingObject.Intersection(others.First(), isComplex);

            foreach (IBoundingObject other in others.Skip(1))
            {
                result = result.Intersection(other, isComplex);
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupSymmetricDifference"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one. If this <see cref="IBoundingObject"/> or <paramref name="other"/> 
        /// is already a <see cref="BoundingObjectGroupSymmetricDifference"/>, their <see cref="BoundingObjectGroupSymmetricDifference.Content"/> 
        /// will be used instead, to reduce nesting.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="other"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupSymmetricDifference"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one.</returns>
        public static BoundingObjectGroupSymmetricDifference SymmetricDifference(this IBoundingObject boundingObject, IBoundingObject other, bool isComplex = true)
        {
            List<IBoundingObject> boundingObjects = new List<IBoundingObject>();

            if (boundingObject is BoundingObjectGroupSymmetricDifference thisSymmetricDifference)
            {
                boundingObjects.AddRange(thisSymmetricDifference.Content);
            }
            else
            {
                boundingObjects.Add(boundingObject);
            }

            if (other is BoundingObjectGroupSymmetricDifference otherSymmetricDifference)
            {
                boundingObjects.AddRange(otherSymmetricDifference.Content);
            }
            else
            {
                boundingObjects.Add(other);
            }

            return new BoundingObjectGroupSymmetricDifference(boundingObjects, isComplex);
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupSymmetricDifference"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>. If this <see cref="IBoundingObject"/> or any of the 
        /// <paramref name="others"/> are already a <see cref="BoundingObjectGroupSymmetricDifference"/>, their 
        /// <see cref="BoundingObjectGroupSymmetricDifference.Content"/> will be used instead, to reduce nesting.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="others"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupSymmetricDifference"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>.</returns>
        public static BoundingObjectGroupSymmetricDifference SymmetricDifference(this IBoundingObject boundingObject, IEnumerable<IBoundingObject> others, bool isComplex = true)
        {
            if (!others.Any())
            {
                throw new ArgumentException("others must have at least one element.", nameof(others));
            }
            BoundingObjectGroupSymmetricDifference result = boundingObject.SymmetricDifference(others.First(), isComplex);

            foreach (IBoundingObject other in others.Skip(1))
            {
                result = result.SymmetricDifference(other, isComplex);
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupDifference"/>, that contains this <see cref="IBoundingObject"/> 
        /// as a positive component and the <paramref name="other"/> as negative component. 
        /// If this <see cref="IBoundingObject"/> is already a <see cref="BoundingObjectGroupDifference"/>, or 
        /// the <paramref name="other"/> is a <see cref="BoundingObjectGroupDifference"/> or 
        /// a <see cref="BoundingObjectGroupUnion"/> their <see cref="IBoundingObjectGroup.Content"/> will be 
        /// used instead, to reduce nesting. 
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="other"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupDifference"/>, that contains both the <paramref name="boundingObject"/> 
        /// and the <paramref name="other"/> one.</returns>
        public static BoundingObjectGroupDifference Difference(this IBoundingObject boundingObject, IBoundingObject other, bool isComplex = true)
        {
            IBoundingObject positive;
            
            List<IBoundingObject> negatives = new List<IBoundingObject>();

            if (boundingObject is BoundingObjectGroupDifference thisDifference)
            {
                positive = thisDifference.Positive;
                negatives.AddRange(thisDifference.Negatives);
            }
            else
            {
                positive = boundingObject;
            }

            if (other is BoundingObjectGroupUnion otherUnion)
            {
                negatives.AddRange(otherUnion.Content);
            }
            else if (other is BoundingObjectGroupDifference otherDifference)
            {
                positive = positive.Union(otherDifference.Positive, isComplex); // TODO: not sure what to set isComplex here to
                negatives.AddRange(otherDifference.Negatives);
            }
            else
            {
                negatives.Add(other);
            }

            return new BoundingObjectGroupDifference(positive, negatives, isComplex);
        }

        /// <summary>
        /// Creates a <see cref="BoundingObjectGroupDifference"/>, that contains this <see cref="IBoundingObject"/> 
        /// as a positive component and the <paramref name="others"/> as negative components. 
        /// If this <see cref="IBoundingObject"/> is already a <see cref="BoundingObjectGroupDifference"/>, or 
        /// any of the <paramref name="others"/> is a <see cref="BoundingObjectGroupDifference"/> or 
        /// a <see cref="BoundingObjectGroupUnion"/> their <see cref="IBoundingObjectGroup.Content"/> will be 
        /// used instead, to reduce nesting. 
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <param name="others"></param>
        /// <param name="isComplex"></param>
        /// <returns>The <see cref="BoundingObjectGroupDifference"/>, that contains the <paramref name="boundingObject"/> 
        /// and the <paramref name="others"/>.</returns>
        public static BoundingObjectGroupDifference Difference(this IBoundingObject boundingObject, IEnumerable<IBoundingObject> others, bool isComplex = true)
        {
            if (!others.Any())
            {
                throw new ArgumentException("others must have at least one element.", nameof(others));
            }
            BoundingObjectGroupDifference result = boundingObject.Difference(others.First(), isComplex);

            foreach (IBoundingObject other in others.Skip(1))
            {
                result = result.Difference(other, isComplex);
            }

            return result;
        }

        #endregion
    }
}
