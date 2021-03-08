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
        #region --- Members ---
        protected static int Object3DIdCount;
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
        public List<Material> Materials;
        public Layers Layers = new Layers();
        public Skeleton Skeleton;
        public IList<Object3D> Children = new List<Object3D>();
        public int id = Object3DIdCount++;
        public bool MatrixAutoUpdate = true;

        [CanBeNull]
        public string Name;

        [CanBeNull]
        public Object3D Parent;
        public string Tag;
        public object UserData;
        public Guid Uuid = Guid.NewGuid();
        private readonly PreventCircularUpdate preventCircularUpdate = new PreventCircularUpdate();
        #endregion

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

        #region --- Already in R116 ---
        public void ApplyMatrix(Matrix4 matrix)
        {
            if (this.MatrixAutoUpdate)
            {
                this.UpdateMatrix();
            }

            this.Matrix.Premultiply(matrix);
            this.Matrix.Decompose(this.Position, this.Quaternion, this.Scale);
        }

        public Object3D ApplyQuaternion(Quaternion quat)
        {
            this.Quaternion.PreMultiply(quat);
            return this;
        }


        public virtual Vector3 GetWorldPosition(Vector3 target)
        {
            this.UpdateMatrixWorld(true);
            return target.SetFromMatrixPosition(this.MatrixWorld);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual Quaternion GetWorldQuaternion(Quaternion target)
        {
            var position = new Vector3();
            var scale = new Vector3();
            this.UpdateMatrixWorld(true);
            this.MatrixWorld.Decompose(position, target, scale);
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual Vector3 GetWorldScale(Vector3 target)
        {
            var position = new Vector3();
            var quaternion = new Quaternion();
            this.UpdateMatrixWorld(true);
            this.MatrixWorld.Decompose(position, quaternion, target);
            return target;
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual Vector3 GetWorldDirection(Vector3 target)
        {
            if (target == null)
            {
                target = new Vector3();
            }

            this.UpdateMatrixWorld(true);
            var e = this.MatrixWorld.elements;
            return target.Set(e[8], e[9], e[10]).Normalize();
        }

        public Vector3 LocalToWorld(Vector3 vector)
        {
            return vector.ApplyMatrix4(this.MatrixWorld);
        }

        public Object3D RotateOnAxis(Vector3 axis, float angle)
        {
            var q1 = new Quaternion().SetFromAxisAngle(axis, angle);
            this.Quaternion.Multiply(q1);
            return this;
        }


        public Object3D RotateOnWorldAxis(Vector3 axis, float angle)
        {
            // rotate object on axis in world space
            // axis is assumed to be normalized
            // method assumes no rotated parent
            var q1 = new Quaternion().SetFromAxisAngle(axis, angle);
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

        public void SetRotationFromAxisAngle(Vector3 axis, float angle)
        {
            // assumes axis is normalized
            this.Quaternion.SetFromAxisAngle(axis, angle);
        }

        public void SetRotationFromEuler(Euler euler)
        {
            this.Quaternion.SetFromEuler(euler, true);
        }

        public void SetRotationFromMatrix(Matrix4 m)
        {
            // assumes the upper 3x3 of m is A pure rotation matrix (i.e, unscaled)
            this.Quaternion.SetFromRotationMatrix(m);
        }

        public void SetRotationFromQuaternion(Quaternion q)
        {
            // assumes q is normalized
            this.Quaternion.Copy(q);
        }

        public Object3D TranslateOnAxis(Vector3 axis, float distance)
        {
            // translate object by distance along axis in object space
            // axis is assumed to be normalized
            var v1 = new Vector3().Copy(axis).ApplyQuaternion(this.Quaternion);
            this.Position.Add(v1.MultiplyScalar(distance));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public Object3D TranslateX(float distance)
        {
            return this.TranslateOnAxis(Vector3.UnitX(), distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public Object3D TranslateY(float distance)
        {
            return this.TranslateOnAxis(Vector3.UnitY(), distance);
        }

        public Object3D TranslateZ(float distance)
        {
            return this.TranslateOnAxis(Vector3.UnitZ(), distance);
        }

        public void Traverse(Action<Object3D> callback)
        {
            callback(this);

            for (var i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].Traverse(callback);
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
        public virtual void UpdateMatrixWorld(bool force = false)
        {
            if (this.MatrixAutoUpdate)
            {
                this.UpdateMatrix();
            }

            if (this.MatrixWorldNeedsUpdate || force)
            {
                if (this.Parent == null)
                {
                    this.MatrixWorld.Copy(this.Matrix);
                }
                else
                {
                    this.MatrixWorld.MultiplyMatrices(this.Parent.MatrixWorld, this.Matrix);
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

        public virtual void UpdateWorldMatrix(bool updateParents, bool updateChildren)
        {
            var parent = this.Parent;
            if (updateParents)
            {
                parent?.UpdateWorldMatrix(true, false);
            }

            if (this.MatrixAutoUpdate)
            {
                this.UpdateMatrix();
            }

            if (this.Parent == null)
            {
                this.MatrixWorld.Copy(this.Matrix);
            }
            else
            {
                this.MatrixWorld.MultiplyMatrices(this.Parent.MatrixWorld, this.Matrix);
            }

            // update children
            if (updateChildren)
            {
                var children = this.Children;
                foreach (var child in children)
                {
                    child.UpdateWorldMatrix(false, true);
                }
            }
        }

        public Vector3 WorldToLocal(Vector3 vector)
        {
            return vector.ApplyMatrix4(new Matrix4().GetInverse(this.MatrixWorld));
        }
        #endregion


        public void Translate()
        {
            throw new NotImplementedException();
        }


        public virtual void LookAt(float x, float y, float z)
        {
            this.LookAt(new Vector3(x, y, z));
        }

        public virtual void LookAt(Vector3 vector)
        {
            // This routine does not support objects with rotated and/or translated parent(s)
            var m1 = new Matrix4().LookAt(vector, this.Position, this.Up);

            this.Quaternion.SetFromRotationMatrix(m1);
        }

        public void Render(Scene scene, Camera camera, int width, int height)
        {

        }

        public virtual void Raycast(Raycaster raycaster, List<Intersect> intersects)
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

            if (!raycaster.Ray.IntersectsSphere(sphere))
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
                Trace.TraceError("THREE.Object3D.add:", object3D, "can't be added as A child of itself.");
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

            this.Layers.Mask = source.Layers.Mask;
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


        public Object3D Clone(bool recursive = true)
        {
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
            // from executing A second time.
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
    }

    #endregion
}