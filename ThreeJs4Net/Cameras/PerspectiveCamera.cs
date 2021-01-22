using ThreeJs4Net.Math;

namespace ThreeJs4Net.Cameras
{
    public class PerspectiveCamera : Camera
    {
        #region Fields
        
        public float Aspect;

        public float Fov;

        public float X = -1;

        public float Y = -1;

        public float FullWidth = -1;

        public float FullHeight = -1;

        public float Width = -1;

        public float Height = -1;

        public float Zoom = 1;

        #endregion

        /**
        * Uses Focal Length (in mm) to estimate and Set FOV
        * 35mm (fullframe) camera is used if frame size is not specified;
        * Formula based on http://www.bobatkins.com/photography/technical/field_of_view.html
        */
        public void SetLens(float focalLength, float frameHeight)
        {
            this.Fov = (float)(2.0f * Mat.RadToDeg(System.Math.Atan(frameHeight / (focalLength * 2.0f))));
            this.UpdateProjectionMatrix();
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public PerspectiveCamera(float fov = 50, float aspect = 1.0f, float near = 0.1f, float far = 2000)
        {
            this.type = "PerspectiveCamera";

            this.Zoom = 1;

            this.Fov = fov;
            this.Aspect = aspect;
            this.Near = near;
            this.Far = far;

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected PerspectiveCamera(PerspectiveCamera other)
            : base(other)
        {
            this.Aspect = other.Aspect;
            this.Fov = other.Fov;
            this.Near = other.Near;
            this.Far = other.Far;
        }

        #endregion

        #region Public Properties


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new PerspectiveCamera(this);
        }

        /**
        * Sets an offset in a larger frustum. This is useful for multi-window or
        * multi-monitor/multi-machine setups.
        *
        * For example, if you have 3x2 monitors and each monitor is 1920x1080 and
        * the monitors are in grid like this
        *
        * +---+---+---+
        * | A | B | C |
        * +---+---+---+
        * | D | E | F |
        * +---+---+---+
        *
        * then for each monitor you would call it like this
        *
        * var w = 1920;
        * var h = 1080;
        * var fullWidth = w * 3;
        * var fullHeight = h * 2;
        *
        * --A--
        * camera.setOffset( fullWidth, fullHeight, w * 0, h * 0, w, h );
        * --B--
        * camera.setOffset( fullWidth, fullHeight, w * 1, h * 0, w, h );
        * --C--
        * camera.setOffset( fullWidth, fullHeight, w * 2, h * 0, w, h );
        * --D--
        * camera.setOffset( fullWidth, fullHeight, w * 0, h * 1, w, h );
        * --E--
        * camera.setOffset( fullWidth, fullHeight, w * 1, h * 1, w, h );
        * --F--
        * camera.setOffset( fullWidth, fullHeight, w * 2, h * 1, w, h );
        *
        * Note there is no reason monitors have to be the same size or in a grid.
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullWidth"></param>
        /// <param name="fullHeight"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetViewOffset(float fullWidth, float fullHeight, float x, float y, float width, float height)
        {
            this.FullWidth = fullWidth;
            this.FullHeight = fullHeight;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            var fov = Mat.RadToDeg(2 * System.Math.Atan(System.Math.Tan(Mat.DegToRad(this.Fov) * 0.5) / this.Zoom));
            
            if (this.FullWidth > 0)
            {
                var aspect = this.FullWidth / this.FullHeight;
                var top = (float)System.Math.Tan(Mat.DegToRad(fov * 0.5f)) * this.Near;
                var bottom = -top;
                var left = aspect * bottom;
                var right = aspect * top;
                var width = (float)System.Math.Abs(right - left);
                var height = (float)System.Math.Abs(top - bottom);
                this.ProjectionMatrix.MakeFrustum(
                        left + this.X * width / this.FullWidth,
                        left + (this.X + this.Width) * width / this.FullWidth,
                        top - (this.Y + this.Height) * height / this.FullHeight,
                        top - this.Y * height / this.FullHeight,
                        this.Near,
                        this.Far
                    );
            }
            else
            {
                this.ProjectionMatrix.MakePerspective(this.Fov, this.Aspect, this.Near, this.Far);
            }
        }


        #endregion
    }
}