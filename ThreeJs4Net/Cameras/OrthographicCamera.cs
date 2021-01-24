namespace ThreeJs4Net.Cameras
{
    public class OrthographicCamera : Camera
    {
        #region Fields
        public float Bottom;
        public float Left;
        public float Right;
        public float Top;
        public float Zoom;
        public CameraView View;
        public float Near;
        public float Far;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public OrthographicCamera(float left = -1, float right = 1, float top = 1, float bottom = -1, float near = 0.1f, int far = 2000)
        {
            this.type = "OrthographicCamera";

            this.Zoom = 1;

            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            this.Near = near;
            this.Far = far;

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        protected OrthographicCamera(OrthographicCamera other) : base(other)
        {
            this.Zoom = other.Zoom;

            this.Left = other.Left;
            this.Right = other.Right;
            this.Top = other.Top;
            this.Bottom = other.Bottom;

            this.Near = other.Near;
            this.Far = other.Far;
        }

        #endregion


        #region --- Already in R116 ---
        public override object Clone()
        {
            return new OrthographicCamera(this);
        }

        public OrthographicCamera Copy(OrthographicCamera source, bool recursive)
        {
            base.Copy(source, recursive);

            this.Left = source.Left;
            this.Right = source.Right;
            this.Top = source.Top;
            this.Bottom = source.Bottom;
            this.Near = source.Near;
            this.Far = source.Far;

            this.Zoom = source.Zoom;
            this.View = this.View == null ? new CameraView().Copy(source.View) : this.View.Copy(source.View);

            return this;
        }

        public void SetViewOffset(float fullWidth, float fullHeight, float x, float y, float width, float height)
        {
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

        public void ClearViewOffset()
        {
            if (this.View != null)
            {
                this.View.Enabled = false;
            }

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            var dx = (this.Right - this.Left) / (2 * this.Zoom);
            var dy = (this.Top - this.Bottom) / (2 * this.Zoom);
            var cx = (this.Right + this.Left) / 2;
            var cy = (this.Top + this.Bottom) / 2;

            var left = cx - dx;
            var right = cx + dx;
            var top = cy + dy;
            var bottom = cy - dy;

            if (this.View != null && this.View.Enabled)
            {
                var scaleW = (this.Right - this.Left) / this.View.FullWidth / this.Zoom;
                var scaleH = (this.Top - this.Bottom) / this.View.FullHeight / this.Zoom;

                left += scaleW * this.View.OffsetX;
                right = left + scaleW * this.View.Width;
                top -= scaleH * this.View.OffsetY;
                bottom = top - scaleH * this.View.Height;

            }

            this.ProjectionMatrix.MakeOrthographic(left, right, top, bottom, this.Near, this.Far);

            this.ProjectionMatrixInverse.GetInverse(this.ProjectionMatrix);
        }



        #endregion

    }
}