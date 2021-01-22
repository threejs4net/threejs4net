using System.Drawing;

namespace ThreeJs4Net.Lights
{
    public class HemisphereLight : Light
    {
        #region Fields

        
        public Color groundColor;

        
        public float intensity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public HemisphereLight(Color skyColor, Color groundColor, float intensity = 1)
            : base(skyColor)
        {
            this.type = "HemisphereLight";
            
            //this.position.Set( 0, 100, 0 );
            this.groundColor = groundColor;
            this.intensity = intensity;
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        protected HemisphereLight(HemisphereLight other)
            : base(other)
        {
            this.groundColor = other.groundColor;
            this.intensity = other.intensity;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new HemisphereLight(this);
        }

        #endregion
    }
}