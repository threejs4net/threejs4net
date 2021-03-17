using System;
using System.Runtime.Intrinsics.X86;
using ThreeJs4Net.Core;
using ThreeJs4Net.Examples.Jsm.Math;
using ThreeJs4Net.Examples.Jsm.Utils;
using ThreeJs4Net.Math;
using Xunit;

namespace ThreeJs4Net.Tests.Examples.Jsm.Utils
{
    public class BufferGeometryUtilsTests : BaseTests
    {
        [Fact()]
        public void MergeBufferAttributesBasic()
        {
            var array1 = new float[] { 1, 2, 3, 4 };
            var attr1 = new BufferAttribute<float>(array1, 2, false);

            var array2 = new float[] { 5, 6, 7, 8 };
            var attr2 = new BufferAttribute<float>(array2, 2, false);

            var mergedAttr = BufferGeometryUtils.MergeBufferAttributes<float>(new[] { attr1, attr2 });

            Assert.Equal(mergedAttr.Array, new float[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            Assert.Equal(2, mergedAttr.ItemSize);
            Assert.False(mergedAttr.Normalized);

        }

        [Fact()]
        public void MergeBufferAttributesInvalid()
        {
            var array1 = new float[] { 1, 2, 3, 4 };
            var attr1 = new BufferAttribute<float>(array1, 2, false);

            var array2 = new float[] { 5, 6, 7, 8 };
            var attr2 = new BufferAttribute<float>(array2, 4, false);

            Assert.Throws<Exception>(() => BufferGeometryUtils.MergeBufferAttributes<float>(new[] { attr1, attr2 }));

            attr2.ItemSize = 2;
            attr2.Normalized = true;

            Assert.Throws<Exception>(() => BufferGeometryUtils.MergeBufferAttributes<float>(new[] { attr1, attr2 }));

            attr2.Normalized = false;

            Assert.NotNull(BufferGeometryUtils.MergeBufferAttributes<float>(new[] { attr1, attr2 }));
        }

        [Fact()]
        public void MergeBufferGeometryBasic()
        {
            var geometry1 = new BufferGeometry();
            geometry1.SetAttribute("position", new BufferAttribute<float>(new float[] { 1, 2, 3 }, 1, false));
            geometry1.SetAttribute("uint", new BufferAttribute<uint>(new uint[] { 1, 2, 3 }, 1, false));

            var geometry2 = new BufferGeometry();
            geometry2.SetAttribute("position", new BufferAttribute<float>(new float[] { 4, 5, 6 }, 1, false));
            geometry2.SetAttribute("uint", new BufferAttribute<uint>(new uint[] { 1, 2, 3 }, 1, false));

            var mergedGeometry = BufferGeometryUtils.MergeBufferGeometries(new[] { geometry1, geometry2 });

            Assert.NotNull(mergedGeometry);
            var position = mergedGeometry.GetAttribute<float>("position");
            var positions = position.Array;
            Assert.Equal(positions, new float[] { 1, 2, 3, 4, 5, 6 });
            Assert.Equal(1, position.ItemSize);
        }

        [Fact()]
        public void MergeBufferGeometryIndexed()
        {
            var geometry1 = new BufferGeometry();
            geometry1.SetAttribute("position", new BufferAttribute<float>(new float[] { 1, 2, 3 }, 1, false));
            geometry1.SetIndex(new BufferAttribute<uint>(new uint[] { 0, 1, 2, 2, 1, 0 }, 1, false));

            var geometry2 = new BufferGeometry();
            geometry2.SetAttribute("position", new BufferAttribute<float>(new float[] { 4, 5, 6 }, 1, false));
            geometry2.SetIndex(new BufferAttribute<uint>(new uint[] { 0, 1, 2 }, 1, false));

            var mergedGeometry = BufferGeometryUtils.MergeBufferGeometries(new[] { geometry1, geometry2 });

            Assert.NotNull(mergedGeometry);

            var position = mergedGeometry.GetAttribute<float>("position");
            var positions = position.Array;

            Assert.Equal(positions, new float[] { 1, 2, 3, 4, 5, 6 });
            Assert.Equal(mergedGeometry.Index.Array, new uint[] { 0, 1, 2, 2, 1, 0, 3, 4, 5 });
            Assert.Equal(1, position.ItemSize);

        }

        [Fact()]
        public void MergeBufferGeometryInvalid()
        {
            var geometry1 = new BufferGeometry();
            geometry1.SetAttribute("position", new BufferAttribute<float>(new float[] { 1, 2, 3 }, 1, false));
            geometry1.SetIndex(new BufferAttribute<uint>(new uint[] { 0, 1, 2 }, 1, false));

            var geometry2 = new BufferGeometry();
            geometry2.SetAttribute("position", new BufferAttribute<float>(new float[] { 4, 5, 6 }, 1, false));

            Assert.Throws<Exception>(() => BufferGeometryUtils.MergeBufferGeometries(new[] { geometry1, geometry2 }));

            geometry2.SetIndex(new BufferAttribute<uint>(new uint[] { 0, 1, 2 }, 1, false));

            Assert.NotNull(BufferGeometryUtils.MergeBufferGeometries(new[] { geometry1, geometry2 }));

            geometry2.SetAttribute("foo", new BufferAttribute<float>(new float[] { 1, 2, 3 }, 1, false));

            Assert.Throws<Exception>(() => BufferGeometryUtils.MergeBufferGeometries(new[] { geometry1, geometry2 }));
        }
    }
}