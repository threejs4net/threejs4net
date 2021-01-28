using System;
using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Core
{
    public class BufferAttributeTests
    {
        [Fact()]
        public void Instancing()
        {
            var a = new BufferAttribute<float>(new float[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, false);

            Assert.Throws<IndexOutOfRangeException>(() => a.GetZ(10));
            Assert.Equal(7, a.GetZ(1));

            a = new BufferAttribute<float>(new float[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 3, false);
            Assert.Equal(6, a.GetZ(1));

            a = new BufferAttribute<float>(new float[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 2, false);
            Assert.Equal(4, a.GetY(1));
        }

        [Fact()]
        public void SetAndGetTest()
        {
            var f32a = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var a = new BufferAttribute<float>(f32a, 4, false);
            var expected = new float[] { 1, 2, -3, -4, -5, -6, 7, 8 };

            a.SetX(1, a.GetX(1) * -1);
            a.SetY(1, a.GetY(1) * -1);
            a.SetZ(0, a.GetZ(0) * -1);
            a.SetW(0, a.GetW(0) * -1);

            Assert.Equal(expected, a.Array);
        }

        [Fact()]
        public void CopyArrayTest()
        {
            var array1 = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var array2 = new float[] { 0, 0 };
            var array3 = new float[] { };

            var a = new BufferAttribute<float>(array2, 4, false);

            a.CopyArray(array1);
            Assert.Equal(array1, a.Array);

            a.CopyArray(array2);
            Assert.Equal(array2, a.Array);

            a.CopyArray(array3);
            Assert.Equal(array3, a.Array);

            a.CopyArray(array1);
            Assert.Equal(array1, a.Array);
        }


        [Fact()]
        public void CopyVector4sArrayTest()
        {
            var vec4 = new List<Vector4>
            {
                new Vector4(1, 2, 3, 4), new Vector4(5, 6, 7, 8), new Vector4(9, 10, 11, 12)
            };


            var array2 = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var array1 = new float[] { };

            var a = new BufferAttribute<float>(array1, 4, false);

            a.CopyVector4sArray(vec4);
            Assert.Equal(array2, a.Array);
        }

        [Fact()]
        public void CopyVector3sArrayTest()
        {
            var vectors = new List<Vector3>
            {
                new Vector3(1, 2, 3), new Vector3(4, 5, 6), new Vector3(7, 8, 9)
            };

            var array2 = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var array1 = new float[] { };

            var a = new BufferAttribute<float>(array1, 3, false);

            a.CopyVector3sArray(vectors);
            Assert.Equal(array2, a.Array);
        }

        [Fact()]
        public void CopyVector2sArrayTest()
        {
            var vectors = new List<Vector2>
            {
                new Vector2(1, 2), new Vector2(3, 4), new Vector2(5, 6)
            };

            var array2 = new float[] { 1, 2, 3, 4, 5, 6 };
            var array1 = new float[] { };

            var a = new BufferAttribute<float>(array1, 2, false);

            a.CopyVector2sArray(vectors);
            Assert.Equal(array2, a.Array);
        }

        [Fact()]
        public void CopyTest()
        {
            //var geometry = new BufferGeometry();
            //geometry.SetAttribute("attrName", new BufferAttribute<float>(new float[] { 1, 2, 3, 4, 5, 6}, 3) );
            //geometry.SetAttribute("attrName2", new BufferAttribute<float>(new float[] { 0, 1, 3, 5, 6 }, 1) );

            //var copy = new BufferGeometry().Copy(geometry);

            //Assert.True(copy != geometry && geometry.Id != copy.Id);

            //foreach (var key in geometry.AttributesKeys)
            //{
            //    var attribute = geometry.Attributes[key];
            //    Assert.True(attribute != null);

            //    for (var i = 0; i < attribute.Values.Count; i++)
            //    {
            //        //Assert.True(attribute.Values[i] == copy.attributes[key].array[i],
            //        //    "values of the attribute are equal");
            //    }

            //}
        }

        [Fact()]
        public void CloneTest()
        {
            var f32a = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var a = new BufferAttribute<float>(f32a, 4, false);

            var b = a.Clone();

            Assert.Equal(f32a, b.Array);
            Assert.Equal(a.Count, b.Count);
            Assert.Equal(a.ItemSize, b.ItemSize);
            Assert.Equal(a.Usage, b.Usage);
        }


        [Fact()]
        public void SetTest()
        {
            var f32a = new float[] { 1, 2, 3, 4 };
            var a = new BufferAttribute<float>(f32a, 2, false);
            var expected = new float[] { 9, 2, 8, 4, 0, 0, 0, 0, 0, 0, 8, 11 };

            a.Set(new float[] { 9 });
            a.Set(new float[] { 8 }, 2);
            a.Set(new float[] { 8, 11 }, 10);

            Assert.Equal(expected, a.Array);
        }

        [Fact()]
        public void SetXYTest()
        {
            var f32a = new float[] { 1, 2, 3, 4 };
            var a = new BufferAttribute<float>(f32a, 2, false);
            var expected = new float[] { -1, -2, 3, 4 };

            a.SetXY(0, -1, -2);

            Assert.Equal(expected, a.Array);
        }

        [Fact()]
        public void SetXYZTest()
        {
            var f32a = new float[] { 1, 2, 3, 4, 5, 6 };
            var a = new BufferAttribute<float>(f32a, 3, false);
            var expected = new float[] { 1, 2, 3, -4, -5, -6 };

            a.SetXYZ(1, -4, -5, -6);

            Assert.Equal(expected, a.Array);
        }

        [Fact()]
        public void SetXYZWTest()
        {
            var f32a = new float[] { 1, 2, 3, 4 };
            var a = new BufferAttribute<float>(f32a, 4, false);
            var expected = new float[] { -1, -2, -3, -4 };

            a.SetXYZW(0, -1, -2, -3, -4);

            Assert.Equal(expected, a.Array);
        }
    }
}