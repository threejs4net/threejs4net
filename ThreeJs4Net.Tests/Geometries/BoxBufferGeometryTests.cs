using ThreeJs4Net.Geometries;
using Xunit;

namespace ThreeJs4Net.Tests.Geometries
{
    public class BoxBufferGeometryTests
    {
        [Fact()]
        public void BoxBufferGeometryTest()
        {
            var geometry = new BoxBufferGeometry(5,6,7);
            Assert.True(geometry.groups[0].Start == 0 && geometry.groups[0].Count == 6);
            Assert.True(geometry.groups[1].Start == 6 && geometry.groups[0].Count == 6);
            Assert.True(geometry.groups[2].Start == 12 && geometry.groups[0].Count == 6);
            Assert.True(geometry.groups[3].Start == 18 && geometry.groups[0].Count == 6);
            Assert.True(geometry.groups[4].Start == 24 && geometry.groups[0].Count == 6);
            Assert.True(geometry.groups[5].Start == 30 && geometry.groups[0].Count == 6);

        }
    }
}