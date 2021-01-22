using ThreeJs4Net.Core;

namespace ThreeJs4Net.Extras
{
    public class PlaneBufferGeometry : BufferGeometry
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="widthSegments"></param>
        /// <param name="heightSegments"></param>
        public PlaneBufferGeometry(int width, int height, int widthSegments = 1, int heightSegments = 1)
        {
            this.type = "PlaneBufferGeometry";

            var widthHalf = width / 2;
            var heightHalf = height / 2;

            var gridX = widthSegments;
            var gridY = heightSegments;

            var gridX1 = gridX + 1;
            var gridY1 = gridY + 1;

            var segmentWidth = width / gridX;
            var segmentHeight = height / gridY;

            var vertices = new float[gridX1 * gridY1 * 3];
            var normals = new float[gridX1 * gridY1 * 3];
            var uvs = new float[gridX1 * gridY1 * 2];

            var offset = 0;
            var offset2 = 0;

            for (var iy = 0; iy < gridY1; iy++)
            {

                var y = iy * segmentHeight - heightHalf;

                for (var ix = 0; ix < gridX1; ix++)
                {

                    var x = ix * segmentWidth - widthHalf;

                    vertices[offset] = x;
                    vertices[offset + 1] = -y;

                    normals[offset + 2] = 1;

                    uvs[offset2] = ix / (float)gridX;
                    uvs[offset2 + 1] = 1 - (iy / gridY);

                    offset += 3;
                    offset2 += 2;

                }

            }

            offset = 0;

            var typeArray = ((vertices.Length / 3) > ushort.MaxValue) ? typeof(uint) : typeof(ushort);
  //          var indices = Array.CreateInstance(typeArray, gridX * gridY * 6) as IList;

            var indices = new uint[gridX * gridY * 6];

            for (var iy = 0; iy < gridY; iy++)
            {
                for (var ix = 0; ix < gridX; ix++)
                {
                    var a = ix + gridX1 * iy;
                    var b = ix + gridX1 * (iy + 1);
                    var c = (ix + 1) + gridX1 * (iy + 1);
                    var d = (ix + 1) + gridX1 * iy;

                    indices[offset] = (uint)a;
                    indices[offset + 1] = (uint)b;
                    indices[offset + 2] = (uint)d;

                    indices[offset + 3] = (uint)b;
                    indices[offset + 4] = (uint)c;
                    indices[offset + 5] = (uint)d;

                    offset += 6;
                }
            }

            this.AddAttribute("index", new BufferAttribute<uint>(indices, 1));
            this.AddAttribute("position", new BufferAttribute<float>(vertices, 3));
            this.AddAttribute("normal", new BufferAttribute<float>(normals, 3));
            this.AddAttribute("uv", new BufferAttribute<float>(uvs, 2));
        }
    }
}
