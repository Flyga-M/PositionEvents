using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace PositionEvents.Area
{
    public class BoundingObjectBox : IBoundingObject
    {
        private readonly BoundingBox _box;

        [JsonIgnore]
        public BoundingBox Bounds => _box;

        public BoundingBox Box => _box;

        [JsonIgnore]
        public bool IsComplex => false;

        [JsonConstructor]
        public BoundingObjectBox(BoundingBox box)
        {
            _box = box;
        }

        public BoundingObjectBox(Vector3 min, Vector3 max)
        {
            _box = new BoundingBox(min, max);
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
