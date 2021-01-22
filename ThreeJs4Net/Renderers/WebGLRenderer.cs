using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Core;
using ThreeJs4Net.Extras.Objects;
using ThreeJs4Net.Helpers;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Renderers.WebGL;
using ThreeJs4Net.Renderers.WebGL.PlugIns;
using ThreeJs4Net.Scenes;
using ThreeJs4Net.Textures;
using Attribute = ThreeJs4Net.Renderers.Shaders.Attribute;

namespace ThreeJs4Net.Renderers
{
    // Based on version 68, 69

    public struct Info
    {
        public struct Memory
        {
            public int Programs;

            public int Geometries;

            public int Textures;
        }

        public struct Render
        {
            public int Calls;

            public int Vertices;

            public int Faces;

            public int Points;
        }

        public Memory memory;

        public Render render;
    }

    public class WebGLRenderer : IDisposable
    {
        //private var _canvas = parameters.canvas != = undefined ? parameters.canvas : document.createElement('canvas'),
        //            _context = null;

        private bool _disposed;

        #region Fields

        public int MaxAnisotropy;

        public int MaxTextureSize;

        public int MaxTextures;

        public int MaxVertexTextures;

        public Info Info;

        public Size Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;

                //      _canvas.width = size.Width;
                //      _canvas.height = size.Height;

                //if ( updateStyle !== false ) {

                //    _canvas.style.width = width + 'px';
                //    _canvas.style.height = height + 'px';

                //}

                this.SetViewport(0, 0, this.Size.Width, this.Size.Height);
            }
        }

        private int[] _enabledAttributes = new int[16];

        private int[] _newAttributes = new int[16];

        private List<WebGlProgram> _programs = new List<WebGlProgram>();

        public bool AutoClear;

        private bool autoClearColor;

        private bool autoClearDepth;

        private bool autoClearStencil;

        public int ClearAlpha;

        private int maxMorphNormals = 4;

        private int maxMorphTargets = 8;

        private List<WebGlObject> opaqueObjects = new List<WebGlObject>();

        //private List<Object3D> renderPluginsPost = new List<Object3D>();

        private ShaderLib shaderLib;

        private bool shadowMapCascade;

        private bool shadowMapDebug;

        public bool shadowMapEnabled;

        public int shadowMapType = Three.PCFShadowMap;

        public bool SortObjects;

        // physically based shading

        public bool gammaInput = false;

        public bool gammaOutput = false;

        private readonly bool supportsBoneTextures;

        private readonly bool supportsVertexTextures;

        private readonly List<WebGlObject> transparentObjects = new List<WebGlObject>();

        private bool _alpha = false;

        private bool _antialias = false;

        private int _contextGlobalCompositeOperation;

        private Camera _currentCamera;

        private long _currentGeometryGroupHash;

        private int _currentMaterialId;

        private int? _currentProgram = null;

        private int _currentFramebuffer = -1;

        private bool _depth = true;

        private Matrix4 _frustum = new Matrix4();

        private bool _logarithmicDepthBuffer = false;

        // GL state cache

        private float _oldLineWidth = -1;

        private bool? _oldDepthTest = null;

        private bool? _oldDepthWrite = null;

        private int _oldBlendDst = -1;

        private int _oldBlendEquation = -1;

        private int _oldBlendSrc = -1;

        private int _oldBlending = -1;

        private int _oldDoubleSided = -1;

        private int _oldFlipSided = -1;

        private bool _oldPolygonOffset;

        private float _oldPolygonOffsetFactor = -1;

        private float _oldPolygonOffsetUnits = -1;

        private string _precision = "highp";

        private bool _premultipliedAlpha = true;

        private bool _preserveDrawingBuffer = false;

        private Matrix4 _projScreenMatrix;

        private Matrix4 _projScreenMatrixPS = new Matrix4();

        private bool _stencil = true;

        private int _usedTextureUnits;

        private bool autoScaleCubemaps = true;

        public Color ClearColor;

        private int[] compressedTextureFormats;

        private float fragmentShaderPrecisionHighpFloat;

        private float fragmentShaderPrecisionLowpFloat;

        private float fragmentShaderPrecisionMediumpFloat;

        private bool glExtensionCompressedTextureS3TC;

        private bool glExtensionElementIndexUint;

        private bool glExtensionStandardDerivatives;

        private bool glExtensionTextureFilterAnisotropic;

        private bool glExtensionTextureFloat;

        private Int32 maxCubemapSize;

        private bool shadowMapAutoUpdate = true;

        private int shadowMapCullFace = Three.CullFaceFront;

        private float vertexShaderPrecisionHighpFloat;

        private float vertexShaderPrecisionLowpFloat;

        private float vertexShaderPrecisionMediumpFloat;

        private Size _size;

        private int _viewportX;

        private int _viewportY;

        private int _viewportWidth;

        private int _viewportHeight;

        private int _currentWidth;

        private int _currentHeight;

        private int devicePixelRatio = 1;

        // light arrays cache

        private Vector3 _direction = new Vector3();

        private bool _lightsNeedUpdate = true;

        private readonly List<Light> Lights = new List<Light>();

        private LightCollection _lights = new LightCollection();

        private Dictionary<int, List<WebGlObject>> _webglObjects = new Dictionary<int, List<WebGlObject>>();

        private List<WebGlObject> _webglObjectsImmediate = new List<WebGlObject>();

        private List<Object3D> sprites = new List<Object3D>();

        private List<Object3D> lensFlares = new List<Object3D>();

        private ShadowMapPlugin shadowMapPlugin;

        private SpritePlugin spritePlugin;

        private LensFlarePlugin lensFlarePlugin;

        private List<string> extensions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public WebGLRenderer(Control control)
        {
            //	        Trace.TraceInformation( "Three.WebGLRenderer {0}", Three.Version );

            this.shaderLib = new ShaderLib();

            this.ClearColor = Color.Black;
            this.ClearAlpha = 0;

            this.opaqueObjects.Clear();
            this.transparentObjects.Clear();

            this._viewportX = 0;
            this._viewportY = 0;
            this._viewportWidth = control.Width;
            this._viewportHeight = control.Width;

            // clearing

            this.AutoClear = true;
            this.autoClearColor = true;
            this.autoClearDepth = true;
            this.autoClearStencil = true;

            // scene graph

            this.SortObjects = true;

            // shadow map

            this.shadowMapEnabled = false;
            this.shadowMapAutoUpdate = true;
            this.shadowMapType = Three.PCFShadowMap;
            this.shadowMapCullFace = Three.CullFaceFront;
            this.shadowMapDebug = false;
            this.shadowMapCascade = false;

            // morphs

            this.maxMorphTargets = 8;
            this.maxMorphNormals = 4;

            // flags

            this.autoScaleCubemaps = true;

            this.extensions = new List<string>((GL.GetString(StringName.Extensions)).Split(' '));

            this.InitGl();

            this.SetDefaultGlState();

            // Plugins

            this.shadowMapPlugin = new ShadowMapPlugin(this, this._lights, this._webglObjects, this._webglObjectsImmediate);

            this.spritePlugin = new SpritePlugin(this, this.sprites);
            this.lensFlarePlugin = new LensFlarePlugin(this, this.lensFlares);

            // GPU capabilities

            GL.GetInteger(GetPName.MaxTextureSize, out this.MaxTextureSize);
            GL.GetInteger(GetPName.MaxTextureImageUnits, out this.MaxTextures);
            GL.GetInteger(GetPName.MaxVertexTextureImageUnits, out this.MaxVertexTextures);
            GL.GetInteger(GetPName.MaxCubeMapTextureSize, out this.maxCubemapSize);

            if (this.glExtensionTextureFilterAnisotropic)
                GL.GetInteger((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out this.MaxAnisotropy);

            this.supportsVertexTextures = (this.MaxVertexTextures > 0);
            this.supportsBoneTextures = this.supportsVertexTextures && this.glExtensionTextureFloat;

            int compressedTextureFormatCount;
            GL.GetInteger(GetPName.NumCompressedTextureFormats, out compressedTextureFormatCount);
            if (compressedTextureFormatCount > 0)
            {
                this.compressedTextureFormats = new int[compressedTextureFormatCount];
                GL.GetInteger(GetPName.CompressedTextureFormats, this.compressedTextureFormats);
            }

            //vertexShaderPrecisionHighpFloat   = _gl.getShaderPrecisionFormat( _gl.VERTEX_SHADER, _gl.HIGH_FLOAT );
            //vertexShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat( _gl.VERTEX_SHADER, _gl.MEDIUM_FLOAT );
            //vertexShaderPrecisionLowpFloat    = _gl.getShaderPrecisionFormat( _gl.VERTEX_SHADER, _gl.LOW_FLOAT );

            //fragmentShaderPrecisionHighpFloat = _gl.getShaderPrecisionFormat( _gl.FRAGMENT_SHADER, _gl.HIGH_FLOAT );
            //fragmentShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat( _gl.FRAGMENT_SHADER, _gl.MEDIUM_FLOAT );
            //fragmentShaderPrecisionLowpFloat = _gl.getShaderPrecisionFormat( _gl.FRAGMENT_SHADER, _gl.LOW_FLOAT );

        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
	    public void SetClearColor(Color color, int alpha = 255)
        {
            this.ClearColor = color;
            this.ClearAlpha = alpha;

            GL.ClearColor(Color.FromArgb(this.ClearAlpha, this.ClearColor));
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetViewport(int x, int y, int width, int height)
        {
            this._viewportX = x * this.devicePixelRatio;
            this._viewportY = y * this.devicePixelRatio;

            this._viewportWidth = width * this.devicePixelRatio;
            this._viewportHeight = height * this.devicePixelRatio;

            GL.Viewport(this._viewportX, this._viewportY, this._viewportWidth, this._viewportHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetScissor(int x, int y, int width, int height)
        {
            GL.Scissor(
                x * this.devicePixelRatio,
                y * this.devicePixelRatio,
                width * this.devicePixelRatio,
                height * this.devicePixelRatio);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
	    public void EnableScissorTest(bool enable)
        {
            if (enable)
                GL.Enable(EnableCap.ScissorTest);
            else
                GL.Disable(EnableCap.ScissorTest);
        }

        /// <summary>
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        public void ClearTarget(WebGLRenderTarget renderTarget, bool color, bool depth, bool stencil)
        {
            this.SetRenderTarget(renderTarget);

            this.Clear(color, depth, stencil);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="framebuffer"></param>
        /// <param name="renderTarget"></param>
        /// <param name="textureTarget"></param>
        private void SetupFrameBuffer(int framebuffer, WebGLRenderTarget renderTarget, TextureTarget textureTarget)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, textureTarget, renderTarget.__webglTexture, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderbuffer"></param>
        /// <param name="renderTarget"></param>
        private void SetupRenderBuffer(int renderbuffer, WebGLRenderTarget renderTarget)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbuffer);

            if (renderTarget.DepthBuffer.Value && !renderTarget.StencilBuffer.Value)
            {

                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, renderTarget.Width, renderTarget.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, renderbuffer);

                /* For some reason this is not working. Defaulting to RGBA4.
                } else if ( ! renderTarget.depthBuffer && renderTarget.stencilBuffer ) {

                    GL.renderbufferStorage( GL.RENDERBUFFER, GL.STENCIL_INDEX8, renderTarget.width, renderTarget.height );
                    GL.framebufferRenderbuffer( GL.FRAMEBUFFER, GL.STENCIL_ATTACHMENT, GL.RENDERBUFFER, renderbuffer );
                */
            }
            else if (renderTarget.DepthBuffer.Value && renderTarget.StencilBuffer.Value)
            {
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthStencil, renderTarget.Width, renderTarget.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer);
            }
            else
            {
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Rgba4, renderTarget.Width, renderTarget.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderTarget"></param>
        private void SetRenderTarget(WebGLRenderTarget renderTarget)
        {
            var isCube = (renderTarget is WebGLRenderTargetCube);

            if ((null != renderTarget) && (renderTarget.__webglFramebuffer <= 0))
            {
                if (renderTarget.DepthBuffer == null) renderTarget.DepthBuffer = true;
                if (renderTarget.StencilBuffer == null) renderTarget.StencilBuffer = true;

                renderTarget.Disposed += onRenderTargetDispose;

                renderTarget.__webglTexture = GL.GenTexture();

                this.Info.memory.Textures++;

                // Setup texture, create render and frame buffers

                var isTargetPowerOfTwo = IsPowerOfTwo(renderTarget.Width) && IsPowerOfTwo(renderTarget.Height);
                var glInternalFormat = (PixelInternalFormat)paramThreeToGL(renderTarget.Format);
                var glFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                var glType = (PixelType)paramThreeToGL(renderTarget.Type);

                if (isCube)
                {
                    throw new NotImplementedException();
                    /*
                                        renderTarget.__webglFramebuffer = [];
                                        renderTarget.__webglRenderbuffer = [];

                                        GL.BindTexture(TextureTarget.TextureCubeMap, renderTarget.__webglTexture );
                                        setTextureParameters( GL.TEXTURE_CUBE_MAP, renderTarget, isTargetPowerOfTwo );

                                        for ( var i = 0; i < 6; i ++ ) {

                                            renderTarget.__webglFramebuffer[ i ] = GL.CreateFramebuffer();
                                            renderTarget.__webglRenderbuffer[ i ] = GL.CreateRenderbuffer();

                                            GL.TexImage2D( GL.TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, glFormat, renderTarget.width, renderTarget.height, 0, glFormat, glType, null );

                                            setupFrameBuffer( renderTarget.__webglFramebuffer[ i ], renderTarget, GL.TEXTURE_CUBE_MAP_POSITIVE_X + i );
                                            setupRenderBuffer( renderTarget.__webglRenderbuffer[ i ], renderTarget );
                                        }
                    */
                    if (isTargetPowerOfTwo)
                        GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

                }
                else
                {
                    renderTarget.__webglFramebuffer = GL.GenFramebuffer();

                    if (null != renderTarget.ShareDepthFrom)
                    {
                        renderTarget.__webglRenderbuffer = renderTarget.ShareDepthFrom.__webglRenderbuffer;
                    }
                    else
                    {
                        renderTarget.__webglRenderbuffer = GL.GenRenderbuffer();
                    }

                    GL.BindTexture(TextureTarget.Texture2D, renderTarget.__webglTexture);
                    SetTextureParameters(TextureTarget.Texture2D, renderTarget, isTargetPowerOfTwo);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, glInternalFormat, renderTarget.Width, renderTarget.Height, 0, glFormat, glType, IntPtr.Zero);

                    this.SetupFrameBuffer(renderTarget.__webglFramebuffer, renderTarget, TextureTarget.Texture2D);

                    if (null != renderTarget.ShareDepthFrom)
                    {

                        if (renderTarget.DepthBuffer.Value && !renderTarget.StencilBuffer.Value)
                        {

                            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, renderTarget.__webglRenderbuffer);

                        }
                        else if (renderTarget.DepthBuffer.Value && renderTarget.StencilBuffer.Value)
                        {

                            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderTarget.__webglRenderbuffer);

                        }

                    }
                    else
                    {
                        this.SetupRenderBuffer(renderTarget.__webglRenderbuffer, renderTarget);
                    }

                    if (isTargetPowerOfTwo) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }

                // Release everything

                if (isCube)
                {
                    GL.BindTexture(TextureTarget.TextureCubeMap, 0);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            var framebuffer = -1;
            int width;
            int height;
            int vx;
            int vy;

            if (null != renderTarget)
            {
                if (isCube)
                {
                    var renderTargetCube = renderTarget as WebGLRenderTargetCube;
                    //framebuffer = renderTarget.__webglFramebuffer[ renderTargetCube.activeCubeFace ];
                }
                else
                {
                    framebuffer = renderTarget.__webglFramebuffer;
                }

                width = renderTarget.Width;
                height = renderTarget.Height;

                vx = 0;
                vy = 0;
            }
            else
            {
                framebuffer = -1;

                width = this._viewportWidth;
                height = this._viewportHeight;

                vx = this._viewportX;
                vy = this._viewportY;
            }

            if (framebuffer != this._currentFramebuffer)
            {

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
                GL.Viewport(vx, vy, width, height);

                this._currentFramebuffer = framebuffer;
            }

            this._currentWidth = width;
            this._currentHeight = height;
        }

        void onRenderTargetDispose(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateRenderTargetMipmap(WebGLRenderTarget renderTarget)
        {
            if (renderTarget is WebGLRenderTargetCube)
            {
                GL.BindTexture(TextureTarget.TextureCubeMap, renderTarget.__webglTexture);
                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.__webglTexture);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="renderTarget"></param>
        /// <param name="forceClear"></param>
        public void Render(Scene scene, Camera camera, WebGLRenderTarget renderTarget = null, bool forceClear = false)
        {
            Debug.Assert(null != scene, "OpenTKRenderer.Render: scene can not be null");
            Debug.Assert(null != camera, "OpenTKRenderer.Render: camera can not be null");

            var fog = scene.Fog;

            // reset caching for this frame

            this._currentGeometryGroupHash = -1;
            this._currentMaterialId = -1;
            this._currentCamera = null;
            this._lightsNeedUpdate = true;

            // update scene graph

            if (scene.AutoUpdate) scene.UpdateMatrixWorld();

            // update camera matrices and frustum

            if (null == camera.Parent) camera.UpdateMatrixWorld();

            // update Skeleton objects

            scene.Traverse(
                object3D =>
                {
                    if (object3D is SkinnedMesh)
                    {
                        //object3D.skeleton.update();
                    }
                });

            camera.MatrixWorldInverse = camera.MatrixWorld.GetInverse();

            this._projScreenMatrix = camera.ProjectionMatrix * camera.MatrixWorldInverse;
            //     this._frustum = new Matrix4().FromMatrix(this._projScreenMatrix); // TODO


            this.Lights.Clear();
            this.opaqueObjects.Clear();
            this.transparentObjects.Clear();

            this.sprites.Clear();
            this.lensFlares.Clear();

            this.ProjectObject(scene, scene, camera);

            if (this.SortObjects)
            {
                this.opaqueObjects.Sort((a, b) =>
                       {
                           if (a.material.Id != b.material.Id)
                           {
                               return b.material.Id - a.material.Id;
                           }
                           else if (a.z != b.z)
                           {
                               return (int)(b.z - a.z);
                           }
                           else
                           {
                               return (int)(a.id - b.id);
                           }
                       });

                this.transparentObjects.Sort((a, b) =>
                       {
                           if (a.z != b.z)
                           {
                               return (int)(a.z - b.z);
                           }
                           else
                           {
                               return (int)(a.id - b.id);
                           }
                       });
            }

            // custom render plugins (pre pass)

            this.shadowMapPlugin.Render(scene, camera);

            //
            this.Info.render.Calls = 0;
            this.Info.render.Vertices = 0;
            this.Info.render.Faces = 0;
            this.Info.render.Points = 0;

            this.SetRenderTarget(renderTarget);

            if (this.AutoClear || forceClear)
            {
                this.Clear(this.autoClearColor, this.autoClearDepth, this.autoClearStencil);
            }

            // Set matrices for regular objects (frustum culled)

            // Set matrices for immediate objects

            foreach (var webglObject in this._webglObjectsImmediate)
            {
                var object3D = webglObject.object3D;
                if (object3D.Visible)
                {
                    SetupMatrices(object3D, camera);

                    //UnrollImmediateBufferMaterial( webglObject );
                }
            }

            if (null != scene.OverrideMaterial)
            {
                var material = scene.OverrideMaterial;

                this.SetBlending(material.Blending, material.BlendEquation, material.BlendSrc, material.BlendDst);
                this.SetDepthTest(material.DepthTest);
                this.SetDepthWrite(material.DepthWrite);
                this.SetPolygonOffset(material.PolygonOffset, material.PolygonOffsetFactor, material.PolygonOffsetUnits);

                this.RenderObjects(this.opaqueObjects, camera, this.Lights, fog, true, material);
                this.RenderObjects(this.transparentObjects, camera, this.Lights, fog, true, material);
                this.RenderObjectsImmediate(this._webglObjectsImmediate, string.Empty, camera, this.Lights, fog, false, material);
            }
            else
            {
                Material material = null;

                // opaque pass (front-to-back Order)

                this.SetBlending(Three.NoBlending);

                this.RenderObjects(this.opaqueObjects, camera, this.Lights, fog, false, material);
                this.RenderObjectsImmediate(this._webglObjectsImmediate, "opaque", camera, this.Lights, fog, false, material);

                // transparent pass (back-to-front Order)

                this.RenderObjects(this.transparentObjects, camera, this.Lights, fog, true, material);
                this.RenderObjectsImmediate(this._webglObjectsImmediate, "transparent", camera, this.Lights, fog, true, material);
            }

            // custom render plugins (post pass)

            this.spritePlugin.Render(scene, camera);
            this.lensFlarePlugin.Render(scene, camera, this._currentWidth, this._currentHeight);


            // Generate mipmap if we're using any kind of mipmap filtering
            if ((null != renderTarget) && renderTarget.GenerateMipmaps
                                       && renderTarget.MinFilter != Three.NearestFilter
                                       && renderTarget.MinFilter != Three.LinearFilter)
            {
                this.UpdateRenderTargetMipmap(renderTarget);
            }

            // Ensure depth buffer writing is enabled so it can be cleared on next render

            this.SetDepthTest(true);
            this.SetDepthWrite(true);

            // GL.finish();
        }

        /// <summary>
        /// </summary>
        public void InitGl()
        {
            if (this.extensions.Contains("OES_texture_float") || this.extensions.Contains("GL_ARB_texture_float"))
            {
                this.glExtensionTextureFloat = true;
            }

            if (this.extensions.Contains("OES_standard_derivatives"))
            {
                this.glExtensionStandardDerivatives = true;
            }

            if (this.extensions.Contains("EXT_texture_filter_anisotropic")
                || this.extensions.Contains("GL_EXT_texture_filter_anisotropic")
                || this.extensions.Contains("MOZ_EXT_texture_filter_anisotropic")
                || this.extensions.Contains("WEBKIT_EXT_texture_filter_anisotropic"))
            {
                this.glExtensionTextureFilterAnisotropic = true;
            }

            if (this.extensions.Contains("WEBGL_compressed_texture_s3tc")
                || this.extensions.Contains("MOZ_WEBGL_compressed_texture_s3tc")
                || this.extensions.Contains("WEBKIT_WEBGL_compressed_texture_s3tc") || this.extensions.Contains("GL_S3_s3tc"))
            {
                this.glExtensionCompressedTextureS3TC = true;
            }

            this.glExtensionElementIndexUint = true;
        }

        /// <summary>
        /// </summary>
        public void SetDefaultGlState()
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.ClearDepth(1.0f);
            GL.ClearStencil(0);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Viewport(this._viewportX, this._viewportY, this._viewportWidth, this._viewportHeight);

            GL.ClearColor(Color.FromArgb(this.ClearAlpha, this.ClearColor));
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        public void Clear(bool color = true, bool depth = true, bool stencil = true)
        {
            ClearBufferMask bits = 0;

            if (color)
            {
                bits |= ClearBufferMask.ColorBufferBit;
            }
            if (depth)
            {
                bits |= ClearBufferMask.DepthBufferBit;
            }
            if (stencil)
            {
                bits |= ClearBufferMask.StencilBufferBit;
            }

            GL.Clear(bits);
        }

        //private static void ClearColor()
        //{
        //    GL.Clear(ClearBufferMask.ColorBufferBit);
        //}

        //private static void ClearDepth()
        //{
        //    GL.Clear(ClearBufferMask.DepthBufferBit);
        //}

        //private static void ClearStencil()
        //{
        //    GL.Clear(ClearBufferMask.StencilBufferBit);
        //}

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="something"></param>
        /// <param name="object3D"></param>
        private void AddBufferImmediate(object something, Object3D object3D)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="buffer"></param>
        /// <param name="object3D"></param>
        private void AddBuffer(Dictionary<int, List<WebGlObject>> objlist, BaseGeometry buffer, Object3D object3D)
        {
            var id = object3D.id;

            List<WebGlObject> webGlObjects = null;
            if (!objlist.TryGetValue(id, out webGlObjects))
            {
                webGlObjects = new List<WebGlObject>();
                objlist.Add(id, webGlObjects);
            }

            var webGlObject = new WebGlObject { id = id, buffer = buffer, object3D = object3D, material = null, z = 0 };

            webGlObjects.Add(webGlObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private static bool AreCustomAttributesDirty(IAttributes material)
        {
            if (null == material) return false;

            foreach (var entry in material.Attributes)
            {

                var attribute = material.Attributes[entry.Key];
                Debug.Assert(null != attribute, "Failed to cast material.Attributes[{0}] to Hashtable", entry.Key);

                if (attribute.ContainsKey("needsUpdate"))
                {
                    if ((bool)attribute["needsUpdate"])
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        private static void ClearCustomAttributes(IAttributes material)
        {
            if (null == material) return;

            foreach (var entry in material.Attributes)
            {
                var attribute = material.Attributes[entry.Key];
                Debug.Assert(null != attribute, "Failed to cast material.Attributes[{0}] to Hashtable", entry.Key);

                if (attribute.ContainsKey("needsUpdate"))
                {
                    attribute["needsUpdate"] = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        private void RemoveObject(Object3D object3D)
        {
            if (object3D is Mesh ||
                 object3D is PointCloud ||
                 object3D is Line)
            {
                _webglObjects[object3D.id].Clear();
                _webglObjects[object3D.id] = null;
            }
            else if (object3D is ImmediateRenderObject || ((ImmediateRenderObject)object3D).immediateRenderCallback != null)
            {
                RemoveInstances(_webglObjectsImmediate, object3D);
            }

            object3D.__webglInit = false;
            object3D._modelViewMatrix = null;
            object3D._normalMatrix = null;

            object3D.__webglActive = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="object3D"></param>
        private void RemoveInstances(IList<WebGlObject> objlist, Object3D object3D)
        {
            for (var o = objlist.Count - 1; o >= 0; o--)
            {
                if (objlist[o].object3D == object3D)
                {

                    objlist.RemoveAt(o);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private static void InitLineBuffers(Geometry geometry, Object3D object3D)
        {
            var nvertices = geometry.Vertices.Count;

            geometry.__vertexArray = new float[nvertices * 3];
            geometry.__colorArray = new float[nvertices * 3];
            geometry.__lineDistanceArray = new float[nvertices * 1];

            geometry.__webglLineCount = nvertices;

            InitCustomAttributes(geometry, object3D);
        }

        // Reset

        public void ResetGlState()
        {
            this._currentProgram = null;
            this._currentCamera = null;

            this._oldBlending = -1;
            this._oldDepthTest = null;
            this._oldDepthWrite = null;
            this._oldDoubleSided = -1;
            this._oldFlipSided = -1;
            this._currentGeometryGroupHash = -1;
            this._currentMaterialId = -1;

            this._lightsNeedUpdate = true;
        }

        // Buffer allocation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        private void CreateParticleBuffers(Geometry geometry)
        {
            GL.GenBuffers(1, out geometry.__webglVertexBuffer);
            GL.GenBuffers(1, out geometry.__webglColorBuffer);

            this.Info.memory.Geometries++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        private void CreateLineBuffers(Geometry geometry)
        {
            GL.GenBuffers(1, out geometry.__webglVertexBuffer);
            GL.GenBuffers(1, out geometry.__webglColorBuffer);
            GL.GenBuffers(1, out geometry.__webglLineDistanceBuffer);

            this.Info.memory.Geometries++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private static void InitParticleBuffers(Geometry geometry, Object3D object3D)
        {
            var nvertices = geometry.Vertices.Count;

            geometry.__vertexArray = new float[nvertices * 3];
            geometry.__colorArray = new float[nvertices * 3];

            geometry.__sortArray = new Hashtable();

            geometry.__webglParticleCount = nvertices;

            InitCustomAttributes(geometry, object3D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private static void InitCustomAttributes(Geometry geometry, Object3D object3D)
        {
            var nvertices = geometry.Vertices.Count;

            if (object3D.Material is IAttributes)
            {
                var material = object3D.Material as IAttributes;

                if (geometry.__webglCustomAttributesList == null)
                {
                    geometry.__webglCustomAttributesList = new List<Shaders.Attribute>();
                }

                foreach (var a in material.Attributes)
                {
                    var attribute = material.Attributes[a.Key];

                    if (!attribute.ContainsKey("__webglInitialized") || attribute.ContainsKey("createUniqueBuffers"))
                    {
                        attribute["__webglInitialized"] = true;

                        var size = 1;   // "f" and "i"

                        if ((string)attribute["type"] == "v2") size = 2;
                        else if ((string)attribute["type"] == "v3") size = 3;
                        else if ((string)attribute["type"] == "v4") size = 4;
                        else if ((string)attribute["type"] == "C") size = 3;

                        attribute["size"] = size;

                        attribute["array"] = new float[nvertices * size];

                        int bufferId = 0;
                        GL.GenBuffers(1, out bufferId);

                        attribute["buffer"] = new Hashtable();

                        ((Hashtable)attribute["buffer"]).Add("id", bufferId);
                        ((Hashtable)attribute["buffer"]).Add("belongsToAttribute", a.Key);

                        attribute["needsUpdate"] = true;
                    }

                    geometry.__webglCustomAttributesList.Add(attribute);
                }
            }
        }

        private readonly Dictionary<int, List<GeometryGroup>> geometryGroups = new Dictionary<int, List<GeometryGroup>>();
        private int geometryGroupCounter = 0;

        /// <summary>
        /// 
        /// </summary>
        public List<GeometryGroup> MakeGroups(Geometry geometry, bool usesFaceMaterial)
        {
            var maxVerticesInGroup = this.extensions.Contains("OES_element_index_uint") ? uint.MaxValue : (uint)short.MaxValue;

            var hash_map = new Hashtable();

            var numMorphTargets = geometry.MorphTargets.Count;
            var numMorphNormals = geometry.MorphNormals.Count;

            GeometryGroup group = null;
            var groups = new Hashtable();
            var groupsList = new List<GeometryGroup>();

            for (var f = 0; f < geometry.Faces.Count; f++)
            {
                var face = geometry.Faces[0];

                var materialIndex = (usesFaceMaterial) ? face.MaterialIndex : 0;

                if (!hash_map.ContainsKey(materialIndex))
                {
                    hash_map[materialIndex] = 0;
                }
                var groupHash = string.Format("{0}_{1}", materialIndex, hash_map[materialIndex]);

                if (!groups.ContainsKey(groupHash))
                {
                    group = new GeometryGroup { Id = this.geometryGroupCounter++, Faces3 = new List<int>(), MaterialIndex = materialIndex, Vertices = 0, NumMorphTargets = numMorphTargets, NumMorphNormals = numMorphNormals };
                    groups[groupHash] = group;
                    groupsList.Add(group);
                }

                if (((GeometryGroup)groups[groupHash]).Vertices + 3 > maxVerticesInGroup)
                {
                    hash_map[materialIndex] = (int)hash_map[materialIndex] + 1;
                    groupHash = string.Format("{0}_{1}", materialIndex, hash_map[materialIndex]);

                    if (!groups.ContainsKey(groupHash))
                    {
                        group = new GeometryGroup { Id = this.geometryGroupCounter++, Faces3 = new List<int>(), MaterialIndex = materialIndex, Vertices = 0, NumMorphTargets = numMorphTargets, NumMorphNormals = numMorphNormals };
                        groups[groupHash] = group;
                        groupsList.Add(group);
                    }
                }

                ((GeometryGroup)groups[groupHash]).Faces3.Add(f);
                ((GeometryGroup)groups[groupHash]).Vertices = ((GeometryGroup)groups[groupHash]).Vertices + 3;
            }

            return groupsList;
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="geometry"></param>
        private void InitGeometryGroups(Scene scene, Object3D object3D, Geometry geometry)
        {
            var addBuffers = false;

            var material = object3D.Material;

            if (!this.geometryGroups.ContainsKey(geometry.Id) || geometry.GroupsNeedUpdate)
            {
                //     	    delete this._webglObjects[object3D.id].;
                this._webglObjects.Remove(object3D.id);

                this.geometryGroups[geometry.Id] = this.MakeGroups(geometry, material is MeshFaceMaterial);

                geometry.GroupsNeedUpdate = false;
            }

            var geometryGroupsList = this.geometryGroups[geometry.Id];

            // create separate VBOs per geometry chunk

            foreach (var geometryGroup in geometryGroupsList)
            {
                // initialise VBO on the first access

                if (0 == geometryGroup.__webglVertexBuffer)
                {
                    this.CreateMeshBuffers(geometryGroup);
                    this.InitMeshBuffers(geometryGroup, object3D);

                    geometry.VerticesNeedUpdate = true;
                    geometry.MorphTargetsNeedUpdate = true;
                    geometry.ElementsNeedUpdate = true;
                    geometry.UvsNeedUpdate = true;
                    geometry.NormalsNeedUpdate = true;
                    geometry.ColorsNeedUpdate = true;

                    Debug.Assert(0 != geometryGroup.__webglVertexBuffer);

                    addBuffers = true;
                }
                else
                {
                    addBuffers = false;
                }

                if (addBuffers || object3D.__webglActive == false)
                {
                    this.AddBuffer(this._webglObjects, geometryGroup, object3D);
                }
            }

            object3D.__webglActive = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="scene"></param>
        private void InitObject(Object3D object3D, Scene scene)
        {
            if (object3D.__webglInit == false)
            {
                object3D.__webglInit = true;
                object3D._modelViewMatrix = new Matrix4().Identity();
                object3D._normalMatrix = new Matrix3().Identity();

                object3D.Removed += this.OnObjectRemoved;
            }

            var geometry = object3D.Geometry;

            if (null == geometry)
            {
                // ImmediateRenderObject
            }
            else if (object3D.Geometry.__webglInit == false)
            {
                geometry.__webglInit = true;
                geometry.Disposed += this.OnGeometryDispose;

                if (geometry is BufferGeometry)
                {
                    //
                }
                else if (object3D is Mesh)
                {
                    this.InitGeometryGroups(scene, object3D, object3D.Geometry as Geometry);
                }
                else if (object3D is Line)
                {
                    if (object3D.Geometry.__webglVertexBuffer == 0)
                    {
                        this.CreateLineBuffers((Geometry)geometry);
                        InitLineBuffers((Geometry)geometry, object3D);

                        ((Geometry)geometry).VerticesNeedUpdate = true;
                        ((Geometry)geometry).ColorsNeedUpdate = true;
                        ((Geometry)geometry).LineDistancesNeedUpdate = true;
                    }
                }
                else if (object3D is PointCloud)
                {
                    if (object3D.Geometry.__webglVertexBuffer == 0)
                    {
                        this.CreateParticleBuffers((Geometry)geometry);
                        InitParticleBuffers((Geometry)geometry, object3D);

                        ((Geometry)geometry).VerticesNeedUpdate = true;
                        ((Geometry)geometry).ColorsNeedUpdate = true;
                    }
                }
            }

            if (object3D.__webglActive == false)
            {
                object3D.__webglActive = true;

                if (object3D is Mesh)
                {
                    if (object3D.Geometry is BufferGeometry)
                    {
                        this.AddBuffer(this._webglObjects, object3D.Geometry as BufferGeometry, object3D);
                    }
                    else if (object3D.Geometry is Geometry)
                    {
                        var geometryGroupsList = this.geometryGroups[geometry.Id];

                        foreach (var geometryGroup in geometryGroupsList)
                        {
                            this.AddBuffer(this._webglObjects, geometryGroup, object3D);
                        }
                    }
                }
                else if (object3D is Line || object3D is PointCloud)
                {
                    this.AddBuffer(this._webglObjects, object3D.Geometry, object3D);
                }
                else if (object3D is ImmediateRenderObject && ((ImmediateRenderObject)object3D).immediateRenderCallback != null)
                {
                    this.AddBufferImmediate(this._webglObjectsImmediate, object3D);
                }
            }
        }

        // Events

        void OnObjectRemoved(object sender, EventArgs e)
        {
            var object3D = sender as Object3D;
            Debug.Assert(null != object3D);

            object3D.Traverse(child =>
            {
                child.Removed -= this.OnObjectRemoved;
                RemoveObject(child);
            });
        }

        void OnGeometryDispose(object sender, EventArgs e)
        {
            var geometry = sender as BaseGeometry;
            Debug.Assert(null != geometry);

            geometry.Disposed -= this.OnGeometryDispose;

            DeallocateGeometry(geometry);
        }

        void OnTextureDispose(object sender, EventArgs e)
        {
            var texture = sender as Texture;
            Debug.Assert(null != texture);

            texture.Disposed -= this.OnTextureDispose;

            this.DeallocateTexture(texture);

            this.Info.memory.Textures--;
        }

        void OnMaterialDispose(object sender, EventArgs e)
        {
            var material = sender as Material;
            Debug.Assert(null != material);

            material.Disposed -= this.OnMaterialDispose;

            this.DeallocateMaterial(material);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        public void DeallocateGeometry(BaseGeometry geometry)
        {
            Debug.Assert(null != geometry);

            geometry.__webglInit = false;

            if (geometry is BufferGeometry)
            {
                var bufferGeometry = (BufferGeometry)geometry;
                foreach (var name in bufferGeometry.Attributes)
                {
                    throw new NotImplementedException();

                    //var attribute = bufferGeometry.Attributes[name];

                    //if ( attribute.buffer != null ) {

                    //    GL.DeleteBuffer( attribute.buffer );

                    //    //delete attribute.buffer;
                    //}
                }
            }
            else
            {

                if ((geometry is GeometryGroup))
                {
                    var geometryGroup = (GeometryGroup)geometry;
                    var geometryGroupsList = geometryGroups[geometryGroup.Id];

                    if (geometryGroupsList != null)
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    this.DeleteBuffers(geometry);
                }
            }

            // TOFIX: Workaround for deleted geometry being currently bound

            _currentGeometryGroupHash = -1;
        }

        // Buffer deallocation

        public void DeleteBuffers(BaseGeometry geometry)
        {
            GL.DeleteBuffer(geometry.__webglVertexBuffer);
            GL.DeleteBuffer(geometry.__webglNormalBuffer);
            GL.DeleteBuffer(geometry.__webglTangentBuffer);
            GL.DeleteBuffer(geometry.__webglColorBuffer);
            GL.DeleteBuffer(geometry.__webglUVBuffer);
            GL.DeleteBuffer(geometry.__webglUV2Buffer);

            GL.DeleteBuffer(geometry.__webglSkinIndicesBuffer);
            GL.DeleteBuffer(geometry.__webglSkinWeightsBuffer);

            GL.DeleteBuffer(geometry.__webglFaceBuffer);
            GL.DeleteBuffer(geometry.__webglLineBuffer);

            GL.DeleteBuffer(geometry.__webglLineDistanceBuffer);

            geometry.__webglVertexBuffer = 0;
            geometry.__webglNormalBuffer = 0;
            geometry.__webglTangentBuffer = 0;
            geometry.__webglColorBuffer = 0;
            geometry.__webglUVBuffer = 0;
            geometry.__webglUV2Buffer = 0;

            geometry.__webglSkinIndicesBuffer = 0;
            geometry.__webglSkinWeightsBuffer = 0;

            geometry.__webglFaceBuffer = 0;
            geometry.__webglLineBuffer = 0;

            geometry.__webglLineDistanceBuffer = 0;



            // custom attributes

            if (geometry.__webglCustomAttributesList != null)
            {

                foreach (var attribute in geometry.__webglCustomAttributesList)
                {

                    var buffer = attribute["buffer"] as Hashtable;
                    var id = (int)buffer["id"];

                    GL.DeleteBuffer(id);
                }

                geometry.__webglCustomAttributesList.Clear();
                geometry.__webglCustomAttributesList = null;
            }

            this.Info.memory.Geometries--;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public void DeallocateTexture(Texture texture)
        {
            //if ( texture.Image && texture.Image.__webglTextureCube ) {

            //    // cube texture

            //    GL.DeleteTexture( texture.image.__webglTextureCube );

            //    //    delete texture.image.__webglTextureCube;
            //    //    texture.image.__webglTextureCube = -1;

            //}
            //else
            {
                // 2D texture

                if (texture.__webglInit == false) return;

                GL.DeleteTexture(texture.__webglTexture);

                texture.__webglTexture = -1;
                texture.__webglInit = false;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="camera"></param>
        private void UnprojectObject(Scene scene, Object3D object3D, Camera camera)
        {
            var projectionMatrixInverse = new Matrix4();

            //return function ( vector, camera ) {

            //    projectionMatrixInverse.getInverse( camera.projectionMatrix );
            //    _projScreenMatrix= camera.matrixWorld * projectionMatrixInverse;

            //    return vector.applyProjection( _projScreenMatrix );
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="camera"></param>
        private void ProjectObject(Scene scene, Object3D object3D, Camera camera)
        {

            if (object3D.Visible == false)
            {
                return;
            }

            if (object3D is Scene || object3D is Group)
            {
                // Skip
            }
            else
            {
                this.InitObject(object3D, scene);

                if (object3D is Light)
                {
                    this.Lights.Add((Light)object3D);
                }
                else if (object3D is Sprite)
                {
                    this.sprites.Add(object3D);
                }
                else if (object3D is LensFlare)
                {
                    this.lensFlares.Add(object3D);
                }
                else
                {
                    List<WebGlObject> webglObjects = null;
                    this._webglObjects.TryGetValue(object3D.id, out webglObjects);

                    if (null != webglObjects
                        && (object3D.FrustumCulled == false || /*this._frustum.intersectsObject(object3D)*/ true))
                    {
                        this.UpdateObject(scene, object3D);

                        foreach (var webglObject in webglObjects)
                        {
                            this.UnrollBufferMaterial(webglObject);

                            webglObject.render = true;

                            if (this.SortObjects)
                            {
                                if (object3D.RenderDepth > 0)
                                {
                                    webglObject.z = object3D.RenderDepth;
                                }
                                else
                                {
                                    var vector3 = new Vector3().SetFromMatrixPosition(object3D.MatrixWorld);
                                    vector3.ApplyProjection(this._projScreenMatrix);

                                    webglObject.z = vector3.Z;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var o in object3D.Children)
            {
                this.ProjectObject(scene, o, camera);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        private void RenderBuffer(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, BaseGeometry geometryGroup, Object3D object3D)
        {
            Debug.Assert(null != camera, "");
            Debug.Assert(null != lights, "");
            Debug.Assert(null != material, "");
            Debug.Assert(null != geometryGroup, "");

            if (material.Visible == false)
            {
                return;
            }

            var program = this.SetProgram(camera, lights, fog, material, object3D);

            var attributesLocation = program.Attributes;

            var updateBuffers = false;

            var wireframeBit = 0;
            var frameable = material as IWireframe;
            if (frameable != null)
                wireframeBit = frameable.Wireframe ? 1 : 0;

            var geometryGroupHash = (geometryGroup.Id * 0xffffff) + (program.Id * 2) + wireframeBit;

            if (geometryGroupHash != this._currentGeometryGroupHash)
            {
                this._currentGeometryGroupHash = geometryGroupHash;
                updateBuffers = true;
            }

            if (updateBuffers)
            {
                this.InitAttributes();
            }

            // vertices

            var morphTargetsMaterial = material as IMorphTargets;

            var attributeLocation = attributesLocation["position"];
            if (null != material /* && !material.morphTargets */ && null != attributeLocation)
            {
                if (updateBuffers)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglVertexBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                }
            }
            else
            {
                //    if (object3D.morphTargetBase)
                //    {
                //        this.setupMorphTargets(material, geometryGroup, object3D);
                //    }
            }

            if (updateBuffers)
            {
                // custom attributesLocation

                // Use the per-geometryGroup custom attribute arrays which are setup in initMeshBuffers

                if (null != geometryGroup.__webglCustomAttributesList)
                {
                    for (var i = 0; i < geometryGroup.__webglCustomAttributesList.Count; i++)
                    {

                        var attribute = geometryGroup.__webglCustomAttributesList[i];

                        var buffer = attribute["buffer"] as Hashtable;
                        var belongsTo = buffer["belongsToAttribute"] as string;

                        if (attributesLocation[belongsTo] != null)
                        {
                            var id = (int)buffer["id"];
                            var location = (int)attributesLocation[belongsTo];

                            var size = (int)attribute["size"];

                            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
                            this.EnableAttribute(location);
                            GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
                        }
                    }
                }

                // colors

                var geometry = object3D.Geometry as Geometry;
                Debug.Assert(null != geometry, "object3D.Geometry is not Geometry");

                attributeLocation = attributesLocation["Color"];
                if (null != attributeLocation)
                {
                    if (geometry.Colors.Count > 0 || geometry.Faces.Count > 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglColorBuffer);
                        this.EnableAttribute((int)attributeLocation);
                        GL.VertexAttribPointer((int)attributeLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                    }
                    //else if (material.defaultAttributeValues)
                    //{
                    //    GL.VertexAttrib3fv(attributesLocation.Color, material.defaultAttributeValues.Color);
                    //}
                }

                // normals

                attributeLocation = attributesLocation["normal"];
                if (null != attributeLocation)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglNormalBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                }

                // tangents
                attributeLocation = attributesLocation["tangent"];
                if (null != attributeLocation)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglTangentBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 4, VertexAttribPointerType.Float, false, 0, 0);
                }

                // uvs
                attributeLocation = attributesLocation["uv"];
                if (null != attributeLocation)
                {
                    if (geometry.FaceVertexUvs[0].Count > 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUVBuffer);
                        this.EnableAttribute((int)attributeLocation);
                        GL.VertexAttribPointer((int)attributeLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
                    }
                    //else if (material.defaultAttributeValues)
                    //{
                    //    GL.VertexAttrib2fv((int)attributesLocation["uv"], material.defaultAttributeValues.uv);
                    //}
                }

                attributeLocation = attributesLocation["uv2"];
                if (null != attributeLocation)
                {
                    if (geometry.FaceVertexUvs[1].Count > 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUV2Buffer);
                        this.EnableAttribute((int)attributeLocation);
                        GL.VertexAttribPointer((int)attributeLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
                    }
                    //else if (material.defaultAttributeValues)
                    //{
                    //    GL.VertexAttrib2fv((int)attributesLocation["uv2"], material.defaultAttributeValues.uv2);
                    //}
                }

                //if (material.skinning && (int)attributesLocation["skinIndex"] >= 0 && (int)attributesLocation["skinWeight"] >= 0)
                //{
                //    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinIndicesBuffer);
                //    this.enableAttribute((int)attributesLocation["skinIndex"]);
                //    GL.VertexAttribPointer((int)attributesLocation["skinIndex"], 4, VertexAttribPointerType.Float, false, 0, 0);

                //    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinWeightsBuffer);
                //    this.enableAttribute((int)attributesLocation["skinWeight"]);
                //    GL.VertexAttribPointer((int)attributesLocation["skinWeight"], 4, VertexAttribPointerType.Float, false, 0, 0);
                //}

                // line distances

                attributeLocation = attributesLocation["lineDistance"];
                if (null != attributeLocation)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglLineDistanceBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 1, VertexAttribPointerType.Float, false, 0, 0);
                }
            }

            this.DisableUnusedAttributes();

            // render mesh

            if (object3D is Mesh)
            {
                var type = geometryGroup.__typeArray == typeof(ushort)
                                            ? DrawElementsType.UnsignedShort
                                            : DrawElementsType.UnsignedInt;

                // wireframe

                if (wireframeBit > 0)
                {
                    var wireFrameMaterial = material as IWireframe;
                    Debug.Assert(null != wireFrameMaterial, "casting material to IWireFrameable failed");

                    this.SetLineWidth(wireFrameMaterial.WireframeLinewidth);
                    if (updateBuffers)
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglLineBuffer);
                    }
                    GL.DrawElements(BeginMode.Lines, geometryGroup.__webglLineCount, type, 0);
                }
                else
                {
                    // triangles

                    if (updateBuffers)
                    {
                        Debug.Assert(geometryGroup.__webglFaceBuffer > 0, "geometryGroup.__webglFaceBuffer has not been created");
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglFaceBuffer);
                    }
                    GL.DrawElements(BeginMode.Triangles, geometryGroup.__webglFaceCount, DrawElementsType.UnsignedShort, 0);

                    this.Info.render.Vertices += geometryGroup.__webglFaceCount;
                    this.Info.render.Faces += geometryGroup.__webglFaceCount / 3;
                }

                this.Info.render.Calls++;

            }
            else if (object3D is Line)
            {
                // render lines

                var mode = (((Line)object3D).Mode == Three.LineStrip) ? BeginMode.LineStrip : BeginMode.Lines;

                this.SetLineWidth(((LineBasicMaterial)material).Linewidth);

                GL.DrawArrays(mode, 0, geometryGroup.__webglLineCount);

                this.Info.render.Calls++;

            }
            else if (object3D is PointCloud)
            {
                // render particles

                GL.DrawArrays(BeginMode.Points, 0, geometryGroup.__webglParticleCount);

                this.Info.render.Calls++;
                this.Info.render.Points += geometryGroup.__webglParticleCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="program"></param>
        /// <param name="geometry"></param>
        /// <param name="startIndex"></param>
        private void SetupVertexAttributes(Material material, WebGlProgram program, BufferGeometry geometry, int startIndex)
        {
            var geometryAttributes = geometry.Attributes;

            var programAttributes = program.Attributes;
            var programAttributesKeys = program.AttributesKeys;

            foreach (var key in programAttributesKeys)
            {
                var programAttribute = programAttributes[key];

                if (null != programAttribute && geometryAttributes.ContainsKey(key))
                {
                    if (null != geometryAttributes[key] as BufferAttribute<float>)
                    {
                        var attributeItem = geometryAttributes[key] as BufferAttribute<float>;
                        var attributeSize = attributeItem.ItemSize;

                        Debug.Assert(attributeItem.buffer > 0, "buffer has not been initialized");

                        GL.BindBuffer(BufferTarget.ArrayBuffer, attributeItem.buffer);
                        this.EnableAttribute((int)programAttribute);
                        GL.VertexAttribPointer((int)programAttribute, attributeSize, VertexAttribPointerType.Float, false, 0, startIndex * attributeSize * sizeof(float));
                    }

                    if (null != geometryAttributes[key] as BufferAttribute<uint>)
                    {
                        var attributeItem = geometryAttributes[key] as BufferAttribute<uint>;
                        var attributeSize = attributeItem.ItemSize;

                        Debug.Assert(attributeItem.buffer > 0, "buffer has not been initialized");

                        GL.BindBuffer(BufferTarget.ArrayBuffer, attributeItem.buffer);
                        this.EnableAttribute((int)programAttribute);
                        GL.VertexAttribPointer((int)programAttribute, attributeSize, VertexAttribPointerType.UnsignedInt, false, 0, startIndex * attributeSize * sizeof(uint));
                    }

                    if (null != geometryAttributes[key] as BufferAttribute<ushort>)
                    {
                        var attributeItem = geometryAttributes[key] as BufferAttribute<ushort>;
                        var attributeSize = attributeItem.ItemSize;

                        Debug.Assert(attributeItem.buffer > 0, "buffer has not been initialized");

                        GL.BindBuffer(BufferTarget.ArrayBuffer, attributeItem.buffer);
                        this.EnableAttribute((int)programAttribute);
                        GL.VertexAttribPointer((int)programAttribute, attributeSize, VertexAttribPointerType.UnsignedShort, false, 0, startIndex * attributeSize * sizeof(ushort));
                    }
                    //else if ( material.defaultAttributeValues ) 
                    //{
                    //    if ( material.defaultAttributeValues[ attributeName ].length == 2 )
                    //    {
                    //        GL.VertexAttrib2fv( attributePointer, material.defaultAttributeValues[ attributeName ] );
                    //    } else if ( material.defaultAttributeValues[ attributeName ].length == 3 ) {
                    //        GL.VertexAttrib3fv( attributePointer, material.defaultAttributeValues[ attributeName ] );
                    //    }
                    //}

                }
            }

            this.DisableUnusedAttributes();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private void RenderBufferDirect(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, BufferGeometry geometry, Object3D object3D)
        {
            if (material.Visible == false)
            {
                return;
            }

            var program = this.SetProgram(camera, lights, fog, material, object3D);

            //var programAttributes = program.Attributes;

            var updateBuffers = false;
            var wireframeBit = 0;
            if (material is IWireframe)
                wireframeBit = ((IWireframe)material).Wireframe ? 1 : 0;
            var geometryHash = (geometry.Id * 0xffffff) + (program.Id * 2) + wireframeBit;

            if (geometryHash != this._currentGeometryGroupHash)
            {
                this._currentGeometryGroupHash = geometryHash;
                updateBuffers = true;
            }

            if (updateBuffers)
            {
                this.InitAttributes();
            }

            // render mesh

            if (object3D is Mesh)
            {
                var mode = BeginMode.Lines;
                var wireframe = material as IWireframe;
                if (wireframe != null) mode = wireframe.Wireframe ? BeginMode.Lines : BeginMode.Triangles;

                if (geometry.Attributes.ContainsKey("index"))
                {
                    var index = geometry.Attributes["index"] as IBufferAttribute;
                    Debug.Assert(null != index);

                    // indexed triangles

                    DrawElementsType type;
                    var size = 0;

                    if (index.Type == typeof(uint))
                    {
                        type = DrawElementsType.UnsignedInt;
                        size = 4;
                    }
                    else
                    {
                        type = DrawElementsType.UnsignedShort;
                        size = 2;
                    }

                    var offsets = geometry.Offsets;

                    if (offsets.Count == 0)
                    {

                        if (updateBuffers)
                        {

                            this.SetupVertexAttributes(material, program, geometry, 0);
                            GL.BindBuffer(BufferTarget.ElementArrayBuffer, index.buffer);

                        }

                        GL.DrawElements(mode, index.length, type, 0);

                        this.Info.render.Calls++;
                        this.Info.render.Vertices += index.length; // not really true, here vertices can be shared
                        this.Info.render.Faces += index.length / 3;

                    }
                    else
                    {

                        // if there is more than 1 chunk
                        // must Set attribute pointers to use new offsets for each chunk
                        // even if geometry and materials didn"t change

                        updateBuffers = true;

                        for (var i = 0; i < offsets.Count; i++)
                        {

                            var startIndex = offsets[i].Index;

                            if (updateBuffers)
                            {

                                this.SetupVertexAttributes(material, program, geometry, startIndex);
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, index.buffer);

                            }

                            // render indexed triangles

                            GL.DrawElements(mode, offsets[i].Count, type, offsets[i].Start * size);

                            this.Info.render.Calls++;
                            this.Info.render.Vertices += offsets[i].Count; // not really true, here vertices can be shared
                            this.Info.render.Faces += offsets[i].Count / 3;

                        }

                    }

                }
                else
                {

                    // non-indexed triangles

                    if (updateBuffers)
                    {
                        this.SetupVertexAttributes(material, program, geometry, 0);
                    }

                    var position = geometry.Attributes["position"] as BufferAttribute<float>;

                    // render non-indexed triangles

                    GL.DrawArrays(BeginMode.Triangles, 0, position.length);

                    this.Info.render.Calls++;
                    this.Info.render.Vertices += position.length;
                    this.Info.render.Faces += position.length / 3;
                }

            }
            else if (object3D is PointCloud)
            {
                // render particles

                if (updateBuffers)
                {
                    this.SetupVertexAttributes(material, program, geometry, 0);
                }

                var position = geometry.Attributes["position"] as BufferAttribute<float>;

                // render particles
                GL.DrawArrays(BeginMode.Points, 0, position.length);

                this.Info.render.Calls++;
                this.Info.render.Points += position.length / 3;
            }
            else if (object3D is Line)
            {
                var mode = (((Line)object3D).Mode == Three.LineStrip) ? BeginMode.LineStrip : BeginMode.Lines;

                this.SetLineWidth(((LineBasicMaterial)material).Linewidth);


                if (geometry.Attributes.ContainsKey("index"))
                {
                    var index = geometry.Attributes["index"];

                    // indexed lines

                    DrawElementsType type;
                    var size = 0;

                    throw new NotImplementedException();

                    if (index is short[])
                    {
                        type = DrawElementsType.UnsignedInt;
                        size = 4;
                    }
                    else
                    {
                        type = DrawElementsType.UnsignedShort;
                        size = 2;
                    }

                    var offsets = geometry.Offsets;

                    if (offsets.Count == 0)
                    {

                        if (updateBuffers)
                        {
                            this.SetupVertexAttributes(material, program, geometry, 0);

                            throw new NotImplementedException();

                            if (index is ushort[])
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<ushort>)index).buffer);
                            if (index is uint[])
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<uint>)index).buffer);
                            if (index is float[])
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<float>)index).buffer);
                        }

                        var length = ((IBufferAttribute)index).length;

                        GL.DrawElements(mode, length, type, 0); // 2 bytes per Uint16Array

                        this.Info.render.Calls++;
                        this.Info.render.Vertices += length; // not really true, here vertices can be shared

                    }
                    else
                    {

                        // if there is more than 1 chunk
                        // must Set attribute pointers to use new offsets for each chunk
                        // even if geometry and materials didn"t change

                        if (offsets.Count > 1) updateBuffers = true;

                        for (var i = 0; i < offsets.Count; i++)
                        {

                            var startIndex = offsets[i].Index;

                            if (updateBuffers)
                            {
                                this.SetupVertexAttributes(material, program, geometry, startIndex);

                                if (index is ushort[])
                                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<ushort>)index).buffer);
                                if (index is uint[])
                                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<uint>)index).buffer);
                                if (index is float[])
                                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<float>)index).buffer);
                            }

                            // render indexed lines

                            GL.DrawElements(mode, offsets[i].Count, type, offsets[i].Start * size); // 2 bytes per Uint16Array

                            this.Info.render.Calls++;
                            this.Info.render.Vertices += offsets[i].Count; // not really true, here vertices can be shared
                        }

                    }

                }
                else
                {

                    // non-indexed lines

                    if (updateBuffers)
                    {
                        this.SetupVertexAttributes(material, program, geometry, 0);
                    }

                    var position = geometry.Attributes["position"] as BufferAttribute<float>;

                    var array = position.Array;

                    GL.DrawArrays(mode, 0, array.Length / 3);

                    this.Info.render.Calls++;
                    this.Info.render.Points += array.Length / 3;
                }

            }

        }

        /// <summary>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="webGlObject"></param>
        private void RenderImmediateObject(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, WebGlObject webGlObject)
        {
            var object3D = webGlObject.object3D;

            var program = this.SetProgram(camera, lights, fog, material, object3D);

            this._currentGeometryGroupHash = -1;

            this.SetMaterialFaces(material);

            //if ( webGlObject.immediateRenderCallback ) {
            //    webGlObject.immediateRenderCallback( program, _gl, _frustum );
            //} else {
            //    webGlObject.render( function ( object3D ) { _this.renderBufferImmediate( object3D, program, material ); } );
            //}
        }

        /// <summary>
        /// </summary>
        /// <param name="renderList"></param>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="useBlending"></param>
        /// <param name="overrideMaterial"></param>
        private void RenderObjects(IEnumerable<WebGlObject> renderList, Camera camera, IEnumerable<Light> lights, Fog fog, bool useBlending, Material overrideMaterial)
        {
            foreach (var webglObject in renderList)
            {
                var object3D = webglObject.object3D;

                SetupMatrices(object3D, camera);

                Material material = null;
                if (null != overrideMaterial)
                {
                    material = overrideMaterial;
                }
                else
                {
                    material = webglObject.material;

                    if (null == material)
                    {
                        continue;
                    }

                    if (useBlending)
                    {
                        this.SetBlending(material.Blending, material.BlendEquation, material.BlendSrc, material.BlendDst);
                    }

                    this.SetDepthTest(material.DepthTest);
                    this.SetDepthWrite(material.DepthWrite);
                    this.SetPolygonOffset(material.PolygonOffset, material.PolygonOffsetFactor, material.PolygonOffsetUnits);
                }

                this.SetMaterialFaces(material);

                if (webglObject.buffer is BufferGeometry)
                {
                    this.RenderBufferDirect(camera, lights, fog, material, webglObject.buffer as BufferGeometry, object3D);
                }
                else
                {
                    this.RenderBuffer(camera, lights, fog, material, webglObject.buffer, object3D);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="renderList"></param>
        /// <param name="materialType"></param>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="useBlending"></param>
        /// <param name="overrideMaterial"></param>
        private void RenderObjectsImmediate(IEnumerable<WebGlObject> renderList, string materialType, Camera camera, IEnumerable<Light> lights, Fog fog, bool useBlending, Material overrideMaterial)
        {
            Material material = null;

            foreach (var webGlObject in renderList)
            {
                if (webGlObject.object3D.Visible)
                {
                    if (null != overrideMaterial)
                    {
                    }

                    this.RenderImmediateObject(camera, lights, fog, material, webGlObject);
                }
            }
        }

        ///// <summary>
        ///// </summary>
        ///// <param name="sceneNodes"></param>
        ///// <param name="scene"></param>
        ///// <param name="camera"></param>
        //private void RenderPlugins(IEnumerable<Object3D> sceneNodes, Scene scene, Camera camera)
        //{
        //    foreach (var object3D in sceneNodes)
        //    {
        //        // reset state for plugin (to start from clean slate)

        //        this._currentProgram = -1;
        //        this._currentCamera = null;
        //        this._oldBlending = -1;
        //        this._oldDepthTest = -1;
        //        this._oldDepthWrite = -1;
        //        this._oldDoubleSided = -1;
        //        this._oldFlipSided = -1;
        //        this._currentGeometryGroupHash = -1;
        //        this._currentMaterialId = -1;
        //        this._lightsNeedUpdate = true;

        //        object3D.Render(scene, camera, this._currentWidth, this._currentHeight);

        //        // reset state after plugin (anything could have changed)

        //        this._currentProgram = -1;
        //        this._currentCamera = null;
        //        this._oldBlending = -1;
        //        this._oldDepthTest = -1;
        //        this._oldDepthWrite = -1;
        //        this._oldDoubleSided = -1;
        //        this._oldFlipSided = -1;
        //        this._currentGeometryGroupHash = -1;
        //        this._currentMaterialId = -1;
        //        this._lightsNeedUpdate = true;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <returns></returns>
        private int AllocateBones(Object3D object3D)
        {
            if (this.supportsBoneTextures && null != object3D && /*null != object3D.skeleton && object3D.skeleton.useVertexTexture*/ false)
            {

                return 1024;

            }
            else
            {

                // default for when object is not specified
                // ( for example when prebuilding shader
                //   to be used with multiple objects )
                //
                //  - leave some extra space for other uniformsLocation
                //  - limit here is ANGLE's 254 max uniform vectors
                //    (up to 54 should be safe)

                var nVertexUniforms = 0;
                GL.GetInteger(GetPName.MaxVertexUniformVectors, out nVertexUniforms);
                var nVertexMatrices = System.Math.Floor((nVertexUniforms - 20) / 4.0);

                var maxBones = (int)nVertexMatrices;

                if (object3D != null && object3D is SkinnedMesh)
                {
                    throw new NotImplementedException();
                    //maxBones = Math.Min(object3D.skeleton.bones.Count, maxBones);

                    //if (maxBones < object3D.skeleton.bones.length)
                    //{

                    //    //Trace.TraceError( 'WebGLRenderer: too many bones - ' + object.skeleton.bones.length + ', this GPU supports just ' + maxBones + ' (try OpenGL instead of ANGLE)' );

                    //}

                }

                return maxBones;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lights"></param>
        /// <returns></returns>
        private static LightCountInfo AllocateLights(IEnumerable<Light> lights)
        {
            var dirLights = 0;
            var pointLights = 0;
            var spotLights = 0;
            var hemiLights = 0;

            foreach (var light in lights)
            {
                if (light.Visible == false)
                {
                    continue;
                }

                if (light is ILightShadow && ((ILightShadow)light).onlyShadow)
                {
                    continue;
                }

                if (light is DirectionalLight)
                {
                    dirLights++;
                }
                if (light is PointLight)
                {
                    pointLights++;
                }
                if (light is SpotLight)
                {
                    spotLights++;
                }
                if (light is HemisphereLight)
                {
                    hemiLights++;
                }
            }

            LightCountInfo lci;
            lci.directional = dirLights;
            lci.point = pointLights;
            lci.spot = spotLights;
            lci.hemi = hemiLights;

            return lci;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lights"></param>
        /// <returns></returns>
        private static int AllocateShadows(IEnumerable<Light> lights)
        {
            var maxShadows = 0;

            foreach (var light in lights)
            {
                if (!light.CastShadow)
                {
                    continue;
                }

                if (light is SpotLight)
                {
                    maxShadows++;
                }
                if (light is DirectionalLight && !((DirectionalLight)light).shadowCascade)
                {
                    maxShadows++;
                }
            }
            return maxShadows;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private static bool materialNeedsSmoothNormals(Material material)
        {
            if (null == material) return false;

            var meshNormalMaterial = material as MeshNormalMaterial;
            if (null != meshNormalMaterial)
            {
                if (meshNormalMaterial.Shading == Three.SmoothShading)
                    return true;
            }

            var shaderMaterial = material as ShaderMaterial;
            if (null != shaderMaterial)
            {
                if (shaderMaterial.Shading == Three.SmoothShading)
                    return true;
            }

            var meshLambertMaterial = material as MeshLambertMaterial;
            if (null != meshLambertMaterial)
            {
                if (meshLambertMaterial.Shading == Three.SmoothShading)
                    return true;
            }

            // TODO: do also for other material that carries shading

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryGroup"></param>
        private void CreateMeshBuffers(GeometryGroup geometryGroup)
        {
            GL.GenBuffers(1, out geometryGroup.__webglVertexBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglNormalBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglTangentBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglColorBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglUVBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglUV2Buffer);

            GL.GenBuffers(1, out geometryGroup.__webglSkinIndicesBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglSkinWeightsBuffer);

            GL.GenBuffers(1, out geometryGroup.__webglFaceBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglLineBuffer);

            if (geometryGroup.NumMorphTargets > 0)
            {
                geometryGroup.__webglMorphTargetsBuffers = null;

                for (var i = 0; i < geometryGroup.NumMorphTargets; i++)
                {
                    throw new NotImplementedException();
                    //   geometryGroup.__webglMorphTargetsBuffers.Add(null);
                }
            }

            if (geometryGroup.NumMorphNormals > 0)
            {
                geometryGroup.__webglMorphNormalsBuffers = null;

                for (var i = 0; i < geometryGroup.NumMorphNormals; i++)
                {
                    throw new NotImplementedException();
                    //   geometryGroup.__webglMorphNormalsBuffers.Add(null);
                }
            }

            this.Info.memory.Geometries++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        private void DeallocateMaterial(Material material)
        {
            Debug.Assert(null != material);
            Debug.Assert(null != material["program"]);

            int program = ((WebGlProgram)material["program"]).Program;

            (material["program"]) = null;

            // only deallocate GL program if this was the last use of shared program
            // assumed there is only single copy of any program in the _programs list
            // (that's how it's constructed)

            var deleteProgram = false;

            foreach (var programInfo in _programs)
            {
                if (programInfo.Program == program)
                {
                    programInfo.UsedTimes--;

                    if (programInfo.UsedTimes == 0)
                    {
                        deleteProgram = true;
                    }
                    break;
                }
            }

            if (deleteProgram == true)
            {
                // avoid using array.splice, this is costlier than creating new array from scratch

                var newPrograms = new List<WebGlProgram>();

                foreach (var programInfo in _programs)
                {
                    if (programInfo.Program != program)
                    {
                        newPrograms.Add(programInfo);
                    }
                }

                _programs = newPrograms;

                GL.DeleteProgram(program);

                this.Info.memory.Programs--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisableUnusedAttributes()
        {
            for (var i = 0; i < this._enabledAttributes.Length; i++)
            {
                if (this._enabledAttributes[i] != this._newAttributes[i])
                {
                    GL.DisableVertexAttribArray(i);
                    this._enabledAttributes[i] = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        private void EnableAttribute(int attribute)
        {
            this._newAttributes[attribute] = 1;

            if (this._enabledAttributes[attribute] == 0)
            {
                GL.EnableVertexAttribArray(attribute);
                this._enabledAttributes[attribute] = 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private Material getBufferMaterial(Object3D object3D, GeometryGroup geometryGroup)
        {
            return (object3D.Material is MeshFaceMaterial)
                ? ((MeshFaceMaterial)object3D.Material).Materials[geometryGroup.MaterialIndex]
                : object3D.Material;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="object3D"></param>
        ///// <param name="geometryGroup"></param>
        ///// <returns></returns>
        private Material getBufferMaterial(Object3D object3D, Geometry geometryGroup)
        {
            Debug.Assert(null != object3D);
            Debug.Assert(null != object3D.Material);

            return (object3D.Material is MeshFaceMaterial) ? ((MeshFaceMaterial)object3D.Material).Materials[geometryGroup.Id] : object3D.Material;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private int GetTextureUnit()
        {
            var textureUnit = this._usedTextureUnits;

            if (textureUnit >= this.MaxTextures)
            {
                Trace.TraceWarning("WebGLRenderer: trying to use " + textureUnit + " texture units while this GPU supports only " + this.MaxTextures);
            }

            this._usedTextureUnits += 1;

            return textureUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitAttributes()
        {
            for (var i = 0; i < this._newAttributes.Length; i++)
            {
                this._newAttributes[i] = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="hint"></param>
        private static void SetDirectBuffers(BufferGeometry geometry, BufferUsageHint hint)
        {
            var attributes = geometry.Attributes;
            var attributesKeys = geometry.AttributesKeys;

            foreach (var key in attributesKeys)
            {
                var attribute = attributes[key] as IBufferAttribute;
                Debug.Assert(null != attribute, "casting to IBufferAttribute failed");

                if (attribute.buffer < 0)
                {
                    var buffer = 0;
                    GL.GenBuffers(1, out buffer);

                    attribute.buffer = buffer;
                    attribute.needsUpdate = true;
                }

                if (attribute.needsUpdate)
                {
                    var bufferType = (key == "index") ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

                    Debug.Assert(attribute.buffer > 0, "attributeItem.buffer has not been created");
                    GL.BindBuffer(bufferType, attribute.buffer);

                    var array = ((Shaders.Attribute)attribute)["array"];

                    if (null != array as float[])
                        GL.BufferData(bufferType, (IntPtr)(attribute.length * sizeof(float)), (float[])array, hint);
                    if (null != array as ushort[])
                        GL.BufferData(bufferType, (IntPtr)(attribute.length * sizeof(ushort)), (ushort[])array, hint);
                    if (null != array as uint[])
                        GL.BufferData(bufferType, (IntPtr)(attribute.length * sizeof(uint)), (uint[])array, hint);

                    attribute.needsUpdate = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="object3D"></param>
        private void InitMaterial(Material material, IEnumerable<Light> lights, Fog fog, Object3D object3D)
        {
            material.Disposed += this.OnMaterialDispose;


            var shaderId = string.Empty;

            if (material is MeshDepthMaterial)
            {
                shaderId = "depth";
            }
            else if (material is MeshNormalMaterial)
            {
                shaderId = "normal";
            }
            else if (material is MeshBasicMaterial)
            {
                shaderId = "basic";
            }
            else if (material is MeshLambertMaterial)
            {
                shaderId = "lambert";
            }
            else if (material is MeshPhongMaterial)
            {
                shaderId = "phong";
            }
            else if (material is LineBasicMaterial)
            {
                shaderId = "basic";
            }
            else if (material is LineDashedMaterial)
            {
                shaderId = "dashed";
            }
            else if (material is PointCloudMaterial)
            {
                shaderId = "particle_basic";
            }

            if (!string.IsNullOrEmpty(shaderId))
            {
                var shader = (Shader)this.shaderLib[shaderId];

                material["__webglShader"] = new WebGlShader();

                // TODO: good enough as Clone?
                ((WebGlShader)material["__webglShader"]).Uniforms = new Uniforms();
                foreach (var e in shader.Uniforms)
                    ((WebGlShader)material["__webglShader"]).Uniforms.Add(e.Key, e.Value);

                ((WebGlShader)material["__webglShader"]).VertexShader = shader.VertexShader;
                ((WebGlShader)material["__webglShader"]).FragmentShader = shader.FragmentShader;

                if (null == material["__webglShader"])
                {
                    Trace.TraceError("Shader '{0}' could not be found. Check if it was created in UniformsLib", shaderId);
                    return;
                }
            }
            else
            {
                var sm = material as ShaderMaterial;

                material["__webglShader"] = new WebGlShader
                {
                    Uniforms = sm.Uniforms,
                    VertexShader = sm.VertexShader,
                    FragmentShader = sm.FragmentShader
                };
            }

            // heuristics to create shader parameters according to __lights in the scene
            // (not to blow over maxLights budget)

            var maxLightCount = AllocateLights(lights);

            var maxShadows = AllocateShadows(lights);

            var maxBones = this.AllocateBones(object3D);

            var parameters = new Hashtable
                                 {
                                     { "precision", this._precision },
                                     { "supportsVertexTextures", this.supportsVertexTextures },
                                     { "fog", fog },
                                     { "logarithmicDepthBuffer", this._logarithmicDepthBuffer },
                                     { "maxBones", maxBones },
                                     //{ "useVertexTexture", (this.supportsBoneTextures) && (null != object3D)
                                     //    /*&& (null != object3D.skeleton) && (object3D.skeleton.useVertexTexture) */
                                     //}, // skinnedMesh
                                     { "maxMorphTargets", this.maxMorphTargets },
                                     { "maxMorphNormals", this.maxMorphNormals },
                                     { "maxDirLights", maxLightCount.directional },
                                     { "maxPointLights", maxLightCount.point },
                                     { "maxSpotLights", maxLightCount.spot },
                                     { "maxHemiLights", maxLightCount.hemi },
                                     { "maxShadows", maxShadows },
                                     { "shadowMapEnabled", this.shadowMapEnabled && object3D.ReceiveShadow && maxShadows > 0 },
                                     { "shadowMapType", this.shadowMapType },
                                     { "shadowMapDebug", this.shadowMapDebug },
                                     { "shadowMapCascade", this.shadowMapCascade },
                                     { "doubleSided", (material.Side == Three.DoubleSide) },
                                     { "flipSided", (material.Side == Three.BackSide) }
                                 };

            var meshBasicMaterial = material as MeshBasicMaterial;
            if (null != meshBasicMaterial)
            {
                parameters.Add("map", meshBasicMaterial.Map != null);
                parameters.Add("useFog", meshBasicMaterial.Fog);
                parameters.Add("envMap", meshBasicMaterial.EnvMap != null);
                parameters.Add("lightMap", meshBasicMaterial.LightMap != null);
                parameters.Add("specularMap", meshBasicMaterial.SpecularMap != null);
                parameters.Add("alphaMap", meshBasicMaterial.AlphaMap != null);
                parameters.Add("vertexColors", meshBasicMaterial.VertexColors);
                parameters.Add("skinning", meshBasicMaterial.Skinning);
                parameters.Add("morphTargets", meshBasicMaterial.MorphTargets);
                parameters.Add("alphaTest", meshBasicMaterial.AlphaTest);
            }

            var lineBasicMaterial = material as LineBasicMaterial;
            if (null != lineBasicMaterial)
            {
                parameters.Add("useFog", lineBasicMaterial.Fog);
                parameters.Add("vertexColors", lineBasicMaterial.VertexColors);
            }

            var shaderMaterial = material as ShaderMaterial;
            if (null != shaderMaterial)
            {
                parameters.Add("alphaTest", shaderMaterial.AlphaTest);
                parameters.Add("useFog", shaderMaterial.Fog);
                parameters.Add("vertexColors", shaderMaterial.VertexColors);
                parameters.Add("skinning", shaderMaterial.Skinning);

                parameters.Add("morphTargets", shaderMaterial.MorphTargets);
                parameters.Add("morphNormals", shaderMaterial.MorphNormals);
            }

            var pointCloudMaterial = material as PointCloudMaterial;
            if (null != pointCloudMaterial)
            {
                parameters.Add("map", pointCloudMaterial.Map != null);
                parameters.Add("useFog", pointCloudMaterial.Fog);
                parameters.Add("vertexColors", pointCloudMaterial.VertexColors);
                parameters.Add("alphaTest", pointCloudMaterial.AlphaTest);

                parameters.Add("sizeAttenuation", pointCloudMaterial.SizeAttenuation);
            }

            var meshNormalMaterial = material as MeshNormalMaterial;
            if (null != meshNormalMaterial)
            {
                parameters.Add("alphaTest", meshNormalMaterial.AlphaTest);

                parameters.Add("morphTargets", meshNormalMaterial.MorphTargets);
            }

            var meshLambertMaterial = material as MeshLambertMaterial;
            if (null != meshLambertMaterial)
            {
                parameters.Add("map", meshLambertMaterial.Map != null);
                parameters.Add("useFog", meshLambertMaterial.Fog);
                parameters.Add("envMap", meshLambertMaterial.EnvMap != null);
                parameters.Add("lightMap", meshLambertMaterial.LightMap != null);
                parameters.Add("specularMap", meshLambertMaterial.SpecularMap != null);
                parameters.Add("alphaMap", meshLambertMaterial.AlphaMap != null);
                parameters.Add("vertexColors", meshLambertMaterial.VertexColors);
                parameters.Add("skinning", meshLambertMaterial.Skinning);
                parameters.Add("morphTargets", meshLambertMaterial.MorphTargets);
                parameters.Add("alphaTest", meshLambertMaterial.AlphaTest);

                parameters.Add("wrapAround", meshLambertMaterial.WrapAround);
                parameters.Add("morphNormals", meshLambertMaterial.MorphNormals);
            }

            var meshPhongMaterial = material as MeshPhongMaterial;
            if (null != meshPhongMaterial)
            {
                parameters.Add("map", meshPhongMaterial.Map != null);
                parameters.Add("useFog", meshPhongMaterial.Fog);
                parameters.Add("envMap", meshPhongMaterial.EnvMap != null);
                parameters.Add("lightMap", meshPhongMaterial.LightMap != null);
                parameters.Add("specularMap", meshPhongMaterial.SpecularMap != null);
                parameters.Add("alphaMap", meshPhongMaterial.AlphaMap != null);
                parameters.Add("vertexColors", meshPhongMaterial.VertexColors);
                parameters.Add("skinning", meshPhongMaterial.Skinning);
                parameters.Add("morphTargets", meshPhongMaterial.MorphTargets);
                parameters.Add("alphaTest", meshPhongMaterial.AlphaTest);

                parameters.Add("metal", meshPhongMaterial.Metal);
                parameters.Add("wrapAround", meshPhongMaterial.WrapAround);
                parameters.Add("morphNormals", meshPhongMaterial.MorphNormals);
            }

            // Generate code

            var chunks = new List<object>();

            if (!string.IsNullOrEmpty(shaderId))
            {
                chunks.Add(shaderId);
            }
            else
            {
                if (null != shaderMaterial)
                {
                    chunks.Add(shaderMaterial.FragmentShader);
                    chunks.Add(shaderMaterial.VertexShader);
                }
            }

            // 
            foreach (DictionaryEntry entry in material.Defines)
            {
                chunks.Add(entry.Key);
                chunks.Add(entry.Value);
            }

            // 
            foreach (DictionaryEntry entry in parameters)
            {
                chunks.Add(entry.Key);
                chunks.Add(entry.Value);
            }

            // join
            var code = String.Join(",", chunks);

            WebGlProgram program = null;

            // Check if code has been already compiled

            foreach (var programInfo in this._programs)
            {
                if (programInfo.Code == code)
                {
                    program = programInfo;
                    program.UsedTimes++;

                    //Console.WriteLine("Reusing Shader Program {0}", programInfo.Id);

                    break;
                }
            }

            if (program == null)
            {
                program = new WebGlProgram(this, code, material, parameters);
                this._programs.Add(program);

                Console.WriteLine("New Shader Program {0}", program.Id);

                this.Info.memory.Programs = this._programs.Count;
            }

            material["program"] = program;

            var attributes = ((WebGlProgram)material["program"]).Attributes;

            if (null != meshBasicMaterial && meshBasicMaterial.MorphTargets)
            {
                meshBasicMaterial.NumSupportedMorphTargets = 0;

                var basis = "morphTarget";

                for (var i = 0; i < this.maxMorphTargets; i++)
                {
                    var id = basis + i;
                    if ((int)attributes[id] >= 0)
                    {
                        meshBasicMaterial.NumSupportedMorphTargets++;
                    }
                }
            }



            //if ( material.morphNormals ) {

            //    material.numSupportedMorphNormals = 0;

            //    var basis = "morphNormal";

            //    for (int i = 0; i < this.maxMorphNormals; i ++ ) {

            //        var id = basis + i;
            //        if ( attributesLocation[ id ] >= 0 ) {
            //            material.numSupportedMorphNormals ++;
            //        }
            //    }
            //}

            material.UniformsList = new List<UniformLocation>();

            foreach (var u in ((WebGlShader)material["__webglShader"]).Uniforms)
            {
                var location = ((WebGlProgram)material["program"]).Uniforms[u.Key];
                if (location != null)
                {
                    var uniform = ((WebGlShader)material["__webglShader"]).Uniforms[u.Key];
                    material.UniformsList.Add(new UniformLocation() { Uniform = uniform, Location = (int)location });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        private void InitMeshBuffers(GeometryGroup geometryGroup, Object3D object3D)
        {
            var geometry = object3D.Geometry as Geometry;
            Debug.Assert(null != geometry, "object3D.Geometry is not Geometry");

            var faces3 = geometryGroup.Faces3;

            var nvertices = faces3.Count * 3;
            var ntris = faces3.Count * 1;
            var nlines = faces3.Count * 3;

            var material = this.getBufferMaterial(object3D, geometryGroup);

            geometryGroup.__vertexArray = new float[nvertices * 3];
            geometryGroup.__normalArray = new float[nvertices * 3];
            geometryGroup.__colorArray = new float[nvertices * 3];
            geometryGroup.__uvArray = new float[nvertices * 2];

            if (geometry.FaceVertexUvs.Count > 1)
            {
                geometryGroup.__uv2Array = new float[nvertices * 2];
            }

            if (geometry.HasTangents)
            {
                geometryGroup.__tangentArray = new float[nvertices * 4];
            }

            if (geometry.SkinWeights.Count > 0 && geometry.SkinIndices.Count > 0)
            {
                geometryGroup.__skinIndexArray = new float[nvertices * 4];
                geometryGroup.__skinWeightArray = new float[nvertices * 4];
            }

            var typeArray = (this.glExtensionElementIndexUint && ntris > (ushort.MaxValue / 3)) ? typeof(uint) : typeof(ushort);

            geometryGroup.__typeArray = typeArray;
            geometryGroup.__faceArray = new ushort[ntris * 3];//Array.CreateInstance(typeArray, ntris * 3); TODO
            geometryGroup.__lineArray = new ushort[nlines * 2];//Array.CreateInstance(typeArray, nlines * 2); TODO

            if (geometryGroup.NumMorphTargets > 0)
            {
                geometryGroup.__morphTargetsArrays = new List<float[]>();
                for (var i = 0; i < geometryGroup.NumMorphTargets; i++)
                {
                    geometryGroup.__morphTargetsArrays.Add(new float[nvertices * 3]);
                }
            }

            if (geometryGroup.NumMorphNormals > 0)
            {
                geometryGroup.__morphNormalsArrays = new List<float[]>();
                for (var i = 0; i < geometryGroup.NumMorphNormals; i++)
                {
                    geometryGroup.__morphNormalsArrays.Add(new float[nvertices * 3]);
                }
            }

            geometryGroup.__webglFaceCount = ntris * 3;
            geometryGroup.__webglLineCount = nlines * 2;

            // custom Attributes

            if (material is IAttributes && ((IAttributes)material).Attributes != null)
            {
                var attributesMaterial = material as IAttributes;

                if (geometryGroup.__webglCustomAttributesList == null)
                {
                    geometryGroup.__webglCustomAttributesList = new List<Shaders.Attribute>();
                }

                foreach (var a in attributesMaterial.Attributes)
                {
                    var originalAttribute = a.Value;

                    // Do A shallow copy of the attribute object so different geometryGroup chunks use different
                    // attribute buffers which are correctly indexed in the setMeshBuffers function

                    var attribute = new Attribute();

                    foreach (var entry in originalAttribute)
                    {
                        var property = entry.Key;

                        attribute[property] = originalAttribute[property];
                    }

                    if (!attribute.ContainsKey("__webglInitialized") || attribute.ContainsKey("createUniqueBuffers"))
                    {
                        attribute["__webglInitialized"] = true;

                        var size = 1;   // "f" and "i"

                        if ((string)attribute["type"] == "v2") size = 2;
                        else if ((string)attribute["type"] == "v3") size = 3;
                        else if ((string)attribute["type"] == "v4") size = 4;
                        else if ((string)attribute["type"] == "C") size = 3;

                        attribute["size"] = size;

                        attribute["array"] = new float[nvertices * size];

                        int bufferId = 0;
                        GL.GenBuffers(1, out bufferId);

                        attribute["buffer"] = new Hashtable();

                        ((Hashtable)attribute["buffer"]).Add("id", bufferId);
                        ((Hashtable)attribute["buffer"]).Add("belongsToAttribute", a.Key);

                        originalAttribute["needsUpdate"] = true;
                        attribute["__original"] = originalAttribute;
                    }

                    geometryGroup.__webglCustomAttributesList.Add(attribute);
                }
            }

            geometryGroup.__inittedArrays = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="uniforms"></param>
        private void LoadUniformsGeneric(IEnumerable<UniformLocation> uniforms)
        {
            //         Console.WriteLine("------");

            foreach (var uniformLocation in uniforms)
            {
                int location = -1;
                string type = string.Empty;

                try
                {
                    var uniform = uniformLocation.Uniform;
                    Debug.Assert(null != uniform, "key is null or could not cast to KVP");

                    // needsUpdate property is not added to all uniformsLocation.
                    if (uniform.ContainsKey("needsUpdate"))
                        if ((bool)uniform["needsUpdate"] == false)
                            continue;

                    type = (string)uniform["type"];
                    object value = uniform["value"];
                    location = uniformLocation.Location;

                    //        Console.WriteLine("loadUniformsGeneric: {0} {1} {2}", location, type, value);

                    switch (type)
                    {
                        case "1i":
                            GL.Uniform1(location, (int)value);
                            break;

                        case "1f":
                            GL.Uniform1(location, (float)value);
                            break;

                        case "2f":
                            var v2f = ((List<float>)value).ToArray();
                            GL.Uniform2(location, v2f[0], v2f[1]);
                            break;

                        case "3f":
                            var v3f = ((List<float>)value).ToArray();
                            GL.Uniform3(location, v3f[0], v3f[1], v3f[2]);
                            break;

                        case "4f":
                            var v4f = ((List<float>)value).ToArray();
                            GL.Uniform4(location, v4f[0], v4f[1], v4f[2], v4f[3]);
                            break;

                        case "1iv":
                            var oneiv = ((List<int>)value).ToArray();
                            GL.Uniform1(location, oneiv.Length, oneiv);
                            break;

                        case "3iv":
                            var threeiv = ((List<int>)value).ToArray();
                            GL.Uniform3(location, threeiv.Length, threeiv);
                            break;

                        case "1fv":
                            var onefv = ((List<float>)value).ToArray();
                            GL.Uniform1(location, onefv.Length, onefv);
                            break;

                        case "2fv":
                            var twofv = ((List<float>)value).ToArray();
                            GL.Uniform2(location, twofv.Length, twofv);
                            break;

                        case "3fv":
                            var threefv = ((List<float>)value).ToArray();
                            GL.Uniform3(location, threefv.Length, threefv);
                            break;

                        case "4fv":
                            var fourfv = ((List<float>)value).ToArray();
                            GL.Uniform4(location, fourfv.Length, fourfv);
                            break;

                        case "Matrix3fv":
                            var matrix3fv = ((List<float>)value).ToArray();
                            GL.UniformMatrix3(location, matrix3fv.Length, false, matrix3fv);
                            break;

                        case "Matrix4fv":
                            var matrix4fv = ((List<float>)value).ToArray();
                            GL.UniformMatrix4(location, matrix4fv.Length, false, matrix4fv);
                            break;

                        case "i":
                            GL.Uniform1(location, (int)value);
                            break;

                        case "f":
                            GL.Uniform1(location, Convert.ToSingle(value));
                            break;

                        case "v2":
                            var v2 = (Vector2)value;
                            GL.Uniform2(location, v2.X, v2.Y);
                            break;

                        case "v3":
                            var v3 = (Vector3)value;
                            GL.Uniform3(location, v3.X, v3.Y, v3.Z);
                            break;

                        case "v4":
                            // single Vector4
                            var v4 = (Vector4)value;
                            GL.Uniform4(location, v4.X, v4.Y, v4.Z, v4.W);
                            break;

                        case "C":
                            var color = (Color)value;
                            GL.Uniform3(location, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
                            break;

                        case "iv1":
                            // flat array of integers (JS or typed array)
                            var iv1 = ((List<int>)value).ToArray();
                            GL.Uniform1(location, iv1.Length, iv1);
                            break;

                        case "iv":
                            // flat array of integers with 3 x N size (JS or typed array)
                            var iv = ((List<int>)value).ToArray();
                            GL.Uniform3(location, iv.Length / 3, iv);

                            break;

                        case "fv1":
                            // flat array of floats (JS or typed array)
                            var fv1 = ((List<float>)value).ToArray();
                            GL.Uniform1(location, fv1.Length, fv1);
                            break;

                        case "fv":
                            // flat array of floats with 3 x N size (JS or typed array)
                            var fv = ((List<float>)value).ToArray();
                            GL.Uniform3(location, fv.Length / 3, fv);
                            break;
                        /*
                                                        case "v2v":

                                                            // array of Three.Vector2

                                                            if ( uniform._array == null ) {

                                                                uniform._array = new Float32Array( 2 * value.length );

                                                            }

                                                            for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                offset = i * 2;

                                                                uniform._array[ offset ]   = value[ i ].x;
                                                                uniform._array[ offset + 1 ] = value[ i ].y;

                                                            }

                                                            GL.Uniform2fv( location, uniform._array );

                                                            break;

                                                        case "v3v":

                                                            // array of Three.Vector3

                                                            if ( uniform._array == null ) {

                                                                uniform._array = new Float32Array( 3 * value.length );

                                                            }

                                                            for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                offset = i * 3;

                                                                uniform._array[ offset ]   = value[ i ].x;
                                                                uniform._array[ offset + 1 ] = value[ i ].y;
                                                                uniform._array[ offset + 2 ] = value[ i ].z;

                                                            }

                                                            GL.Uniform3fv( location, uniform._array );

                                                            break;

                                                        case "v4v":

                                                            // array of Three.Vector4

                                                            if ( uniform._array == null ) {

                                                                uniform._array = new Float32Array( 4 * value.length );

                                                            }

                                                            for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                offset = i * 4;

                                                                uniform._array[ offset ]   = value[ i ].x;
                                                                uniform._array[ offset + 1 ] = value[ i ].y;
                                                                uniform._array[ offset + 2 ] = value[ i ].z;
                                                                uniform._array[ offset + 3 ] = value[ i ].w;

                                                            }

                                                            GL.Uniform4fv( location, uniform._array );

                                                            break;

                                                        case "m3":

                                                            // single Three.Matrix3
                                                            GL.UniformMatrix3fv( location, false, value.elements );

                                                            break;

                                                        case "m3v":

                                                            // array of Three.Matrix3

                                                            if ( uniform._array == null ) {

                                                                uniform._array = new Float32Array( 9 * value.length );

                                                            }

                                                            for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                value[ i ].flattenToArrayOffset( uniform._array, i * 9 );

                                                            }

                                                            GL.UniformMatrix3fv( location, false, uniform._array );

                                                            break;

                                                        case "m4":

                                                            // single Three.Matrix4
                                                            GL.UniformMatrix4fv( location, false, value.elements );

                                                            break;

                                                        case "m4v":

                                                            // array of Three.Matrix4

                                                            if ( uniform._array == null ) {

                                                                uniform._array = new Float32Array( 16 * value.length );

                                                            }

                                                            for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                value[ i ].flattenToArrayOffset( uniform._array, i * 16 );

                                                            }

                                                            GL.UniformMatrix4fv( location, false, uniform._array );

                                                            break;
                        */
                        case "t":

                            // single Three.Texture (2d or cube)

                            var texture = (ITexture)value;
                            var textureUnit = this.GetTextureUnit();

                            GL.Uniform1(location, textureUnit);

                            if (null == texture)
                            {
                                continue;
                            }

                            //if ( texture is CubeTexture ||
                            //   ( texture.Image is Array && texture.Image.length == 6 ) ) { // CompressedTexture can have Array in image :/
                            //       this.setCubeTexture(texture, textureUnit);
                            //} else if ( texture is WebGLRenderTargetCube ) {
                            //    this.setCubeTextureDynamic(texture, textureUnit);
                            //} else {
                            this.SetTexture(texture, textureUnit);
                            //}

                            break;
                            /*
                                                            case "tv":

                                                                // array of Three.Texture (2d)

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = [];

                                                                }

                                                                for ( var i = 0, il = uniform.value.length; i < il; i ++ ) {

                                                                    uniform._array[ i ] = getTextureUnit();

                                                                }

                                                                GL.Uniform1iv( location, uniform._array );

                                                                for ( var i = 0, il = uniform.value.length; i < il; i ++ ) {

                                                                    texture = uniform.value[ i ];
                                                                    textureUnit = uniform._array[ i ];

                                                                    if ( ! texture ) continue;

                                                                    _this.setTexture( texture, textureUnit );

                                                                }

                                                                break;

                                                            default:

                                                                Trace.TraceWarning( "Three.WebGLRenderer: Unknown uniform type: " + type );
                                                */
                    }
                }
                catch (/*OpenTK.exGraphicsException*/ Exception e)
                {
                    Trace.TraceError("Setting KVP at location {0}, of type {1}; Message: {2}", location, type, e.Message);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="uniformsLocation"></param>
        /// <param name="object3D"></param>
        private static void LoadUniformsMatrices(Hashtable uniformsLocation, Object3D object3D)
        {
            //Console.WriteLine("object3D._modelViewMatrix\n{0}", object3D._modelViewMatrix);

            var location = uniformsLocation["modelViewMatrix"];
            if (null != location)
                GL.UniformMatrix4((int)location, 1, false, object3D._modelViewMatrix.Elements);

            location = uniformsLocation["normalMatrix"];
            if (location != null)
            {
                GL.UniformMatrix3((int)location, 1, false, object3D._normalMatrix.Elements);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private bool MaterialNeedsSmoothNormals(Material material)
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsCommon(Uniforms uniforms, Material material)
        {
            Texture uvScaleMap = null;

            Uniforms.SetValue(uniforms, "opacity", material.Opacity);

            if (material is IMap)
            {
                var m = material as IMap;

                Uniforms.SetValue(uniforms, "map", m.Map);
                Uniforms.SetValue(uniforms, "lightMap", m.LightMap);
                Uniforms.SetValue(uniforms, "specularMap", m.SpecularMap);
                Uniforms.SetValue(uniforms, "alphaMap", m.AlphaMap);
            }

            if (material is MeshLambertMaterial)
            {
                var m = material as MeshLambertMaterial;

                Uniforms.SetValue(uniforms, "diffuse", this.gammaInput ? this.copyGammaToLinear(m.Color) : m.Color);

                // others here?
            }

            if (material is MeshBasicMaterial)
            {
                var m = material as MeshBasicMaterial;

                Uniforms.SetValue(uniforms, "diffuse", this.gammaInput ? this.copyGammaToLinear(m.Color) : m.Color);

                //Uniforms.SetValue(uniforms, "map", m.Map);
                //Uniforms.SetValue(uniforms, "lightMap", m.LightMap);
                //Uniforms.SetValue(uniforms, "specularMap", m.SpecularMap);
                //Uniforms.SetValue(uniforms, "alphaMap", m.AlphaMap);

                if (null != m.Map)
                    uvScaleMap = m.Map;
                else if (null != m.SpecularMap)
                    uvScaleMap = m.SpecularMap;
                else if (null != m.AlphaMap)
                    uvScaleMap = m.AlphaMap;

                Uniforms.SetValue(uniforms, "envMap", m.EnvMap);
                Uniforms.SetValue(uniforms, "flipEnvMap", (m.EnvMap is WebGLRenderTargetCube) ? 1 : -1);

                if (this.gammaInput)
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.Reflectivity * m.Reflectivity);
                }
                else
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.Reflectivity);
                }

                Uniforms.SetValue(uniforms, "refractionRatio", m.RefractionRatio);
                Uniforms.SetValue(uniforms, "combine", m.Combine);
                Uniforms.SetValue(uniforms, "useRefract", ((null != m.EnvMap) && (m.EnvMap.Mapping is Three.CubeRefractionMapping)) ? 1 : 0);
            }

            if (material is MeshPhongMaterial)
            {
                var m = material as MeshPhongMaterial;

                Uniforms.SetValue(uniforms, "diffuse", this.gammaInput ? this.copyGammaToLinear(m.Color) : m.Color);

                //Uniforms.SetValue(uniforms, "map", m.Map);
                //Uniforms.SetValue(uniforms, "lightMap", m.LightMap);
                //Uniforms.SetValue(uniforms, "specularMap", m.SpecularMap);
                //Uniforms.SetValue(uniforms, "alphaMap", m.AlphaMap);

                if (null != m.BumpMap)
                {
                    Uniforms.SetValue(uniforms, "bumpMap", m.BumpMap);
                    Uniforms.SetValue(uniforms, "bumpScale", m.BumpScale);
                }

                if (null != m.NormalMap)
                {
                    Uniforms.SetValue(uniforms, "normalMap", m.NormalMap);
                    Uniforms.SetValue(uniforms, "normalScale", m.NormalScale);
                }

                Uniforms.SetValue(uniforms, "envMap", m.EnvMap);
                Uniforms.SetValue(uniforms, "flipEnvMap", (m.EnvMap is WebGLRenderTargetCube) ? 1 : -1);

                if (this.gammaInput)
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.Reflectivity * m.Reflectivity);
                }
                else
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.Reflectivity);
                }

                Uniforms.SetValue(uniforms, "refractionRatio", m.RefractionRatio);
                Uniforms.SetValue(uniforms, "combine", m.Combine);
                Uniforms.SetValue(uniforms, "useRefract", ((null != m.EnvMap) && (m.EnvMap.Mapping is Three.CubeRefractionMapping)) ? 1 : 0);
            }

            //  1. Color map
            //  2. specular map
            //  3. normal map
            //  4. bump map
            //  5. alpha map

            var imap = material as IMap;
            if (null != imap)
            {
                if (null != imap.Map)
                {
                    uvScaleMap = imap.Map;
                }
                else if (null != imap.SpecularMap)
                {
                    uvScaleMap = imap.SpecularMap;
                }
                else if (null != imap.NormalMap)
                {
                    uvScaleMap = imap.NormalMap;
                }
                else if (null != imap.BumpMap)
                {
                    uvScaleMap = imap.BumpMap;
                }
                else if (null != imap.AlphaMap)
                {
                    uvScaleMap = imap.AlphaMap;
                }

                if (uvScaleMap != null)
                {
                    var offset = uvScaleMap.Offset;
                    var repeat = uvScaleMap.Repeat;

                    uniforms["offsetRepeat"]["value"] = new Vector4(offset.X, offset.Y, repeat.X, repeat.Y);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetBlending(int value)
        {
            if (this._contextGlobalCompositeOperation != value)
            {
                throw new NotImplementedException();

                if (value == Three.NormalBlending)
                {
                    //                   _context.globalCompositeOperation = "source-over";
                }
                else if (value == Three.AdditiveBlending)
                {
                    //                   _context.globalCompositeOperation = "lighter";
                }
                else if (value == Three.SubtractiveBlending)
                {
                    //                  _context.globalCompositeOperation = "darker";
                }

                this._contextGlobalCompositeOperation = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="blending"></param>
        /// <param name="blendEquation"></param>
        /// <param name="blendSrc"></param>
        /// <param name="blendDst"></param>
        private void SetBlending(int blending, int blendEquation, int blendSrc, int blendDst)
        {
            if (blending != this._oldBlending)
            {
                if (blending == Three.NoBlending)
                {
                    GL.Disable(EnableCap.Blend);
                }
                else if (blending == Three.AdditiveBlending)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                }
                else if (blending == Three.SubtractiveBlending)
                {
                    // TOD: Find blendFuncSeparate() combination
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor);
                }
                else if (blending == Three.MultiplyBlending)
                {
                    // TOD: Find blendFuncSeparate() combination
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.SrcAlpha);
                }
                else if (blending == Three.CustomBlending)
                {
                    GL.Enable(EnableCap.Blend);
                }
                else
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
                    GL.BlendFuncSeparate(
                        BlendingFactorSrc.SrcAlpha,
                        BlendingFactorDest.OneMinusSrcAlpha,
                        BlendingFactorSrc.One,
                        BlendingFactorDest.OneMinusSrcAlpha);
                }
                this._oldBlending = blending;
            }

            if (blending == Three.CustomBlending)
            {
                if (blendEquation != this._oldBlendEquation)
                {
                    // GL.BlendEquation(paramThreeToGL(blendEquation));
                    this._oldBlendEquation = blendEquation;
                }
                if (blendSrc != this._oldBlendSrc || blendDst != this._oldBlendDst)
                {
                    //   GL.BlendFunc(paramThreeToGL(blendSrc), paramThreeToGL(blendDst));
                    this._oldBlendSrc = blendSrc;
                    this._oldBlendDst = blendDst;
                }
            }
            else
            {
                this._oldBlendEquation = -1;
                this._oldBlendSrc = -1;
                this._oldBlendDst = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthTest"></param>
        private void SetDepthTest(bool depthTest)
        {
            if (this._oldDepthTest != depthTest)
            {
                if (depthTest)
                {
                    GL.Enable(EnableCap.DepthTest);
                }
                else
                {
                    GL.Enable(EnableCap.DepthTest);
                }

                this._oldDepthTest = depthTest;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthWrite"></param>
        private void SetDepthWrite(bool depthWrite)
        {
            if (this._oldDepthWrite != depthWrite)
            {
                GL.DepthMask(depthWrite);

                this._oldDepthWrite = depthWrite;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        private void SetLineWidth(float width)
        {
            if (width != this._oldLineWidth)
            {

                GL.LineWidth(width);

                this._oldLineWidth = width;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        private void SetMaterialFaces(Material material)
        {
            var doubleSided = (material.Side == Three.DoubleSide) ? 1 : 0;
            var flipSided = (material.Side == Three.BackSide) ? 1 : 0;

            if (this._oldDoubleSided != doubleSided)
            {
                if (doubleSided > 0)
                {
                    GL.Disable(EnableCap.CullFace);
                }
                else
                {
                    GL.Enable(EnableCap.CullFace);
                }

                this._oldDoubleSided = doubleSided;
            }

            if (this._oldFlipSided != flipSided)
            {
                if (flipSided > 0)
                {
                    GL.FrontFace(FrontFaceDirection.Cw);
                }
                else
                {
                    GL.FrontFace(FrontFaceDirection.Ccw);
                }

                this._oldFlipSided = flipSided;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        /// <param name="hint"></param>
        /// <param name="dispose"></param>
        /// <param name="material"></param>
        private void SetMeshBuffers(GeometryGroup geometryGroup, Object3D object3D, BufferUsageHint hint, bool dispose, Material material)
        {
            if (!geometryGroup.__inittedArrays)
            {
                return;
            }

            var needsSmoothNormals = materialNeedsSmoothNormals(material);

            var vertexIndex = 0;

            var offset = 0;
            var offset_uv = 0;
            var offset_uv2 = 0;
            var offset_face = 0;
            var offset_normal = 0;
            var offset_tangent = 0;
            var offset_line = 0;
            var offset_color = 0;
            var offset_skin = 0;
            var offset_morphTarget = 0;
            var offset_custom = 0;
            var offset_customSrc = 0;

            var vertexArray = geometryGroup.__vertexArray;
            var uvArray = geometryGroup.__uvArray;
            var uv2Array = geometryGroup.__uv2Array;
            var normalArray = geometryGroup.__normalArray;
            var tangentArray = geometryGroup.__tangentArray;
            var colorArray = geometryGroup.__colorArray;

            var skinIndexArray = geometryGroup.__skinIndexArray;
            var skinWeightArray = geometryGroup.__skinWeightArray;

            var morphTargetsArrays = geometryGroup.__morphTargetsArrays;
            var morphNormalsArrays = geometryGroup.__morphNormalsArrays;

            var customAttributes = geometryGroup.__webglCustomAttributesList;
            //	    var customAttribute;

            var faceArray = geometryGroup.__faceArray;
            var lineArray = geometryGroup.__lineArray;

            var geometry = object3D.Geometry as Geometry; // this is shared for all chunks
            Debug.Assert(null != geometry, "object3D.Geometry is not Geometry");

            var dirtyVertices = geometry.VerticesNeedUpdate;
            var dirtyElements = geometry.ElementsNeedUpdate;
            var dirtyUvs = geometry.UvsNeedUpdate;
            var dirtyNormals = geometry.NormalsNeedUpdate;
            var dirtyColors = geometry.ColorsNeedUpdate;
            var dirtyMorphTargets = geometry.MorphTargetsNeedUpdate;

            var vertices = geometry.Vertices;
            var chunk_faces3 = geometryGroup.Faces3;
            var obj_faces = geometry.Faces;

            var obj_uvs = geometry.FaceVertexUvs[0];
            List<List<Vector2>> obj_uvs2 = null;
            if (geometry.FaceVertexUvs.Count > 1)
            {
                obj_uvs2 = geometry.FaceVertexUvs[1];
            }

            var obj_colors = geometry.Colors;

            var obj_skinIndices = geometry.SkinIndices;
            var obj_skinWeights = geometry.SkinWeights;

            var morphTargets = geometry.MorphTargets;
            var morphNormals = geometry.MorphNormals;

            if (dirtyVertices)
            {
                foreach (var chuck_face in chunk_faces3)
                {
                    var face = obj_faces[chuck_face];

                    var v1 = vertices[face.A];
                    var v2 = vertices[face.B];
                    var v3 = vertices[face.C];

                    vertexArray[offset] = v1.X;
                    vertexArray[offset + 1] = v1.Y;
                    vertexArray[offset + 2] = v1.Z;

                    vertexArray[offset + 3] = v2.X;
                    vertexArray[offset + 4] = v2.Y;
                    vertexArray[offset + 5] = v2.Z;

                    vertexArray[offset + 6] = v3.X;
                    vertexArray[offset + 7] = v3.Y;
                    vertexArray[offset + 8] = v3.Z;

                    offset += 9;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglVertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * sizeof(float)), vertexArray, hint);

                Debug.WriteLine("BufferData for __webglVertexBuffer float[] id {0}", geometryGroup.__webglVertexBuffer);
            }

            if (dirtyMorphTargets)
            {
                for (var vk = 0; vk < morphTargets.Count; vk++)
                {
                    offset_morphTarget = 0;

                    throw new NotImplementedException();

                    foreach (var chf in chunk_faces3)
                    {
                        var face = obj_faces[chf];
                        /*
                        // morph positions

                        var v1 = morphTargets[vk].vertices[face.A];
                        var v2 = morphTargets[vk].vertices[face.B];
                        var v3 = morphTargets[vk].vertices[face.C];

                        var vka = morphTargetsArrays[vk];

                        vka[offset_morphTarget] = v1.X;
                        vka[offset_morphTarget + 1] = v1.y;
                        vka[offset_morphTarget + 2] = v1.z;

                        vka[offset_morphTarget + 3] = v2.x;
                        vka[offset_morphTarget + 4] = v2.y;
                        vka[offset_morphTarget + 5] = v2.z;

                        vka[offset_morphTarget + 6] = v3.x;
                        vka[offset_morphTarget + 7] = v3.y;
                        vka[offset_morphTarget + 8] = v3.z;

                        // morph normals

                        if (material.morphNormals)
                        {
                            if (needsSmoothNormals)
                            {
                                var faceVertexNormals = morphNormals[vk].vertexNormals[chf];

                                n1 = faceVertexNormals.A;
                                n2 = faceVertexNormals.B;
                                n3 = faceVertexNormals.C;
                            }
                            else
                            {
                                n1 = morphNormals[vk].faceNormals[chf];
                                n2 = n1;
                                n3 = n1;
                            }

                            var nka = morphNormalsArrays[vk];

                            nka[offset_morphTarget] = n1.x;
                            nka[offset_morphTarget + 1] = n1.y;
                            nka[offset_morphTarget + 2] = n1.z;

                            nka[offset_morphTarget + 3] = n2.x;
                            nka[offset_morphTarget + 4] = n2.y;
                            nka[offset_morphTarget + 5] = n2.z;

                            nka[offset_morphTarget + 6] = n3.x;
                            nka[offset_morphTarget + 7] = n3.y;
                            nka[offset_morphTarget + 8] = n3.z;
                        }

                        //
                        offset_morphTarget += 9;
 * */
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglMorphTargetsBuffers[vk]);
                    //          GL.BufferData(BufferTarget.ArrayBuffer, morphTargetsArrays[vk], hint);

                    //                 if (material.morphNormals)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglMorphNormalsBuffers[vk]);
                        //              GL.BufferData(BufferTarget.ArrayBuffer, morphNormalsArrays[vk], hint);
                    }
                }
            }

            if (obj_skinWeights.Count > 0)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    // weights

                    var sw1 = obj_skinWeights[face.A];
                    var sw2 = obj_skinWeights[face.B];
                    var sw3 = obj_skinWeights[face.C];

                    skinWeightArray[offset_skin] = sw1.X;
                    skinWeightArray[offset_skin + 1] = sw1.Y;
                    skinWeightArray[offset_skin + 2] = sw1.Z;
                    skinWeightArray[offset_skin + 3] = sw1.W;

                    skinWeightArray[offset_skin + 4] = sw2.X;
                    skinWeightArray[offset_skin + 5] = sw2.Y;
                    skinWeightArray[offset_skin + 6] = sw2.Z;
                    skinWeightArray[offset_skin + 7] = sw2.W;

                    skinWeightArray[offset_skin + 8] = sw3.X;
                    skinWeightArray[offset_skin + 9] = sw3.Y;
                    skinWeightArray[offset_skin + 10] = sw3.Z;
                    skinWeightArray[offset_skin + 11] = sw3.W;

                    // indices

                    var si1 = obj_skinIndices[face.A];
                    var si2 = obj_skinIndices[face.B];
                    var si3 = obj_skinIndices[face.C];

                    skinIndexArray[offset_skin] = si1.X;
                    skinIndexArray[offset_skin + 1] = si1.Y;
                    skinIndexArray[offset_skin + 2] = si1.Z;
                    skinIndexArray[offset_skin + 3] = si1.W;

                    skinIndexArray[offset_skin + 4] = si2.X;
                    skinIndexArray[offset_skin + 5] = si2.Y;
                    skinIndexArray[offset_skin + 6] = si2.Z;
                    skinIndexArray[offset_skin + 7] = si2.W;

                    skinIndexArray[offset_skin + 8] = si3.X;
                    skinIndexArray[offset_skin + 9] = si3.Y;
                    skinIndexArray[offset_skin + 10] = si3.Z;
                    skinIndexArray[offset_skin + 11] = si3.W;

                    offset_skin += 12;
                }

                if (offset_skin > 0)
                {
                    throw new NotImplementedException();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinIndicesBuffer);
                    //           GL.BufferData(BufferTarget.ArrayBuffer, skinIndexArray, hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinWeightsBuffer);
                    //           GL.BufferData(BufferTarget.ArrayBuffer, skinWeightArray, hint);
                }
            }

            if (dirtyColors)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexColors = face.VertexColors;
                    var faceColor = face.Color;

                    Color c1, c2, c3;

                    if (vertexColors.Length == 3 && material.VertexColors == Three.VertexColors)
                    {
                        c1 = vertexColors[0];
                        c2 = vertexColors[1];
                        c3 = vertexColors[2];
                    }
                    else
                    {
                        c1 = faceColor;
                        c2 = faceColor;
                        c3 = faceColor;
                    }

                    colorArray[offset_color] = c1.R;
                    colorArray[offset_color + 1] = c1.G;
                    colorArray[offset_color + 2] = c1.B;

                    colorArray[offset_color + 3] = c2.R;
                    colorArray[offset_color + 4] = c2.G;
                    colorArray[offset_color + 5] = c2.B;

                    colorArray[offset_color + 6] = c3.R;
                    colorArray[offset_color + 7] = c3.G;
                    colorArray[offset_color + 8] = c3.B;

                    offset_color += 9;
                }

                if (offset_color > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglColorBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorArray.Length * sizeof(float)), colorArray, hint);

                    Debug.WriteLine("BufferData for __webglColorBuffer float[] id {0}", geometryGroup.__webglColorBuffer);
                }
            }

            //if (dirtyTangents && geometry.HasTangents)
            //{
            //    for (var f = 0; f < chunk_faces3.Count; f++)
            //    {
            //        var face = obj_faces[chunk_faces3[f]];

            //        var vertexTangents = face.VertexTangents;

            //        var t1 = vertexTangents[0];
            //        var t2 = vertexTangents[1];
            //        var t3 = vertexTangents[2];

            //        tangentArray[offset_tangent] = t1.X;
            //        tangentArray[offset_tangent + 1] = t1.Y;
            //        tangentArray[offset_tangent + 2] = t1.Z;
            //        tangentArray[offset_tangent + 3] = t1.W;

            //        tangentArray[offset_tangent + 4] = t2.X;
            //        tangentArray[offset_tangent + 5] = t2.Y;
            //        tangentArray[offset_tangent + 6] = t2.Z;
            //        tangentArray[offset_tangent + 7] = t2.W;

            //        tangentArray[offset_tangent + 8] = t3.X;
            //        tangentArray[offset_tangent + 9] = t3.Y;
            //        tangentArray[offset_tangent + 10] = t3.Z;
            //        tangentArray[offset_tangent + 11] = t3.W;

            //        offset_tangent += 12;
            //    }

            //    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglTangentBuffer);
            //    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tangentArray.Length * sizeof(float)), tangentArray, hint); // * 3 ??????

            //    Debug.WriteLine("BufferData for __webglTangentBuffer float[] id {0}", geometryGroup.__webglTangentBuffer);
            //}

            if (dirtyNormals)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexNormals = face.VertexNormals;
                    var faceNormal = face.Normal;

                    if (vertexNormals.Count == 3 && needsSmoothNormals)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            var vn = vertexNormals[i];

                            normalArray[offset_normal] = vn.X;
                            normalArray[offset_normal + 1] = vn.Y;
                            normalArray[offset_normal + 2] = vn.Z;

                            offset_normal += 3;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            normalArray[offset_normal] = faceNormal.X;
                            normalArray[offset_normal + 1] = faceNormal.Y;
                            normalArray[offset_normal + 2] = faceNormal.Z;

                            offset_normal += 3;
                        }
                    }
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglNormalBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normalArray.Length * sizeof(float)), normalArray, hint); // * 3 ??????

                Debug.WriteLine("BufferData for __webglNormalBuffer float[] id {0}", geometryGroup.__webglNormalBuffer);
            }

            if (dirtyUvs && obj_uvs.Count > 0)
            {
                foreach (var fi in chunk_faces3)
                {
                    var uv = obj_uvs[fi];

                    if (uv == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        var uvi = uv[i];

                        uvArray[offset_uv] = uvi.X;
                        uvArray[offset_uv + 1] = uvi.Y;

                        offset_uv += 2;
                    }
                }

                if (offset_uv > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUVBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvArray.Length * sizeof(float)), uvArray, hint);  // * 2 ??????

                    Debug.WriteLine("BufferData for __webglUVBuffer[] float id {0}", geometryGroup.__webglUVBuffer);
                }
            }

            if (dirtyUvs && null != obj_uvs2 && obj_uvs2.Count > 0)
            {
                foreach (var fi in chunk_faces3)
                {
                    var uv2 = obj_uvs2[fi];

                    if (uv2 == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        var uv2i = uv2[i];

                        uv2Array[offset_uv2] = uv2i.X;
                        uv2Array[offset_uv2 + 1] = uv2i.Y;

                        offset_uv2 += 2;
                    }
                }

                if (offset_uv2 > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUV2Buffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uv2Array.Length * sizeof(float)), uv2Array, hint); // * 3 ??????

                    Debug.WriteLine("BufferData for __webglUV2Buffer float[] id {0}", geometryGroup.__webglUV2Buffer);
                }
            }

            if (dirtyElements)
            {
                foreach (var fi in chunk_faces3)
                {
                    faceArray[offset_face + 0] = (ushort)vertexIndex;
                    faceArray[offset_face + 1] = (ushort)(vertexIndex + 1);
                    faceArray[offset_face + 2] = (ushort)(vertexIndex + 2);

                    offset_face += 3;

                    lineArray[offset_line + 0] = (ushort)vertexIndex;
                    lineArray[offset_line + 1] = (ushort)(vertexIndex + 1);

                    lineArray[offset_line + 2] = (ushort)vertexIndex;
                    lineArray[offset_line + 3] = (ushort)(vertexIndex + 2);

                    lineArray[offset_line + 4] = (ushort)(vertexIndex + 1);
                    lineArray[offset_line + 5] = (ushort)(vertexIndex + 2);

                    offset_line += 6;

                    vertexIndex += 3;
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglFaceBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(faceArray.Length * sizeof(ushort)), faceArray, hint);

                Debug.WriteLine("BufferData for __webglFaceBuffer ushort[] id {0}", geometryGroup.__webglFaceBuffer);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglLineBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(lineArray.Length * sizeof(ushort)), lineArray, hint);

                Debug.WriteLine("BufferData for __webglLineBuffer ushort[] id {0}", geometryGroup.__webglLineBuffer);
            }

            if (null != customAttributes && customAttributes.Count > 0)
            {
                for (var i = 0; i < customAttributes.Count; i++)
                {
                    var customAttribute = customAttributes[i];

                    var original = customAttribute["__original"] as Attribute;

                    if (!(bool)((original)["needsUpdate"])) continue;

                    offset_custom = 0;
                    offset_customSrc = 0; // not used

                    var size = ((int)customAttribute["size"]);

                    if (size == 1)
                    {
                        if (!customAttribute.ContainsKey("boundTo") || (string)customAttribute["boundTo"] == "vertices")
                        {
                            var array = (float[])customAttribute["array"];
                            var values = (float[])customAttribute["value"];

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {
                                var face = obj_faces[chunk_faces3[f]];

                                array[offset_custom + 0] = values[face.A];
                                array[offset_custom + 1] = values[face.B];
                                array[offset_custom + 2] = values[face.C];

                                offset_custom += 3;

                            }

                        }
                        else if (customAttribute["boundTo"] == "faces")
                        {
                            throw new NotImplementedException();

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {

                                //var value = customAttribute.value[chunk_faces3[f]];

                                //customAttribute["array"][offset_custom] = value;
                                //customAttribute["array"][offset_custom + 1] = value;
                                //customAttribute["array"][offset_custom + 2] = value;

                                offset_custom += 3;

                            }

                        }

                    }
                    else if (size == 2)
                    {

                        if (customAttribute["boundTo"] == null || (string)customAttribute["boundTo"] == "vertices")
                        {
                            throw new NotImplementedException();

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {
                                var face = obj_faces[chunk_faces3[f]];

                                //var v1 = customAttribute.value[face.A];
                                //var v2 = customAttribute.value[face.B];
                                //var v3 = customAttribute.value[face.C];

                                //customAttribute["array"][offset_custom] = v1.X;
                                //customAttribute["array"][offset_custom + 1] = v1.Y;

                                //customAttribute["array"][offset_custom + 2] = v2.X;
                                //customAttribute["array"][offset_custom + 3] = v2.Y;

                                //customAttribute["array"][offset_custom + 4] = v3.X;
                                //customAttribute["array"][offset_custom + 5] = v3.Y;

                                offset_custom += 6;

                            }

                        }
                        else if ((string)customAttribute["boundTo"] == "faces")
                        {
                            throw new NotImplementedException();

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {

                                //var value = customAttribute.value[chunk_faces3[f]];

                                //var v1 = value;
                                //var v2 = value;
                                //var v3 = value;

                                //customAttribute["array"][offset_custom] = v1.X;
                                //customAttribute["array"][offset_custom + 1] = v1.Y;

                                //customAttribute["array"][offset_custom + 2] = v2.X;
                                //customAttribute["array"][offset_custom + 3] = v2.Y;

                                //customAttribute["array"][offset_custom + 4] = v3.X;
                                //customAttribute["array"][offset_custom + 5] = v3.Y;

                                offset_custom += 6;

                            }

                        }

                    }
                    else if (size == 3)
                    {
                        List<string> pp = null;

                        if ((string)customAttribute["type"] == "C")
                        {

                            pp = new List<string>() { "r", "g", "B" };

                        }
                        else
                        {

                            pp = new List<string>() { "x", "y", "z" };

                        }

                        if (customAttribute["boundTo"] == null || (string)customAttribute["boundTo"] == "vertices")
                        {

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {
                                var array = (float[])customAttribute["array"];

                                var face = obj_faces[chunk_faces3[f]];

                                var v1 = ((float[])customAttribute["value"])[face.A];
                                var v2 = ((float[])customAttribute["value"])[face.B];
                                var v3 = ((float[])customAttribute["value"])[face.C];

                                //array[offset_custom + 0] = v1[pp[0]];
                                //array[offset_custom + 1] = v1[pp[1]];
                                //array[offset_custom + 2] = v1[pp[2]];

                                //array[offset_custom + 3] = v2[pp[0]];
                                //array[offset_custom + 4] = v2[pp[1]];
                                //array[offset_custom + 5] = v2[pp[2]];

                                //array[offset_custom + 6] = v3[pp[0]];
                                //array[offset_custom + 7] = v3[pp[1]];
                                //array[offset_custom + 8] = v3[pp[2]];

                                offset_custom += 9;

                            }

                        }
                        else if ((string)customAttribute["boundTo"] == "faces")
                        {
                            throw new NotImplementedException();

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {
                                var array = (float[])customAttribute["array"];

                                //var value = customAttribute.value[chunk_faces3[f]];

                                //var v1 = value;
                                //var v2 = value;
                                //var v3 = value;

                                //array[offset_custom] = v1[pp[0]];
                                //array[offset_custom + 1] = v1[pp[1]];
                                //array[offset_custom + 2] = v1[pp[2]];

                                //array[offset_custom + 3] = v2[pp[0]];
                                //array[offset_custom + 4] = v2[pp[1]];
                                //array[offset_custom + 5] = v2[pp[2]];

                                //array[offset_custom + 6] = v3[pp[0]];
                                //array[offset_custom + 7] = v3[pp[1]];
                                //array[offset_custom + 8] = v3[pp[2]];

                                offset_custom += 9;

                            }

                        }
                        else if ((string)customAttribute["boundTo"] == "faceVertices")
                        {
                            var array = (float[])customAttribute["array"];
                            var values = (List<List<Vector3>>)customAttribute["value"];

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {
                                var value = values[chunk_faces3[f]];

                                var v1 = value[0];
                                var v2 = value[1];
                                var v3 = value[2];

                                array[offset_custom + 0] = v1.X;
                                array[offset_custom + 1] = v1.Y;
                                array[offset_custom + 2] = v1.Z;

                                array[offset_custom + 3] = v2.X;
                                array[offset_custom + 4] = v2.Y;
                                array[offset_custom + 5] = v2.Z;

                                array[offset_custom + 6] = v3.X;
                                array[offset_custom + 7] = v3.Y;
                                array[offset_custom + 8] = v3.Z;

                                offset_custom += 9;

                            }

                        }

                    }
                    else if (size == 4)
                    {
                        throw new NotImplementedException();

                        if (customAttribute["boundTo"] == null || (string)customAttribute["boundTo"] == "vertices")
                        {

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {

                                var face = obj_faces[chunk_faces3[f]];

                                //var v1 = customAttribute.value[face.A];
                                //var v2 = customAttribute.value[face.B];
                                //var v3 = customAttribute.value[face.C];

                                //customAttribute["array"][offset_custom] = v1.X;
                                //customAttribute["array"][offset_custom + 1] = v1.Y;
                                //customAttribute["array"][offset_custom + 2] = v1.Z;
                                //customAttribute["array"][offset_custom + 3] = v1.W;

                                //customAttribute["array"][offset_custom + 4] = v2.X;
                                //customAttribute["array"][offset_custom + 5] = v2.Y;
                                //customAttribute["array"][offset_custom + 6] = v2.Z;
                                //customAttribute["array"][offset_custom + 7] = v2.W;

                                //customAttribute["array"][offset_custom + 8] = v3.X;
                                //customAttribute["array"][offset_custom + 9] = v3.Y;
                                //customAttribute["array"][offset_custom + 10] = v3.Z;
                                //customAttribute["array"][offset_custom + 11] = v3.W;

                                offset_custom += 12;

                            }

                        }
                        else if ((string)customAttribute["boundTo"] == "faces")
                        {

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {

                                //var value = customAttribute.value[chunk_faces3[f]];

                                //var v1 = value;
                                //var v2 = value;
                                //var v3 = value;

                                //customAttribute["array"][offset_custom] = v1.X;
                                //customAttribute["array"][offset_custom + 1] = v1.Y;
                                //customAttribute["array"][offset_custom + 2] = v1.Z;
                                //customAttribute["array"][offset_custom + 3] = v1.W;

                                //customAttribute["array"][offset_custom + 4] = v2.X;
                                //customAttribute["array"][offset_custom + 5] = v2.Y;
                                //customAttribute["array"][offset_custom + 6] = v2.Z;
                                //customAttribute["array"][offset_custom + 7] = v2.W;

                                //customAttribute["array"][offset_custom + 8] = v3.X;
                                //customAttribute["array"][offset_custom + 9] = v3.Y;
                                //customAttribute["array"][offset_custom + 10] = v3.Z;
                                //customAttribute["array"][offset_custom + 11] = v3.W;

                                offset_custom += 12;

                            }

                        }
                        else if ((string)customAttribute["boundTo"] == "faceVertices")
                        {

                            for (var f = 0; f < chunk_faces3.Count; f++)
                            {

                                //var value = customAttribute.value[chunk_faces3[f]];

                                //var v1 = value[0];
                                //var v2 = value[1];
                                //var v3 = value[2];

                                //customAttribute["array"][offset_custom] = v1.X;
                                //customAttribute["array"][offset_custom + 1] = v1.Y;
                                //customAttribute["array"][offset_custom + 2] = v1.Z;
                                //customAttribute["array"][offset_custom + 3] = v1.W;

                                //customAttribute["array"][offset_custom + 4] = v2.X;
                                //customAttribute["array"][offset_custom + 5] = v2.Y;
                                //customAttribute["array"][offset_custom + 6] = v2.Z;
                                //customAttribute["array"][offset_custom + 7] = v2.W;

                                //customAttribute["array"][offset_custom + 8] = v3.X;
                                //customAttribute["array"][offset_custom + 9] = v3.Y;
                                //customAttribute["array"][offset_custom + 10] = v3.Z;
                                //customAttribute["array"][offset_custom + 11] = v3.W;

                                offset_custom += 12;

                            }

                        }

                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, (int)((Hashtable)customAttribute["buffer"])["id"]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(((float[])customAttribute["array"]).Length * sizeof(float)), (float[])customAttribute["array"], hint);

                    //            Debug.WriteLine("BufferData for custumAttributes float[] id {0}", geometryGroup.__webglLineBuffer);
                }
            }

            if (dispose)
            {
                //delete geometryGroup.__inittedArrays;
                //delete geometryGroup.__inittedArrays;
                //delete geometryGroup.__colorArray;
                //delete geometryGroup.__normalArray;
                //delete geometryGroup.__tangentArray;
                //delete geometryGroup.__uvArray;
                //delete geometryGroup.__uv2Array;
                //delete geometryGroup.__faceArray;
                //delete geometryGroup.__vertexArray;
                //delete geometryGroup.__lineArray;
                //delete geometryGroup.__skinIndexArray;
                //delete geometryGroup.__skinWeightArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygonoffset"></param>
        /// <param name="factor"></param>
        /// <param name="units"></param>
        private void SetPolygonOffset(bool polygonoffset, float factor, float units)
        {
            if (this._oldPolygonOffset != polygonoffset)
            {
                if (polygonoffset)
                {
                    GL.Enable(EnableCap.PolygonOffsetFill);
                }
                else
                {
                    GL.Disable(EnableCap.PolygonOffsetFill);
                }

                this._oldPolygonOffset = polygonoffset;
            }

            if (polygonoffset && (this._oldPolygonOffsetFactor != factor || this._oldPolygonOffsetUnits != units))
            {
                GL.PolygonOffset(factor, units);

                this._oldPolygonOffsetFactor = factor;
                this._oldPolygonOffsetUnits = units;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="intensitySq"></param>
        private void setColorGamma(List<float> array, int offset, Color color, float intensitySq)
        {
            array.Resize(offset + 1 + 2);

            array[offset] = (color.R / 255.0f * color.R / 255.0f * intensitySq);
            array[offset + 1] = (color.G / 255.0f * color.G / 255.0f * intensitySq);
            array[offset + 2] = (color.B / 255.0f * color.B / 255.0f * intensitySq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        private void setColorLinear(List<float> array, int offset, Color color, float intensity)
        {
            array.Resize(offset + 1 + 2);

            array[offset] = (color.R / 255.0f * intensity);
            array[offset + 1] = (color.G / 255.0f * intensity);
            array[offset + 2] = (color.B / 255.0f * intensity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Color copyGammaToLinear(Color color)
        {
            var value = Color.FromArgb(
                (int)((color.R / 255.0f * color.R / 255.0f) * 255.0f),
                (int)((color.G / 255.0f * color.G / 255.0f) * 255.0f),
                (int)((color.B / 255.0f * color.B / 255.0f) * 255.0f));

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="fog"></param>
        private void RefreshUniformsFog(Uniforms uniforms, Fog fog)
        {

            uniforms["fogColor"]["value"] = fog.Color;

            if (fog is Fog)
            {

                uniforms["fogNear"]["value"] = fog.Near;
                uniforms["fogFar"]["value"] = fog.Far;

            }
            else if (fog is FogExp2)
            {

                //   uniforms["fogDensity"]["value"] = fog.density;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsPhong(Uniforms uniforms, MeshPhongMaterial material)
        {
            uniforms["shininess"]["value"] = material.Shininess;

            if (this.gammaInput)
            {
                uniforms["ambient"]["value"] = this.copyGammaToLinear(material.Ambient);
                uniforms["emissive"]["value"] = this.copyGammaToLinear(material.Emissive);
                uniforms["specular"]["value"] = this.copyGammaToLinear(material.Specular);
            }
            else
            {
                uniforms["ambient"]["value"] = material.Ambient;
                uniforms["emissive"]["value"] = material.Emissive;
                uniforms["specular"]["value"] = material.Specular;
            }

            if (material.WrapAround)
            {
                uniforms["wrapRGB"]["value"] = material.WrapRgb;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lights"></param>
        private void SetupLights(IEnumerable<Light> lights)
        {
            var zlights = this._lights;

            var ambiColors = zlights.ambient.colors;

            var dirColors = zlights.directional.colors;
            var dirPositions = zlights.directional.positions;

            var pointColors = zlights.point.colors;
            var pointPositions = zlights.point.positions;
            var pointDistances = zlights.point.distances;

            var spotColors = zlights.spot.colors;
            var spotPositions = zlights.spot.positions;
            var spotDistances = zlights.spot.distances;
            var spotDirections = zlights.spot.directions;
            var spotAnglesCos = zlights.spot.anglesCos;
            var spotExponents = zlights.spot.exponents;

            var hemiSkyColors = zlights.hemi.skyColors;
            var hemiGroundColors = zlights.hemi.groundColors;
            var hemiPositions = zlights.hemi.positions;

            var dirLength = 0;
            var pointLength = 0;
            var spotLength = 0;
            var hemiLength = 0;

            var dirCount = 0;
            var pointCount = 0;
            var spotCount = 0;
            var hemiCount = 0;

            foreach (var light in lights)
            {
                if (light is ILightShadow && ((ILightShadow)light).onlyShadow)
                {
                    continue;
                }

                var color = light.color;

                if (light is AmbientLight)
                {
                    if (!light.Visible) continue;

                    ambiColors.Resize(3);

                    float r = 0; float g = 0; float b = 0;

                    if (this.gammaInput)
                    {
                        r += (color.R / 255.0f * color.R / 255.0f);
                        g += (color.G / 255.0f * color.G / 255.0f);
                        b += (color.B / 255.0f * color.B / 255.0f);
                    }
                    else
                    {
                        r += (color.R / 255.0f);
                        g += (color.G / 255.0f);
                        b += (color.B / 255.0f);
                    }

                    ambiColors[0] = r;
                    ambiColors[1] = g;
                    ambiColors[2] = b;

                }
                else if (light is DirectionalLight)
                {
                    var directionalLight = light as DirectionalLight;

                    dirCount += 1;

                    if (!light.Visible) continue;

                    this._direction = new Vector3().SetFromMatrixPosition(directionalLight.MatrixWorld);
                    var vector3 = new Vector3().SetFromMatrixPosition(directionalLight.target.MatrixWorld);
                    this._direction -= vector3;
                    this._direction.Normalize();

                    var dirOffset = dirLength * 3;

                    dirPositions.Resize(dirOffset + 1 + 2);

                    dirPositions[dirOffset] = this._direction.X;
                    dirPositions[dirOffset + 1] = this._direction.Y;
                    dirPositions[dirOffset + 2] = this._direction.Z;

                    if (this.gammaInput)
                    {
                        this.setColorGamma(dirColors, dirOffset, color, directionalLight.intensity * directionalLight.intensity);
                    }
                    else
                    {
                        this.setColorLinear(dirColors, dirOffset, color, directionalLight.intensity);
                    }

                    dirLength += 1;

                }
                else if (light is PointLight)
                {

                    var pointLight = light as PointLight;

                    pointCount += 1;

                    if (!light.Visible) continue;

                    var pointOffset = pointLength * 3;

                    if (this.gammaInput)
                    {

                        this.setColorGamma(pointColors, pointOffset, color, pointLight.intensity * pointLight.intensity);

                    }
                    else
                    {

                        this.setColorLinear(pointColors, pointOffset, color, pointLight.intensity);

                    }

                    var vector3 = new Vector3().SetFromMatrixPosition(light.MatrixWorld);

                    pointPositions.Resize(pointOffset + 1 + 2);

                    pointPositions[pointOffset] = vector3.X;
                    pointPositions[pointOffset + 1] = vector3.Y;
                    pointPositions[pointOffset + 2] = vector3.Z;

                    pointDistances.Resize(pointLength + 1);
                    pointDistances[pointLength] = pointLight.distance;

                    pointLength += 1;

                }
                else if (light is SpotLight)
                {
                    var spotLight = light as SpotLight;

                    spotCount += 1;

                    if (!light.Visible) continue;

                    var spotOffset = spotLength * 3;

                    if (this.gammaInput)
                    {

                        this.setColorGamma(spotColors, spotOffset, color, spotLight.intensity * spotLight.intensity);

                    }
                    else
                    {

                        this.setColorLinear(spotColors, spotOffset, color, spotLight.intensity);

                    }

                    var vector3 = new Vector3().SetFromMatrixPosition(light.MatrixWorld);

                    spotPositions.Resize(spotOffset + 1 + 2);
                    spotPositions[spotOffset] = vector3.X;
                    spotPositions[spotOffset + 1] = vector3.Y;
                    spotPositions[spotOffset + 2] = vector3.Z;

                    spotDistances.Resize(spotLength + 1);
                    spotDistances[spotLength] = spotLight.distance;

                    this._direction = vector3;

                    vector3 = new Vector3().SetFromMatrixPosition(spotLight.target.MatrixWorld);
                    this._direction -= vector3;
                    this._direction.Normalize();

                    spotDirections.Resize(spotOffset + 1 + 2);
                    spotDirections[spotOffset] = this._direction.X;
                    spotDirections[spotOffset + 1] = this._direction.Y;
                    spotDirections[spotOffset + 2] = this._direction.Z;

                    spotAnglesCos.Resize(spotLength + 1);
                    spotAnglesCos[spotLength] = (float)System.Math.Cos(spotLight.angle);

                    spotExponents.Resize(spotLength + 1);
                    spotExponents[spotLength] = spotLight.exponent;

                    spotLength += 1;

                }
                else if (light is HemisphereLight)
                {
                    var hemisphereLight = light as HemisphereLight;

                    hemiCount += 1;

                    if (!light.Visible) continue;

                    this._direction = new Vector3().SetFromMatrixPosition(light.MatrixWorld).Normalize();

                    var hemiOffset = hemiLength * 3;

                    hemiPositions.Resize(hemiOffset + 1 + 2);
                    hemiPositions[hemiOffset] = this._direction.X;
                    hemiPositions[hemiOffset + 1] = this._direction.Y;
                    hemiPositions[hemiOffset + 2] = this._direction.Z;

                    var skyColor = light.color;
                    var groundColor = hemisphereLight.groundColor;

                    if (this.gammaInput)
                    {

                        var intensitySq = hemisphereLight.intensity * hemisphereLight.intensity;

                        this.setColorGamma(hemiSkyColors, hemiOffset, skyColor, intensitySq);
                        this.setColorGamma(hemiGroundColors, hemiOffset, groundColor, intensitySq);

                    }
                    else
                    {

                        this.setColorLinear(hemiSkyColors, hemiOffset, skyColor, hemisphereLight.intensity);
                        this.setColorLinear(hemiGroundColors, hemiOffset, groundColor, hemisphereLight.intensity);

                    }

                    hemiLength += 1;

                }

            }

            // null eventual remains from removed Lights
            // (this is to avoid if in shader)

            for (var l = dirLength * 3; l < System.Math.Max(dirColors.Count, dirCount * 3); l++) dirColors[l] = 0.0f;
            for (var l = pointLength * 3; l < System.Math.Max(pointColors.Count, pointCount * 3); l++) pointColors[l] = 0.0f;
            for (var l = spotLength * 3; l < System.Math.Max(spotColors.Count, spotCount * 3); l++) spotColors[l] = 0.0f;
            for (var l = hemiLength * 3; l < System.Math.Max(hemiSkyColors.Count, hemiCount * 3); l++) hemiSkyColors[l] = 0.0f;
            for (var l = hemiLength * 3; l < System.Math.Max(hemiGroundColors.Count, hemiCount * 3); l++) hemiGroundColors[l] = 0.0f;

            zlights.directional.length = dirLength;
            zlights.point.length = pointLength;
            zlights.spot.length = spotLength;
            zlights.hemi.length = hemiLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsLine(Uniforms uniforms, Material material)
        {
            var lineBasicMaterial = material as LineBasicMaterial;
            if (null != lineBasicMaterial)
            {
                uniforms["diffuse"]["value"] = lineBasicMaterial.Color;
            }
            else
            {
                Debug.Assert(1 == 0, "Other material than LineBasicMaterial???");
            }
            /*
            var meshBasicMaterial = material as MeshBasicMaterial;
            if (null != meshBasicMaterial)
                uniforms["diffuse"]["value"] = meshBasicMaterial.Color;

            var meshLambertMaterial = material as MeshLambertMaterial;
            if (null != meshLambertMaterial)
                uniforms["diffuse"]["value"] = meshLambertMaterial.Color;

            var meshPhongMaterial = material as MeshPhongMaterial;
            if (null != meshPhongMaterial)
                uniforms["diffuse"]["value"] = meshPhongMaterial.Color;

            var pointCloudMaterial = material as PointCloudMaterial;
            if (null != pointCloudMaterial)
                uniforms["diffuse"]["value"] = pointCloudMaterial.Color;
     */
            //           uniforms["opacity"]["value"] = material.opacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsDash(Uniforms uniforms, Material material)
        {
            throw new NotImplementedException();
            //uniforms["dashSize"]["value"] = material.dashSize;
            //uniforms["totalSize"]["value"] = material.dashSize + material.gapSize;
            //uniforms["scale"]["value"] = material.scale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsParticle(Uniforms uniforms, Material material)
        {
            if (material is PointCloudMaterial)
            {
                var m = material as PointCloudMaterial;

                uniforms["psColor"]["value"] = m.Color;
                uniforms["opacity"]["value"] = m.Opacity;
                uniforms["size"]["value"] = m.Size;
                uniforms["scale"]["value"] = this._currentHeight / 2.0f; // TODO: Cache this.

                uniforms["map"]["value"] = m.Map;

                return;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsLambert(Uniforms uniforms, MeshLambertMaterial material)
        {
            if (this.gammaInput)
            {
                uniforms["ambient"]["value"] = this.copyGammaToLinear(material.Ambient);
                uniforms["emissive"]["value"] = this.copyGammaToLinear(material.Emissive);
            }
            else
            {
                uniforms["ambient"]["value"] = material.Ambient;
                uniforms["emissive"]["value"] = material.Emissive;
            }

            if (material.WrapAround)
            {
                uniforms["wrapRGB"]["value"] = material.WrapRgb;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="lights"></param>
        private void refreshUniformsShadow(Uniforms uniforms, IEnumerable<Light> lights)
        {
            if (uniforms["shadowMatrix"] != null)
            {
                foreach (var light in lights)
                {
                    if (light is ILightShadow)
                    {
                        var lightShadow = light as ILightShadow;

                        if (!light.CastShadow) continue;
                        //        if (lightShadow.shadowCascade) continue;

                        if (light is SpotLight || (light is DirectionalLight/* && !lightShadow.shadowCascade*/))
                        {
                            ((List<Texture>)uniforms["shadowMap"]["value"]).Add(lightShadow.shadowMap);
                            ((List<Size>)uniforms["shadowMapSize"]["value"]).Add(lightShadow.shadowMapSize);

                            ((List<Matrix4>)uniforms["shadowMatrix"]["value"]).Add(lightShadow.shadowMatrix);

                            ((List<float>)uniforms["shadowDarkness"]["value"]).Add(lightShadow.shadowDarkness);
                            ((List<float>)uniforms["shadowBias"]["value"]).Add(lightShadow.shadowBias);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="lights"></param>
        private void RefreshUniformsLights(Uniforms uniforms, LightCollection lights)
        {
            uniforms["ambientLightColor"]["value"] = this._lights.ambient.colors;

            uniforms["directionalLightColor"]["value"] = this._lights.directional.colors;
            uniforms["directionalLightDirection"]["value"] = this._lights.directional.positions;

            uniforms["pointLightColor"]["value"] = this._lights.point.colors;
            uniforms["pointLightPosition"]["value"] = this._lights.point.positions;
            uniforms["pointLightDistance"]["value"] = this._lights.point.distances;

            uniforms["spotLightColor"]["value"] = this._lights.spot.colors;
            uniforms["spotLightPosition"]["value"] = this._lights.spot.positions;
            uniforms["spotLightDistance"]["value"] = this._lights.spot.distances;
            uniforms["spotLightDirection"]["value"] = this._lights.spot.directions;
            uniforms["spotLightAngleCos"]["value"] = this._lights.spot.anglesCos;
            uniforms["spotLightExponent"]["value"] = this._lights.spot.exponents;

            uniforms["hemisphereLightSkyColor"]["value"] = this._lights.hemi.skyColors;
            uniforms["hemisphereLightGroundColor"]["value"] = this._lights.hemi.groundColors;
            uniforms["hemisphereLightDirection"]["value"] = this._lights.hemi.positions;
        }

        /// <summary>
        /// If uniforms are marked as clean, they don't need to be loaded to the GPU.
        /// </summary>
        private void markUniformsLightsNeedsUpdate(Uniforms uniforms, bool value)
        {
            uniforms["ambientLightColor"]["needsUpdate"] = value;

            uniforms["directionalLightColor"]["needsUpdate"] = value;
            uniforms["directionalLightDirection"]["needsUpdate"] = value;

            uniforms["pointLightColor"]["needsUpdate"] = value;
            uniforms["pointLightPosition"]["needsUpdate"] = value;
            uniforms["pointLightDistance"]["needsUpdate"] = value;

            uniforms["spotLightColor"]["needsUpdate"] = value;
            uniforms["spotLightPosition"]["needsUpdate"] = value;
            uniforms["spotLightDistance"]["needsUpdate"] = value;
            uniforms["spotLightDirection"]["needsUpdate"] = value;
            uniforms["spotLightAngleCos"]["needsUpdate"] = value;
            uniforms["spotLightExponent"]["needsUpdate"] = value;

            uniforms["hemisphereLightSkyColor"]["needsUpdate"] = value;
            uniforms["hemisphereLightGroundColor"]["needsUpdate"] = value;
            uniforms["hemisphereLightDirection"]["needsUpdate"] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="object3D"></param>
        /// <returns></returns>
        private WebGlProgram SetProgram(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, Object3D object3D)
        {
            this._usedTextureUnits = 0;

            if (material.NeedsUpdate)
            {
                if (material["program"] != null)
                {
                    this.DeallocateMaterial(material);
                }

                this.InitMaterial(material, lights, fog, object3D);
                material.NeedsUpdate = false;
            }

            var materialMorphTargets = material as IMorphTargets;
            if (null != materialMorphTargets && materialMorphTargets.MorphTargets)
            {
                if (null == object3D.__webglMorphTargetInfluences)
                {
                    object3D.__webglMorphTargetInfluences = new float[this.maxMorphTargets];
                }
            }

            var refreshProgram = false;
            var refreshMaterial = false;
            var refreshLights = false;

            var program = ((WebGlProgram)material["program"]);
            var uniformsLocation = program.Uniforms;
            var m_uniforms = ((WebGlShader)material["__webglShader"]).Uniforms;

            object uniformLocation = null;

            if (program.Id != this._currentProgram)
            {
                GL.UseProgram(program.Program);
                this._currentProgram = program.Id;

                refreshProgram = true;
                refreshMaterial = true;
                refreshLights = true;
            }

            if (material.Id != this._currentMaterialId)
            {
                if (this._currentMaterialId == -1)
                {
                    refreshLights = true;
                }

                this._currentMaterialId = material.Id;

                refreshMaterial = true;
            }

            if (refreshProgram || camera != this._currentCamera)
            {
                uniformLocation = uniformsLocation["projectionMatrix"];
                if (null != uniformLocation)
                    GL.UniformMatrix4((int)uniformLocation, 1, false, camera.ProjectionMatrix.Elements);

                if (this._logarithmicDepthBuffer)
                {
                    throw new NotImplementedException();
                    //   GL.Uniform1(p_uniforms["logDepthBufFC"], 2.0 / (Math.Log(camera.far + 1.0) / Math.LN2));

                }

                if (camera != this._currentCamera)
                {
                    this._currentCamera = camera;
                }

                // load material specific uniformsLocation
                // (shader material also gets them for the sake of genericity)

                if (material is ShaderMaterial ||
                     material is MeshPhongMaterial ||
                     (material.EnvMap != null))
                {
                    uniformLocation = uniformsLocation["cameraPosition"];
                    if (uniformLocation != null)
                    {
                        var vector3 = new Vector3().SetFromMatrixPosition(camera.MatrixWorld);
                        GL.Uniform3((int)uniformLocation, vector3.X, vector3.Y, vector3.Z);
                    }
                }


                if (material is MeshPhongMaterial
                || material is MeshLambertMaterial
                || //               material.skinning ||
                    material is ShaderMaterial)
                {
                    uniformLocation = uniformsLocation["viewMatrix"];
                    if (uniformLocation != null)
                    {
                        GL.UniformMatrix4((int)uniformLocation, 1, false, camera.MatrixWorldInverse.Elements);
                    }
                }
            }

            // skinning uniformsLocation must be Set even if material didn't change
            // auto-setting of texture unit for bone texture must go before other textures
            // not sure why, but otherwise weird things happen

            //if ( material.skinning ) {

            //    if ( object3D.bindMatrix && p_uniforms["bindMatrix"] !== null ) {

            //        GL.uniformMatrix4fv( p_uniforms["bindMatrix"], false, object3D.bindMatrix.elements );

            //    }

            //    if ( object3D.bindMatrixInverse && p_uniforms.bindMatrixInverse !== null ) {

            //        GL.uniformMatrix4fv( p_uniforms["bindMatrixInverse"], false, object3D.bindMatrixInverse.elements );

            //    }

            //    if ( supportsBoneTextures && object3D.skeleton && object3D.skeleton.useVertexTexture ) {

            //        if ( p_uniforms.boneTexture !== null ) {

            //            var textureUnit = getTextureUnit();

            //            GL.uniform1i( p_uniforms["boneTexture"], textureUnit );
            //            _this.setTexture( object3D.skeleton.boneTexture, textureUnit );

            //        }

            //        if ( p_uniforms.boneTextureWidth !== null ) {

            //            GL.uniform1i( p_uniforms["boneTextureWidth"], object3D.skeleton.boneTextureWidth );

            //        }

            //        if ( p_uniforms.boneTextureHeight !== null ) {

            //            GL.uniform1i( p_uniforms["boneTextureHeight"], object3D.skeleton.boneTextureHeight );

            //        }

            //    } else if ( object3D.skeleton && object3D.skeleton.boneMatrices ) {

            //        if ( p_uniforms.boneGlobalMatrices !== null ) {

            //            GL.uniformMatrix4fv( p_uniforms["boneGlobalMatrices"], false, object3D.skeleton.boneMatrices );

            //        }

            //    }

            //}

            if (refreshMaterial)
            {
                // refresh uniformsLocation common to several materials

                if (null != fog && material is MeshPhongMaterial && ((MeshPhongMaterial)material).Fog)
                {
                    this.RefreshUniformsFog(m_uniforms, fog);
                }

                if (material is MeshPhongMaterial ||
                     material is MeshLambertMaterial
           /*          material.lights*/) // TODO
                {
                    if (this._lightsNeedUpdate)
                    {

                        refreshLights = true;
                        this.SetupLights(lights);
                        this._lightsNeedUpdate = false;
                    }

                    if (refreshLights)
                    {
                        this.RefreshUniformsLights(m_uniforms, this._lights);
                        this.markUniformsLightsNeedsUpdate(m_uniforms, true);
                    }
                    else
                    {
                        this.markUniformsLightsNeedsUpdate(m_uniforms, false);
                    }
                }

                if (material is MeshBasicMaterial || material is MeshLambertMaterial || material is MeshPhongMaterial)
                {
                    this.RefreshUniformsCommon(m_uniforms, material);
                }

                // refresh single material specific uniformsLocation

                if (material is LineBasicMaterial)
                {
                    this.RefreshUniformsLine(m_uniforms, material);
                }
                else if (material is LineDashedMaterial)
                {
                    this.RefreshUniformsLine(m_uniforms, material);
                    this.RefreshUniformsDash(m_uniforms, material);
                }
                else if (material is PointCloudMaterial)
                {
                    this.RefreshUniformsParticle(m_uniforms, material);
                }
                else if (material is MeshPhongMaterial)
                {
                    this.RefreshUniformsPhong(m_uniforms, material as MeshPhongMaterial);
                }
                else if (material is MeshLambertMaterial)
                {
                    this.RefreshUniformsLambert(m_uniforms, (MeshLambertMaterial)material);
                }
                else if (material is MeshDepthMaterial)
                {
                    m_uniforms["mNear"]["value"] = camera.Near;
                    m_uniforms["mFar"]["value"] = camera.Far;
                    m_uniforms["opacity"]["value"] = material.Opacity;
                }
                else if (material is MeshNormalMaterial)
                {
                    m_uniforms["opacity"]["value"] = material.Opacity;
                }

                if (object3D.ReceiveShadow/* && ! material._shadowPass*/ )
                {
                    this.refreshUniformsShadow(m_uniforms, lights);
                }

                // load common uniformsLocation

                this.LoadUniformsGeneric(material.UniformsList);
            }

            LoadUniformsMatrices(uniformsLocation, object3D);

            uniformLocation = uniformsLocation["modelMatrix"];
            if (uniformLocation != null)
            {
                //Console.WriteLine("object3D.MatrixWorld\n{0}", object3D.MatrixWorld);

                GL.UniformMatrix4((int)uniformLocation, 1, false, object3D.MatrixWorld.Elements);
            }

            return program;
        }

        //NOTE: This is A new method added to convert an array to IntPtr - needs testing
        public IntPtr GetIntPtr(Byte[] byteBuf)
        {
            IntPtr ptr = Marshal.AllocHGlobal(byteBuf.Length);
            for (int i = 0; i < byteBuf.Length; i++)
            {
                Marshal.WriteByte(ptr, i, byteBuf[i]);
            }
            return ptr;
        }

        /// <summary>
        /// </summary>
        /// <param name="texture"></param>
        private void UploadTexture(Texture texture)
        {
            if (!texture.__webglInit)
            {
                texture.__webglInit = true;

                texture.Disposed += this.OnTextureDispose;

                texture.__webglTexture = GL.GenTexture();

                this.Info.memory.Textures++;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture.__webglTexture);

            //  GL.PixelStore( PixelStoreParameter.     GL.UNPACK_FLIP_Y_WEBGL, texture.flipY );
            //  GL.PixelStore( PixelStoreParameter.   GL.UNPACK_PREMULTIPLY_ALPHA_WEBGL, texture.premultiplyAlpha );
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, texture.UnpackAlignment);

            texture.Image = this.clampToMaxSize(texture.Image, this.MaxTextureSize);

            var image = texture.Image;
            var isImagePowerOfTwo = IsPowerOfTwo(image.Width) && IsPowerOfTwo(image.Height);
            //var glFormat = (OpenTK.Graphics.OpenGL.PixelFormat)this.paramThreeToGL(texture.Format);

            var glFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;

            var glInternalFormat = (PixelInternalFormat)this.paramThreeToGL(texture.Format);
            var glType = (PixelType)this.paramThreeToGL(texture.Type);

            this.SetTextureParameters(TextureTarget.Texture2D, texture, isImagePowerOfTwo);

            var mipmaps = texture.Mipmaps;

            if (texture is DataTexture)
            {
                Debug.Assert(null != mipmaps, "mipmaps not Set");

                // use manually created mipmaps if available
                // if there are no manual mipmaps
                // Set 0 level mipmap and then use GL to generate other mipmap levels

                if (mipmaps.Count > 0 && isImagePowerOfTwo)
                {
                    for (var i = 0; i < mipmaps.Count; i++)
                    {
                        throw new NotImplementedException();

                        //var mipmap = mipmaps[i];
                        //GL.TexImage2D(TextureTarget.Texture2D, i, texture.internalFormat, mipmap.width, mipmap.height, 0, texture.format, texture.type, mipmap.data);
                    }
                    texture.GenerateMipmaps = false;
                }
                else
                {
                    // From .Net Image class to raw scan data
                    var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, glInternalFormat, image.Width, image.Height, 0, glFormat, glType, data.Scan0);
                    image.UnlockBits(data);
                }
            }
            else if (texture is CompressedTexture)
            {
                Debug.Assert(null != mipmaps, "mipmaps not Set");

                for (var level = 0; level < mipmaps.Count; level++)
                {
                    var mipmap = mipmaps[level];
                    if (texture.Format != Three.RGBAFormat)
                    {
                        GL.CompressedTexImage2D(TextureTarget.Texture2D
                            , level
                            , (InternalFormat)this.paramThreeToGL(texture.Format) //!! Changed - test
                            , mipmap.Width
                            , mipmap.Height
                            , 0
                            , mipmap.Data.Length
                            , GetIntPtr(mipmap.Data));
                    }
                    else
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, level, glInternalFormat, mipmap.Width, mipmap.Height, 0, glFormat, glType, mipmap.Data);
                    }
                }
            }
            else
            {
                // regular Texture (image, video, canvas)

                // use manually created mipmaps if available
                // if there are no manual mipmaps
                // Set 0 level mipmap and then use GL to generate other mipmap levels

                if (null != mipmaps && mipmaps.Count > 0 && isImagePowerOfTwo)
                {
                    for (var i = 0; i < mipmaps.Count; i++)
                    {
                        throw new NotImplementedException();

                        var mipmap = mipmaps[i];
                        // GL.TexImage2D(TextureTarget.Texture2D, i, glInternalFormat, image.Width, image.Height, 0, glFormat, texture.type, mipmap);
                    }
                    texture.GenerateMipmaps = false;
                }
                else
                {
                    var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, glInternalFormat, image.Width, image.Height, 0, glFormat, glType, data.Scan0);
                    image.UnlockBits(data);
                }
            }

            if (texture.GenerateMipmaps && isImagePowerOfTwo)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            texture.NeedsUpdate = false;

            //if ( texture.onUpdate ) 
            //    texture.onUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="slot"></param>
        private void SetCubeTexture(Texture texture, int slot)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="slot"></param>
        private void SetCubeTextureDynamic(Texture texture, int slot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + slot);
            GL.BindTexture(TextureTarget.TextureCubeMap, texture.__webglTexture);
        }

        /// <summary>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="slot"></param>
        private void SetTexture(ITexture texture, int slot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + slot);

            if (texture.NeedsUpdate)
            {
                this.UploadTexture((Texture)texture);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, texture.__webglTexture);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        private Bitmap clampToMaxSize(Bitmap image, int maxSize)
        {
            if (image.Width > maxSize || image.Height > maxSize)
            {

                // Warning: Scaling through the canvas will only work with images that use
                // premultiplied alpha.

                var scale = maxSize / System.Math.Max(image.Width, image.Height);

                //var canvas = document.createElement( 'canvas' );
                //canvas.width = Math.floor( image.width * scale );
                //canvas.height = Math.floor( image.height * scale );

                //var context = canvas.getContext( '2d' );
                //context.drawImage( image, 0, 0, image.width, image.height, 0, 0, canvas.width, canvas.height );

                //console.log( 'THREE.WebGLRenderer:', image, 'is too big (' + image.width + 'x' + image.height + '). Resized to ' + canvas.width + 'x' + canvas.height + '.' );

                return image;

            }

            return image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textureTarget"></param>
        /// <param name="texture"></param>
        /// <param name="isImagePowerOfTwo"></param>
        private void SetTextureParameters(TextureTarget textureTarget, ITexture texture, bool isImagePowerOfTwo)
        {
            if (isImagePowerOfTwo)
            {
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapS, this.paramThreeToGL(texture.WrapS));
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapT, this.paramThreeToGL(texture.WrapT));

                GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, this.paramThreeToGL(texture.MagFilter));
                GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, this.paramThreeToGL(texture.MinFilter));
            }
            else
            {
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, this.filterFallback(texture.MagFilter));
                GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, this.filterFallback(texture.MinFilter));
            }

            if (this.glExtensionTextureFilterAnisotropic && (texture.Type != Three.FloatType))
            {
                if (texture.Anisotropy > 1)// || texture.__oldAnisotropy < 0 ) // TODO
                {
                    GL.TexParameter(textureTarget, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, System.Math.Min(texture.Anisotropy, this.MaxAnisotropy));
                    //             texture.__oldAnisotropy = texture.Anisotropy;// TODO
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="camera"></param>
        private static void SetupMatrices(Object3D object3D, Camera camera)
        {
            object3D._modelViewMatrix = camera.MatrixWorldInverse * object3D.MatrixWorld;
            object3D._normalMatrix.GetNormalMatrix(object3D._modelViewMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        private void SetupMorphTargets(Material material, GeometryGroup geometryGroup, Object3D object3D)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
		private void UpdateSkeletons(Object3D object3D)
        {
            if (object3D is SkinnedMesh)
            {
                //object3D.skeleton.update();
            }

            foreach (var child in object3D.Children)
            {
                this.UpdateSkeletons(child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webglObject"></param>
        private void UnrollBufferMaterial(WebGlObject webglObject)
        {
            var object3D = webglObject.object3D;
            Debug.Assert(null != object3D);

            var geometry = object3D.Geometry;
            var material = object3D.Material;

            if (material is MeshFaceMaterial)
            {
                throw new NotImplementedException();

                var buffer = webglObject.buffer as GeometryGroup;

                var materialIndex = geometry is BufferGeometry ? 0 : buffer.MaterialIndex;

                //    material = material.materials[ materialIndex ];

                if (material.Transparent)
                {
                    webglObject.material = material;
                    this.transparentObjects.Add(webglObject);
                }
                else
                {
                    webglObject.material = material;
                    this.opaqueObjects.Add(webglObject);
                }
            }
            else
            {
                if (null != material)
                {
                    if (material.Transparent)
                    {
                        webglObject.material = material;
                        this.transparentObjects.Add(webglObject);
                    }
                    else
                    {
                        webglObject.material = material;
                        this.opaqueObjects.Add(webglObject);
                    }
                }
            }
        }

        // Buffer setting

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="hint"></param>
        /// <param name="object3D"></param>
        private void SetParticleBuffers(Geometry geometry, BufferUsageHint hint, PointCloud object3D)
        {
            var vertices = geometry.Vertices;
            var vl = vertices.Count;

            var colors = geometry.Colors;
            var cl = colors.Count;

            var vertexArray = geometry.__vertexArray;
            var colorArray = geometry.__colorArray;

            var sortArray = geometry.__sortArray;

            var dirtyVertices = geometry.VerticesNeedUpdate;
            var dirtyElements = geometry.ElementsNeedUpdate;
            var dirtyColors = geometry.ColorsNeedUpdate;

            var customAttributes = geometry.__webglCustomAttributesList;

            var offset = 0;

            if (object3D.sortParticles)
            {

                throw new NotImplementedException();

                this._projScreenMatrixPS.Copy(this._projScreenMatrix);
                this._projScreenMatrixPS.Multiply(object3D.MatrixWorld);

                for (int v = 0; v < vl; v++)
                {

                    var vertex = vertices[v];

                    var _vector3 = new Vector3();
                    _vector3.Copy(vertex);
                    _vector3.ApplyProjection(this._projScreenMatrixPS);

                    //               sortArray[ v ] = [ _vector3.Z, v ];
                }

                //             sortArray.sort(numericalSort);

                for (int v = 0; v < vl; v++)
                {

                    //                var vertex = vertices[sortArray[v][1]];

                    offset = v * 3;

                    //vertexArray[ offset ]     = vertex.X;
                    //vertexArray[ offset + 1 ] = vertex.Y;
                    //vertexArray[ offset + 2 ] = vertex.Z;

                }

                for (int c = 0; c < cl; c++)
                {

                    offset = c * 3;

                    //                var Color = colors[sortArray[C][1]];

                    //colorArray[ offset ]     = Color.R;
                    //colorArray[ offset + 1 ] = Color.G;
                    //colorArray[ offset + 2 ] = Color.B;

                }

                if (null != customAttributes)
                {
                    /*
                                        for (int i = 0, il = customAttributes.Count; i < il; i ++ ) {

                                            var customAttribute = customAttributes[ i ];

                                            if ( ! ( customAttribute.boundTo == null || customAttribute.boundTo == "vertices" ) ) continue;

                                            offset = 0;

                                            var cal = customAttribute.value.length;

                                            if ( customAttribute.size == 1 ) {

                                                for (int ca = 0; ca < cal; ca ++ ) {

                                                    var index = sortArray[ ca ][ 1 ];

                                                    customAttribute.array[ ca ] = customAttribute.value[ index ];

                                                }

                                            } else if ( customAttribute.size == 2 ) {

                                                for (int ca = 0; ca < cal; ca ++ ) {

                                                    var index = sortArray[ ca ][ 1 ];

                                                    var value = customAttribute.value[ index ];

                                                    customAttribute.array[ offset ]   = value.X;
                                                    customAttribute.array[ offset + 1 ] = value.Y;

                                                    offset += 2;

                                                }

                                            } else if ( customAttribute.size == 3 ) {

                                                if ( customAttribute.type == "C" ) {

                                                    for (int ca = 0; ca < cal; ca ++ ) {

                                                        var index = sortArray[ ca ][ 1 ];

                                                        var value = customAttribute.value[ index ];

                                                        customAttribute.array[ offset ]     = value.R;
                                                        customAttribute.array[ offset + 1 ] = value.G;
                                                        customAttribute.array[ offset + 2 ] = value.B;

                                                        offset += 3;

                                                    }

                                                } else {

                                                    for (int ca = 0; ca < cal; ca ++ ) {

                                                        var index = sortArray[ ca ][ 1 ];

                                                        var value = customAttribute.value[ index ];

                                                        customAttribute.array[ offset ]   = value.X;
                                                        customAttribute.array[ offset + 1 ] = value.Y;
                                                        customAttribute.array[ offset + 2 ] = value.Z;

                                                        offset += 3;

                                                    }

                                                }

                                            } else if ( customAttribute.size == 4 ) {

                                                for (int ca = 0; ca < cal; ca ++ ) {

                                                    var index = sortArray[ ca ][ 1 ];

                                                    var value = customAttribute.value[ index ];

                                                    customAttribute.array[ offset ]      = value.X;
                                                    customAttribute.array[ offset + 1  ] = value.Y;
                                                    customAttribute.array[ offset + 2  ] = value.Z;
                                                    customAttribute.array[ offset + 3  ] = value.w;

                                                    offset += 4;

                                                }

                                            }

                                        }
                    */
                }

            }
            else
            {

                if (dirtyVertices)
                {
                    for (int v = 0; v < vl; v++)
                    {
                        var vertex = vertices[v];

                        offset = v * 3;

                        vertexArray[offset] = vertex.X;
                        vertexArray[offset + 1] = vertex.Y;
                        vertexArray[offset + 2] = vertex.Z;
                    }

                }

                if (dirtyColors)
                {
                    for (int c = 0; c < cl; c++)
                    {
                        var color = colors[c];

                        offset = c * 3;

                        colorArray[offset] = color.R;
                        colorArray[offset + 1] = color.G;
                        colorArray[offset + 2] = color.B;
                    }

                }

                if (null != customAttributes)
                {

                    for (int i = 0, il = customAttributes.Count; i < il; i++)
                    {
                        var customAttribute = customAttributes[i];

                        if ((bool)customAttribute["needsUpdate"]
                        && (!customAttribute.ContainsKey("boundTo") || (string)customAttribute["boundTo"] == "vertices"))
                        {
                            var cal = ((float[])customAttribute["array"]).Length / (int)customAttribute["size"];

                            var array = (float[])customAttribute["array"];
                            var values = customAttribute["value"];

                            offset = 0;

                            if ((int)customAttribute["size"] == 1)
                            {
                                for (int ca = 0; ca < cal; ca++)
                                {
                                    array[ca] = ((float[])values)[ca];
                                }

                            }
                            else if ((int)customAttribute["size"] == 2)
                            {

                                for (int ca = 0; ca < cal; ca++)
                                {

                                    var value = ((Vector2[])values)[ca];

                                    array[offset] = value.X;
                                    array[offset + 1] = value.Y;

                                    offset += 2;
                                }

                            }
                            else if ((int)customAttribute["size"] == 3)
                            {

                                if ((string)customAttribute["type"] == "C")
                                {

                                    for (int ca = 0; ca < cal; ca++)
                                    {

                                        var value = ((Color[])values)[ca];

                                        array[offset] = value.R;
                                        array[offset + 1] = value.G;
                                        array[offset + 2] = value.B;

                                        offset += 3;
                                    }

                                }
                                else
                                {

                                    for (int ca = 0; ca < cal; ca++)
                                    {

                                        var value = ((Vector3[])values)[ca];

                                        array[offset] = value.X;
                                        array[offset + 1] = value.Y;
                                        array[offset + 2] = value.Z;

                                        offset += 3;
                                    }
                                }
                            }
                            else if ((int)customAttribute["size"] == 4)
                            {

                                for (int ca = 0; ca < cal; ca++)
                                {

                                    var value = ((Vector4[])values)[ca];

                                    array[offset] = value.X;
                                    array[offset + 1] = value.Y;
                                    array[offset + 2] = value.Z;
                                    array[offset + 3] = value.W;

                                    offset += 4;
                                }
                            }
                        }
                    }
                }

            }

            if (dirtyVertices || object3D.sortParticles)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglVertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length), vertexArray, hint);
            }

            if (dirtyColors || object3D.sortParticles)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglColorBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorArray.Length), colorArray, hint);
            }

            if (null != customAttributes)
            {

                for (int i = 0, il = customAttributes.Count; i < il; i++)
                {
                    var customAttribute = customAttributes[i];

                    if ((bool)customAttribute["needsUpdate"] || object3D.sortParticles)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, (int)((Hashtable)customAttribute["buffer"])["id"]);
                        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(((float[])customAttribute["array"]).Length * sizeof(float)), (float[])customAttribute["array"], hint);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="hint"></param>
        private void setLineBuffers(Geometry geometry, BufferUsageHint hint)
        {
            var vertices = geometry.Vertices;
            var colors = geometry.Colors;
            var lineDistances = geometry.LineDistances;

            var vl = vertices.Count;
            var cl = colors.Count;
            var dl = lineDistances.Count;

            var vertexArray = geometry.__vertexArray;
            var colorArray = geometry.__colorArray;
            var lineDistanceArray = geometry.__lineDistanceArray;

            var dirtyVertices = geometry.VerticesNeedUpdate;
            var dirtyColors = geometry.ColorsNeedUpdate;
            var dirtyLineDistances = geometry.LineDistancesNeedUpdate;

            var customAttributes = geometry.__webglCustomAttributesList; // maybe unitialzed

            var offset = 0;

            if (dirtyVertices)
            {

                for (int v = 0; v < vl; v++)
                {

                    var vertex = vertices[v];

                    offset = v * 3;

                    vertexArray[offset] = vertex.X;
                    vertexArray[offset + 1] = vertex.Y;
                    vertexArray[offset + 2] = vertex.Z;

                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglVertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * sizeof(float)), vertexArray, hint);
            }

            if (dirtyColors)
            {

                for (int c = 0; c < cl; c++)
                {

                    var color = colors[c];

                    offset = c * 3;

                    colorArray[offset] = color.R;
                    colorArray[offset + 1] = color.G;
                    colorArray[offset + 2] = color.B;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglColorBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorArray.Length * sizeof(float)), colorArray, hint); // float ??
            }

            if (dirtyLineDistances)
            {

                for (int d = 0; d < dl; d++)
                {

                    lineDistanceArray[d] = lineDistances[d];

                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglLineDistanceBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(lineDistanceArray.Length * sizeof(float)), lineDistanceArray, hint); // float ??

            }

            if (null != customAttributes)
            {

                for (int i = 0, il = customAttributes.Count; i < il; i++)
                {
                    var customAttribute = customAttributes[i];
                    /*
                                        if ( customAttribute.needsUpdate &&
                                             ( customAttribute.boundTo == null ||
                                                 customAttribute.boundTo == "vertices" ) ) {

                                            offset = 0;

                                            var cal = customAttribute.value.length;

                                            if ( customAttribute.size == 1 ) {

                                                for (int ca = 0; ca < cal; ca ++ ) {

                                                    customAttribute.array[ ca ] = customAttribute.value[ ca ];

                                                }

                                            } else if ( customAttribute.size == 2 ) {

                                                for (int ca = 0; ca < cal; ca ++ ) {

                                                    var value = customAttribute.value[ ca ];

                                                    customAttribute.array[ offset ]   = value.X;
                                                    customAttribute.array[ offset + 1 ] = value.Y;

                                                    offset += 2;

                                                }

                                            } else if ( customAttribute.size == 3 ) {

                                                if ( customAttribute.type == "C" ) {

                                                    for (int ca = 0; ca < cal; ca ++ ) {

                                                        var value = customAttribute.value[ ca ];

                                                        customAttribute.array[ offset ]   = value.R;
                                                        customAttribute.array[ offset + 1 ] = value.G;
                                                        customAttribute.array[ offset + 2 ] = value.B;

                                                        offset += 3;

                                                    }

                                                } else {

                                                    for (int ca = 0; ca < cal; ca ++ ) {

                                                        var value = customAttribute.value[ ca ];

                                                        customAttribute.array[ offset ]   = value.X;
                                                        customAttribute.array[ offset + 1 ] = value.Y;
                                                        customAttribute.array[ offset + 2 ] = value.Z;

                                                        offset += 3;

                                                    }

                                                }

                                            } else if ( customAttribute.size == 4 ) {

                                                for (int ca = 0; ca < cal; ca ++ ) {

                                                    var value = customAttribute.value[ ca ];

                                                    customAttribute.array[ offset ]    = value.X;
                                                    customAttribute.array[ offset + 1  ] = value.Y;
                                                    customAttribute.array[ offset + 2  ] = value.Z;
                                                    customAttribute.array[ offset + 3  ] = value.w;

                                                    offset += 4;

                                                }

                                            }

                                            GL.BindBuffer(BufferTarget.ArrayBuffer, customAttribute.buffer);
                                            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(customAttribute.array * ????), customAttribute.array, hint);

                                        }
                                        */
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        private void UpdateObject(Scene scene, Object3D object3D)
        {
            var geometry = object3D.Geometry as Geometry;
            Material material = null;

            if (object3D.Geometry is BufferGeometry)
            {
                SetDirectBuffers(object3D.Geometry as BufferGeometry, BufferUsageHint.DynamicDraw);
            }
            else if (object3D is Mesh)
            {
                // check all geometry groups

                //var geometry = object3D.Geometry as Geometry;
                Debug.Assert(null != geometry, "Casting object3D.Geometry as Geometry");

                // check all geometry groups
                if (geometry.GroupsNeedUpdate)
                {
                    this.InitGeometryGroups(scene, object3D, geometry);
                }

                var geometryGroupsList = this.geometryGroups[geometry.Id];

                foreach (var geometryGroup in geometryGroupsList)
                {
                    material = this.getBufferMaterial(object3D, geometryGroup);

                    if (geometry.GroupsNeedUpdate)
                    {
                        this.InitMeshBuffers(geometryGroup, object3D);
                    }

                    var customAttributesDirty = (material is IAttributes && (null != ((IAttributes)material).Attributes) && AreCustomAttributesDirty(material as IAttributes));

                    if (geometry.VerticesNeedUpdate || geometry.MorphTargetsNeedUpdate || geometry.ElementsNeedUpdate
                    || geometry.UvsNeedUpdate || geometry.NormalsNeedUpdate || geometry.ColorsNeedUpdate
                    || customAttributesDirty)
                    {
                        this.SetMeshBuffers(geometryGroup, object3D, BufferUsageHint.DynamicDraw, !geometry.Dynamic, material);
                    }
                }

                geometry.VerticesNeedUpdate = false;
                geometry.MorphTargetsNeedUpdate = false;
                geometry.ElementsNeedUpdate = false;
                geometry.UvsNeedUpdate = false;
                geometry.NormalsNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;

                if (material is IAttributes && (null != ((IAttributes)material).Attributes))
                    ClearCustomAttributes(material as IAttributes);
            }
            else if (object3D is Line)
            {
                //var geometry = object3D.Geometry as Geometry;

                material = this.getBufferMaterial(object3D, geometry);

                var customAttributesDirty = (material is IAttributes && (null != ((IAttributes)material).Attributes) && AreCustomAttributesDirty(material as IAttributes));

                if (geometry.VerticesNeedUpdate || geometry.ColorsNeedUpdate || geometry.LineDistancesNeedUpdate || customAttributesDirty)
                {
                    this.setLineBuffers(geometry, BufferUsageHint.DynamicDraw);
                }

                geometry.VerticesNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;
                geometry.LineDistancesNeedUpdate = false;

                if (material is IAttributes && (null != ((IAttributes)material).Attributes))
                    ClearCustomAttributes(material as IAttributes);
            }
            else if (object3D is PointCloud)
            {
                var pointCloud = object3D as PointCloud;

                material = this.getBufferMaterial(object3D, geometry);

                var customAttributesDirty = (material is IAttributes && (null != ((IAttributes)material).Attributes) && AreCustomAttributesDirty(material as IAttributes));

                if (geometry.VerticesNeedUpdate || geometry.ColorsNeedUpdate || pointCloud.sortParticles || customAttributesDirty)
                {
                    this.SetParticleBuffers(geometry, BufferUsageHint.DynamicDraw, pointCloud);
                }

                geometry.VerticesNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;

                if (material is IAttributes && (null != ((IAttributes)material).Attributes))
                    ClearCustomAttributes(material as IAttributes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private int filterFallback(int f)
        {
            if (f == Three.NearestFilter || f == Three.NearestMipMapNearestFilter || f == Three.NearestMipMapLinearFilter)
            {
                return (int)TextureMinFilter.Nearest;
            }
            return (int)TextureMinFilter.Linear;
        }

        // Map three.js constants to WebGL constants

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
	    private int paramThreeToGL(int p)
        {

            if (p == Three.RepeatWrapping) return (int)TextureWrapMode.Repeat;
            if (p == Three.ClampToEdgeWrapping) return (int)TextureWrapMode.ClampToEdge;
            if (p == Three.MirroredRepeatWrapping) return (int)TextureWrapMode.MirroredRepeat;

            if (p == Three.NearestFilter) return (int)TextureMinFilter.Nearest;
            if (p == Three.NearestMipMapNearestFilter) return (int)TextureMinFilter.NearestMipmapNearest;
            if (p == Three.NearestMipMapLinearFilter) return (int)TextureMinFilter.NearestMipmapLinear;

            if (p == Three.LinearFilter) return (int)TextureMinFilter.Linear;
            if (p == Three.LinearMipMapNearestFilter) return (int)TextureMinFilter.LinearMipmapNearest;
            if (p == Three.LinearMipMapLinearFilter) return (int)TextureMinFilter.LinearMipmapLinear;

            if (p == Three.UnsignedByteType) return (int)PixelType.UnsignedByte;
            if (p == Three.UnsignedShort4444Type) return (int)PixelType.UnsignedShort4444;
            if (p == Three.UnsignedShort5551Type) return (int)PixelType.UnsignedShort5551;
            if (p == Three.UnsignedShort565Type) return (int)PixelType.UnsignedShort565;

            if (p == Three.ByteType) return (int)PixelType.Byte;
            if (p == Three.ShortType) return (int)PixelType.Short;
            if (p == Three.UnsignedShortType) return (int)PixelType.UnsignedShort;
            if (p == Three.IntType) return (int)PixelType.Int;
            if (p == Three.UnsignedIntType) return (int)PixelType.UnsignedInt;
            if (p == Three.FloatType) return (int)PixelType.Float;

            if (p == Three.AlphaFormat) return (int)PixelInternalFormat.Alpha;
            if (p == Three.RGBFormat) return (int)PixelInternalFormat.Rgb;
            if (p == Three.RGBAFormat) return (int)PixelInternalFormat.Rgba;
            if (p == Three.LuminanceFormat) return (int)PixelInternalFormat.Luminance;
            if (p == Three.LuminanceAlphaFormat) return (int)PixelInternalFormat.LuminanceAlpha;

            if (p == Three.BGRFormat) return (int)OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
            if (p == Three.BGRAFormat) return (int)OpenTK.Graphics.OpenGL.PixelFormat.Bgra;

            if (p == Three.AddEquation) return (int)BlendEquationMode.FuncAdd;
            if (p == Three.SubtractEquation) return (int)BlendEquationMode.FuncSubtract;
            if (p == Three.ReverseSubtractEquation) return (int)BlendEquationMode.FuncReverseSubtract;

            if (p == Three.ZeroFactor) return (int)BlendingFactorSrc.Zero;
            if (p == Three.OneFactor) return (int)BlendingFactorSrc.One;
            if (p == Three.SrcColorFactor) return (int)BlendingFactorSrc.SrcColor;
            if (p == Three.OneMinusSrcColorFactor) return (int)BlendingFactorSrc.OneMinusSrcColor;
            if (p == Three.SrcAlphaFactor) return (int)BlendingFactorSrc.SrcAlpha;
            if (p == Three.OneMinusSrcAlphaFactor) return (int)BlendingFactorSrc.OneMinusSrcAlpha;
            if (p == Three.DstAlphaFactor) return (int)BlendingFactorSrc.DstAlpha;
            if (p == Three.OneMinusDstAlphaFactor) return (int)BlendingFactorSrc.OneMinusDstAlpha;

            if (p == Three.DstColorFactor) return (int)BlendingFactorDest.DstColor;
            if (p == Three.OneMinusDstColorFactor) return (int)BlendingFactorDest.OneMinusDstColor;
            if (p == Three.SrcAlphaSaturateFactor) return (int)BlendingFactorDest.SrcAlphaSaturate;

            if (this.glExtensionCompressedTextureS3TC != null)
            {

                if (p == Three.RGB_S3TC_DXT1_Format) return (int)ExtTextureCompressionS3tc.CompressedRgbS3tcDxt1Ext;
                if (p == Three.RGBA_S3TC_DXT1_Format) return (int)ExtTextureCompressionS3tc.CompressedRgbaS3tcDxt1Ext;
                if (p == Three.RGBA_S3TC_DXT3_Format) return (int)ExtTextureCompressionS3tc.CompressedRgbaS3tcDxt3Ext;
                if (p == Three.RGBA_S3TC_DXT5_Format) return (int)ExtTextureCompressionS3tc.CompressedRgbaS3tcDxt5Ext;

            }

            return 0;

        }

        #endregion

        private struct LightCountInfo
        {
            #region Fields

            public int directional;

            public int hemi;

            public int point;

            public int spot;

            #endregion
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

                    // TODO
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