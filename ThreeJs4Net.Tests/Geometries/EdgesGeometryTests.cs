using Xunit;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Materials;

namespace ThreeJs4Net.Tests.Geometries
{
   public class EdgesGeometryTests
    {
        [Fact()]
        public void TestEdgesGeometry()
        {
            var box = new BoxBufferGeometry();
            var edges = new EdgesGeometry(box);
            var material = new LineBasicMaterial();
            var lines = new LineSegments(edges,material);

        }
    }
}


