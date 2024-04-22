using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace PositionEvents.Area
{
    public class BoundingObjectSphere : IBoundingObject
    {
        private readonly BoundingSphere _sphere;

        private readonly BoundingBox _bounds;

        [JsonIgnore]
        public BoundingBox Bounds => _bounds;

        [JsonIgnore]
        public bool IsComplex => false;

        public BoundingSphere Sphere => _sphere;

        [JsonConstructor]
        public BoundingObjectSphere(BoundingSphere sphere)
        {
            _sphere = sphere;

            _bounds = BoundingBox.CreateFromSphere(_sphere);
        }

        public BoundingObjectSphere(Vector3 center, float radius)
        {
            _sphere = new BoundingSphere(center, radius);

            _bounds = BoundingBox.CreateFromSphere(_sphere);
        }

        public bool Contains(Vector3 position)
        {
            return _sphere.Contains(position) != ContainmentType.Disjoint;
        }

        public override string ToString()
        {
            return $"Sphere: {{ center: {_sphere.Center} , radius: {_sphere.Radius} }}";
        }
    }
}
