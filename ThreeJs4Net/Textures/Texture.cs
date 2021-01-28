using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Textures
{
    public class MipMap
    {
        public byte[] Data;

        public int Width;

        public int Height;
    }

    public abstract class TextureMapping { }

    public class Texture : ITexture, ICloneable, IDisposable
    {
        #region Static Fields

        protected static int TextureIdCount;

        #endregion

        #region Fields

        private bool _disposed = false;

        public bool __webglInit = false;

        public int __webglTexture { get; set; }

        public int Anisotropy { get; set; }

        public int __oldAnisotropy = -1;

        public bool flipY = true;

        public bool GenerateMipmaps = true;

        public int Id = TextureIdCount++;

        public Guid Uuid = Guid.NewGuid();

        public string Name;

        public string SourceFile;

        public bool NeedsUpdate { get; set; }

        public Bitmap Image;

        public List<MipMap> Mipmaps;

        public TextureMapping Mapping; // !== undefined ? mapping : Three.Texture.DEFAULT_MAPPING;

        public int WrapS { get; set; }

        public int WrapT { get; set; }

        public int MagFilter { get; set; }

        public int MinFilter { get; set; }

        public int Format = Three.RGBAFormat;

        public int Type { get; set; }
        
        public Vector2 Offset = new Vector2(0, 0);

        public bool PremultiplyAlpha = false;

        public Vector2 Repeat = new Vector2(1, 1);

        // valid values: 1, 2, 4, 8 (see http://www.khronos.org/opengles/sdk/docs/man/xhtml/glPixelStorei.xml)
        public int UnpackAlignment = 4;

        private readonly TextureMapping defaultMapping = new Three.UVMapping();

        #endregion

        #region Constructors and Destructors

        protected Texture()
        {
            this.Anisotropy = 1;
            this.WrapS = Three.ClampToEdgeWrapping;
            this.WrapT = Three.ClampToEdgeWrapping;
            this.MagFilter = Three.LinearFilter;
            this.MinFilter = Three.LinearMipMapLinearFilter;
            this.Type = Three.UnsignedByteType;
            this.NeedsUpdate = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Texture(Bitmap image = null, TextureMapping mapping = null, int wrapS = 0, int wrapT = 0, int magFilter = 0, int minFilter = 0, int format = 0, int type = 0, int anisotropy = 1) : this()
        {
            this.Image = image;

            this.Mapping = mapping ?? this.defaultMapping;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Texture(Texture other)
        {
        }

        #endregion

        ///// <summary>
        ///// 
        ///// </summary>
        //public void Update()
        //{
            
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("id: {0}, filename: {1}, size = {2}", this.Id, Path.GetFileNameWithoutExtension(this.SourceFile), this.Image.Size);
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing A second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    this.RaiseDisposed();
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion

        #region Public Events

        public event EventHandler<EventArgs> Disposed;

        public event EventHandler<EventArgs> Updated;

        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void RaiseUpdated()
        {
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Texture(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}