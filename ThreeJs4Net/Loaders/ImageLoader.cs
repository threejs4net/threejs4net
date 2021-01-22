using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ThreeJs4Net.Loaders
{
    public class ImageLoader
    {
        private readonly Cache cache;

        private LoadingManager manager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager"></param>
        public ImageLoader(LoadingManager manager = null)
        {
            this.cache = new Cache();
            this.manager = manager ?? Three.DefaultLoadingManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelFormat"></param>
        /// <returns></returns>
        public static int PixelFormatToThree(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return Three.RGBAFormat;
                case PixelFormat.Format32bppArgb:
                    return Three.RGBAFormat;
            }

            return Three.RGBAFormat; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="onLoad"></param>
        /// <param name="onProgress"></param>
        /// <param name="onError"></param>
        public Bitmap Load(string filename, Action<Bitmap> onLoad = null, Action onProgress = null, Action onError = null)
        {
            var cached = this.cache.Get(filename);

            if ( cached != null )
            {
                if (null != onLoad)
                    onLoad((Bitmap)cached);
                return (Bitmap)cached;
            }

            var image = new Bitmap(filename);

            image.RotateFlip(RotateFlipType.Rotate180FlipX);

            this.manager.ItemStart(filename);

            if (null != onLoad)
            {
                this.cache.Add(filename, image);

                onLoad(image);
                this.manager.ItemEnd(filename);
            }

            if (null != onProgress)
            {
            }

            if (null != onError)
            {
            }

            //if ( this.crossOrigin != null ) image.crossOrigin = this.crossOrigin;

            return image;
        }

        //public void SetCrossOrigin(object value)
        //{
        //    this.crossOrigin = value;
        //}
    }
}
