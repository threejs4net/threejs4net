using System.Collections;
using System.Drawing;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Materials
{
    public class PointsMaterial : Material
    {
        public Color Color { get; set; }
        public Texture Map { get; set; }
        public Texture AlphaMap { get; set; }
        public float Size { get; set; }
        public bool SizeAttenuation { get; set; }
        public bool MorphTargets { get; set; }

        public PointsMaterial(Hashtable parameters = null)
        {
            this.Color = Color.White;
            this.Map = null;
            this.AlphaMap = null;
            this.Size = 1;
            this.SizeAttenuation = true;
            this.MorphTargets = false;
            this.SetValues(parameters);

            this.SetValues(parameters);
        }
    }
}
