using System;
using System.Drawing;

namespace ThreeJs4Net.Scenes
{
    public class FogExp2 : Fog, ICloneable
    {
        #region Fields

        
        private Color color;

        
        private float density;

        
        private string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public FogExp2(Color color, float density)
        {
            this.color = color;
            this.density = density;
        }

        /// <summary>
        /// </summary>
        protected FogExp2(FogExp2 other)
        {
            this.color = other.Color;
            this.density = other.density;
        }

        #endregion

        #region Public Events


        #endregion

        #region Public Properties

        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;

                
            }
        }

        public float Density
        {
            get
            {
                return this.density;
            }
            set
            {
                this.density = value;

                
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;

                
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new FogExp2(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}