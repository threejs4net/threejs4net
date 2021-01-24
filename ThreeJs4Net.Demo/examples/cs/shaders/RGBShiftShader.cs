using System;
using System.Collections.Generic;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Renderers.WebGL;

namespace ThreeJs4Net.Demo.examples.cs.shaders
{
    public class RGBShiftShader : WebGlShader
    {
         /// <summary>
        /// Constructor
        /// </summary>
        public RGBShiftShader()
        {
            #region construct uniform variables

            this.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                {
                    new Uniforms { { "tDiffuse", new Uniform() { {"type", "t"},  {"value", null } } }},
                    new Uniforms { { "amount",   new Uniform() { {"type", "f"},  {"value", 0.005f } } }},
                    new Uniforms { { "angle",    new Uniform() { {"type", "f"},  {"value", 1.0f } }}}
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

            fs.Add("uniform sampler2D tDiffuse;");
            fs.Add("uniform float amount;");
            fs.Add("uniform float angle;");

            fs.Add("varying vec2 vUv;");

            fs.Add("void main() {");

            fs.Add("vec2 offset = amount * vec2( cos(angle), sin(angle));");
            fs.Add("vec4 cr = texture2D(tDiffuse, vUv + offset);");
            fs.Add("vec4 cga = texture2D(tDiffuse, vUv);");
            fs.Add("vec4 cb = texture2D(tDiffuse, vUv - offset);");
            fs.Add("gl_FragColor = vec4(cr.r, cga.g, cb.b, cga.a);");

            fs.Add("}");

            // join
            this.FragmentShader = String.Join("\n", fs).Trim();
            #endregion

        }
   }
}
