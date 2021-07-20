using Xunit;
using ThreeJs4Net.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Geometries.Tests
{
    public class ExtrudeBufferGeometryTests
    {
        [Fact()]
        public void ExtrudeBufferGeometryTest1()
        {
            var triangleShape = new Shape();
            var triangleShape2 = new Shape();
            var vecCenter = new Vector2(0,0);
            var vecA = new Vector2(0, 5);
            var vecB = new Vector2(2, 0);
            triangleShape.MoveTo(vecCenter.X, vecCenter.Y);
            triangleShape.LineTo(vecA.X, vecA.Y);
            triangleShape.MoveTo(vecCenter.X, vecCenter.Y);
            triangleShape.LineTo(vecB.X, vecB.Y);
            triangleShape2.MoveTo(0, 0);
            triangleShape2.Ellipse(0, 0, vecB.X, vecA.Y, 0, Mathf.PI, false, 0);

            var options = new Hashtable {{"depth", 1}, {"bevelEnabled", false}};
            var geometry = new ExtrudeBufferGeometry(new[] {triangleShape2}, options);
            var mesh = new Mesh(geometry, new MeshBasicMaterial { Color = Color.Aqua });
            geometry.RotateX((Mathf.PI / 180) * -90);
        }

        [Fact()]
        public void ExtrudeBufferGeometryTest2()
        {
            var triangleShape = new Shape();
            var vecCenter = new Vector2(0,0);
            var vecA = new Vector2(0, 2);
            var vecB = new Vector2(2, 0);
            triangleShape.MoveTo(vecCenter.X, vecCenter.Y);
            triangleShape.LineTo(vecA.X, vecA.Y);
            triangleShape.MoveTo(vecCenter.X, vecCenter.Y);
            triangleShape.LineTo(vecB.X, vecB.Y);

            var options = new Hashtable {{"depth", 1}, {"bevelEnabled", false}};
            var geometry = new ExtrudeBufferGeometry(new[] {triangleShape}, options);
            geometry.RotateX((Mathf.PI / 180) * -90);
        }
    }
}