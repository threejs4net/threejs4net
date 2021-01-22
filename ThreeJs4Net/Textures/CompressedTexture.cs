using System.Collections.Generic;

namespace ThreeJs4Net.Textures
{
    public class CompressedTexture : Texture
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mipmaps"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="mapping"></param>
        /// <param name="wrapS"></param>
        /// <param name="wrapT"></param>
        /// <param name="magFilter"></param>
        /// <param name="minFilter"></param>
        /// <param name="anisotropy"></param>
        public CompressedTexture(List<MipMap> mipmaps = null, int width = 0, int height = 0, int format = 0, int type = 0, TextureMapping mapping = null, int wrapS = 0, int wrapT = 0, int magFilter = 0, int minFilter = 0, int anisotropy = 1) 
            : base (null, mapping, wrapS, wrapT , magFilter , minFilter , format ,  type , anisotropy)
        {
       //     this.Image = { width: width, height: height }; // new Bitmap ????
            this.Mipmaps = mipmaps;

            this.GenerateMipmaps = false;
        }
    }
}