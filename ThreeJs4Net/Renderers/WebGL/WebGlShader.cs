using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using ThreeJs4Net.Renderers.Shaders;

namespace ThreeJs4Net.Renderers.WebGL
{
    public class WebGlShader
    {
        public string VertexShader;
        public Uniforms Uniforms;
        public string FragmentShader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        public static int BuildShader(ShaderType type, string code)
        {
            var shader = GL.CreateShader(type);

            GL.ShaderSource(shader, code);
            GL.CompileShader( shader );

            int compileResult;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out compileResult);
            if (compileResult != 1)
            {
                Trace.TraceWarning( "THREE.WebGLShader: Shader couldn\'t compile." );
            }

            if ( GL.GetShaderInfoLog(shader) != string.Empty ) 
            {
                Trace.TraceWarning( "THREE.WebGLShader: gl.getShaderInfoLog()\n {0}", GL.GetShaderInfoLog(shader) );
                Trace.TraceWarning( addLineNumbers(code) );

                throw new ApplicationException("compilation warning or error, see console");
            }

            // --enable-privileged-webgl-extension
            // console.log( type, gl.getExtension( 'WEBGL_debug_shaders' ).getTranslatedShaderSource( shader ) );

            return shader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string addLineNumbers (string code) 
        {
            var lines = code.Split('\n');

            for ( var i = 0; i < lines.Length; i ++ ) {
                lines[ i ] = ( i + 1 ) + ": " + lines[ i ];
            }
            return string.Join( "\n", lines );
        }
    }
}
