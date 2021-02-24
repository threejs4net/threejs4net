using Xunit;
using ThreeJs4Net.Core;
using System;
using System.Collections.Generic;
using System.Text;
using ThreeJs4Net.Math;
using ThreeJs4Net.Tests;

namespace ThreeJs4Net.Core.Tests
{
    public class RaycasterTests : BaseTests
    {
        private Raycaster getRaycaster()
        {
            return new Raycaster(
                new Vector3(0, 0, 0),
                new Vector3(0, 0, -1),
                1,
                100
            );
        }

        [Fact()]
        public void RaycasterTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void RaycasterTest1()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectObjectTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void IntersectObjectsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void SetFromCameraTest()
        {
            Assert.True(false, "This test needs an implementation");
        }


        [Fact()]
        public void PointsIntersectionThresholdTest()
        {

            //var raycaster = getRaycaster();
            //var coordinates = new Vector3[] { new Vector3(-2, 0, -5) };
            //var geometry = new BufferGeometry().SetFromPoints(coordinates);
            //var points = new Points(geometry, null);

            //raycaster.params.Points.threshold = 1.999;
            //assert.ok(raycaster.intersectObject(points).length === 0,
            //    "no Points intersection with a not-large-enough threshold");

            //raycaster.params.Points.threshold = 2.001;
            //assert.ok(raycaster.intersectObject(points).length === 1,
            //    "successful Points intersection with a large-enough threshold");
        }


    }
}