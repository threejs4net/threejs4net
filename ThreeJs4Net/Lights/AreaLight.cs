using System.Drawing;

namespace ThreeJs4Net.Lights
{
    public class AreaLight : Light
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public AreaLight(Color color, float intensity = 1)
            : base(color)
        {
            this.type = "AreaLight";

            //this.normal = new THREE.Vector3( 0, - 1, 0 );
            //this.right = new THREE.Vector3( 1, 0, 0 );
            //this.intensity = ( intensity !== undefined ) ? intensity : 1;
            //this.width = 1.0;
            //this.height = 1.0;
            //this.constantAttenuation = 1.5;
            //this.linearAttenuation = 0.5;
            //this.quadraticAttenuation = 0.1;        }
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        protected AreaLight(AreaLight other)
            : base(other)
        {
            //this.normal = new THREE.Vector3( 0, - 1, 0 );
            //this.right = new THREE.Vector3( 1, 0, 0 );
            //this.intensity = ( intensity !== undefined ) ? intensity : 1;
            //this.width = 1.0;
            //this.height = 1.0;
            //this.constantAttenuation = 1.5;
            //this.linearAttenuation = 0.5;
            //this.quadraticAttenuation = 0.1;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new AreaLight(this);
        }

        #endregion
    }
}