using ThreeJs4Net.Math;

namespace ThreeJs4Net.Cameras
{
    public class PerspectiveCamera : Camera
    {
        #region --- Members ---

        public float Aspect;
        public float Fov;
        public float Near;
        public float Far;
        public float Focus;
        public float FilmGauge = 35; // width of the film (default in millimeters)
        public float FilmOffset = 0; // horizontal film offset (same unit as gauge)

        public float X = -1;
        public float Y = -1;
        public float FullWidth = -1;
        public float FullHeight = -1;
        public float Width = -1;
        public float Height = -1;
        public float Zoom = 1;

        public CameraView View = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public PerspectiveCamera(float fov = 50, float aspect = 1.0f, float near = 0.1f, float far = 2000)
        {
            this.type = "PerspectiveCamera";

            this.Zoom = 1;

            this.Focus = 10;
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
        protected PerspectiveCamera(PerspectiveCamera other) : base(other)
        {
            this.Aspect = other.Aspect;
            this.Fov = other.Fov;
            this.Near = other.Near;
            this.Far = other.Far;
            this.View = this.View == null ? new CameraView().Copy(other.View) : this.View.Copy(other.View);
        }

        #endregion






        #region --- Already in R116 ---

        public PerspectiveCamera Copy(PerspectiveCamera source, bool recursive)
        {
            base.Copy(source, recursive);

            this.Fov = source.Fov;
            this.Zoom = source.Zoom;

            this.Near = source.Near;
            this.Far = source.Far;
            this.Focus = source.Focus;

            this.Aspect = source.Aspect;
            this.View = this.View == null ? new CameraView().Copy(source.View) : this.View.Copy(source.View);

            this.FilmGauge = source.FilmGauge;
            this.FilmOffset = source.FilmOffset;

            return this;
        }


        /// <summary>
        /// Sets the FOV by focal length in respect to the current .filmGauge. The default film
        /// gauge is 35, so that the focal length can be specified for a 35mm (full frame) camera.
        /// Values for focal length and film gauge must have the same unit.
        /// </summary>
        /// <param name="focalLength"></param>
        public void SetFocalLength(float focalLength)
        {
            // see http://www.bobatkins.com/photography/technical/field_of_view.html
            var vExtentSlope = (float)0.5 * this.GetFilmHeight() / focalLength;

            this.Fov = MathUtils.RAD2DEG * 2 * Mathf.Atan(vExtentSlope);
            this.UpdateProjectionMatrix();
        }


        public float GetFilmWidth()
        {
            // film not completely covered in portrait format (aspect < 1)
            return this.FilmGauge * Mathf.Min(this.Aspect, 1);
        }

        public float GetFilmHeight()
        {
            // film not completely covered in landscape format (aspect > 1)
            return this.FilmGauge / Mathf.Max(this.Aspect, 1);
        }

        /// <summary>
        ///	Calculates the focal length from the current .fov and .filmGauge.
        /// </summary>
        /// <returns></returns>
        public float GetFocalLength()
        {
            var vExtentSlope = Mathf.Tan(MathUtils.DEG2RAD * (float)0.5 * this.Fov);
            return (float)0.5 * this.GetFilmHeight() / vExtentSlope;
        }

        public float GetEffectiveFov()
        {
            return MathUtils.RAD2DEG * 2 * Mathf.Atan(Mathf.Tan(MathUtils.DEG2RAD * (float)0.5 * this.Fov) / this.Zoom);
        }

        public void ClearViewOffset()
        {
            if (this.View != null)
            {
                this.View.Enabled = false;
            }

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Sets an offset in A larger frustum. This is useful for multi-window or
        /// multi-monitor/multi-machine setups.
        ///
        /// For example, if you have 3x2 monitors and each monitor is 1920x1080 and
        /// the monitors are in grid like this
        ///
        /// +---+---+---+
        /// | A | B | C |
        /// +---+---+---+
        /// | D | E | F |
        /// +---+---+---+
        ///
        /// then for each monitor you would call it like this
        ///
        ///     var w = 1920;
        ///     var h = 1080;
        ///     var fullWidth = w * 3;
        ///     var fullHeight = h * 2;
        ///
        /// --A--
        /// camera.setOffset( fullWidth, fullHeight, w * 0, h * 0, w, h );
        /// --B--
        /// camera.setOffset( fullWidth, fullHeight, w * 1, h * 0, w, h );
        /// --C--
        /// camera.setOffset( fullWidth, fullHeight, w * 2, h * 0, w, h );
        /// --D--
        /// camera.setOffset( fullWidth, fullHeight, w * 0, h * 1, w, h );
        /// --E--
        /// camera.setOffset( fullWidth, fullHeight, w * 1, h * 1, w, h );
        /// --F--
        /// camera.setOffset( fullWidth, fullHeight, w * 2, h * 1, w, h );
        ///
        /// Note there is no reason monitors have to be the same size or in A grid.
        /// </remarks>
        /// <param name="fullWidth"></param>
        /// <param name="fullHeight"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetViewOffset(float fullWidth, float fullHeight, float x, float y, float width, float height)
        {
            this.Aspect = fullWidth / fullHeight;

            if (this.View == null)
            {
                this.View = new CameraView();
            }

            this.View.Enabled = true;
            this.View.FullWidth = fullWidth;
            this.View.FullHeight = fullHeight;
            this.View.OffsetX = x;
            this.View.OffsetY = y;
            this.View.Width = width;
            this.View.Height = height;

            this.UpdateProjectionMatrix();
        }

        public void UpdateProjectionMatrix()
        {
            float near = this.Near;
            float top = near * Mathf.Tan(MathUtils.DEG2RAD * (float)0.5 * this.Fov) / this.Zoom;
            float height = 2 * top;
            float width = this.Aspect * height;
            float left = (float)(-0.5) * width;
            var view = this.View;

            if (this.View != null && this.View.Enabled)
            {
                var fullWidth = view.FullWidth;
                var fullHeight = view.FullHeight;

                left += view.OffsetX * width / fullWidth;
                top -= view.OffsetY * height / fullHeight;
                width *= view.Width / fullWidth;
                height *= view.Height / fullHeight;

            }

            var skew = this.FilmOffset;
            if (skew != 0)
            {
                left += near * skew / this.GetFilmWidth();
            }

            this.ProjectionMatrix.MakePerspective(left, left + width, top, top - height, near, this.Far);

            this.ProjectionMatrixInverse.GetInverse(this.ProjectionMatrix);
        }

        public override object Clone()
        {
            return new PerspectiveCamera(this);
        }

        #endregion
    }
}