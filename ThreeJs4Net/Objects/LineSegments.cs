using System;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Renderers.Shaders;

namespace ThreeJs4Net.Objects
{
    public class LineSegments : Line
    {
        public LineSegments()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LineSegments(BaseGeometry geometry, Material material) : base(geometry, material)
        {
            this.type = "LineSegments";
        }

        public LineSegments ComputeLineDistances()
        {
            var start = new Vector3();
            var end = new Vector3();

            var geometry = this.Geometry;

            if (geometry is BufferGeometry bufferGeometry)
            {
                // we assume non-indexed geometry
                if (bufferGeometry.Index == null)
                {
                    var positionAttribute = bufferGeometry.GetAttribute<float>("position");
                    var lineDistances = new float[positionAttribute.Count/2];

                    for (int i = 0; i < positionAttribute.Count; i+=2)
                    {
                        start.FromBufferAttribute(positionAttribute, i);
                        end.FromBufferAttribute(positionAttribute, i + 1);

                        lineDistances[i] = (i == 0) ? 0 : lineDistances[i - 1];
                        lineDistances[i + 1] = lineDistances[i] + start.DistanceTo(end);
                    }

                    bufferGeometry.SetAttribute("lineDistance", new BufferAttribute<float>(lineDistances, 1));
                }
                else
                {
                    throw new Exception("THREE.LineSegments.computeLineDistances(): Computation only possible with non-indexed BufferGeometry.");
                }
            }
            else
            {
                var lGeometry = geometry as Geometry;

                var vertices = lGeometry.Vertices;
                var lineDistances = lGeometry.LineDistances;

                for (int i = 0; i < vertices.Count; i+=2)
                {
                    start.Copy(vertices[i]);
                   end.Copy(vertices[i + 1]);

                    lineDistances[i] = (i == 0) ? 0 : lineDistances[i - 1];
                    lineDistances[i + 1] = lineDistances[i] + start.DistanceTo(end);
                }
            }
            return this;
        }
    }
}
