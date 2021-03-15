using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
	public class EdgesGeometry : BufferGeometry
	{
		//public int A;
		//public int B;
		//public int C;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="thresholdAngle"></param>
		public EdgesGeometry(BufferGeometry geometry, int thresholdAngle = 1)
		{
			List<float> vertices = new List<float>();

			var thresholdDot = Mathf.Cos(MathUtils.DEG2RAD * thresholdAngle);
			int[] edge = new int[2];
			List<string> edges = new List<string>();
			//int edge1;
			//int edge2;

			//int[] key, keys = { A, B, C };
			string key = null;
			string[] keys = { "A", "B", "C" };

			var geometry2 = new Geometry().FromBufferGeometry(geometry);

			geometry2.MergeVertices();
			geometry2.ComputeFaceNormals();

			var sourceVertices = geometry2.Vertices;
			var faces = geometry2.Faces;

			// now create a data structure where each entry represents an edge with its adjoining faces

			for (int i = 0; i < faces.Count; i++)
			{
				var face = faces[i];

				for (int j = 0; j < 3; j++)
				{
					var edge1 = (int)face.GetType().GetProperty(keys[j]).GetValue(face);
					var edge2 = (int)face.GetType().GetProperty(keys[(j + 1) % 3]).GetValue(face);
					edge[0] = Mathf.Min(edge1, edge2);
					edge[1] = Mathf.Min(edge1, edge2);

					key = edge[0].ToString() + ',' + edge[1].ToString();

					if (edges[key] == null)
					{
						edges[key] = { index1: edge[0], index2: edge[1], face1: i, face2: null };
					}

					else
					{
						edges[key] = i;
					}
				}
			}

			foreach (var e in edges)
			{
				if (e.face2 == null || faces[e.face1].Normal.Dot(faces[e.face2].Normal) <= thresholdAngle)
				{
					var vertex = sourceVertices[e.index1];
					vertices.Add(vertex.X, vertex.Y, vertex.Z);

					vertex = sourceVertices[e.index2];
					vertices.Add(vertex.X, vertex.Y, vertex.Z);
				}
			}

			this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
		}
	}
}
