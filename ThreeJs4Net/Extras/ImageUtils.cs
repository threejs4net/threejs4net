using System.Drawing;
using ThreeJs4Net.Loaders;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Extras
{
    public class ImageUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static Texture LoadTexture(string url, TextureMapping mapping = null)
        {
            var image = (Bitmap)Image.FromFile(url, true);

            image.RotateFlip(RotateFlipType.Rotate180FlipX);

            return new Texture(image, mapping) { NeedsUpdate = true, SourceFile = url, Format = ImageLoader.PixelFormatToThree(image.PixelFormat) };
        }
    }
}

