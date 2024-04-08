using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using System.Collections.Generic;
using System;
using SharpDX.Direct3D9;

namespace PositionEvents.Area
{
    public class BoundingObjectBuilder
    {
        IBoundingObject _internalBoundingObject;

        public BoundingObjectBuilder()
        {
            /** NOOP **/
        }

        public BoundingObjectBuilder Add(IBoundingObject boundingObject)
        {
            AddOrSetInternal(boundingObject);
            return this;
        }

        public BoundingObjectBuilder AddBox(BoundingBox box)
        {
            IBoundingObject boundingObject = new BoundingObjectBox(box);

            return Add(boundingObject);
        }

        public BoundingObjectBuilder AddBox(Vector3 min, Vector3 max)
        {
            return AddBox(new BoundingBox(min, max));
        }

        public BoundingObjectBuilder AddSphere(BoundingSphere sphere)
        {
            IBoundingObject boundingObject = new BoundingObjectSphere(sphere);

            return Add(boundingObject);
        }

        public BoundingObjectBuilder AddSphere(Vector3 center, float radius)
        {
            return AddSphere(new BoundingSphere(center, radius));
        }

        public BoundingObjectBuilder AddPrism(float top, float bottom, Polygon polygon, Axis3 alignment = Axis3.Z)
        {
            IBoundingObject boundingObject = new BoundingObjectPrism(top, bottom, polygon, alignment);

            return Add(boundingObject);
        }

        public BoundingObjectBuilder AddPrism(float top, float bottom, IEnumerable<Vector2> polygon, Axis3 alignment = Axis3.Z)
        {
            IBoundingObject boundingObject = new BoundingObjectPrism(top, bottom, polygon, alignment);

            return Add(boundingObject);
        }

        public BoundingObjectBuilder Subtract(IBoundingObject boundingObject)
        {
            if (_internalBoundingObject == null)
            {
                throw new InvalidOperationException("Need to add a IBoundingObject, before you can subtract anything.");
            }

            _internalBoundingObject = _internalBoundingObject.Difference(boundingObject);

            return this;
        }

        public BoundingObjectBuilder SubtractBox(BoundingBox box)
        {
            IBoundingObject boundingObject = new BoundingObjectBox(box);

            return Subtract(boundingObject);
        }

        public BoundingObjectBuilder SubtractBox(Vector3 min, Vector3 max)
        {
            return SubtractBox(new BoundingBox(min, max));
        }

        public BoundingObjectBuilder SubtractSphere(BoundingSphere sphere)
        {
            IBoundingObject boundingObject = new BoundingObjectSphere(sphere);

            return Subtract(boundingObject);
        }

        public BoundingObjectBuilder SubtractSphere(Vector3 center, float radius)
        {
            return SubtractSphere(new BoundingSphere(center, radius));
        }

        public BoundingObjectBuilder SubtractPrism(float top, float bottom, Polygon polygon, Axis3 alignment = Axis3.Z)
        {
            IBoundingObject boundingObject = new BoundingObjectPrism(top, bottom, polygon, alignment);

            return Subtract(boundingObject);
        }

        public BoundingObjectBuilder SubtractPrism(float top, float bottom, IEnumerable<Vector2> polygon, Axis3 alignment = Axis3.Z)
        {
            IBoundingObject boundingObject = new BoundingObjectPrism(top, bottom, polygon, alignment);

            return Subtract(boundingObject);
        }

        public BoundingObjectBuilder Intersect(IBoundingObject boundingObject)
        {
            if (_internalBoundingObject == null)
            {
                throw new InvalidOperationException("Need to add a IBoundingObject, before you can intersect anything.");
            }

            _internalBoundingObject = _internalBoundingObject.Intersection(boundingObject);

            return this;
        }

        public BoundingObjectBuilder IntersectBox(BoundingBox box)
        {
            IBoundingObject boundingObject = new BoundingObjectBox(box);

            return Intersect(boundingObject);
        }

        public BoundingObjectBuilder IntersectBox(Vector3 min, Vector3 max)
        {
            return IntersectBox(new BoundingBox(min, max));
        }

        public BoundingObjectBuilder IntersectSphere(BoundingSphere sphere)
        {
            IBoundingObject boundingObject = new BoundingObjectSphere(sphere);

            return Intersect(boundingObject);
        }

        public BoundingObjectBuilder IntersectSphere(Vector3 center, float radius)
        {
            return IntersectSphere(new BoundingSphere(center, radius));
        }

        public BoundingObjectBuilder IntersectPrism(float top, float bottom, Polygon polygon, Axis3 alignment = Axis3.Z)
        {
            IBoundingObject boundingObject = new BoundingObjectPrism(top, bottom, polygon, alignment);

            return Intersect(boundingObject);
        }

        public BoundingObjectBuilder IntersectPrism(float top, float bottom, IEnumerable<Vector2> polygon, Axis3 alignment = Axis3.Z)
        {
            IBoundingObject boundingObject = new BoundingObjectPrism(top, bottom, polygon, alignment);

            return Intersect(boundingObject);
        }

        private void AddOrSetInternal(IBoundingObject boundingObject)
        {
            if (_internalBoundingObject == null)
            {
                _internalBoundingObject = boundingObject;
            }
            else
            {
                _internalBoundingObject = _internalBoundingObject.Union(boundingObject);
            }
        }

        public IBoundingObject Build()
        {
            if (_internalBoundingObject == null)
            {
                throw new InvalidOperationException("Need to add something before building.");
            }

            return _internalBoundingObject;
        }
    }
}
