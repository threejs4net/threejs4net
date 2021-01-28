using System;
using System.Collections;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Renderers
{
    public class WebGLRenderTarget : Texture, IDisposable
    {
        private bool _disposed;

        public int Width;
        public int Height;

        public bool? DepthBuffer = true;
        public bool? StencilBuffer = true;

        public int __webglRenderbuffer = -1;

        public int __webglFramebuffer = -1;

        public WebGLRenderTarget ShareDepthFrom = null;

        /// <summary>
        /// Constructor
        /// </summary>
        protected WebGLRenderTarget()
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
        public WebGLRenderTarget(int width, int height, IDictionary options = null) : this()
        {
            this.Width = width;
            this.Height = height;

            if (null != options)
            {
                this.DepthBuffer   = (null != options["depthBuffer"])   ? (bool)options["depthBuffer"]   : true;
                this.StencilBuffer = (null != options["stencilBuffer"]) ? (bool)options["stencilBuffer"] : true;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other"></param>
        protected WebGLRenderTarget(WebGLRenderTarget other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void setSize (int width,int height ) 
        {
		    this.Width = width;
		    this.Height = height;
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var tmp = new WebGLRenderTarget(this.Width, this.Height);

            tmp.WrapS = this.WrapS;
            tmp.WrapT = this.WrapT;

            tmp.MagFilter = this.MagFilter;
            tmp.MinFilter = this.MinFilter;

            tmp.Anisotropy = this.Anisotropy;

            tmp.Offset.Copy(this.Offset);
            tmp.Repeat.Copy(this.Repeat);

            tmp.Format = this.Format;
            tmp.Type = this.Type;

            tmp.DepthBuffer = this.DepthBuffer;
            tmp.StencilBuffer = this.StencilBuffer;

            tmp.GenerateMipmaps = this.GenerateMipmaps;

            tmp.ShareDepthFrom = this.ShareDepthFrom;

            return tmp;
        }

        public event EventHandler<EventArgs> Disposed;

        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
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
    }
}
