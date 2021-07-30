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
        public void ExtrudeBufferGeometryTest4()
        {
            Shape smileyShape = new Shape(null);
            smileyShape.MoveTo(80, 40);
            smileyShape.Absarc(40, 40, 40, 0, Mathf.PI * 2, false);

            var smileyEye1Path = new Path();
            smileyEye1Path.MoveTo(35, 20);
            smileyEye1Path.Absellipse(25, 20, 10, 10, 0, Mathf.PI * 2, true, 0);

            var smileyEye2Path = new Path();
            smileyEye2Path.MoveTo(65, 20);
            smileyEye2Path.Absarc(55, 20, 10, 0, Mathf.PI * 2, true);

            var smileyMouthPath = new Path();
            smileyMouthPath.MoveTo(20, 40);
            smileyMouthPath.QuadraticCurveTo(40, 60, 60, 40);
            smileyMouthPath.BezierCurveTo(70, 45, 70, 50, 60, 60);
            smileyMouthPath.QuadraticCurveTo(40, 80, 20, 60);
            smileyMouthPath.QuadraticCurveTo(5, 50, 20, 40);

            //smileyShape.holes = new List<Path>();
            smileyShape.holes.Add(smileyEye1Path);
            smileyShape.holes.Add(smileyEye2Path);
            smileyShape.holes.Add(smileyMouthPath);

            var options = new Hashtable {{"depth", 1}, {"bevelEnabled", false}};
            //var geometry = new ExtrudeBufferGeometry(new[] {smileyShape}, options);
            var geometry = new ShapeBufferGeometry(new[] {smileyShape});
            geometry.RotateX((Mathf.PI / 180) * -90);
        }


        [Fact()]
        public void ExtrudeBufferGeometryTest3()
        {
            Shape smileyShape = new Shape(null);
            smileyShape.MoveTo(80, 40);
            smileyShape.Absarc(40, 40, 40, 0, Mathf.PI * 2, false);

            var smileyEye1Path = new Path();
            smileyEye1Path.MoveTo(35, 20);
            smileyEye1Path.Absellipse(25, 20, 10, 10, 0, Mathf.PI * 2, true, 0);

            var smileyEye2Path = new Path();
            smileyEye2Path.MoveTo(65, 20);
            smileyEye2Path.Absarc(55, 20, 10, 0, Mathf.PI * 2, true);

            var smileyMouthPath = new Path();
            smileyMouthPath.MoveTo(20, 40);
            smileyMouthPath.QuadraticCurveTo(40, 60, 60, 40);
            smileyMouthPath.BezierCurveTo(70, 45, 70, 50, 60, 60);
            smileyMouthPath.QuadraticCurveTo(40, 80, 20, 60);
            smileyMouthPath.QuadraticCurveTo(5, 50, 20, 40);

            //smileyShape.holes = new List<Path>();
            smileyShape.holes.Add(smileyEye1Path);
            //smileyShape.holes.Add(smileyEye2Path);
            //smileyShape.holes.Add(smileyMouthPath);

            var options = new Hashtable {{"depth", 1}, {"bevelEnabled", false}};
            var geometry = new ExtrudeBufferGeometry(new[] {smileyShape}, options);
            var mesh = new Mesh(geometry, new MeshBasicMaterial { Color = Color.Aqua });
            geometry.RotateX((Mathf.PI / 180) * -90);
        }


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