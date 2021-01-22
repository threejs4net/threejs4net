using System;
using System.Collections.Generic;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Renderers.WebGL;

namespace ThreeJs4Net.Demo.examples.cs.shaders
{
    public class CopyShader : WebGlShader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CopyShader()
        {
            #region construct uniform variables

            this.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                {
                    new Uniforms { { "tDiffuse", new Uniform() { {"type", "t"},  {"value", null } } }},
                    new Uniforms { { "opacity",  new Uniform() { {"type", "f"},  {"value", 1.0f } }}}
                });
            #endregion

            #region construct VertexShader
            var vs = new List<string>();

            vs.Add("varying vec2 vUv;");

            vs.Add("void main() {");

            vs.Add("vUv = uv;");
            vs.Add("gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );");

            vs.Add("}");

            // join
            this.VertexShader = String.Join("\n", vs).Trim();
            #endregion

            #region construct FragmentShader
            var fs = new List<string>();

            fs.Add("uniform float opacity;");

            fs.Add("uniform sampler2D tDiffuse;");

            fs.Add("varying vec2 vUv;");

            fs.Add("void main() {");

            fs.Add("vec4 texel = texture2D( tDiffuse, vUv );");
            fs.Add("gl_FragColor = opacity * texel;");

            fs.Add("}");

            // join
            this.FragmentShader = String.Join("\n", fs).Trim();
            #endregion

        }
    }
}
