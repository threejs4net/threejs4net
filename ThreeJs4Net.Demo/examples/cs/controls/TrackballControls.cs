using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Demo.examples.cs.controls
{
    class TrackballControls
    {
        private Object3D object3D;

        enum STATE : int
        {
            NONE  = -1,
            ROTATE = 0,
            ZOOM = 1, 
            PAN = 2, 
            TOUCH_ROTATE = 3, 
            TOUCH_ZOOM_PAN = 4,
        }


        public bool Enabled = true;

	    public float RotateSpeed = 1.0f;
	    public float ZoomSpeed = 1.2f;
	    public float PanSpeed = 0.3f;

	    public bool NoRotate = false;
	    public bool NoZoom = false;
	    public bool NoPan = false;
	    public bool NoRoll = false;

	    public bool StaticMoving = false;
	    public float DynamicDampingFactor = 0.2f;

	    public float MinDistance = 0;
        public float MaxDistance = float.PositiveInfinity;

        // internals

        private Vector3 target = new Vector3();

        private float EPS = 0.000001f;

        private Vector3 lastPosition = new Vector3();

        private STATE _state = STATE.NONE;

        private STATE _prevState = STATE.NONE;

        private Vector3 _eye = new Vector3();

        private Vector3 _rotateStart = new Vector3();

        private Vector3 _rotateEnd = new Vector3();

        private Vector2 _zoomStart = new Vector2();

        private Vector2 _zoomEnd = new Vector2();

        private float _touchZoomDistanceStart = 0;

        private float _touchZoomDistanceEnd = 0;

        private Vector2 _panStart = new Vector2();

        private Vector2 _panEnd = new Vector2();

        // for reset

        private Vector3 target0;

        private Vector3 position0;

        private Vector3 up0;

        private Rectangle screen;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="camera"></param>
        public TrackballControls(Control control, Object3D camera)
        {
            this.object3D = camera;

            this.screen = control.ClientRectangle;

            control.MouseDown += MouseDown;
            control.MouseMove += MouseMove;
            control.MouseUp += MouseUp;
            control.MouseWheel += MouseWheel;

            target0 = (Vector3)this.target.Clone();
            position0 = (Vector3)this.object3D.Position.Clone();
            up0 =  (Vector3)this.object3D.Up.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageX"></param>
        /// <param name="pageY"></param>
        /// <returns></returns>
        private Vector2 GetMouseOnScreen  (int pageX, int pageY)
        {
            var vector = new Vector2(
		            ( pageX - this.screen.Left ) / (float)this.screen.Width,
		            ( pageY - this.screen.Top ) / (float)this.screen.Height
	            );

	        return vector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    private Vector3 GetMouseProjectionOnBall  (int pageX, int pageY)
        {
            var vector = new Vector3();
            var objectUp = new Vector3();
            var mouseOnBall = new Vector3();

            mouseOnBall = new Vector3(
                (pageX - this.screen.Width * 0.5f - this.screen.Left) / (this.screen.Width * .5f),
                (this.screen.Height * 0.5f + this.screen.Top - pageY) / (this.screen.Height * .5f),
                0.0f);
 
        	var length = mouseOnBall.Length();

			if ( this.NoRoll ) {

				if ( length < Mat.SQRT1_2 ) {

					mouseOnBall.Z = (float)System.Math.Sqrt( 1.0 - length * length );

				} else {

					mouseOnBall.Z = .5f / length;
					
				}

			} else if ( length > 1.0 ) {

				mouseOnBall.Normalize();

			} else {

				mouseOnBall.Z = (float)System.Math.Sqrt( 1.0 - length * length );

			}

            _eye.Copy(this.object3D.Position).Sub(this.target);

            vector.Copy(this.object3D.Up).SetLength(mouseOnBall.Y);
            vector.Add(objectUp.Copy(this.object3D.Up).Cross(_eye).SetLength(mouseOnBall.X));
            vector.Add(_eye.SetLength(mouseOnBall.Z));

			return vector;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckDistances ()
        {

		    if ( !this.NoZoom || !this.NoPan ) {

			    if ( _eye.LengthSq() > this.MaxDistance * this.MaxDistance ) {

				    this.object3D.Position.AddVectors( this.target, _eye.SetLength( this.MaxDistance ) );

			    }

			    if ( _eye.LengthSq() < this.MinDistance * this.MinDistance ) {

				    this.object3D.Position.AddVectors( this.target, _eye.SetLength( this.MinDistance ) );

			    }

		    }

	    }

        /// <summary>
        /// 
        /// </summary>
        private void RotateCamera()
        {
            var axis = new Vector3();
            var quaternion = new Quaternion();

            var angle = (float)System.Math.Acos(_rotateStart.Dot(_rotateEnd) / _rotateStart.Length() / _rotateEnd.Length());

            if (angle > 0)
            {
                axis.CrossVectors(_rotateStart, _rotateEnd).Normalize();

                angle *= this.RotateSpeed;

                quaternion.SetFromAxisAngle(axis, -angle);

				_eye.ApplyQuaternion( quaternion );
                this.object3D.Up.ApplyQuaternion(quaternion);

                _rotateEnd.ApplyQuaternion(quaternion);

                if (this.StaticMoving)
                {

                    _rotateStart.Copy(_rotateEnd);

                }
                else
                {

                    quaternion.SetFromAxisAngle(axis, angle * (this.DynamicDampingFactor - 1.0f));
                    _rotateStart.ApplyQuaternion(quaternion);

                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ZoomCamera()
        {
            if (_state == STATE.TOUCH_ZOOM_PAN)
            {

                var factor = _touchZoomDistanceStart / _touchZoomDistanceEnd;
                _touchZoomDistanceStart = _touchZoomDistanceEnd;
                _eye.MultiplyScalar(factor);

            }
            else
            {
                var factor = 1.0f + (_zoomEnd.Y - _zoomStart.Y) * this.ZoomSpeed;

			    if ( factor != 1.0f && factor > 0.0f ) {

				    _eye.MultiplyScalar( factor );

				    if ( this.StaticMoving ) {

					    _zoomStart.Copy( _zoomEnd );

				    } else {

					    _zoomStart.Y += ( _zoomEnd.Y - _zoomStart.Y ) * this.DynamicDampingFactor;

				    }

			    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PanCamera()
        {
            var mouseChange = new Vector2();
            var objectUp = new Vector3();
            var pan = new Vector3();

            mouseChange.Copy(_panEnd).Sub(_panStart);

   			if ( mouseChange.LengthSq() > 0 )
   			{
   			    mouseChange.MultiplyScalar(_eye.Length() * this.PanSpeed);

   			    pan.Copy(_eye).Cross(this.object3D.Up).SetLength(mouseChange.X);
   			    pan.Add(objectUp.Copy(this.object3D.Up).SetLength(mouseChange.Y));

   			    this.object3D.Position.Add(pan);
   			    this.target.Add(pan);

				if ( this.StaticMoving )
				{
				    _panStart.Copy(_panEnd);
				} 
                else
				{
				    _panStart.Add(mouseChange.SubVectors(_panEnd, _panStart).MultiplyScalar(this.DynamicDampingFactor));
				}
			}
   
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
		    _eye.SubVectors( this.object3D.Position, this.target );

		    if ( !this.NoRotate ) {
			    this.RotateCamera();
		    }

		    if ( !this.NoZoom ) {
			    this.ZoomCamera();
		    }

		    if ( !this.NoPan ) {
			    this.PanCamera();
		    }


            this.object3D.Position.AddVectors(this.target, _eye);

            this.CheckDistances();

            this.object3D.LookAt(this.target);

		    if ( lastPosition.DistanceToSquared( this.object3D.Position ) > EPS )
            {
//			    this.dispatchEvent( changeEvent );

                lastPosition.Copy(this.object3D.Position);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
		    _state = STATE.NONE;
		    _prevState = STATE.NONE;

		    this.target.Copy( this.target0 );
		    this.object3D.Position.Copy( this.position0 );
		    this.object3D.Up.Copy( this.up0 );

            _eye.SubVectors(this.object3D.Position, this.target);

		    this.object3D.LookAt( this.target );

		    //this.dispatchEvent( changeEvent );

		    lastPosition.Copy( this.object3D.Position );
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MouseDown(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            Debug.Assert(null != control);

            if (this.Enabled == false) return;

            //event.preventDefault();
            //event.stopPropagation();

		    if ( _state == STATE.NONE ) {
		        switch (e.Button) {
                    case MouseButtons.Left:
		                _state = STATE.ROTATE;
                         break;
                    case MouseButtons.Middle:
		                _state = STATE.ZOOM;
                        break;
                    case MouseButtons.Right:
		                _state = STATE.PAN;
                        break;
                }
		    }

		    if ( _state == STATE.ROTATE && !this.NoRotate )
		    {
		        _rotateStart.Copy(GetMouseProjectionOnBall(e.X, e.Y));
                _rotateEnd.Copy(_rotateStart);
            } 
            else if ( _state == STATE.ZOOM && !this.NoZoom )
            {
                _zoomStart.Copy(GetMouseOnScreen(e.X, e.Y));
			    _zoomEnd.Copy(_zoomStart);
		    } 
            else if ( _state == STATE.PAN && !this.NoPan )
		    {
		        _panStart.Copy(GetMouseOnScreen(e.X, e.Y));
		        _panEnd.Copy(_panStart);
		    }

            //document.addEventListener( 'mousemove', mousemove, false );
            //document.addEventListener( 'mouseup', mouseup, false );

		    //this.dispatchEvent( startEvent );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMove(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            Debug.Assert(null != control);
 
            if ( this.Enabled == false ) return;
  
            //event.preventDefault();
            //event.stopPropagation();

       		if ( _state == STATE.ROTATE && !this.NoRotate )
       		{
       		    _rotateEnd.Copy(this.GetMouseProjectionOnBall(e.X, e.Y));
       		} 
            else if ( _state == STATE.ZOOM && !this.NoZoom )
       		{
       		    _zoomEnd.Copy(this.GetMouseOnScreen(e.X, e.Y));
       		} 
            else if ( _state == STATE.PAN && !this.NoPan )
       		{
       		    _panEnd.Copy(this.GetMouseOnScreen(e.X, e.Y));
       		}
 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MouseUp(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            Debug.Assert(null != control);

            if (this.Enabled == false) return;
  
            //event.preventDefault();
            //event.stopPropagation();

            _state = STATE.NONE;

            //document.removeEventListener( 'mousemove', mousemove );
            //document.removeEventListener( 'mouseup', mouseup );
            //this.dispatchEvent( endEvent );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MouseWheel(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            Debug.Assert(null != control);

            if (this.Enabled == false) return;

            //event.preventDefault();
            //event.stopPropagation();

            var delta = e.Delta / 40;

            _zoomStart.Y += delta * 0.01f;

            //_this.dispatchEvent( startEvent );
            //_this.dispatchEvent( endEvent );
        }
    }
}
