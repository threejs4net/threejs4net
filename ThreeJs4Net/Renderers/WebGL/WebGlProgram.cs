using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Materials;

namespace ThreeJs4Net.Renderers.WebGL
{
    public class WebGlProgram
    {
        protected static int ProgramIdCount;

        public int Id = ProgramIdCount++;

        public string Code;

        public int UsedTimes = 1;

        public int Program;
		
        public int VertexShader;
		
        public int FragmentShader;

        public Hashtable Attributes = new Hashtable();

        public List<string> AttributesKeys = new List<string>();

        public Hashtable Uniforms = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defines"></param>
        private static string GenerateDefines(Hashtable defines)
        {
            if (defines.Count <= 0) 
                return string.Empty;

            var chunks = new List<string>();

		    foreach ( DictionaryEntry entry in defines ) {

                if ((bool)entry.Value == false)
                    continue;

                var chunk = string.Format("#define {0} {1}", entry.Key, entry.Value);
                chunks.Add(chunk);

		    }

            return string.Join("", chunks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pgmid"></param>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        private static Hashtable CacheUniformLocations(int pgmid, IEnumerable<string> identifiers) 
        {
            var uniformsLocation = new Hashtable();

            try
            {
                foreach (var identifier in identifiers)
                {
                    var location = GL.GetUniformLocation(pgmid, identifier);
                    if (location >= 0)
                        uniformsLocation[identifier] = location;
                    else 
                        uniformsLocation[identifier] = null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message);
            }

            return uniformsLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pgmid"></param>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        private static Hashtable CacheAttributeLocations(int pgmid, IEnumerable<string> identifiers) 
        {
            var attributes = new Hashtable();
            try
            {
                foreach (var identifier in identifiers)
                {
                    var location = GL.GetAttribLocation(pgmid, identifier);
                    if (location >= 0)
                        attributes[identifier] = location;
                    else
                        attributes[identifier] = null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message);
            }

            return attributes;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="code"></param>
        /// <param name="material"></param>
        /// <param name="parameters"></param>
        public WebGlProgram(WebGLRenderer renderer, string code, Material material, Hashtable parameters)
        {
            Debug.Assert(null != material);
            Debug.Assert(null != material.Defines);
            Debug.Assert(null != material["__webglShader"]);
            Debug.Assert(null != ((WebGlShader)material["__webglShader"]).Uniforms);
            Debug.Assert(null != ((WebGlShader)material["__webglShader"]).VertexShader);
            Debug.Assert(null != ((WebGlShader)material["__webglShader"]).FragmentShader);

            var _this = renderer;

            var defines = material.Defines;
            var uniforms = ((WebGlShader)material["__webglShader"]).Uniforms;

            var vertexShader = ((WebGlShader)material["__webglShader"]).VertexShader;
            var fragmentShader = ((WebGlShader)material["__webglShader"]).FragmentShader;

            string index0AttributeName = null;
            if (null != material as ShaderMaterial)
            {
                index0AttributeName = ((ShaderMaterial)material).Index0AttributeName;
                if ( index0AttributeName == null && (bool)parameters["morphTargets"] == true ) 
                {
                    // programs with morphTargets displace position out of attribute 0
                    index0AttributeName = "position";
                }
            }

            var shadowMapTypeDefine = "SHADOWMAP_TYPE_BASIC";
            if ((int)parameters["shadowMapType"] == Three.PCFShadowMap)
            {
                shadowMapTypeDefine = "SHADOWMAP_TYPE_PCF";
            }
            else if ((int)parameters["shadowMapType"] == Three.PCFSoftShadowMap)
            {
                shadowMapTypeDefine = "SHADOWMAP_TYPE_PCF_SOFT";
            }

            // Trace.TraceInformation( "building new program " );

            var customDefines = GenerateDefines(defines);

            var program = GL.CreateProgram();

            var prefix_vertex = string.Empty;
            var prefix_fragment = string.Empty;

            if ( material is RawShaderMaterial ) 
            {
			    prefix_vertex = string.Empty;
			    prefix_fragment = string.Empty;
		    } 
            else
            {
                #region Prefix Vertex
                var pv = new List<string>();

                pv.Add("#version 120");

                pv.Add("#ifdef GL_ES");
                pv.Add("precision " + parameters["precision"] + " float;");
                pv.Add("precision " + parameters["precision"] + " int;");
                pv.Add("#endif");

                pv.Add(customDefines);

                pv.Add(parameters["supportsVertexTextures"] != null ? "#define VERTEX_TEXTURES" : "");

                pv.Add(_this.gammaInput ? "#define GAMMA_INPUT" : "");
                pv.Add(_this.gammaOutput ? "#define GAMMA_OUTPUT" : "");

				pv.Add("#define MAX_DIR_LIGHTS " + parameters["maxDirLights"]);
				pv.Add("#define MAX_POINT_LIGHTS " + parameters["maxPointLights"]);
				pv.Add("#define MAX_SPOT_LIGHTS " + parameters["maxSpotLights"]);
				pv.Add("#define MAX_HEMI_LIGHTS " + parameters["maxHemiLights"]);

				pv.Add("#define MAX_SHADOWS " + parameters["maxShadows"]);

				pv.Add("#define MAX_BONES " + parameters["maxBones"]); // zou 58 moeten zijn

                pv.Add(parameters["map"] != null && (bool)parameters["map"]  ? "#define USE_MAP" : "");
                pv.Add(parameters["envMap"] != null && (bool)parameters["envMap"] ? "#define USE_ENVMAP" : "");
                pv.Add(parameters["lightMap"] != null && (bool)parameters["lightMap"] ? "#define USE_LIGHTMAP" : "");
                pv.Add(parameters["bumpMap"] != null && (bool)parameters["bumpMap"] ? "#define USE_BUMPMAP" : "");
                pv.Add(parameters["normalMap"] != null && (bool)parameters["normalMap"] ? "#define USE_NORMALMAP" : "");
                pv.Add(parameters["specularMap"] != null && (bool)parameters["specularMap"] ? "#define USE_SPECULARMAP" : "");
                pv.Add(parameters["alphaMap"] != null && (bool)parameters["alphaMap"] ? "#define USE_ALPHAMAP" : "");
                pv.Add(parameters["vertexColors"] != null && (int)parameters["vertexColors"] > 0 ? "#define USE_COLOR" : "");

                pv.Add(parameters["skinning"] != null && (bool)parameters["skinning"] ? "#define USE_SKINNING" : "");
                pv.Add(parameters["useVertexTexture"] != null && (bool)parameters["useVertexTexture"] ? "#define BONE_TEXTURE" : "");

                pv.Add(parameters["morphTargets"] != null && (bool)parameters["morphTargets"] ? "#define USE_MORPHTARGETS" : "");
                pv.Add(parameters["morphNormals"] != null && (bool)parameters["morphNormals"] ? "#define USE_MORPHNORMALS" : "");
                pv.Add(parameters["wrapAround"] != null && (bool)parameters["wrapAround"] ? "#define WRAP_AROUND" : "");
                pv.Add(parameters["doubleSided"] != null && (bool)parameters["doubleSided"] ? "#define DOUBLE_SIDED" : "");
                pv.Add(parameters["flipSided"] != null && (bool)parameters["flipSided"] ? "#define FLIP_SIDED" : "");

                pv.Add(parameters["shadowMapEnabled"] != null && (bool)parameters["shadowMapEnabled"] ? "#define USE_SHADOWMAP" : "");
                pv.Add(parameters["shadowMapEnabled"] != null && (bool)parameters["shadowMapEnabled"] ? "#define " + shadowMapTypeDefine : "");
                pv.Add(parameters["shadowMapDebug"] != null && (bool)parameters["shadowMapDebug"] ? "#define SHADOWMAP_DEBUG" : "");
                pv.Add(parameters["shadowMapCascade"] != null && (bool)parameters["shadowMapCascade"] ? "#define SHADOWMAP_CASCADE" : "");

                pv.Add(parameters["sizeAttenuation"] != null && (bool)parameters["sizeAttenuation"] ? "#define USE_SIZEATTENUATION" : "");

                pv.Add(parameters["logarithmicDepthBuffer"] != null && (bool)parameters["logarithmicDepthBuffer"] ? "#define USE_LOGDEPTHBUF" : "");
		//		pv.Add(//_this._glExtensionFragDepth ? "#define USE_LOGDEPTHBUF_EXT" : "");

                pv.Add("uniform mat4 modelMatrix;");
				pv.Add("uniform mat4 modelViewMatrix;");
				pv.Add("uniform mat4 projectionMatrix;");
				pv.Add("uniform mat4 viewMatrix;");
				pv.Add("uniform mat3 normalMatrix;");
				pv.Add("uniform vec3 cameraPosition;");

                pv.Add("attribute vec3 position;");
				pv.Add("attribute vec3 normal;");
				pv.Add("attribute vec2 uv;");
				pv.Add("attribute vec2 uv2;");

                pv.Add("#ifdef USE_COLOR");

                pv.Add("    attribute vec3 Color;");

                pv.Add("#endif");

                pv.Add("#ifdef USE_MORPHTARGETS");

                pv.Add("     attribute vec3 morphTarget0;");
				pv.Add("     attribute vec3 morphTarget1;");
				pv.Add("     attribute vec3 morphTarget2;");
				pv.Add("     attribute vec3 morphTarget3;");
                         
                pv.Add("    #ifdef USE_MORPHNORMALS");
                         
                pv.Add("        attribute vec3 morphNormal0;");
				pv.Add("        attribute vec3 morphNormal1;");
				pv.Add("        attribute vec3 morphNormal2;");
				pv.Add("        attribute vec3 morphNormal3;");
                         
                pv.Add("    #else");
                         
                pv.Add("          attribute vec3 morphTarget4;");
				pv.Add("          attribute vec3 morphTarget5;");
				pv.Add("          attribute vec3 morphTarget6;");
				pv.Add("          attribute vec3 morphTarget7;");
                         
                pv.Add("    #endif");
                         
                pv.Add("#endif");

                pv.Add("#ifdef USE_SKINNING");

                pv.Add("    attribute vec4 skinIndex;");
				pv.Add("    attribute vec4 skinWeight;");

                pv.Add("#endif");

                pv.Add("");

                prefix_vertex = string.Join("\n", pv);

                #endregion

                #region Prefix Fragment

                var pf = new List<string>();

                pf.Add("#version 120");
                
                pf.Add("#ifdef GL_ES");
                pf.Add("precision " + parameters["precision"] + " float;");
                pf.Add("precision " + parameters["precision"] + " int;");
                pf.Add("#endif");

                pf.Add(( parameters["bumpMap"] != null || parameters["normalMap"] != null ) ? "#extension GL_OES_standard_derivatives : enable" : "");

                pf.Add(customDefines);

                pf.Add("#define MAX_DIR_LIGHTS " + parameters["maxDirLights"]);
				pf.Add("#define MAX_POINT_LIGHTS " + parameters["maxPointLights"]);
				pf.Add("#define MAX_SPOT_LIGHTS " + parameters["maxSpotLights"]);
				pf.Add("#define MAX_HEMI_LIGHTS " + parameters["maxHemiLights"]);

                pf.Add("#define MAX_SHADOWS " + parameters["maxShadows"]);

                pf.Add(parameters["alphaTest"] != null && ((float)parameters["alphaTest"] > 0) ? "#define ALPHATEST " + ((float)parameters["alphaTest"]).ToString(CultureInfo.InvariantCulture) : ""); // TEVEEL

                pf.Add(_this.gammaInput ? "#define GAMMA_INPUT" : "");
                pf.Add(_this.gammaOutput ? "#define GAMMA_OUTPUT" : "");

                pf.Add((parameters["useFog"] != null && (bool)parameters["useFog"] && parameters["fog"] != null) ? "#define USE_FOG" : ""); // TEVEEL
                pf.Add((parameters["useFog"] != null && (bool)parameters["useFog"] && parameters["fogExp"] != null) ? "#define FOG_EXP2" : ""); // TEVEEL

                pf.Add(parameters["map"] != null && (bool)parameters["map"] ? "#define USE_MAP" : "");
                pf.Add(parameters["envMap"] != null && (bool)parameters["envMap"] ? "#define USE_ENVMAP" : "");
                pf.Add(parameters["lightMap"] != null && (bool)parameters["lightMap"] ? "#define USE_LIGHTMAP" : "");
                pf.Add(parameters["bumpMap"] != null && (bool)parameters["bumpMap"] ? "#define USE_BUMPMAP" : "");
                pf.Add(parameters["normalMap"] != null && (bool)parameters["normalMap"] ? "#define USE_NORMALMAP" : "");
                pf.Add(parameters["specularMap"] != null && (bool)parameters["specularMap"] ? "#define USE_SPECULARMAP" : "");
                pf.Add(parameters["alphaMap"] != null && (bool)parameters["alphaMap"] ? "#define USE_ALPHAMAP" : "");
                pf.Add(parameters["vertexColors"] != null && (int)parameters["vertexColors"] > 0 ? "#define USE_COLOR" : "");

                pf.Add(parameters["metal"] != null && (bool)parameters["metal"] ? "#define METAL" : "");
                pf.Add(parameters["wrapAround"] != null && (bool)parameters["wrapAround"] ? "#define WRAP_AROUND" : "");
                pf.Add(parameters["doubleSided"] != null && (bool)parameters["doubleSided"] ? "#define DOUBLE_SIDED" : "");
                pf.Add(parameters["flipSided"] != null && (bool)parameters["flipSided"] ? "#define FLIP_SIDED" : "");

                pf.Add(parameters["shadowMapEnabled"] != null && (bool)parameters["shadowMapEnabled"] ? "#define USE_SHADOWMAP" : "");
                pf.Add(parameters["shadowMapEnabled"] != null && (bool)parameters["shadowMapEnabled"] ? "#define " + shadowMapTypeDefine : "");
                pf.Add(parameters["shadowMapDebug"] != null && (bool)parameters["shadowMapDebug"] ? "#define SHADOWMAP_DEBUG" : "");
                pf.Add(parameters["shadowMapCascade"] != null && (bool)parameters["shadowMapCascade"] ? "#define SHADOWMAP_CASCADE" : "");

                pf.Add(parameters["logarithmicDepthBuffer"] != null && (bool)parameters["logarithmicDepthBuffer"] ? "#define USE_LOGDEPTHBUF" : "");

				//pf.Add(//_this._glExtensionFragDepth ? "#define USE_LOGDEPTHBUF_EXT" : "");

				pf.Add("uniform mat4 viewMatrix;");
				pf.Add("uniform vec3 cameraPosition;");
                pf.Add("");

                prefix_fragment = string.Join("\n", pf);

                #endregion
            }

            try
            {
                var glVertexShader = WebGlShader.BuildShader(ShaderType.VertexShader, prefix_vertex + vertexShader);
                var glFragmentShader = WebGlShader.BuildShader(ShaderType.FragmentShader, prefix_fragment + fragmentShader);

                GL.AttachShader(program, glVertexShader);
                GL.AttachShader(program, glFragmentShader);

                if (index0AttributeName != null)
                {
                    // Force A particular attribute to index 0.
                    // because potentially expensive emulation is done by browser if attribute 0 is disabled.
                    // And, Color, for example is often automatically bound to index 0 so disabling it
                    GL.BindAttribLocation(program, 0, index0AttributeName);
                }

                GL.LinkProgram(program);

                var linkStatus = -1;
                GL.GetProgram(program, ProgramParameter.LinkStatus, out linkStatus);
                if (linkStatus == 0)
                {
                    int validateStatus;
                    GL.GetProgram(program, ProgramParameter.ValidateStatus, out validateStatus);

                    Trace.TraceError("THREE.WebGLProgram: Could not initialise shader.");
                    Trace.TraceError("gl.VALIDATE_STATUS {0}", validateStatus);
                    Trace.TraceError("gl.getError() {0}", GL.GetError());
                }

                if (GL.GetProgramInfoLog(program) != string.Empty)
                {
                    Trace.TraceError("THREE.WebGLProgram: gl.getProgramInfoLog() {0}", GL.GetProgramInfoLog(program));

                    throw new ApplicationException("Program failed to link - see Output console. When do the .glsl files loaded???");
                }
                else
                {
                    Console.WriteLine("\nProgram {0} linked OK", program);
                }
                // clean up

                GL.DeleteShader(glVertexShader);
                GL.DeleteShader(glFragmentShader);

                // cache uniform locations

                var identifiers = new List<string> {
                        "viewMatrix", "modelViewMatrix", "projectionMatrix", "normalMatrix", "modelMatrix", "cameraPosition",
                        "morphTargetInfluences", "bindMatrix", "bindMatrixInverse"
                };

                if (parameters["useVertexTexture"] != null)
                {
                    identifiers.Add("boneTexture");
                    identifiers.Add("boneTextureWidth");
                    identifiers.Add("boneTextureHeight");
                }
                else
                {
                    identifiers.Add("boneGlobalMatrices");
                }

                if (parameters["logarithmicDepthBuffer"] != null)
                {
                    identifiers.Add("logDepthBufFC");
                }

                foreach (var u in uniforms)
                {
                    identifiers.Add(u.Key);
                }

                this.Uniforms = CacheUniformLocations(program, identifiers);

#if DEBUG

                Console.WriteLine("\nUniform Locations for Program {0}", program);
                foreach (DictionaryEntry entry in this.Uniforms)
                {
                    if (null != entry.Value)
                        Console.WriteLine("{1} \t {0}", entry.Key, entry.Value);
                }

#endif

                // cache attributesLocation locations

                identifiers = new List<string> {
                    "position", "normal", "uv", "uv2", "tangent", "Color",
                    "skinIndex", "skinWeight", "lineDistance" 
                };

                for (var i = 0; i < (int)parameters["maxMorphTargets"]; i++)
                {
                    identifiers.Add("morphTarget" + i);
                }

                for (var i = 0; i < (int)parameters["maxMorphNormals"]; i++)
                {
                    identifiers.Add("morphNormal" + i);
                }

                if (material is IAttributes)
                {
                    var attributesMaterial = material as IAttributes;
                    foreach (var entry in attributesMaterial.Attributes)
                    {
                        identifiers.Add(entry.Key as string);
                    }
                }

                this.Attributes = CacheAttributeLocations(program, identifiers);

                //this.AttributesKeys = Object.keys(this.Attributes);
                this.AttributesKeys = new List<string>();
                foreach (DictionaryEntry entry in this.Attributes)
                    this.AttributesKeys.Add((string)entry.Key);


#if DEBUG

                Console.WriteLine("\nAttribute Locations for Program {0}", program);
                foreach (DictionaryEntry entry in this.Attributes)
                {
                    if (null != entry.Value)
                        Console.WriteLine("{1} \t {0}", entry.Key, entry.Value);
                }

#endif

                //

                this.Id = ProgramIdCount++;
                this.Code = code;
                this.UsedTimes = 1;
                this.Program = program;
                this.VertexShader = glVertexShader;
                this.FragmentShader = glFragmentShader;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

        }
    }
}
