using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PositionEvents.Area
{
    public class BoundingObjectPrism : IBoundingObject
    {
        private readonly float _top;
        private readonly float _bottom;

        private readonly Polygon _polygon;

        private readonly Axis3 _alignment;

        private readonly BoundingBox _bounds;

        private readonly bool _isComplex;

        /// <summary>
        /// The top Z coordinate (relative to the <see cref="Alignment"/> of this <see cref="BoundingObjectPrism"/>.
        /// </summary>
        public float Top => _top;

        /// <summary>
        /// The bottom Z coordinate (relative to the <see cref="Alignment"/> of this <see cref="BoundingObjectPrism"/>.
        /// </summary>
        public float Bottom => _bottom;

        /// <summary>
        /// The <see cref="Polygon"/>, that describes the base of the <see cref="BoundingObjectPrism"/>.
        /// </summary>
        public Polygon Polygon => _polygon;

        /// <summary>
        /// The direction in which the base of the <see cref="BoundingObjectPrism"/> is pointing.
        /// </summary>
        public Axis3 Alignment => _alignment;

        public BoundingBox Bounds => _bounds;

        public bool IsComplex => _isComplex;

        public BoundingObjectPrism(float top, float bottom, IEnumerable<Vector2> polygon, Axis3 alignment = Axis3.Z, bool isComplex = true)
        {
            if (top <= bottom)
            {
                throw new ArgumentException("top must be greater than bottom.", nameof(top));
            }
            
            if (polygon.Count() < 3)
            {
                throw new ArgumentException("polygon must have at least 3 points.", nameof(polygon));
            }

            if (alignment == Axis3.All)
            {
                throw new ArgumentException("prism must be aligned with a specific axis.", nameof(alignment));
            }

            _top = top;
            _bottom = bottom;
            _polygon = new Polygon(polygon);
            _alignment = alignment;
            _isComplex = isComplex;

            // Polygon.Top < Polygon.Bottom
            Vector3 boundsMin = new Vector3(_polygon.Left, _polygon.Top, _bottom);
            Vector3 boundsMax = new Vector3(_polygon.Right, _polygon.Bottom, _top);

            _bounds = new BoundingBox(boundsMin, boundsMax);
        }

        public BoundingObjectPrism(float top, float bottom, Polygon polygon, Axis3 alignment = Axis3.Z, bool isComplex = true)
        {
            if (top <= bottom)
            {
                throw new ArgumentException("top must be greater than bottom.", nameof(top));
            }

            if (alignment == Axis3.All)
            {
                throw new ArgumentException("prism must be aligned with a specific axis.", nameof(alignment));
            }

            _top = top;
            _bottom = bottom;
            _polygon = polygon;
            _alignment = alignment;
            _isComplex = isComplex;

            Vector3 boundsMin = new Vector3(_polygon.Left, _polygon.Bottom, _bottom);
            Vector3 boundsMax = new Vector3(_polygon.Right, _polygon.Top, _top);

            _bounds = new BoundingBox(boundsMin, boundsMax);
        }

        public bool Contains(Vector3 position)
        {
            float alignedUp = GetAlignedUp(position);

            if (alignedUp < _bottom || alignedUp > _top)
            {
                return false;
            }

            float alignedRight = GetAlignedRight(position);
            float alignedFront = GetAlignedFront(position);

            Vector2 position2d = new Vector2(alignedRight, alignedFront);

            return _polygon.Contains(position2d);
        }

        private float GetAlignedRight(Vector3 position)
        {
            switch (_alignment)
            {
                case Axis3.X:
                    {
                        return -position.Z;
                    }
                case Axis3.Y:
                    {
                        return position.X;
                    }
                case Axis3.Z:
                    {
                        return position.X;
                    }
                case Axis3.All:
                    {
                        throw new InvalidOperationException("alignment must can't be Axis3.All.");
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private float GetAlignedFront(Vector3 position)
        {
            switch (_alignment)
            {
                case Axis3.X:
                    {
                        return position.Y;
                    }
                case Axis3.Y:
                    {
                        return -position.Z;
                    }
                case Axis3.Z:
                    {
                        return position.Y;
                    }
                case Axis3.All:
                    {
                        throw new InvalidOperationException("alignment must can't be Axis3.All.");
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private float GetAlignedUp(Vector3 position)
        {
            switch(_alignment)
            {
                case Axis3.X:
                    {
                        return position.X;
                    }
                case Axis3.Y:
                    {
                        return position.Y;
                    }
                case Axis3.Z:
                    {
                        return position.Z;
                    }
                case Axis3.All:
                    {
                        throw new InvalidOperationException("alignment must can't be Axis3.All.");
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return $"Prism: {{ top: {_top} , bottom: {_bottom}, polygon: {{ {string.Join(", ", _polygon)} }} }}";
        }
    }
}
