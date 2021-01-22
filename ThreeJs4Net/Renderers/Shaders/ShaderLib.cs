using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Renderers.Shaders
{
    public sealed class ShaderLib : Hashtable
    {
        public readonly UniformsLib UniformsLib = new UniformsLib();

        private readonly Dictionary<string, string> glslCache = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public ShaderLib()
        {
            // cache files in Dictionary
            var glslFiles = Directory.EnumerateFiles(@".\Renderers\shaders\ShaderChunk", "*.glsl");
            if (glslFiles.Count() <= 0)
            {
                throw new FileNotFoundException(".glsl files not found - check the path in ShaderLib.cs, line 25");
            }

            foreach (var path in glslFiles)
                this.glslCache.Add(Path.GetFileNameWithoutExtension(path), path);

            this.Add("basic", this.BasicShader());
            this.Add("lambert", this.LambertShader());
            this.Add("phong", this.PhongShader());
            this.Add("particle_basic", this.ParticleBasicShader());
            //this.Add("dashed", DashedShader());
            //this.Add("depth", DepthShader());
            this.Add("normal", this.NormalShader());
            //this.Add("normalmap", NormalMapShader());
            //this.Add("cube", CubeShader());
            //this.Add("depthRGBA", DepthRGBAShader());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string getChunk(string chunkName)
        {
            string path;
            return this.glslCache.TryGetValue(chunkName, out path) ? File.ReadAllText(path) : string.Empty;
        }

        /// <summary>
        /// basic
        /// </summary>
        private Shader BasicShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms = UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["common"],
                                      this.UniformsLib["fog"],
                                      this.UniformsLib["shadowmap"]
                                  });
            #endregion

            #region construct vertexShader source

            {
                var vs = new List<string>();

                vs.Add(this.getChunk("map_pars_vertex"));
                vs.Add(this.getChunk("lightmap_pars_vertex"));
                vs.Add(this.getChunk("envmap_pars_vertex"));
                vs.Add(this.getChunk("color_pars_vertex"));
                vs.Add(this.getChunk("morphtarget_pars_vertex"));
                vs.Add(this.getChunk("skinning_pars_vertex"));
                vs.Add(this.getChunk("shadowmap_pars_vertex"));
                vs.Add(this.getChunk("logdepthbuf_pars_vertex"));

                vs.Add("void main() {");

                vs.Add(this.getChunk("map_vertex"));
                vs.Add(this.getChunk("lightmap_vertex"));
                vs.Add(this.getChunk("color_vertex"));
                vs.Add(this.getChunk("skinbase_vertex"));

                vs.Add("    #ifdef USE_ENVMAP");

                vs.Add(this.getChunk("morphnormal_vertex"));
                vs.Add(this.getChunk("skinnormal_vertex"));
                vs.Add(this.getChunk("defaultnormal_vertex"));

                vs.Add("    #endif");

                vs.Add(this.getChunk("morphtarget_vertex"));
                vs.Add(this.getChunk("skinning_vertex"));
                vs.Add(this.getChunk("default_vertex"));
                vs.Add(this.getChunk("logdepthbuf_vertex"));
           
                vs.Add(this.getChunk("worldpos_vertex"));
                vs.Add(this.getChunk("envmap_vertex"));
                vs.Add(this.getChunk("shadowmap_vertex"));

                vs.Add("}");

                // join
                shader.VertexShader = String.Join("\n", vs).Trim();
            }

            #endregion

            #region fragmentShader

            {
                var fs = new List<string>();

                fs.Add("uniform vec3 diffuse;");
                fs.Add("uniform float opacity;");

                fs.Add(this.getChunk("color_pars_fragment"));
                fs.Add(this.getChunk("map_pars_fragment"));
                fs.Add(this.getChunk("alphamap_pars_fragment"));
                fs.Add(this.getChunk("lightmap_pars_fragment"));
                fs.Add(this.getChunk("envmap_pars_fragment"));
                fs.Add(this.getChunk("fog_pars_fragment"));
                fs.Add(this.getChunk("shadowmap_pars_fragment"));
                fs.Add(this.getChunk("specularmap_pars_fragment"));
                fs.Add(this.getChunk("logdepthbuf_pars_fragment"));

                fs.Add("void main() {");

                fs.Add("	gl_FragColor = vec4( diffuse, opacity );");

                fs.Add(this.getChunk("logdepthbuf_fragment"));
                fs.Add(this.getChunk("map_fragment"));
                fs.Add(this.getChunk("alphamap_fragment"));
                fs.Add(this.getChunk("alphatest_fragment"));
                fs.Add(this.getChunk("specularmap_fragment"));
                fs.Add(this.getChunk("lightmap_fragment"));
                fs.Add(this.getChunk("color_fragment"));
                fs.Add(this.getChunk("envmap_fragment"));
                fs.Add(this.getChunk("shadowmap_fragment"));

                fs.Add(this.getChunk("linear_to_gamma_fragment"));
         
                fs.Add(this.getChunk("fog_fragment"));

                fs.Add("}");

                // join
                shader.FragmentShader = String.Join("\n", fs).Trim();
            }

            #endregion

            return shader;
        }

        /// <summary>
        /// lambert
        /// </summary>
        private Shader LambertShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                    this.UniformsLib["common"],
                                    this.UniformsLib["fog"],
                                    this.UniformsLib["lights"],
                                    this.UniformsLib["shadowmap"],

                                    new Uniforms { { "ambient",  new Uniform() { {"type", "c"},  {"value", Color.White} } }},
                                    new Uniforms { { "emissive", new Uniform() { {"type", "c"},  {"value", Color.Black} } }},
                                    new Uniforms { { "wrapRGB",  new Uniform() { {"type", "v3"}, {"value", new Vector3(1,1,1)} }}}});
            #endregion

            #region construct VertexShader source
            var vs = new List<string>();

            vs.Add("#define LAMBERT");

            vs.Add("varying vec3 vLightFront;");

            vs.Add("#ifdef DOUBLE_SIDED");

            vs.Add("    varying vec3 vLightBack;");

            vs.Add("#endif");

            vs.Add(this.getChunk("map_pars_vertex"));
            vs.Add(this.getChunk("lightmap_pars_vertex"));
            vs.Add(this.getChunk("envmap_pars_vertex"));
            vs.Add(this.getChunk("lights_lambert_pars_vertex"));
            vs.Add(this.getChunk("color_pars_vertex"));
            vs.Add(this.getChunk("morphtarget_pars_vertex"));
            vs.Add(this.getChunk("skinning_pars_vertex"));
            vs.Add(this.getChunk("shadowmap_pars_vertex"));
            vs.Add(this.getChunk("logdepthbuf_pars_vertex"));

            vs.Add("void main() {");

                vs.Add(this.getChunk("map_vertex"));
                vs.Add(this.getChunk("lightmap_vertex"));
                vs.Add(this.getChunk("color_vertex"));
                vs.Add(this.getChunk("morphnormal_vertex"));
                vs.Add(this.getChunk("skinbase_vertex"));
                vs.Add(this.getChunk("skinnormal_vertex"));
                vs.Add(this.getChunk("defaultnormal_vertex"));
                vs.Add(this.getChunk("morphtarget_vertex"));
                vs.Add(this.getChunk("skinning_vertex"));
                vs.Add(this.getChunk("default_vertex"));
                vs.Add(this.getChunk("logdepthbuf_vertex"));
                vs.Add(this.getChunk("worldpos_vertex"));
                vs.Add(this.getChunk("envmap_vertex"));
                vs.Add(this.getChunk("lights_lambert_vertex"));
                vs.Add(this.getChunk("shadowmap_vertex"));

            vs.Add("}");

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region construct fragmentShader

            var fs = new List<string>();

            fs.Add("uniform float opacity;");

            fs.Add("varying vec3 vLightFront;");

            fs.Add("#ifdef DOUBLE_SIDED");

			fs.Add("	varying vec3 vLightBack;");

			fs.Add("#endif");

            fs.Add(this.getChunk("color_pars_fragment"));
			fs.Add(this.getChunk("map_pars_fragment"));
			fs.Add(this.getChunk("alphamap_pars_fragment"));
			fs.Add(this.getChunk("lightmap_pars_fragment"));
			fs.Add(this.getChunk("envmap_pars_fragment"));
			fs.Add(this.getChunk("fog_pars_fragment"));
			fs.Add(this.getChunk("shadowmap_pars_fragment"));
			fs.Add(this.getChunk("specularmap_pars_fragment"));
			fs.Add(this.getChunk("logdepthbuf_pars_fragment"));

            fs.Add("void main() {");

                fs.Add("	gl_FragColor = vec4( vec3( 1.0 ), opacity );");

                fs.Add(this.getChunk("logdepthbuf_fragment"));
			    fs.Add(this.getChunk("map_fragment"));
			    fs.Add(this.getChunk("alphamap_fragment"));
			    fs.Add(this.getChunk("alphatest_fragment"));
			    fs.Add(this.getChunk("specularmap_fragment"));

                fs.Add("	#ifdef DOUBLE_SIDED");

                //"float isFront = float( gl_FrontFacing );",
                //"gl_FragColor.xyz *= isFront * vLightFront + ( 1.0 - isFront ) * vLightBack;",

                fs.Add("		if ( gl_FrontFacing )");
                fs.Add("			gl_FragColor.xyz *= vLightFront;");
                fs.Add("		else");
                fs.Add("			gl_FragColor.xyz *= vLightBack;");

                fs.Add("	#else");

                fs.Add("		gl_FragColor.xyz *= vLightFront;");

                fs.Add("	#endif");

			    fs.Add(this.getChunk("lightmap_fragment"));
			    fs.Add(this.getChunk("color_fragment"));
			    fs.Add(this.getChunk("envmap_fragment"));
			    fs.Add(this.getChunk("shadowmap_fragment"));

			    fs.Add(this.getChunk("linear_to_gamma_fragment"));
			
                fs.Add(this.getChunk("fog_fragment"));

            fs.Add("}");


            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader PhongShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            var tt = new Uniforms { { "ambient", new Uniform() { {"type", "c"}, {"value", Color.White } }} };

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["common"],
                                      this.UniformsLib["bump"],
                                      this.UniformsLib["normalmap"],
                                      this.UniformsLib["fog"],
                                      this.UniformsLib["lights"],
                                      this.UniformsLib["shadowmap"],

                                      new Uniforms { { "ambient",   new Uniform() { {"type", "c"},  {"value", Color.White} } }},
                                      new Uniforms { { "emissive",  new Uniform() { {"type", "c"},  {"value", Color.Black} } }},
                                      new Uniforms { { "specular",  new Uniform() { {"type", "c"},  {"value", Color.DimGray} } }},
                                      new Uniforms { { "shininess", new Uniform() { {"type", "f"},  {"value", 30.0f} }} },
                                      new Uniforms { { "wrapRGB",   new Uniform() { {"type", "v3"}, {"value", new Vector3(1,1,1)} }}}});

            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add("#define PHONG");

            vs.Add("varying vec3 vViewPosition;");
            vs.Add("varying vec3 vNormal;");

            vs.Add(this.getChunk("map_pars_vertex"));
            vs.Add(this.getChunk("lightmap_pars_vertex"));
            vs.Add(this.getChunk("envmap_pars_vertex"));
            vs.Add(this.getChunk("lights_phong_pars_vertex"));
            vs.Add(this.getChunk("color_pars_vertex"));
            vs.Add(this.getChunk("morphtarget_pars_vertex"));
            vs.Add(this.getChunk("skinning_pars_vertex"));
            vs.Add(this.getChunk("shadowmap_pars_vertex"));
            vs.Add(this.getChunk("logdepthbuf_pars_vertex"));

            vs.Add("void main() {");

            vs.Add(this.getChunk("map_vertex"));
            vs.Add(this.getChunk("lightmap_vertex"));
            vs.Add(this.getChunk("color_vertex"));

            vs.Add(this.getChunk("morphnormal_vertex"));
            vs.Add(this.getChunk("skinbase_vertex"));
            vs.Add(this.getChunk("skinnormal_vertex"));
            vs.Add(this.getChunk("defaultnormal_vertex"));

            vs.Add("	vNormal = normalize( transformedNormal );");

            vs.Add(this.getChunk("morphtarget_vertex"));
            vs.Add(this.getChunk("skinning_vertex"));
            vs.Add(this.getChunk("default_vertex"));
            vs.Add(this.getChunk("logdepthbuf_vertex"));

            vs.Add("	vViewPosition = -mvPosition.xyz;");

            vs.Add(this.getChunk("worldpos_vertex"));
            vs.Add(this.getChunk("envmap_vertex"));
            vs.Add(this.getChunk("lights_phong_vertex"));
            vs.Add(this.getChunk("shadowmap_vertex"));

            vs.Add("}");

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();

            fs.Add("uniform vec3 diffuse;");
            fs.Add("uniform float opacity;");

            fs.Add("uniform vec3 ambient;");
            fs.Add("uniform vec3 emissive;");
            fs.Add("uniform vec3 specular;");
            fs.Add("uniform float shininess;");

            fs.Add(this.getChunk("color_pars_fragment"));
            fs.Add(this.getChunk("map_pars_fragment"));
            fs.Add(this.getChunk("alphamap_pars_fragment"));
            fs.Add(this.getChunk("lightmap_pars_fragment"));
            fs.Add(this.getChunk("envmap_pars_fragment"));
            fs.Add(this.getChunk("fog_pars_fragment"));
            fs.Add(this.getChunk("lights_phong_pars_fragment"));
            fs.Add(this.getChunk("shadowmap_pars_fragment"));
            fs.Add(this.getChunk("bumpmap_pars_fragment"));
            fs.Add(this.getChunk("normalmap_pars_fragment"));
            fs.Add(this.getChunk("specularmap_pars_fragment"));
            fs.Add(this.getChunk("logdepthbuf_pars_fragment"));

            fs.Add("void main() {");

            fs.Add("	gl_FragColor = vec4( vec3( 1.0 ), opacity );");

            fs.Add(this.getChunk("logdepthbuf_fragment"));
            fs.Add(this.getChunk("map_fragment"));
            fs.Add(this.getChunk("alphamap_fragment"));
            fs.Add(this.getChunk("alphatest_fragment"));
            fs.Add(this.getChunk("specularmap_fragment"));

            fs.Add(this.getChunk("lights_phong_fragment"));

            fs.Add(this.getChunk("lightmap_fragment"));
            fs.Add(this.getChunk("color_fragment"));
            fs.Add(this.getChunk("envmap_fragment"));
            fs.Add(this.getChunk("shadowmap_fragment"));

            fs.Add(this.getChunk("linear_to_gamma_fragment"));

            fs.Add(this.getChunk("fog_fragment"));

            fs.Add("}");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader ParticleBasicShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["particle"],
                                      this.UniformsLib["shadowmap"],
                                  });
            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add("uniform float size;");
            vs.Add("uniform float scale;");

            vs.Add(this.getChunk("color_pars_vertex"));
            vs.Add(this.getChunk("shadowmap_pars_vertex"));
            vs.Add(this.getChunk("logdepthbuf_pars_vertex"));

            vs.Add("void main() {");

                vs.Add(this.getChunk("color_vertex"));

                vs.Add("	vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );");

                vs.Add("	#ifdef USE_SIZEATTENUATION");
                vs.Add("		gl_PointSize = size * ( scale / length( mvPosition.xyz ) );");
                vs.Add("	#else");
                vs.Add("		gl_PointSize = size;");
                vs.Add("	#endif");

                vs.Add("	gl_Position = projectionMatrix * mvPosition;");

                vs.Add(this.getChunk("logdepthbuf_vertex"));
                vs.Add(this.getChunk("worldpos_vertex"));
                vs.Add(this.getChunk("shadowmap_vertex"));

            vs.Add("}");

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();

            fs.Add("uniform vec3 psColor;");
            fs.Add("uniform float opacity;");

            fs.Add(this.getChunk("color_pars_fragment"));
            fs.Add(this.getChunk("map_particle_pars_fragment"));
            fs.Add(this.getChunk("fog_pars_fragment"));
            fs.Add(this.getChunk("shadowmap_pars_fragment"));
            fs.Add(this.getChunk("logdepthbuf_pars_fragment"));

            fs.Add("void main() {");

                fs.Add("	gl_FragColor = vec4( psColor, opacity );");

                fs.Add(this.getChunk("logdepthbuf_fragment"));
                fs.Add(this.getChunk("map_particle_fragment"));
                fs.Add(this.getChunk("alphatest_fragment"));
                fs.Add(this.getChunk("color_fragment"));
                fs.Add(this.getChunk("shadowmap_fragment"));
                fs.Add(this.getChunk("fog_fragment"));

            fs.Add("}");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader DashedShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(
                    new List<Uniforms>
                        {
                            this.UniformsLib["xxx"],
                            this.UniformsLib["xxx"],
                            this.UniformsLib["xxx"]
                        });

            #endregion

            #region construct vertexShader source

            var vs = new List<string>();

            vs.Add(this.getChunk("xxx"));
            vs.Add(this.getChunk("xxx"));

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();
            fs.Add("uniform vec3 diffuse;");
            fs.Add("uniform float opacity;");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader DepthShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"]
                                  });
            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add(this.getChunk("xxx"));
            vs.Add(this.getChunk("xxx"));

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();
            fs.Add("uniform vec3 diffuse;");
            fs.Add("uniform float opacity;");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader NormalShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      new Uniforms { { "opacity", new Uniform() { {"type", "f"},  {"value", 1.0f} } }},
                                  });
            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add("varying vec3 vNormal;");

            vs.Add(this.getChunk("morphtarget_pars_vertex"));
            vs.Add(this.getChunk("logdepthbuf_pars_vertex"));

            vs.Add("void main() {");

            vs.Add("    vNormal = normalize( normalMatrix * normal );");

                vs.Add(this.getChunk("morphtarget_vertex"));
                vs.Add(this.getChunk("default_vertex"));
                vs.Add(this.getChunk("logdepthbuf_vertex"));

            vs.Add("}");

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();

            fs.Add("uniform float opacity;");
            fs.Add("varying vec3 vNormal;");

            fs.Add(this.getChunk("logdepthbuf_pars_fragment"));

            fs.Add("void main() {");

            fs.Add("    gl_FragColor = vec4( 0.5 * normalize( vNormal ) + 0.5, opacity );");
            fs.Add(this.getChunk("logdepthbuf_fragment"));
       
            fs.Add("}");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader NormalMapShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"]
                                  });
            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add(this.getChunk("xxx"));
            vs.Add(this.getChunk("xxx"));

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();
            fs.Add("uniform vec3 diffuse;");
            fs.Add("uniform float opacity;");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader CubeShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"]
                                  });
            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add(this.getChunk("xxx"));
            vs.Add(this.getChunk("xxx"));

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();
            fs.Add("uniform vec3 diffuse;");
            fs.Add("uniform float opacity;");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        private Shader DepthRGBAShader()
        {
            var shader = new Shader();

            #region construct uniform variables

            shader.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                                  {
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"],
                                      this.UniformsLib["xxx"]
                                  });
            #endregion

            #region construct vertexShader source
            var vs = new List<string>();

            vs.Add(this.getChunk("xxx"));
            vs.Add(this.getChunk("xxx"));

            // join
            shader.VertexShader = String.Join("\n", vs).Trim();

            #endregion

            #region fragmentShader

            var fs = new List<string>();
            fs.Add("uniform vec3 diffuse;");
            fs.Add("uniform float opacity;");

            // join
            shader.FragmentShader = String.Join("\n", fs).Trim();

            #endregion

            return shader;
        }

    }

}
