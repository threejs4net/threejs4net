using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Helpers
{
    public class BoxHelper : LineSegments
    {
        private Color Color = System.Drawing.Color.Wheat;
        private uint[] Indices;
        private float[] Positions;
        private BufferGeometry Geometry;
        private bool MatrixAutoUpdate;

        public BoxHelper(Object3D obj, Color? color = null)
        {
            //if (color == null)
            //{
            //    this.Color = System.Drawing.Color.Wheat;
            //}


            //var indices = new uint[] { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };
            //var positions = new float[8 * 3];

            //this.Geometry = new BufferGeometry();
            //Geometry.SetIndex(new BufferAttribute<uint>(indices, 1));
            //Geometry.SetAttribute("position", new BufferAttribute<float>(positions, 3));

            //base(this.Geometry, new LineBasicMaterial()
            //{
            //    Color = this.Color,
            //})
        }
    }
}
