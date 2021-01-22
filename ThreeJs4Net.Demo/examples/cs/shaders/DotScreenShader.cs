using System;
using System.Collections.Generic;
using ThreeJs4Net.Math;
using ThreeJs4Net.Renderers.Shaders;
using ThreeJs4Net.Renderers.WebGL;

namespace ThreeJs4Net.Demo.examples.cs.shaders
{
    public class DotScreenShader : WebGlShader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DotScreenShader()
        {
            #region construct uniform variables

            this.Uniforms =
                UniformsUtils.Merge(new List<Uniforms>
                {
                    new Uniforms { { "tDiffuse", new Uniform() { {"type", "t"},  {"value", null } } }},
                    new Uniforms { { "tSize",    new Uniform() { {"type", "v2"}, {"value", new Vector2( 256, 256 ) } } }},
                    new Uniforms { { "center",   new Uniform() { {"type", "v2"}, {"value", new Vector2( 0.5f, 0.5f ) } } }},
                    new Uniforms { { "angle",    new Uniform() { {"type", "f"},  {"value", 1.57f } } }},
                    new Uniforms { { "scale",    new Uniform() { {"type", "f"},  {"value", 1.0f } }}}
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

            fs.Add("uniform vec2 center;");
            fs.Add("uniform float angle;");
            fs.Add("uniform float scale;");
            fs.Add("uniform vec2 tSize;");

            fs.Add("uniform sampler2D tDiffuse;");

            fs.Add("varying vec2 vUv;");

            fs.Add("float pattern() {");

            fs.Add("float s = sin( angle ), c = cos( angle );");

            fs.Add("vec2 tex = vUv * tSize - center;");
            fs.Add("vec2 point = vec2( c * tex.x - s * tex.y, s * tex.x + c * tex.y ) * scale;");

            fs.Add("return ( sin( point.x ) * sin( point.y ) ) * 4.0;");

            fs.Add("}");

            fs.Add("void main() {");

            fs.Add("vec4 color = texture2D( tDiffuse, vUv );");
            fs.Add("float average = ( color.r + color.g + color.b ) / 3.0;");
            fs.Add("gl_FragColor = vec4( vec3( average * 10.0 - 5.0 + pattern() ), color.a );");

            fs.Add("}");

            // join
            this.FragmentShader = String.Join("\n", fs).Trim();
            #endregion

        }
    }
}
