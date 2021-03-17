using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;


namespace ThreeJs4Net.Geometries
{
	public class EdgesGeometry : BufferGeometry
	{
		/// <summary>
		/// This can be used as a helper object to view the edges of a geometry.
		/// </summary>
		/// <param name="geometry"> Any geometry object. </param>
		/// <param name="thresholdAngle">  An edge is only rendered if the angle (in degrees) between the face normals of the adjoining faces exceeds this value. default = 1 degree. </param>
		public EdgesGeometry(BufferGeometry geometry, int thresholdAngle = 1)
		{
			this.type = "EdgesGeometry";

			// buffer

			List<float> vertices = new List<float>();

			// helper variables

			var thresholdDot = Mathf.Cos(MathUtils.DEG2RAD * thresholdAngle);
			int[] edge = new int[2];
			IDictionary<string, Dictionary<string, int>> edges = new Dictionary<string, Dictionary<string, int>>();
			string[] keys = { "A", "B", "C" };

			// prepare source geometry

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
					var edge1 = keys[j] == "A" ? face.A : keys[j] == "B" ? face.B : face.C;
					var edge2 = keys[(j + 1) % 3] == "A" ? face.A : keys[(j + 1) % 3] == "B" ? face.B : face.C;
					edge[0] = Mathf.Min(edge1, edge2);
					edge[1] = Mathf.Max(edge1, edge2);

					string key = $"{edge[0]},{edge[1]}";

					if (!edges.ContainsKey(key))
					{
						var temp = new Dictionary<string, int> { { "index1", edge[0] }, { "index2", edge[1] }, { "face1", i }, { "face2", -1 } };	// ints cant be null
						edges.Add(key, temp);
					}

					else
					{
						edges[key]["face2"] = i;
					}
				}
			}

			// generate vertices

			var edgesValues = edges.Values;

			foreach (var e in edgesValues)
			{
				if (e["face2"] == -1 || faces[e["face1"]].Normal.Dot(faces[e["face2"]].Normal) <= thresholdDot)
				{
					var vertex = sourceVertices[e["index1"]];
					vertices.Add(vertex.X, vertex.Y, vertex.Z);

					vertex = sourceVertices[e["index2"]];
					vertices.Add(vertex.X, vertex.Y, vertex.Z);
				}
			}

			this.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
		}
	}
}
