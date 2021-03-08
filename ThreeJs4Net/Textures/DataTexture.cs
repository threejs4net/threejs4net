namespace ThreeJs4Net.Textures
{
    public class DataTexture : Texture
    {
        public DataTexture(float[] data, TextureMapping mapping = null, int wrapS = 0
            , int wrapT = 0, int magFilter = 1003, int minFilter = 1003, int format = 0
            , int type = 0, int anisotropy = 1, int encoding = 1)
            : base (null, mapping, wrapS, wrapT , magFilter , minFilter , format ,  type , anisotropy)
        {
            //TODO: --> this.image = { data: data || null, width: width || 1, height: height || 1 };
            this.MagFilter = magFilter;
            this.MinFilter = minFilter;
            this.GenerateMipmaps = false;
            this.flipY = false;
            this.UnpackAlignment = 1;
            this.NeedsUpdate = true;
        }

        public DataTexture(int[] data, TextureMapping mapping = null, int wrapS = 0
            , int wrapT = 0, int magFilter = 1003, int minFilter = 1003, int format = 0
            , int type = 0, int anisotropy = 1, int encoding = 1)
            : base (null, mapping, wrapS, wrapT , magFilter , minFilter , format ,  type , anisotropy)
        {
            //TODO: --> this.image = { data: data || null, width: width || 1, height: height || 1 };
            this.MagFilter = magFilter;
            this.MinFilter = minFilter;
            this.GenerateMipmaps = false;
            this.flipY = false;
            this.UnpackAlignment = 1;
            this.NeedsUpdate = true;
        }
    }
}