using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ThreeJs4Net.Math;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Renderers.Shaders
{
    public class UniformsLib : Dictionary<string, Uniforms>
    {
        /// <summary>
        /// 
        /// </summary>
        public UniformsLib()
        {
            this.Add("common", this.MakeCommon());
            this.Add("bump", this.MakeBump());
            this.Add("normalmap", this.MakeNormalMap());
            this.Add("fog", this.MakeFog());
            this.Add("lights", this.MakeLights());
            this.Add("particle", this.MakeParticle());
            this.Add("shadowmap", this.MakeShadowMap());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeCommon()
        {
            return new Uniforms
            {
                { "diffuse",               new Uniform() { {"type", "C"}, {"value", Color.White}}},            
                { "opacity",               new Uniform() { {"type", "f"}, {"value", 1.0f}}},

                { "map",                   new Uniform() { {"type", "t"}, {"value", null}}},
                { "offsetRepeat",          new Uniform() { {"type", "v4"}, {"value", new Vector4(0,0,1,1)}}},

                { "lightmap",              new Uniform() { {"type", "t"}, {"value", null}}},
                { "specularMap",           new Uniform() { {"type", "t"}, {"value", null}}},
                { "alphaMap",              new Uniform() { {"type", "t"}, {"value", null}}},

                { "envMap",                new Uniform() { {"type", "t"}, {"value", null}}},
                { "flipEnvMap",            new Uniform() { {"type", "f"}, {"value", -1}}},
                { "useRefract",            new Uniform() { {"type", "i"}, {"value", 0}}},
                { "reflectivity",          new Uniform() { {"type", "f"}, {"value", 1.0f}}},
                { "refractionRatio",       new Uniform() { {"type", "f"}, {"value", 0.98f}}},
                { "combine",               new Uniform() { {"type", "i"}, {"value", 0}}},

                { "morphTargetInfluences", new Uniform() { {"type", "f"}, {"value", 0.0f}}},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeBump()
        {
            return new Uniforms
            { 
                { "bumpMap",      new Uniform() { {"type", "t"}, {"value", null}}},
                { "bumpScale",    new Uniform() { {"type", "f"}, {"value", 1.0f}}},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeNormalMap()
        {
            return new Uniforms
            {
                { "normalMap",    new Uniform() { {"type", "t"},  {"value", null}}},
                { "normalScale",  new Uniform() { {"type", "v2"}, {"value", new Vector2(1, 1)}}},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeFog()
        {
            return new Uniforms
            {
                { "fogDensity",  new Uniform() { {"type", "f"},  {"value", 0.00025f}}},
                { "fogNear",     new Uniform() { {"type", "f"},  {"value", 1}}},
                { "fogFar",      new Uniform() { {"type", "f"},  {"value", 2000}}},
                { "fogColor",    new Uniform() { {"type", "C"},  {"value", Color.White}}},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeLights()
        {
            return new Uniforms
            {
                { "ambientLightColor",          new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},

                { "directionalLightDirection",  new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "directionalLightColor",      new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
             
                { "hemisphereLightDirection",   new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "hemisphereLightSkyColor",    new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "hemisphereLightGroundColor", new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},

                { "pointLightColor",            new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "pointLightPosition",         new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "pointLightDistance",         new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                
                { "spotLightColor",             new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "spotLightPosition",          new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "spotLightDirection",         new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "spotLightDistance",          new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "spotLightAngleCos",          new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
                { "spotLightExponent",          new Uniform() { {"type", "fv"},  {"value", new Hashtable()}}},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeParticle()
        {
            return new Uniforms
            {
                { "psColor",      new Uniform() { {"type", "C"},  {"value", Color.White}}},
                { "opacity",      new Uniform() { {"type", "f"},  {"value", 1.0f}}},
                { "size",         new Uniform() { {"type", "f"},  {"value", 1.0f}}},
                { "scale",        new Uniform() { {"type", "f"},  {"value", 1.0f}}},
                { "map",          new Uniform() { {"type", "t"},  {"value", null}}},

                { "fogDensity",   new Uniform() { {"type", "f"},  {"value", 0.00025f}}},
                { "fogNear",      new Uniform() { {"type", "f"},  {"value", 1.0f}}},
                { "fogFar",       new Uniform() { {"type", "f"},  {"value", 2000.0f}}},
                { "fogColor",     new Uniform() { {"type", "C"},  {"value", Color.White}}},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeShadowMap()
        {
            return new Uniforms
            {
                { "shadowMap",       new Uniform() { {"type", "tv"},   {"value", new List<Texture>()}}},
                { "shadowMapSize",   new Uniform() { {"type", "v2v"},  {"value", new List<Size>()}}},

                { "shadowBias",      new Uniform() { {"type", "fv1"},  {"value", new List<float>()}}},
                { "shadowDarkness",  new Uniform() { {"type", "fv1"},  {"value", new List<float>()}}},

                { "shadowMatrix",    new Uniform() { {"type", "m4v"},  {"value", new List<Matrix4>()}}},
            };
        }

    }
}
