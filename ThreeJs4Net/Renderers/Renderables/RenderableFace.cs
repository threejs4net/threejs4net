using System.Drawing;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Renderers.Renderables
{
    public class RenderableFace
    {
        public int id = 0;

        public RenderableVertex v1 = new RenderableVertex();
        public RenderableVertex v2 = new RenderableVertex();
        public RenderableVertex v3 = new RenderableVertex();

        public Vector3 normalModel = new Vector3();

        public Vector3[] vertexNormalsModel = { new Vector3(), new Vector3(), new Vector3() };
        public int vertexNormalsLength = 0;

        public Color color;
        public Material material;
        public Vector2[] uvs = {new Vector2(), new Vector2(), new Vector2() };

        public int z = 0;

    }
}
