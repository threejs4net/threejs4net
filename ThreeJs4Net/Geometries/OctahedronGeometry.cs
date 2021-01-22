namespace ThreeJs4Net.Geometries
{
    public class OctahedronGeometry : PolyhedronGeometry
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="detail"></param>
        public OctahedronGeometry(float radius, float detail)
        {
            var vertices = new float[] { 1, 0, 0, -1, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 1, 0, 0, -1 };

            var indices = new int[] { 0, 2, 4, 0, 4, 3, 0, 3, 5, 0, 5, 2, 1, 2, 5, 1, 5, 3, 1, 3, 4, 1, 4, 2 };

            this.Construct(vertices, indices, radius, detail);

            this.type = "OctahedronGeometry";
        }
    }
}
