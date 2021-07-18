using Xunit;
using ThreeJs4Net.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Geometries.Tests
{
    public class ShapeBufferGeometryTests
    {
        [Fact()]
        public void ShapeBufferGeometryTest()
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

            var geoshape = new ShapeBufferGeometry(new [] { triangleShape2 });
            geoshape.RotateX((Mathf.PI / 180) * 90);

            var geomesh = new Mesh(geoshape, new MeshBasicMaterial { Color = Color.Aqua });


        }
    }
}