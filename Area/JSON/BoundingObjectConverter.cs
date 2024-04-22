using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionEvents.Area.JSON
{
    public class BoundingObjectConverter : BasicConverter<IBoundingObject>
    {
        internal static BoundingObjectConverter Default = new BoundingObjectConverter();

        public BoundingObjectConverter() : base(GetSubTypes())
        {
            /** NOOP **/
        }

        private static Dictionary<string, Type> GetSubTypes()
        {
            return new Dictionary<string, Type>()
            {
                { "box", typeof(BoundingObjectBox) },
                { "sphere", typeof(BoundingObjectSphere) },
                { "prism", typeof(BoundingObjectPrism) },
                { "union", typeof(BoundingObjectGroupUnion) },
                { "difference", typeof(BoundingObjectGroupDifference) },
                { "intersection", typeof(BoundingObjectGroupIntersection) },
                { "symmetricDifference", typeof(BoundingObjectGroupSymmetricDifference) }
            };
        }
    }
}
