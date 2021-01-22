using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Materials;
using ThreeJs4Net.Math;
using ThreeJs4Net.Objects;
using ThreeJs4Net.Properties;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Core
{
    [DebuggerDisplay("Object3D")]
    public class Object3D : Hashtable, IDisposable
    {
        #region Static Fields

        protected static int Object3DIdCount;

        #endregion

        private bool _disposed;
        public bool __webglInit = false;
        public bool __webglActive = false;
        public float[] __webglMorphTargetInfluences;
        public Matrix4 _modelViewMatrix;
        public Matrix3 _normalMatrix;
        public Vector3 DefaultUp = new Vector3(0, 1, 0);
        public Vector3 Up;
        public string type = "Object3D";
        public Vector3 Position = new Vector3();
        public Euler Rotation = new Euler();
        public Quaternion Quaternion = Quaternion.Identity();
        public Vector3 Scale = new Vector3(1, 1, 1);
        public int RenderDepth = -1;
        public bool RotationAutoUpdate = true;
        public Matrix4 Matrix = new Matrix4().Identity();
        public Matrix4 MatrixWorld = new Matrix4().Identity();
        public bool MatrixWorldNeedsUpdate = false;
        public bool Visible = true;
        public bool CastShadow = false;
        public bool ReceiveShadow = false;
        public bool FrustumCulled = true;
        public BaseGeometry Geometry;
        public Material Material;

        public IList<Object3D> Children = new List<Object3D>();
        public int id = Object3DIdCount++;
        public bool MatrixAutoUpdate = true;

        [CanBeNull]
        public string Name;
        public Object3D Parent;
        public string Tag;
        public object UserData;
        public Guid Uuid = Guid.NewGuid();
        private readonly PreventCircularUpdate preventCircularUpdate = new PreventCircularUpdate();

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Object3D()
        {
            this.Up = (Vector3)this.DefaultUp.Clone();

            // When the Rotation Euler is changed, the Quaternion is changed automaticaly
            this.Rotation.PropertyChanged += (o, args) => this.preventCircularUpdate.Do(() => this.Quaternion.SetFromEuler(this.Rotation));

            // When the Quaternion is changed, the Rotation euler is changed automaticaly
            this.Quaternion.PropertyChanged += (o, args) => this.preventCircularUpdate.Do(() => this.Rotation.SetFromQuaternion(this.Quaternion));
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Object3D(Object3D other)
        {
            this.id = other.id;
            // ...
        }

        #endregion

        #region Public Events

        public event EventHandler<EventArgs> Added;

        public event EventHandler<EventArgs> Removed;

        protected virtual void RaiseAdded()
        {
            var handler = this.Added;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void RaiseRemoved()
        {
            var handler = this.Removed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #endregion

        #region Public Methods and Operators

        #endregion

        #region Methods

        [Obsolete]
        private bool eulerOrder
        {
            get
            {
                Trace.TraceWarning("THREE.Object3D: .eulerOrder has been moved to .rotation.Order.");
                return false; // this.rotation.Order;
            }
            set
            {
                Trace.TraceWarning("THREE.Object3D: .eulerOrder has been moved to .rotation.Order.");
                //this.rotation.Order = value;
            }
        }

        [Obsolete]
        private bool useQuaternion
        {
            get
            {
                Trace.TraceWarning("THREE.Object3D: .useQuaternion has been removed. The library now uses quaternions by default.");
                throw new NotImplementedException();
            }
            set
            {
                Trace.TraceWarning("THREE.Object3D: .useQuaternion has been removed. The library now uses quaternions by default.");
            }
        }

        public void ApplyMatrix(Matrix4 matrix)
        {
            throw new NotImplementedException();
            //this.matrix.ultiplyMatrices(matrix, this.matrix);

            //this.matrix.decompose(this.position, this.quaternion, this.scale);
        }

        public void SetRotationFromAxisAngle()
        {
            throw new NotImplementedException();
            // assumes axis is normalized

            //this.quaternion.setFromAxisAngle(axis, angle);
        }

        public void SetRotationFromEuler()
        {
            throw new NotImplementedException();
            //this.quaternion.setFromEuler(euler, true);
        }

        public void SetRotationFromMatrix()
        {
            throw new NotImplementedException();
            //// assumes the upper 3x3 of m is a pure rotation matrix (i.e, unscaled)

            //this.quaternion.setFromRotationMatrix(m);
        }

        public void SetRotationFromQuaternion()
        {
            throw new NotImplementedException();
            //// assumes q is normalized

            //this.quaternion.copy(q);
        }

        public Object3D RotateOnAxis(Vector3 axis, float angle)
        {
            var q1 = new Quaternion().SetFromAxisAngle(axis, angle );
            this.Quaternion.PreMultiply(q1);
            return this;
        }

        public Object3D RotateX(float angle)
        {
            return this.RotateOnAxis(Vector3.UnitX(), angle);
        }

        public Object3D RotateY(float angle)
        {
            return this.RotateOnAxis(Vector3.UnitY(), angle);
        }

        public Object3D RotateZ(float angle)
        {
            return this.RotateOnAxis(Vector3.UnitZ(), angle);
        }

        public void TranslateOnAxis()
        {
            throw new NotImplementedException();
        }

        public void Translate()
        {
            throw new NotImplementedException();
        }

        public void localToWorld()
        {
            throw new NotImplementedException();
        }

        public void worldToLocal()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public Object3D TranslateX(float distance)
        {
            var v1 = new Vector3(1, 0, 0);

            return this.translateOnAxis(v1, distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public Object3D TranslateY(float distance)
        {
            var v1 = new Vector3(0, 1, 0);

            return this.translateOnAxis(v1, distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public Object3D TranslateZ(float distance)
        {
            var v1 = new Vector3(0, 0, 1);

            return this.translateOnAxis(v1, distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="distance"></param>
        public Object3D translateOnAxis(Vector3 axis, float distance)
        {
            // translate object by distance along axis in object space
            // axis is assumed to be normalized

            var v1 = new Vector3();

            v1.Copy(axis).ApplyQuaternion(this.Quaternion);

            this.Position.Add(v1.MultiplyScalar(distance));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void Traverse(Action<Object3D> callback)
        {
            callback(this);

            for (var i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].Traverse(callback);
            }
        }

        public virtual void LookAt(float x, float y, float z)
        {
            this.LookAt(new Vector3(x,y,z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public virtual void LookAt(Vector3 vector)
        {
            // This routine does not support objects with rotated and/or translated parent(s)
            var m1 = new Matrix4().LookAt(vector, this.Position, this.Up);

            this.Quaternion.SetFromRotationMatrix(m1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Render(Scene scene, Camera camera, int width, int height)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raycaster"></param>
        /// <param name="intersects"></param>
        public virtual void Raycast(Raycaster raycaster, ref List<Intersect> intersects)
        {
            var inverseMatrix = new Matrix4();
            var ray = new Ray();
            var sphere = new Sphere();

            var precision = raycaster.LinePrecision;
            var precisionSq = precision * precision;

            var geometry = this.Geometry as Geometry;
            Debug.Assert(null != geometry, "this.Geometry as Geometry cast failed");

            if (geometry.BoundingSphere == null) geometry.ComputeBoundingSphere();

            // Checking boundingSphere distance to ray

            sphere.Copy(geometry.BoundingSphere);
            sphere.ApplyMatrix4(this.MatrixWorld);

            if (raycaster.Ray.IsIntersectionSphere(sphere) == false)
            {
                return;
            }

            inverseMatrix = this.MatrixWorld.GetInverse();
            ray.Copy(raycaster.Ray).ApplyMatrix4(inverseMatrix);

            /* if ( geometry instanceof THREE.BufferGeometry ) {
     
  		    } else */
            if (geometry is Geometry)
            {
                var vertices = geometry.Vertices;
                var nbVertices = vertices.Count;
                var interSegment = new Vector3();
                var interRay = new Vector3();
                var step = (this is Line && ((Line)this).Mode == Three.LineStrip ? 1 : 2);

                for (var i = 0; i < nbVertices - 1; i = i + step)
                {
                    var distSq = ray.DistanceSqToSegment(vertices[i], vertices[i + 1], interRay, interSegment);

                    if (distSq > precisionSq) continue;

                    var distance = ray.Origin.DistanceTo(interRay);

                    if (distance < raycaster.Near || distance > raycaster.Far) continue;

                    intersects.Add(new Intersect()
                    {
                        Distance = distance,
                        // What do we want? intersection point on the ray or on the segment??
                        // point: raycaster.ray.at( distance ),
                        Point = ((Vector3)interSegment.Clone()).ApplyMatrix4(this.MatrixWorld),
                        Face = null,
                        FaceIndex = -1,
                        Object3D = this
                    });
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        public void Add(Object3D object3D)
        {
            //if ( arguments.length > 1 ) {
            //    for ( var i = 0; i < arguments.length; i++ ) {
            //        this.add( arguments[ i ] );
            //    }
            //    return this;
            //};

            if (object3D == this)
            {
                Trace.TraceError("THREE.Object3D.add:", object3D, "can't be added as a child of itself.");
            }

            if (object3D is Object3D)
            {

                if (object3D.Parent != null)
                {
                    object3D.Parent.Remove(object3D);
                }

                object3D.Parent = this;

                RaiseAdded();

                this.Children.Add(object3D);
            }
            else
            {
                Trace.TraceError("THREE.Object3D.add: {0} is not an instance of THREE.Object3D.", object3D);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        public void Remove(Object3D object3D)
        {
            //if ( arguments.length > 1 ) {
            //    for ( var i = 0; i < arguments.length; i++ ) {
            //        this.remove( arguments[ i ] );
            //    }
            //};

            var index = this.Children.IndexOf(object3D);

            if (index != -1)
            {

                object3D.Parent = null;

                object3D.RaiseRemoved();

                this.Children.RemoveAt(index);

                // remove from scene

                var scene = this;

                while (scene.Parent != null)
                {

                    scene = scene.Parent;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMatrix()
        {
            this.Matrix.Compose(this.Position, this.Quaternion, this.Scale);

            this.MatrixWorldNeedsUpdate = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        public void UpdateMatrixWorld(bool force = false)
        {
            if (this.MatrixAutoUpdate)
                this.UpdateMatrix();

            if (this.MatrixWorldNeedsUpdate || force)
            {

                if (this.Parent == null)
                {
                    this.MatrixWorld.Copy(this.Matrix);
                }
                else
                {
                    this.MatrixWorld = this.Parent.MatrixWorld * this.Matrix;
                }

                this.MatrixWorldNeedsUpdate = false;

                force = true;
            }

            // update children

            for (var i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].UpdateMatrixWorld(force);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public virtual Vector3 GetWorldPosition(Vector3 optionalTarget)
        {
            var result = new Vector3();
            if (optionalTarget != null)
                result = optionalTarget;

            this.UpdateMatrixWorld(true);

            return result.SetFromMatrixPosition(this.MatrixWorld);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public virtual Quaternion GetWorldQuaternion(Quaternion optionalTarget)
        {
            var position = new Vector3();
            var scale = new Vector3();


            var result = new Quaternion();
            if (optionalTarget != null)
                result = optionalTarget;

            this.UpdateMatrixWorld(true);

            this.MatrixWorld.Decompose(position, result, scale);

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public virtual Euler GetWorldRotation(Euler optionalTarget)
        {
            var quaternion = new Quaternion();

            var result = new Euler();
            if (optionalTarget != null)
                result = optionalTarget;

            this.GetWorldQuaternion(quaternion);

            return result.SetFromQuaternion(quaternion, this.Rotation.Order/*, false */);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public virtual Vector3 GetWorldScale(Vector3 optionalTarget)
        {
            var position = new Vector3();
            var quaternion = new Quaternion();

            var result = new Vector3();
            if (optionalTarget != null)
                result = optionalTarget;

            this.UpdateMatrixWorld(true);

            this.MatrixWorld.Decompose(position, quaternion, result);

            return result;
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public virtual Vector3 GetWorldDirection(Vector3 optionalTarget)
        {
            var quaternion = new Quaternion();

            var result = new Vector3();
            if (optionalTarget != null)
                result = optionalTarget;

            this.GetWorldQuaternion(quaternion);

            return result = new Vector3(0, 0, 1).ApplyQuaternion(quaternion);
        }

        public Object3D Copy(Object3D source, bool recursive = true)
        {
            this.Name = source.Name;

            this.Up.Copy(source.Up);
            this.Position.Copy(source.Position);
            this.Quaternion.Copy(source.Quaternion);
            this.Scale.Copy(source.Scale);

            this.Matrix.Copy(source.Matrix);
            this.MatrixWorld.Copy(source.MatrixWorld);

            this.MatrixAutoUpdate = source.MatrixAutoUpdate;
            this.MatrixWorldNeedsUpdate = source.MatrixWorldNeedsUpdate;

            //!!this.Layers.mask = source.Layers.mask;
            this.Visible = source.Visible;

            this.CastShadow = source.CastShadow;
            this.ReceiveShadow = source.ReceiveShadow;

            this.FrustumCulled = source.FrustumCulled;
            //!!this.RenderOrder = source.RenderOrder;

            //this.userData = JSON.parse(JSON.stringify(source.userData));

            if (recursive)
            {
                foreach (var child in source.Children)
                {
                    this.Add(child.Clone());
                }
            }

            return this;
        }

        
        public Object3D Clone(bool recursive = true) {
            return new Object3D().Copy(this, recursive);
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    // TODO
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion
    }

    #endregion
}