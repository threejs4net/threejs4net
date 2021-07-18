using System;
using System.Collections.Generic;
using System.Linq;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras;
using ThreeJs4Net.Extras.Core;
using ThreeJs4Net.Geometries;

namespace ThreeJs4Net.Geometries
{
    public class ShapeGeometry { }

    public class ShapeBufferGeometry : BufferGeometry
    {
        public ShapeBufferGeometry(Shape[] shapes, int curveSegments = 12) : base()
        {
            //this.parameters = {
            //	shapes: shapes,
            //	curveSegments: curveSegments
            //};

            // buffers

            var indices = new List<uint>();
            var vertices = new List<float>();
            var normals = new List<float>();
            var uvs = new List<float>();

            // helper variables
            var groupStart = 0;
            var groupCount = 0;

            for (var i = 0; i < shapes.Length; i++)
            {
                AddShape(shapes[i]);
                this.AddGroup(groupStart, groupCount, i); // enables MultiMaterial support
                groupStart += groupCount;
                groupCount = 0;
            }

            // build geometry
            this.SetIndex(new BufferAttribute<uint>(indices.ToArray(), 1));
            this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
            this.SetAttribute("normal", new BufferAttribute<float>(normals.ToArray(), 3));
            this.SetAttribute("uv", new BufferAttribute<float>(uvs.ToArray(), 2));

            void AddShape(Shape shape)
            {
                var indexOffset = vertices.Count / 3;
                var points = shape.ExtractPoints(curveSegments);

                var shapeVertices = points.shape;
                var shapeHoles = points.holes;

                // check direction of vertices

                if (!ShapeUtils.isClockWise(shapeVertices))
                {
                    shapeVertices.Reverse();
                }

                for (var i = 0; i < shapeHoles.Count; i++)
                {
                    var shapeHole = shapeHoles[i];
                    if (ShapeUtils.isClockWise(shapeHole))
                    {
                        var temp = shapeHole.ToList();
                        temp.Reverse();
                        shapeHoles[i] = temp.ToArray();
                    }
                }

                var faces = ShapeUtils.TriangulateShape(shapeVertices.ToArray(), shapeHoles);

                // join vertices of inner and outer paths to a single array

                for (var i = 0; i < shapeHoles.Count; i++)
                {
                    var shapeHole = shapeHoles[i];
                    shapeVertices = shapeVertices.Concat(shapeHole).ToList();
                }

                // vertices, normals, uvs
                for (var i = 0; i < shapeVertices.Count; i++)
                {
                    var vertex = shapeVertices[i];
                    vertices.Add(vertex.X);
                    vertices.Add(vertex.Y);
                    vertices.Add(0);
                    normals.Add(0);
                    normals.Add(0);
                    normals.Add(1);
                    uvs.Add(vertex.X);
                    uvs.Add(vertex.Y); // world uvs
                }

                // indices 
                for (var i = 0; i < faces.Count; i++)
                {
                    var face = faces[i];
                    var a = face[0] + indexOffset;
                    var b = face[1] + indexOffset;
                    var c = face[2] + indexOffset;
                    indices.Add((uint)a);
                    indices.Add((uint)b);
                    indices.Add((uint)c);
                    groupCount += 3;
                }
            }
        }
    }
}