using System.Collections;
using System.Drawing;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Materials
{
    public class PointCloudMaterial : Material, IMap
    {
        public Color Color = Color.White;
        public float Size = 1;
        public bool SizeAttenuation = true;
        public bool Fog = true;

        public Texture Map { get; set; }
        public Texture AlphaMap { get; set; }
        public Texture SpecularMap { get; set; }
        public Texture NormalMap { get; set; } // TODO: not in ThreeJs, just to be an IMap. Must be NULL
        public Texture BumpMap { get; set; } // TODO: not in ThreeJs, just to be an IMap.  Must be NULL
        public Texture LightMap { get; set; }
        
        public PointCloudMaterial(Hashtable parameters = null)
        {
            this.type = "PointCloudMaterial";

            this.SetValues(parameters);
        }
    }
}
