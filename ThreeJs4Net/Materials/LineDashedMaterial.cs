using System.Collections;
using System.Drawing;

namespace ThreeJs4Net.Materials
{
    public class LineDashedMaterial : Material
    {
        public Color Color;

        public float Linewidth;

        public float Scale;

        public float DashSize;

        public float GapSize;

        public string Linecap;

        public string Linejoin;

        public bool Fog;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public LineDashedMaterial(Hashtable parameters = null)
        {
            this.Color = Color.White;

            this.type = "LineDashedMaterial";

            this.Linewidth = 1.0f;

            this.Scale = 1;
            this.DashSize = 3;
            this.GapSize = 1;

            //this.vertexColors = false;

            this.Fog = true;

            this.SetValues( parameters );
        }
   }
}
