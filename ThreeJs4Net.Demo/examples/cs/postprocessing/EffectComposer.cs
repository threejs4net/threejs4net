using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Demo.examples.cs.shaders;
using ThreeJs4Net.Renderers;

namespace ThreeJs4Net.Demo.examples.cs.postprocessing
{
    public class EffectComposer
    {
        private WebGLRenderer _renderer;

        private WebGLRenderTarget renderTarget1;

        private WebGLRenderTarget renderTarget2;

        private WebGLRenderTarget writeBuffer;

        private WebGLRenderTarget readBuffer;

        private List<IPass> passes;

        private ShaderPass copyPass;


        /// <summary>
        /// Constructor
        /// Rendertarget will be the same size as the control
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="control"></param>
        public EffectComposer(WebGLRenderer renderer, Control control)
        {
            var width = control.Width;
            var height = control.Height;
            var parameters = new Hashtable {
                { "minFilter", Three.LinearFilter },
                { "magFilter", Three.LinearFilter },
                { "format", Three.RGBFormat },
                { "stencilBuffer", false }};

            var renderTarget = new WebGLRenderTarget(width, height, parameters);

            Initialize(renderer, renderTarget);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderTarget"></param>
        public EffectComposer(WebGLRenderer renderer, WebGLRenderTarget renderTarget)
        {
            Initialize(renderer, renderTarget);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="renderTarget"></param>
        private void Initialize(WebGLRenderer renderer, WebGLRenderTarget renderTarget)
        {
            Debug.Assert(null != renderer);
            Debug.Assert(null != renderTarget);

            this._renderer = renderer;

            this.renderTarget1 = renderTarget;
            this.renderTarget2 = (WebGLRenderTarget)renderTarget.Clone();

            this.writeBuffer = this.renderTarget1;
            this.readBuffer = this.renderTarget2;

            this.passes = new List<IPass>();

            //if ( THREE.CopyShader == null )
            //    Trace.TraceError( "THREE.EffectComposer relies on THREE.CopyShader" );

            this.copyPass = new ShaderPass(new CopyShader());
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwapBuffers()
        {
            var tmp = this.readBuffer;
            this.readBuffer = this.writeBuffer;
            this.writeBuffer = tmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        public void AddPass(IPass pass)
        {
            this.passes.Add(pass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="index"></param>
        public void InsertPass(IPass pass, int index)
        {
            this.passes.Insert(index, pass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Render(float delta = 0)
        {
            this.writeBuffer = this.renderTarget1;
            this.readBuffer = this.renderTarget2;

        	var maskActive = false;

		    foreach (var pass in passes)
            {
			    if ( !pass.Enabled ) continue;

                pass.Render(this._renderer, this.writeBuffer, this.readBuffer, delta);

			    if ( pass.NeedsSwap ) {

				    if ( maskActive )
				    {
				        GL.StencilFunc(StencilFunction.Notequal, 1, 0xffffffff);

                        this.copyPass.Render(this._renderer, this.writeBuffer, this.readBuffer, delta);

				        GL.StencilFunc(StencilFunction.Equal, 1, 0xffffffff);
				    }

			        this.SwapBuffers();
			    }

			    if ( pass is MaskPass ) {
				    maskActive = true;
			    } else if ( pass is ClearMaskPass ) {
				    maskActive = false;
			    }

		    }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderTarget"></param>
        public void Reset(WebGLRenderTarget renderTarget)
        {
		    if ( renderTarget == null ) 
            {
                throw new NotImplementedException();

                renderTarget = (WebGLRenderTarget)this.renderTarget1.Clone();

                //renderTarget.Width = window.innerWidth;
                //renderTarget.Height = window.innerHeight;
		    }

		    this.renderTarget1 = renderTarget;
            this.renderTarget2 = (WebGLRenderTarget)renderTarget.Clone();

		    this.writeBuffer = this.renderTarget1;
		    this.readBuffer = this.renderTarget2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(int width, int height)
        {
            var renderTarget = (WebGLRenderTarget)this.renderTarget1.Clone();

            renderTarget.Width = width;
            renderTarget.Height = height;

            this.Reset(renderTarget);
        }

    }
}
