using Microsoft.Xna.Framework;

namespace PositionEvents
{
    public struct PositionData
    {
        public int MapId { get; set; }

        public Vector3 Position { get; set; }

        public override string ToString()
        {
            return $"{{ MapId: {MapId}, Position: {Position} }}";
        }
    }
}
