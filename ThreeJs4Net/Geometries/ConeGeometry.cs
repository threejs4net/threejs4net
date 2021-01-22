using System.Collections.Generic;
using System.Diagnostics;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Geometries
{
    public class ConeGeometry : CylinderGeometry
    {
        #region Fields
        public float Radius;
        public float ThetaStart;
        public float ThetaLength;
        private float RadiusTop; // HIDE
        private float RadiusBottom; // HIDE
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public ConeGeometry(float radius = 20, float height = 100, int radialSegments = 8, int heightSegments = 1, bool openEnded = false, float thetaStart = 0, float thetaength = Mathf.PI * 2) :
            base(0, radius, height, radialSegments, heightSegments, openEnded)
        {
        }

        #endregion
    }
}
