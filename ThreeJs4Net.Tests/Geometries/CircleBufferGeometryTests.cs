using Xunit;
using ThreeJs4Net.Geometries;
using System;
using System.Collections.Generic;
using System.Text;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries.Tests
{
    public class CircleBufferGeometryTests
    {
        [Fact()]
        public void CircleBufferGeometryTest()
        {
            var circleGeometry = new CircleBufferGeometry(5, 32, 0, MathUtils.DEG2RAD * 90);
        }
    }
}