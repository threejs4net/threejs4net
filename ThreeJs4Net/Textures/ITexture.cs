namespace ThreeJs4Net.Textures
{
    public interface ITexture
    {
        int WrapS { get; set; }

        int WrapT { get; set; }

        int MagFilter { get; set; }

        int MinFilter { get; set; }

        int Type { get; set; }

        int Anisotropy { get; set; }

        int __webglTexture { get; set; }

        bool NeedsUpdate { get; set; }
    }
}
