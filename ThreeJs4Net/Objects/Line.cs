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
        public override void Raycast(Raycaster raycaster, List<Intersect> intersects)
        {
			var geometry = this.Geometry;
			var matrixWorld = this.MatrixWorld;
			var threshold = raycaster.LinePrecision;

			// Checking boundingSphere distance to ray

			if (geometry.BoundingSphere == null) geometry.ComputeBoundingSphere();

			_sphere.Copy(geometry.BoundingSphere);
			_sphere.ApplyMatrix4(matrixWorld);
			_sphere.Radius += threshold;

			if (!raycaster.Ray.IntersectsSphere(_sphere)) return;

			//

			_inverseMatrix.GetInverse(matrixWorld);
			_ray.Copy(raycaster.Ray).ApplyMatrix4(_inverseMatrix);

			var localThreshold = threshold / ((this.Scale.X + this.Scale.Y + this.Scale.Z) / 3);
			var localThresholdSq = localThreshold * localThreshold;

			var vStart = new Vector3();
			var vEnd = new Vector3();
			var interSegment = new Vector3();
			var interRay = new Vector3();
			var step = (this is Line && ((Line)this).Mode == Three.LineStrip ? 1 : 2);

			if (geometry is BufferGeometry)
			{
				var geometryB = (BufferGeometry)geometry;
				var index = geometryB.Index;
				//var attributes = geometryBuffer.Attributes;
				var positions = geometryB.GetAttribute<float>("position");

				if (index != null)
				{
					var indices = index.Array;

					for (var i = 0; i < indices.Length; i += step)
					{

						var a = (int)indices[i];
						var b = (int)indices[i + 1];

						vStart.FromArray(positions.Array, a * 3);
						vStart.FromArray(positions.Array, b * 3);

						var distSq = _ray.DistanceSqToSegment(vStart, vEnd, interRay, interSegment);

						if (distSq > localThresholdSq || float.IsNaN(distSq)) continue;

						interRay.ApplyMatrix4(this.MatrixWorld); //Move back to world space for distance calculation

						var distance = raycaster.Ray.Origin.DistanceTo(interRay);

						if (distance < raycaster.Near || distance > raycaster.Far) continue;

						intersects.Add(new Intersect()
						{
							Distance = distance,
							// What do we want? intersection point on the ray or on the segment??
							// point: raycaster.ray.at( distance ),
							Point = ((Vector3)interSegment.Clone()).ApplyMatrix4(this.MatrixWorld),
							Face = null,
							FaceIndex = -1,
							Object3D = this
						});
					}
				}

				else
				{
					for (var i = 0; i < positions.length / 3 - 1; i += step)	// CHANGE BACK TO i = 0
					{

						vStart.FromArray(positions.Array, 3 * i);
						vEnd.FromArray(positions.Array, 3 * i + 3);

						var distSq = _ray.DistanceSqToSegment(vStart, vEnd, interRay, interSegment);

						if (distSq > localThresholdSq || float.IsNaN(distSq)) continue;

						interRay.ApplyMatrix4(this.MatrixWorld); //Move back to world space for distance calculation

						var distance = raycaster.Ray.Origin.DistanceTo(interRay);

						if (distance < raycaster.Near || distance > raycaster.Far) continue;

						intersects.Add(new Intersect()
						{
							Distance = distance,
							// What do we want? intersection point on the ray or on the segment??
							// point: raycaster.ray.at( distance ),
							Point = ((Vector3)interSegment.Clone()).ApplyMatrix4(this.MatrixWorld),
							Face = null,
							FaceIndex = -1,
							Object3D = this
						});
					}
				}
			}
			else if (geometry is Geometry)
			{
				var geometryG = (Geometry)geometry;
				var vertices = geometryG.Vertices;
				var nbVertices = vertices.Count;

				for (var i = 0; i < nbVertices - 1; i += step)
				{
					var distSq = _ray.DistanceSqToSegment(vertices[i], vertices[i + 1], interRay, interSegment);

					if (distSq > localThresholdSq || float.IsNaN(distSq)) continue;

					interRay.ApplyMatrix4(this.MatrixWorld); //Move back to world space for distance calculation

					var distance = raycaster.Ray.Origin.DistanceTo(interRay);

					if (distance < raycaster.Near || distance > raycaster.Far) continue;

					intersects.Add(new Intersect()
					{
						Distance = distance,
						// What do we want? intersection point on the ray or on the segment??
						// point: raycaster.ray.at( distance ),
						Point = ((Vector3)interSegment.Clone()).ApplyMatrix4(this.MatrixWorld),
						Face = null,
						FaceIndex = -1,
						Object3D = this
					});
				}
			}
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
