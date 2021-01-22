using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Cameras
{
    public class Camera : Object3D
    {
        public Matrix4 MatrixWorldInverse = new Matrix4().Identity();
        public Matrix4 ProjectionMatrix = new Matrix4().Identity();
        public Matrix4 ProjectionMatrixInverse = new Matrix4().Identity();

        public float Far = (float)2000.0;
        public float Near = (float)0.1;

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Camera()
        {
            this.type = "Camera";
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected Camera(Camera other)
            : base(other)
        {
        }

        #endregion

        #region Public Methods and Operators

        public Camera Copy(Camera source, bool recursive)
        {
            //!! TEST this... not sure 
            base.Copy(source, recursive);

            this.MatrixWorldInverse.Copy(source.MatrixWorldInverse);
            this.ProjectionMatrix.Copy(source.ProjectionMatrix);
            this.ProjectionMatrixInverse.Copy(source.ProjectionMatrixInverse);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public override Vector3 GetWorldDirection(Vector3 target)
        {
            this.UpdateMatrixWorld(true);
            var e = this.MatrixWorld.elements;
            return target.Set(-e[8], -e[9], -e[10]).Normalize();
        }

        #endregion
    }
}