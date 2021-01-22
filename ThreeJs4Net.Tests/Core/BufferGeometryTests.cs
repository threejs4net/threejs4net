using Xunit;
using ThreeJs4Net.Core;
using System;
using System.Collections.Generic;
using System.Text;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Core.Tests
{
    public class BufferGeometryTests : BaseGeometry
    {
        #region -- private ---
        private BufferAttribute<float> GetNormalsForVertices(float[] vertices)
        {
            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(vertices, 3));
            geometry.ComputeVertexNormals();
            Assert.True(geometry.Attributes.ContainsKey("normal"));
            return geometry.Attributes["normal"] as BufferAttribute<float>;
        }

        #endregion

        [Fact()]
        public void BufferGeometryTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void AddAttributeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ComputeBoundingSphereTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ComputeBoundingBoxTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ComputeFaceNormalsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ComputeVertexNormalsTest()
        {
            // get normals for a counter clockwise created triangle
            var normals = GetNormalsForVertices(new float[] { -1, 0, 0, 1, 0, 0, 0, 1, 0 }).Array;

            Assert.True(normals[0] == 0 && normals[1] == 0 && normals[2] == 1,
                "first normal is pointing to screen since the the triangle was created counter clockwise");

            Assert.True(normals[3] == 0 && normals[4] == 0 && normals[5] == 1,
                "second normal is pointing to screen since the the triangle was created counter clockwise");

            Assert.True(normals[6] == 0 && normals[7] == 0 && normals[8] == 1,
                "third normal is pointing to screen since the the triangle was created counter clockwise");

            // get normals for a clockwise created triangle
            normals = GetNormalsForVertices( new float[] { 1, 0, 0, -1, 0, 0, 0, 1, 0 }).Array;

            Assert.True(normals[0] == 0 && normals[1] == 0 && normals[2] == -1,
                "first normal is pointing to screen since the the triangle was created clockwise");

            Assert.True(normals[3] == 0 && normals[4] == 0 && normals[5] == -1,
                "second normal is pointing to screen since the the triangle was created clockwise");

            Assert.True(normals[6] == 0 && normals[7] == 0 && normals[8] == -1,
                "third normal is pointing to screen since the the triangle was created clockwise");

            normals = GetNormalsForVertices( new float[] { 0, 0, 1, 0, 0, -1, 1, 1, 0 }).Array;

            // the triangle is rotated by 45 degrees to the right so the normals of the three vertices
            // should point to (1, -1, 0).normalized(). The simplest solution is to check against a normalized
            // vector (1, -1, 0) but you will get calculation errors because of floating calculations so another
            // valid technique is to create a vector which stands in 90 degrees to the normals and calculate the
            // dot product which is the cos of the angle between them. This should be < floating calculation error
            // which can be taken from Number.EPSILON
            var direction = new Vector3(1, 1, 0).Normalize(); // a vector which should have 90 degrees difference to normals
            var difference = direction.Dot(new Vector3(normals[0], normals[1], normals[2]));
            Assert.True(difference < MathUtils.EPS5, "normal is equal to reference vector");

            // get normals for a line should be NAN because you need min a triangle to calculate normals
            normals = GetNormalsForVertices(new float[] { 1, 0, 0, -1, 0, 0 }).Array;

            for (var i = 0; i < normals.Length; i++)
            {
                //Assert.True(!normals[i], "normals can't be calculated which is good");
            }
        }

        [Fact()]
        public void MergeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void NormalizeNormalsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void GetAttributeTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void AddDrawCallTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void CenterTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ApplyMatrix4Test()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void ScaleTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RotateXTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RotateYTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RotateZTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}