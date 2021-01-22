using System;
using System.Diagnostics;
using System.Drawing;

namespace ThreeJs4Net.Scenes
{
    [DebuggerDisplay("Color = {Color}, Near = {Near}, Far = {Far}")]
    public class Fog : ICloneable
    {
        #region Fields

        
        public Color Color = Color.White;

        
        public float Far;

        
        public string name;

        
        public float Near;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public Fog()
        {
        }

        /// <summary>
        /// </summary>
        public Fog(Color color, float near = 1, float far = 2000)
        {
            this.Color = color;
            this.Near = near;
            this.Far = far;
        }

        /// <summary>
        /// </summary>
        protected Fog(Fog other)
        {
            this.Color = other.Color;
            this.Near = other.Near;
            this.Far = other.Far;
        }

        #endregion

        #region Public Events


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Fog(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}