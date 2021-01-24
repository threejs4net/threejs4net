using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Demo.examples.cs.loaders
{
    public class Dds
    {
        public List<MipMap> Mipmaps = new List<MipMap>();

        public int Width;

        public int Height;

        public int Format;

        public int MipmapCount = 1;

        public bool IsCubemap;
    }

    public class TextureLoaderEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryLoaderEventArgs"/> class.
        /// </summary>
        /// <param name="texture">
        /// The channel carrier.
        /// </param>
        public TextureLoaderEventArgs(Texture texture)
        {
            this.Texture = texture;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the channel carrier.
        /// </summary>
        public Texture Texture { get; private set; }

        #endregion
    }

    public class DDSLoader
    {
        public event EventHandler<TextureLoaderEventArgs> Loaded;

        protected virtual void RaiseLoaded(Texture texture)
        {
            var handler = this.Loaded;
            if (handler != null)
            {
                handler(this, new TextureLoaderEventArgs(texture));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CompressedTexture Load(string[] filenames, Action<Texture> callback = null)
        {
            throw new NotImplementedException();

            return null;
        }

        /// <summary>
        /// compressed cubemap texture stored in a single DDS file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public CompressedTexture Load(string filename, Action<Texture> callback = null)
        {
            List<Dds> images = null;

            var texture = new CompressedTexture() { NeedsUpdate = true, SourceFile = filename };
            //texture.Image = images;

            // no flipping for cube textures
            // (also flipping doesn't work for compressed textures )

            texture.flipY = false;

            // can't generate mipmaps for compressed textures
            // mips must be embedded in DDS files

            texture.GenerateMipmaps = false;

            // compressed cubemap texture stored in a single DDS file

            var buffer = File.ReadAllBytes(filename);

            var dds = this.Parse(buffer);

            if (dds.IsCubemap)
            {
                var faces = dds.Mipmaps.Count / dds.MipmapCount;

                images = new List<Dds>(faces);
                for (var i = 0; i < faces; i++)
                    images.Add(new Dds());

                for (var f = 0; f < faces; f ++)
                {
                    for (var i = 0; i < dds.MipmapCount; i ++)
                    {
                        images[f].Mipmaps.Add(dds.Mipmaps[f * dds.MipmapCount + i]);
                        images[f].Format = dds.Format;
                        images[f].Width = dds.Width;
                        images[f].Height = dds.Height;
                    }
                }

                texture.Image = new Bitmap(dds.Width, dds.Height);
                texture.Mipmaps = dds.Mipmaps;
            }
            else
            {
                texture.Image = new Bitmap(dds.Width, dds.Height);
                texture.Mipmaps = dds.Mipmaps;
            }

            Debug.Assert(null != texture.Image, "No image loaded in DDSLoader");
            Debug.Assert(0 < dds.Width, "image with empty width in DDSLoader");
            Debug.Assert(0 < dds.Height, "image with empty height in DDSLoader");

            texture.Format = dds.Format;
            texture.NeedsUpdate = true;

            return texture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="loadMipmaps"></param>
        private Dds Parse(byte[] buffer, bool loadMipmaps = false)
        {
            var dds = new Dds();

            // Adapted from @toji's DDS utils
            //	https://github.com/toji/webgl-texture-utils/blob/master/texture-util/dds.js

            // All values and structures referenced from:
            // http://msdn.microsoft.com/en-us/library/bb943991.aspx/

            var DDS_MAGIC = 0x20534444;

            var DDSD_CAPS = 0x1;
            var DDSD_HEIGHT = 0x2;
            var DDSD_WIDTH = 0x4;
            var DDSD_PITCH = 0x8;
            var DDSD_PIXELFORMAT = 0x1000;
            var DDSD_MIPMAPCOUNT = 0x20000;
            var DDSD_LINEARSIZE = 0x80000;
            var DDSD_DEPTH = 0x800000;

            var DDSCAPS_COMPLEX = 0x8;
            var DDSCAPS_MIPMAP = 0x400000;
            var DDSCAPS_TEXTURE = 0x1000;

            var DDSCAPS2_CUBEMAP = 0x200;
            var DDSCAPS2_CUBEMAP_POSITIVEX = 0x400;
            var DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800;
            var DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000;
            var DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000;
            var DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000;
            var DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000;
            var DDSCAPS2_VOLUME = 0x200000;

            var DDPF_ALPHAPIXELS = 0x1;
            var DDPF_ALPHA = 0x2;
            var DDPF_FOURCC = 0x4;
            var DDPF_RGB = 0x40;
            var DDPF_YUV = 0x200;
            var DDPF_LUMINANCE = 0x20000;
          
            var FOURCC_DXT1 = FourCcToInt32("DXT1");
            var FOURCC_DXT3 = FourCcToInt32("DXT3");
            var FOURCC_DXT5 = FourCcToInt32("DXT5");

            var headerLengthInt = 31; // The header length in 32 bit ints

            // Offsets into the header array

            var off_magic = 0;

            var off_size = 1;
            var off_flags = 2;
            var off_height = 3;
            var off_width = 4;

            var off_mipmapCount = 7;

            var off_pfFlags = 20;
            var off_pfFourCC = 21;
            var off_RGBBitCount = 22;
            var off_RBitMask = 23;
            var off_GBitMask = 24;
            var off_BBitMask = 25;
            var off_ABitMask = 26;

            var off_caps = 27;
            var off_caps2 = 28;
            var off_caps3 = 29;
            var off_caps4 = 30;

		    // Parse header

            var header = new Int32[headerLengthInt];
            Buffer.BlockCopy(buffer, 0, header, 0, headerLengthInt * sizeof(int));

		    if ( header[ off_magic ] != DDS_MAGIC )
		    {
		        Trace.TraceError("THREE.DDSLoader.parse: Invalid magic number in DDS header.");
			    return dds;
		    }

            //if ((header[off_pfFlags] & DDPF_FOURCC) == 0)
            //{
            //    Trace.TraceError("THREE.DDSLoader.parse: Unsupported format, must contain a FourCC code.");
            //    return dds;
            //}

            var blockBytes = 0;

            var fourCC = header[off_pfFourCC];

            var isRGBAUncompressed = false;

            if (fourCC == FOURCC_DXT1)
            {
                blockBytes = 8;
                dds.Format = Three.RGB_S3TC_DXT1_Format;
            }
            else if (fourCC == FOURCC_DXT3)
            {
                blockBytes = 16;
                dds.Format = Three.RGBA_S3TC_DXT3_Format;
            }
            else if (fourCC == FOURCC_DXT5)
            {
                blockBytes = 16;
                dds.Format = Three.RGBA_S3TC_DXT3_Format;
            }
            else
            {
                if (header[off_RGBBitCount] == 32
                &&  (header[off_RBitMask] & 0xff0000) > 0
                &&  (header[off_GBitMask] & 0xff00) > 0
                &&  (header[off_BBitMask] & 0xff) > 0
                &&  (header[off_ABitMask] & 0xff000000) > 0)
                {
                    isRGBAUncompressed = true;
                    blockBytes = 64;
                    dds.Format = Three.RGBAFormat;
                }
                else
                {
                    Trace.TraceError("THREE.DDSLoader.parse: Unsupported FourCC code ");
                    return dds;
                }
            }

            dds.MipmapCount = 1;

            if ( ((header[ off_flags ] & DDSD_MIPMAPCOUNT) > 0) && loadMipmaps != false )
            {
                dds.MipmapCount = System.Math.Max(1, header[off_mipmapCount]);
            }
       
            //TODO: Verify that all faces of the cubemap are present with DDSCAPS2_CUBEMAP_POSITIVEX, etc.

            dds.IsCubemap = ((header[off_caps2] & DDSCAPS2_CUBEMAP) > 0) ? true : false;

            dds.Width = header[off_width];
            dds.Height = header[off_height];

            var dataOffset = header[off_size] + 4;

            // Extract mipmaps buffers

            var width = dds.Width;
            var height = dds.Height;

            var faces = dds.IsCubemap ? 6 : 1;

            for ( var face = 0; face < faces; face ++ ) {

                for ( var i = 0; i < dds.MipmapCount; i ++ )
                {
                    var dataLength = 0;
                    byte[] byteArray = null;

                    if( isRGBAUncompressed ) 
                    {
                        byteArray = loadARGBMip( buffer, dataOffset, width, height );
                        dataLength = byteArray.Length;
                    } 
                    else 
                    {
                        dataLength = System.Math.Max( 4, width ) / 4 * System.Math.Max( 4, height ) / 4 * blockBytes;

                        byteArray = new byte[dataLength];
                        Buffer.BlockCopy(buffer, dataOffset, byteArray, 0, dataLength);
                        //var byteArray = new Uint8Array( buffer, dataOffset, dataLength );
                    }

                    var mipmap = new MipMap() { Data = byteArray, Width = width, Height = height };
                    dds.Mipmaps.Add(mipmap);

                    dataOffset += dataLength;

                    width = (int)System.Math.Max( width * 0.5, 1 );
                    height = (int)System.Math.Max( height * 0.5, 1 );
                }

                width = dds.Width;
                height = dds.Height;
            }
                                    
		    return dds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int FourCcToInt32(string value )
        {
			return value[0] +
				  (value[1] << 8) +
				  (value[2] << 16) +
				  (value[3] << 24);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Int32ToFourCc(int value )
        {
            return "";
            //return String.fromCharCode(
            //    value & 0xff,
            //    (value >> 8) & 0xff,
            //    (value >> 16) & 0xff,
            //    (value >> 24) & 0xff
            //);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="dataOffset"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private byte[] loadARGBMip(byte[] buffer, int dataOffset, int width, int height )
        {
			var dataLength = width * height * 4;

			//var srcBuffer = new Uint8Array( buffer, dataOffset, dataLength );
            var srcBuffer = new byte[dataLength];
            Buffer.BlockCopy(buffer, dataOffset, srcBuffer, 0, dataLength);

            var byteArray = new byte[dataLength];

			var dst = 0;
			var src = 0;
			for ( var y = 0; y < height; y++ ) {
				for ( var x = 0; x < width; x++ ) {
					var b = srcBuffer[src]; src++;
					var g = srcBuffer[src]; src++;
					var r = srcBuffer[src]; src++;
					var a = srcBuffer[src]; src++;
					byteArray[dst] = r; dst++;	//r
					byteArray[dst] = g; dst++;	//g
					byteArray[dst] = b; dst++;	//b
					byteArray[dst] = a; dst++;	//a
				}
			}

			return byteArray;
		}


    }
}
