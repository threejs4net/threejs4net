using System.Drawing;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Lights
{
    public class SpotLight : Light, ILightShadow
    {
        public float angle;

        public float distance;

        public float intensity;

        public float exponent;

        public Object3D target;

                public bool onlyShadow { get; set; }

                public float shadowCameraNear { get; set; }

                public float shadowCameraFar { get; set; }

                public float shadowCameraFov { get; set; }

                public bool shadowCameraVisible { get; set; }

                public float shadowBias { get; set; }

                public float shadowDarkness  { get; set; }

                public float shadowMapWidth { get; set; }

                public float shadowMapHeight { get; set; }

                public Texture shadowMap { get; set; }

                public Size shadowMapSize  { get; set; }

                //     public Texture shadowCamera  { get; Set; }

                public Matrix4 shadowMatrix { get; set; }

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public SpotLight(Color color, float intensity = 1, float distance = 0, float angle = 1.0471666f, float exponent = 10)
            : base(color)
        {
            this.type = "SpotLight";
            
            this.Position = new Vector3(0, 1, 0);
            this.target = new Object3D();
            this.intensity = intensity;
            this.distance = distance;
            this.angle = angle;
            this.exponent = exponent;
            this.CastShadow = false;
            this.onlyShadow = false;
            //
            this.shadowCameraNear = 50;
            this.shadowCameraFar = 5000;
            this.shadowCameraFov = 50;
            this.shadowCameraVisible = false;
            this.shadowBias = 0;
            this.shadowDarkness = 0.5f;
            this.shadowMapWidth = 512;
            this.shadowMapHeight = 512;
            //
            this.shadowMap = null;
            //this.shadowMapSize = null;
            //this.shadowCamera = null;
            this.shadowMatrix = null;
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        protected SpotLight(SpotLight other)
            : base(other)
        {
            //this.position.Set( 0, 1, 0 );
            //this.target = new THREE.Object3D();
            //this.intensity = ( intensity !== undefined ) ? intensity : 1;
            //this.distance = ( distance !== undefined ) ? distance : 0;
            //this.angle = ( angle !== undefined ) ? angle : Math.PI / 3;
            //this.exponent = ( exponent !== undefined ) ? exponent : 10;
            //this.castShadow = false;
            //this.onlyShadow = false;
            ////
            //this.shadowCameraNear = 50;
            //this.shadowCameraFar = 5000;
            //this.shadowCameraFov = 50;
            //this.shadowCameraVisible = false;
            //this.shadowBias = 0;
            //this.shadowDarkness = 0.5;
            //this.shadowMapWidth = 512;
            //this.shadowMapHeight = 512;
            ////
            //this.shadowMap = null;
            //this.shadowMapSize = null;
            //this.shadowCamera = null;
            //this.shadowMatrix = null;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new SpotLight(this);
        }

        #endregion
    }
}