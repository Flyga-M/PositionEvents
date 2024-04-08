using Microsoft.Xna.Framework;

namespace PositionEvents.Area
{
    public class BoundingObjectBox : IBoundingObject
    {
        private readonly BoundingBox _box;

        public BoundingBox Bounds => _box;

        public bool IsComplex => false;

        public BoundingObjectBox(BoundingBox box)
        {
            _box = box;
        }

        public bool Contains(Vector3 position)
        {
            return _box.Contains(position) != ContainmentType.Disjoint;
        }

        public override string ToString()
        {
            return $"Box: {{ min: {_box.Min} , max: {_box.Max} }}";
        }
    }
}
