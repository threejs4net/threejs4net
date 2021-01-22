using System.Drawing;

namespace ThreeJs4Net.Lights
{
    public class PointLight : Light
    {
        #region Fields

        
        public float distance;

        
        public float intensity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public PointLight(Color color, float intensity = 1, float distance = 0)
            : base(color)
        {
            this.type = "PointLight";
            
            this.intensity = intensity;
            this.distance = distance;
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        protected PointLight(PointLight other)
            : base(other)
        {
            this.intensity = other.intensity;
            this.distance = other.distance;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new PointLight(this);
        }

        #endregion
    }
}