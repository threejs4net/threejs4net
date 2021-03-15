using System;
using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Core
{
    public class BufferGeometryTests : BaseGeometry
    {
        #region -- private ---
        private bool BufferAttributeEquals(BufferAttribute<float> a, BufferAttribute<float> b, float tolerance = 0.0001f)
        {
            if (a.Count != b.Count || a.ItemSize != b.ItemSize)
            {
                return false;
            }

            for (int i = 0; i < a.Count * a.ItemSize; i++)
            {
                var delta = a.Array[i] - b.Array[i];
                if (delta > tolerance)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CompareUvs(float[] uvs, Vector2[] u)
        {
            return (
                uvs[0] == u[0].X && uvs[1] == u[0].Y &&
                uvs[2] == u[1].X && uvs[3] == u[1].Y &&
                uvs[4] == u[2].X && uvs[5] == u[2].Y
            );
        }

        private bool ComparePositions(float[] pos, Vector3[] v)
        {
            return (
                pos[0] == v[0].X && pos[1] == v[0].Y && pos[2] == v[0].Z &&
                pos[3] == v[1].X && pos[4] == v[1].Y && pos[5] == v[1].Z &&
                pos[6] == v[2].X && pos[7] == v[2].Y && pos[8] == v[2].Z
            );
        }

        private Box3 GetBBForVertices(float[] vertices)
        {
            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(vertices, 3));
            geometry.ComputeBoundingBox();
            return geometry.BoundingBox;
        }

        private Sphere GetBSForVertices(float[] vertices)
        {
            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(vertices, 3));
            geometry.ComputeBoundingSphere();
            return geometry.BoundingSphere;
        }

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
        public void AddGroupTest()
        {
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
        public void ComputeVertexNormals_IndexedTest()
        {
            float sqrt = 0.5f * Mathf.Sqrt(2);
            var normal = new BufferAttribute<float>(new float[] { -1, 0, 0, -1, 0, 0, -1, 0, 0, sqrt, sqrt, 0, sqrt, sqrt, 0, sqrt, sqrt, 0, -1, 0, 0 }, 3);
            var position = new BufferAttribute<float>(new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, -0.5f, 0.5f, -0.5f, 0.5f, 0.5f, -0.5f, -0.5f, -0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, -0.5f, -0.5f, -0.5f }, 3);
            var index = new BufferAttribute<uint>(new uint[] { 0, 2, 1, 2, 3, 1, 4, 6, 5, 6, 7, 5 }, 1);
            var a = new BufferGeometry();

            a.SetAttribute("position", position);
            a.ComputeVertexNormals();
            Assert.True(BufferAttributeEquals(normal, a.GetAttribute<float>("normal")));

            // a second time to see if the existing normals get properly deleted
            a.ComputeVertexNormals();
            Assert.True(BufferAttributeEquals(normal, a.GetAttribute<float>("normal")));

            // indexed geometry
            a = new BufferGeometry();
            a.SetAttribute("position", position);
            a.SetIndex(index);
            a.ComputeVertexNormals();
            Assert.True(BufferAttributeEquals(normal, a.GetAttribute<float>("normal")));

        }

        [Fact()]
        public void ComputeVertexNormalsTest()
        {
            // get normals for A counter clockwise created triangle
            var normals = GetNormalsForVertices(new float[] { -1, 0, 0, 1, 0, 0, 0, 1, 0 }).Array;

            Assert.True(normals[0] == 0 && normals[1] == 0 && normals[2] == 1,
                "first normal is pointing to screen since the the triangle was created counter clockwise");

            Assert.True(normals[3] == 0 && normals[4] == 0 && normals[5] == 1,
                "second normal is pointing to screen since the the triangle was created counter clockwise");

            Assert.True(normals[6] == 0 && normals[7] == 0 && normals[8] == 1,
                "third normal is pointing to screen since the the triangle was created counter clockwise");

            // get normals for A clockwise created triangle
            normals = GetNormalsForVertices(new float[] { 1, 0, 0, -1, 0, 0, 0, 1, 0 }).Array;

            Assert.True(normals[0] == 0 && normals[1] == 0 && normals[2] == -1,
                "first normal is pointing to screen since the the triangle was created clockwise");

            Assert.True(normals[3] == 0 && normals[4] == 0 && normals[5] == -1,
                "second normal is pointing to screen since the the triangle was created clockwise");

            Assert.True(normals[6] == 0 && normals[7] == 0 && normals[8] == -1,
                "third normal is pointing to screen since the the triangle was created clockwise");

            normals = GetNormalsForVertices(new float[] { 0, 0, 1, 0, 0, -1, 1, 1, 0 }).Array;

            // the triangle is rotated by 45 degrees to the right so the normals of the three vertices
            // should point to (1, -1, 0).normalized(). The simplest solution is to check against A normalized
            // vector (1, -1, 0) but you will get calculation errors because of floating calculations so another
            // valid technique is to create A vector which stands in 90 degrees to the normals and calculate the
            // dot product which is the cos of the angle between them. This should be < floating calculation error
            // which can be taken from Number.EPSILON
            var direction = new Vector3(1, 1, 0).Normalize(); // A vector which should have 90 degrees difference to normals
            var difference = direction.Dot(new Vector3(normals[0], normals[1], normals[2]));
            Assert.True(difference < MathUtils.EPS5, "normal is equal to reference vector");

            // get normals for A line should be NAN because you need min A triangle to calculate normals
            normals = GetNormalsForVertices(new float[] { 1, 0, 0, -1, 0, 0 }).Array;

            for (var i = 0; i < normals.Length; i++)
            {
                //Assert.True(!normals[i], "normals can't be calculated which is good");
            }
        }

        [Fact()]
        public void CopyTest()
        {
            //var geometry = new BufferGeometry();
            //var attributeName = "position";

            //geometry.SetAttribute(attributeName, new BufferAttribute<float>(new float[] { 1, 2, 3 }, 1));
            //Assert.NotNull(geometry.Attributes[attributeName]);

            //var geometry2 = new BufferGeometry();
            //geometry2.Copy(geometry);

            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { 1, 2, 3, 4, 5, 6 }, 3));
            geometry.SetAttribute("attrName", new BufferAttribute<float>(new float[] { 1, 2, 3, 4, 5, 6 }, 3));
            geometry.SetAttribute("attrName2", new BufferAttribute<float>(new float[] { 0, 1, 3, 5, 6 }, 1));

            geometry.ComputeBoundingBox();
            geometry.ComputeBoundingSphere();

            var copy = new BufferGeometry().Copy(geometry);

            Assert.True(copy != geometry && geometry.Id != copy.Id);

            copy.ComputeBoundingBox();
            copy.ComputeBoundingSphere();

            copy.RotateX(10);
            copy.Translate(2, 3, 3);
            copy.Scale(1, 2, 3);

            copy.ComputeBoundingBox();
            copy.ComputeBoundingSphere();



            //foreach (var attr in geometry.Attributes)
            //{
            //   for (var i = 0; i < attribute.array.length; i++)
            //    {

            //        assert.ok(attribute.array[i] === copy.attributes[key].array[i], "values of the attribute are equal");

            //    }
            //}
        }



        [Fact()]
        public void Set_DeleteAttributeTest()
        {
            var geometry = new BufferGeometry();
            var attributeName = "position";

            Assert.Throws<KeyNotFoundException>(() => geometry.Attributes[attributeName]);

            geometry.SetAttribute(attributeName, new BufferAttribute<float>(new float[] { 1, 2, 3 }, 1));
            Assert.NotNull(geometry.Attributes[attributeName]);

            geometry.DeleteAttribute(attributeName);
            Assert.Throws<KeyNotFoundException>(() => geometry.Attributes[attributeName]);
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