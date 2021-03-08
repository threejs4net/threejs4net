using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Objects;
using Xunit;

namespace ThreeJs4Net.Tests.Objects
{
    public class MeshTests
    {
        [Fact()]
        public void MeshTest()
        {
            var obj = new BoxBufferGeometry(10, 10, 10);
            var mesh = new Mesh(obj, new MeshBasicMaterial());
        }

        [Fact()]
        public void CloneTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void UpdateMorphTargetsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RaycastTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}