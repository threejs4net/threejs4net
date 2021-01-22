using System.Drawing;

namespace ThreeJs4Net.Lights
{
    public class AmbientLight : Light
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public AmbientLight(Color color)
            : base(color)
        {
            this.type = "AmbientLight";
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        protected AmbientLight(AmbientLight other)
            : base(other)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new AmbientLight(this);
        }

        #endregion
    }
}