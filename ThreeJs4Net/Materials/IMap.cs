using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Materials
{
    public interface IMap
    {
        Texture Map { get; set; }

        Texture AlphaMap { get; set; }

        Texture SpecularMap { get; set; }

        Texture NormalMap { get; set; }

        Texture BumpMap { get; set; }

        Texture LightMap { get; set; }
    }
}
