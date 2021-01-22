using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;

namespace ThreeJs4Net.Helpers
{
    public class GridHelper : Line
    {
        public Color Color1;

        public Color Color2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="step"></param>
        public GridHelper(float size, float step)
        {
            var colorConvertor = new ColorConverter();

            var geometry = new Geometry();
            var material = new LineBasicMaterial { VertexColors = Three.VertexColors };

            this.Color1 = (Color)colorConvertor.ConvertFromString("#444444");
            this.Color2 = (Color)colorConvertor.ConvertFromString("#888888");

            this.Color1 = Color.DeepSkyBlue; // TODO deze kleur komt door
            this.Color2 = Color.GreenYellow; // TODO deze kleur komt NIET door

            for ( var i = - size; i <= size; i += step ) 
            {
                geometry.Vertices.AddRange( new [] {
                    new Vector3( - size, 0, i ), new Vector3(size, 0, i),
                    new Vector3(i, 0, - size), new Vector3( i, 0, size ) }  
                );

                var color = (i == 0) ? this.Color1 : this.Color2;

                geometry.Colors.AddRange( new [] { color, color, color, color });
            }

            this.Geometry = geometry;
            this.Material = material;
            this.Mode = this.LinePieces;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorCenterLine"></param>
        /// <param name="colorGrid"></param>
        public void SetColors(Color colorCenterLine, Color colorGrid)
        {
            this.Color1 = colorCenterLine;
            this.Color1 = colorGrid;
            ((Geometry)this.Geometry).ColorsNeedUpdate = true;
        }

    }
}
