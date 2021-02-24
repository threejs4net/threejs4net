using Xunit;
using ThreeJs4Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Materials;

namespace ThreeJs4Net.Objects.Tests
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