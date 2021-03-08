using System;
using System.Collections.Generic;
using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Objects
{
    public class Line : Object3D
    {
        public int LineStrip = 0;
        public int LinePieces = 1;

        private Vector3 _start = new Vector3();
        private Vector3 _end = new Vector3();
        private Matrix4 _inverseMatrix = new Matrix4();
        private Ray _ray = new Ray();
        private Sphere _sphere = new Sphere();


        public int Mode;

        /// <summary>
        /// Constructor
        /// </summary>
        public Line(BaseGeometry geometry = null, Material material = null, int? type = null)
        {
            this.type = "Line";

            this.Geometry = geometry ?? new Geometry();
            this.Material = material ?? new LineBasicMaterial { Color = new Color().Random() };

            this.Mode = Three.LineStrip;
            if (null != type) this.Mode = type.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RayCast()
        {
            throw new NotImplementedException();
        }

        public Line ComputeLineDistances()
        {
            if (this.Geometry is BufferGeometry bufGeometry)
            {
                // we assume non-indexed geometry
                if (bufGeometry.Index == null)
                {
                    var positionAttribute = bufGeometry.GetAttribute<float>("position");
                    var lineDistances = new List<float>();
                    lineDistances.Add(0);

                    for (var i = 1; i < positionAttribute.Count; i++)
                    {
                        _start.FromBufferAttribute(positionAttribute, i - 1);
                        _end.FromBufferAttribute(positionAttribute, i);

                        var lDistance = lineDistances[i - 1];
                        lDistance  += _start.DistanceTo(_end);
                        lineDistances.Add(lDistance);
                    }
                    bufGeometry.SetAttribute("lineDistance", new BufferAttribute<float>(lineDistances.ToArray(), 1));
                }
                else
                {
                    throw new Exception("Computation only possible with non-indexed BufferGeometry.");
                }
            }
            else if (this.Geometry is Geometry geometry)
            {
                var vertices = geometry.Vertices;
                var lineDistances = geometry.LineDistances;

                lineDistances.Add(0);

                for (var i = 1; i < vertices.Count; i++)
                {
                    var lDistance = lineDistances[i - 1];
                    lDistance += vertices[i - 1].DistanceTo(vertices[i]);
                    lineDistances.Add(lDistance);
                }
            }

            return this;
        }

    }
}
