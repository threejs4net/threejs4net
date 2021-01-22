using System.Drawing;
using ThreeJs4Net.Materials;

namespace ThreeJs4Net.Renderers.Renderables
{
    public class RenderableLine
    {
        public int id = 0;

        public RenderableVertex v1 = new RenderableVertex();

        public RenderableVertex v2 = new RenderableVertex();

        public Color[] vertexColors = { new Color(), new Color() };

        public Material material;

        public int z = 0;
    }
}
