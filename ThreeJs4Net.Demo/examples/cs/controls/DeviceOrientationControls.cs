using System.Collections;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Demo.examples.cs.controls
{
    public class DeviceOrientationControls
    {
        public Object3D object3D;

        private bool freeze;

        private Hashtable deviceOrientation;

        private object screenOrientation = 0;

//        private SimpleOrientationSensor _simpleorientation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        public DeviceOrientationControls(Object3D object3D)
        {
            this.object3D = object3D;
            this.object3D.Rotation.Reorder(Euler.RotationOrder.YXZ);

            this.freeze = true;

            this.deviceOrientation = new Hashtable();

            this.screenOrientation = 0;
        }

        /// <summary>
        /// The angles alpha, beta and gamma form a Set of intrinsic Tait-Bryan angles of type Z-X'-Y''
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <param name="orient"></param>
        private void SetObjectQuaternion(out Quaternion quaternion, float alpha, float beta, float gamma, float orient)
        {
            var zee = new Vector3(0, 0, 1);

            var q0 = new Quaternion();

            var q1 = new Quaternion(- (float)System.Math.Sqrt(0.5), 0, 0, (float)System.Math.Sqrt(0.5)); // - PI/2 around the x-axis

            var euler = new Euler(beta, alpha, - gamma, Euler.RotationOrder.YXZ); // 'ZXY' for the device, but 'YXZ' for us

            quaternion = new Quaternion().SetFromEuler(euler); // orient the device

            quaternion.Multiply(q1);                                      // camera looks out the back of the device, not the top

            quaternion.Multiply(q0.SetFromAxisAngle(zee, - orient)); // adjust for screen orientation
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
	        if ( this.freeze ) return;

            // TODO: using SimpleOrientationSensor ?
            var alpha = 0;//this.deviceOrientation.gamma ? Mat.DegToRad( this.deviceOrientation.alpha ) : 0; // Z
            var beta = 0;//this.deviceOrientation.beta  ? Mat.DegToRad( this.deviceOrientation.beta  ) : 0; // X'
            var gamma = 0;//this.deviceOrientation.gamma ? Mat.DegToRad( this.deviceOrientation.gamma ) : 0; // Y''
            var orient = 0;//this.screenOrientation       ? Mat.DegToRad( this.screenOrientation       ) : 0; // O

		    SetObjectQuaternion(out this.object3D.Quaternion, alpha, beta, gamma, orient );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Connect()
        {
            this.freeze = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            this.freeze = true;
        }

    }
}
