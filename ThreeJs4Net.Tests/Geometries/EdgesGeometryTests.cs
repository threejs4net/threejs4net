using Xunit;
using ThreeJs4Net.Geometries;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using System.Collections.Generic;

namespace ThreeJs4Net.Tests.Geometries
{
   public class EdgesGeometryTests
    {
        [Fact()]
        public void TestEdgesGeometry()
        {
            var box = new BoxBufferGeometry(100,100,100);
            var edges = new EdgesGeometry(box);
            var material = new LineBasicMaterial();
            var lines = new LineSegments(edges,material);

            Raycaster raycaster = new Raycaster(new Vector3(0, -50, 0), new Vector3(0, 0, 1));
            var intersects = raycaster.IntersectObject(lines);

        }
    }
}


