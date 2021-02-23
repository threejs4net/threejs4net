using ThreeJs4Net.Math;

namespace ThreeJs4Net.Examples.Jsm.Math
{
    public class OBB
    {
        private readonly Vector3 _xAxis = new Vector3();
        private readonly Vector3 _yAxis = new Vector3();
        private readonly Vector3 _zAxis = new Vector3();
        private readonly basis a = new basis();
        private readonly basis b = new basis();
        private readonly Vector3 _v1 = new Vector3();
        private readonly Matrix3 _rotationMatrix = new Matrix3();
        private readonly Box3 _aabb = new Box3();
        private readonly Matrix4 _matrix = new Matrix4();
        private readonly Matrix4 _inverse = new Matrix4();
        private readonly Ray _localRay = new Ray();
        private readonly Vector3 _size = new Vector3();
        private readonly Vector3 _closestPoint = new Vector3();
        private readonly float[][] _Absr = new float[][] { new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 } };
        private readonly float[][] _r = new float[][] { new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 } };
        private readonly float[] _t = new float[] { 0,0,0 };

        public Vector3 Center;
        public Vector3 HalfSize;
        public Matrix3 Rotation;

        public OBB()
        {
            this.Center = new Vector3();
            this.HalfSize = new Vector3();
            this.Rotation = new Matrix3();
        }

        public OBB(Vector3 center, Vector3 halfSize, Matrix3 rotation)
        {
            this.Center = center ?? new Vector3();
            this.HalfSize = halfSize ?? new Vector3();
            this.Rotation = rotation ?? new Matrix3();
        }

        public OBB Set(Vector3 center, Vector3 halfSize, Matrix3 rotation)
        {
            this.Center = center;
            this.HalfSize = halfSize;
            this.Rotation = rotation;

            return this;
        }

        public OBB Copy(OBB obb)
        {
            this.Center.Copy(obb.Center);
            this.HalfSize.Copy(obb.HalfSize);
            this.Rotation.Copy(obb.Rotation);

            return this;
        }

        public OBB Clone()
        {
            return new OBB().Copy(this);
        }

        public Vector3 GetSize(Vector3 result)
        {
            return result.Copy(this.HalfSize).MultiplyScalar(2);
        }

        public bool ContainsPoint(Vector3 point)
        {
            _v1.SubVectors(point, this.Center);
            this.Rotation.ExtractBasis(_xAxis, _yAxis, _zAxis);

            // project v1 onto each axis and check if these points lie inside the OBB
            return Mathf.Abs(_v1.Dot(_xAxis)) <= this.HalfSize.X &&
                    Mathf.Abs(_v1.Dot(_yAxis)) <= this.HalfSize.Y &&
                    Mathf.Abs(_v1.Dot(_zAxis)) <= this.HalfSize.Z;
        }

        public bool IntersectsBox3(Box3 box3)
        {
            return this.IntersectsOBB(new OBB().FromBox3(box3));
        }

        public OBB ApplyMatrix4(Matrix4 matrix)
        {
            var e = matrix.elements;

            var sx = _v1.Set(e[0], e[1], e[2]).Length();
            var sy = _v1.Set(e[4], e[5], e[6]).Length();
            var sz = _v1.Set(e[8], e[9], e[10]).Length();

            var det = matrix.Determinant();
            if (det < 0) sx = -sx;

            this._rotationMatrix.SetFromMatrix4(matrix);

            var invSX = 1 / sx;
            var invSY = 1 / sy;
            var invSZ = 1 / sz;

            this._rotationMatrix.Elements[0] *= invSX;
            this._rotationMatrix.Elements[1] *= invSX;
            this._rotationMatrix.Elements[2] *= invSX;

            this._rotationMatrix.Elements[3] *= invSY;
            this._rotationMatrix.Elements[4] *= invSY;
            this._rotationMatrix.Elements[5] *= invSY;

            this._rotationMatrix.Elements[6] *= invSZ;
            this._rotationMatrix.Elements[7] *= invSZ;
            this._rotationMatrix.Elements[8] *= invSZ;

            this.Rotation.Multiply(_rotationMatrix);

            this.HalfSize.X *= sx;
            this.HalfSize.Y *= sy;
            this.HalfSize.Z *= sz;

            _v1.SetFromMatrixPosition(matrix);
            this.Center.Add(_v1);

            return this;
        }

        public Vector3 ClampPoint(Vector3 point, Vector3 result)
        {
            var halfSize = this.HalfSize;

            _v1.SubVectors(point, this.Center);
            this.Rotation.ExtractBasis(_xAxis, _yAxis, _zAxis);

            // start at the center position of the OBB
            result.Copy(this.Center);

            // project the target onto the OBB axes and walk towards that point
            var x = MathUtils.Clamp(_v1.Dot(_xAxis), -halfSize.X, halfSize.X);
            result.Add(_xAxis.MultiplyScalar(x));

            var y = MathUtils.Clamp(_v1.Dot(_yAxis), -halfSize.Y, halfSize.Y);
            result.Add(_yAxis.MultiplyScalar(y));

            var z = MathUtils.Clamp(_v1.Dot(_zAxis), -halfSize.Z, halfSize.Z);
            result.Add(_zAxis.MultiplyScalar(z));

            return result;
        }


        /**
        * Performs a ray/OBB intersection test. Returns either true or false if
        * there is a intersection or not.
        */
        public bool IntersectsRay(Ray ray)
        {
            return this.IntersectRay(ray, this._v1) != null;
        }

        public OBB FromBox3(Box3 box3)
        {
            box3.GetCenter(this.Center);
            box3.GetSize(this.HalfSize).MultiplyScalar((float)0.5);
            this.Rotation.Identity();
            return this;
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            // find the point on the OBB closest to the sphere center
            this.ClampPoint(sphere.Center, _closestPoint);
            // if that point is inside the sphere, the OBB and sphere intersect
            return _closestPoint.DistanceToSquared(sphere.Center) <= (sphere.Radius * sphere.Radius);
        }

        public bool Equals(OBB obb)
        {
            return obb.Center.Equals(this.Center) &&
                obb.HalfSize.Equals(this.HalfSize) &&
                obb.Rotation.Equals(this.Rotation);

        }

        public Vector3 IntersectRay(Ray ray, Vector3 result)
        {
            // the idea is to perform the intersection test in the local space
            // of the OBB.
            this.GetSize(_size);
            _aabb.SetFromCenterAndSize(_v1.Set(0, 0, 0), _size);

            // create a 4x4 transformation matrix
            Matrix4FromRotationMatrix(_matrix, this.Rotation);
            _matrix.SetPosition(this.Center);

            // transform ray to the local space of the OBB
            _inverse.Copy(_matrix).Invert();
            _localRay.Copy(ray).ApplyMatrix4(_inverse);

            // perform ray <-> AABB intersection test
            if (_localRay.IntersectBox(_aabb, result) != null)
            {
                // transform the intersection point back to world space
                return result.ApplyMatrix4(_matrix);
            }

            return null;
        }

        public bool IntersectsPlane(Plane plane)
        {
            this.Rotation.ExtractBasis(_xAxis, _yAxis, _zAxis);

            // compute the projection interval radius of this OBB onto L(t) = this->center + t * p.normal;
            var r = this.HalfSize.X * Mathf.Abs(plane.Normal.Dot(_xAxis)) +
                    this.HalfSize.Y * Mathf.Abs(plane.Normal.Dot(_yAxis)) +
                    this.HalfSize.Z * Mathf.Abs(plane.Normal.Dot(_zAxis));

            // compute distance of the OBB's center from the plane

            var d = plane.Normal.Dot(this.Center) - plane.Constant;

            // Intersection occurs when distance d falls within [-r,+r] interval

            return Mathf.Abs(d) <= r;
        }

        public bool IntersectsOBB(OBB obb, float epsilon = float.Epsilon)
        {
            // prepare data structures (the code uses the same nomenclature like the reference)
            a.c = this.Center;
            a.e[0] = this.HalfSize.X;
            a.e[1] = this.HalfSize.Y;
            a.e[2] = this.HalfSize.Z;
            this.Rotation.ExtractBasis(a.u[0], a.u[1], a.u[2]);

            b.c = obb.Center;
            b.e[0] = obb.HalfSize.X;
            b.e[1] = obb.HalfSize.Y;
            b.e[2] = obb.HalfSize.Z;
            obb.Rotation.ExtractBasis(b.u[0], b.u[1], b.u[2]);

            // compute rotation matrix expressing b in a's coordinate frame
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    _r[i][j] = a.u[i].Dot(b.u[j]);
                }
            }

            // compute translation vector
            _v1.SubVectors(b.c, a.c);

            // bring translation into a's coordinate frame
            _t[0] = _v1.Dot(a.u[0]);
            _t[1] = _v1.Dot(a.u[1]);
            _t[2] = _v1.Dot(a.u[2]);

            // compute common subexpressions. Add in an epsilon term to
            // counteract arithmetic errors when two edges are parallel and
            // their cross product is (near) null
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    _Absr[i][j] = Mathf.Abs(_r[i][j]) + epsilon;
                }
            }

            float ra, rb;

            // test axes L = A0, L = A1, L = A2
            for (var i = 0; i < 3; i++)
            {
                ra = a.e[i];
                rb = b.e[0] * _Absr[i][0] + b.e[1] * _Absr[i][1] + b.e[2] * _Absr[i][2];
                if (Mathf.Abs(_t[i]) > ra + rb) return false;
            }

            // test axes L = B0, L = B1, L = B2
            for (var i = 0; i < 3; i++)
            {
                ra = a.e[0] * _Absr[0][i] + a.e[1] * _Absr[1][i] + a.e[2] * _Absr[2][i];
                rb = b.e[i];
                if (Mathf.Abs(_t[0] * _r[0][i] + _t[1] * _r[1][i] + _t[2] * _r[2][i]) > ra + rb) return false;
            }

            // test axis L = A0 x B0
            ra = a.e[1] * _Absr[2][0] + a.e[2] * _Absr[1][0];
            rb = b.e[1] * _Absr[0][2] + b.e[2] * _Absr[0][1];
            if (Mathf.Abs(_t[2] * _r[1][0] - _t[1] * _r[2][0]) > ra + rb) return false;

            // test axis L = A0 x B1
            ra = a.e[1] * _Absr[2][1] + a.e[2] * _Absr[1][1];
            rb = b.e[0] * _Absr[0][2] + b.e[2] * _Absr[0][0];
            if (Mathf.Abs(_t[2] * _r[1][1] - _t[1] * _r[2][1]) > ra + rb) return false;

            // test axis L = A0 x B2
            ra = a.e[1] * _Absr[2][2] + a.e[2] * _Absr[1][2];
            rb = b.e[0] * _Absr[0][1] + b.e[1] * _Absr[0][0];
            if (Mathf.Abs(_t[2] * _r[1][2] - _t[1] * _r[2][2]) > ra + rb) return false;

            // test axis L = A1 x B0
            ra = a.e[0] * _Absr[2][0] + a.e[2] * _Absr[0][0];
            rb = b.e[1] * _Absr[1][2] + b.e[2] * _Absr[1][1];
            if (Mathf.Abs(_t[0] * _r[2][0] - _t[2] * _r[0][0]) > ra + rb) return false;

            // test axis L = A1 x B1
            ra = a.e[0] * _Absr[2][1] + a.e[2] * _Absr[0][1];
            rb = b.e[0] * _Absr[1][2] + b.e[2] * _Absr[1][0];
            if (Mathf.Abs(_t[0] * _r[2][1] - _t[2] * _r[0][1]) > ra + rb) return false;

            // test axis L = A1 x B2
            ra = a.e[0] * _Absr[2][2] + a.e[2] * _Absr[0][2];
            rb = b.e[0] * _Absr[1][1] + b.e[1] * _Absr[1][0];
            if (Mathf.Abs(_t[0] * _r[2][2] - _t[2] * _r[0][2]) > ra + rb) return false;

            // test axis L = A2 x B0
            ra = a.e[0] * _Absr[1][0] + a.e[1] * _Absr[0][0];
            rb = b.e[1] * _Absr[2][2] + b.e[2] * _Absr[2][1];
            if (Mathf.Abs(_t[1] * _r[0][0] - _t[0] * _r[1][0]) > ra + rb) return false;

            // test axis L = A2 x B1
            ra = a.e[0] * _Absr[1][1] + a.e[1] * _Absr[0][1];
            rb = b.e[0] * _Absr[2][2] + b.e[2] * _Absr[2][0];
            if (Mathf.Abs(_t[1] * _r[0][1] - _t[0] * _r[1][1]) > ra + rb) return false;

            // test axis L = A2 x B2
            ra = a.e[0] * _Absr[1][2] + a.e[1] * _Absr[0][2];
            rb = b.e[0] * _Absr[2][1] + b.e[1] * _Absr[2][0];
            if (Mathf.Abs(_t[1] * _r[0][2] - _t[0] * _r[1][2]) > ra + rb) return false;

            // since no separating axis is found, the OBBs must be intersecting
            return true;
        }

        private void Matrix4FromRotationMatrix(Matrix4 matrix4, Matrix3 matrix3)
        {
            var e = matrix4.elements;
            var me = matrix3.Elements;

            e[0] = me[0];
            e[1] = me[1];
            e[2] = me[2];
            e[3] = 0;

            e[4] = me[3];
            e[5] = me[4];
            e[6] = me[5];
            e[7] = 0;

            e[8] = me[6];
            e[9] = me[7];
            e[10] = me[8];
            e[11] = 0;

            e[12] = 0;
            e[13] = 0;
            e[14] = 0;
            e[15] = 1;
        }

        internal class basis
        {
            public Vector3 c { get; set; }
            public Vector3[] u { get; set; } = new Vector3[] { new Vector3(), new Vector3(), new Vector3() };
            public float[] e { get; set; } = new float[3] { 0, 0, 0 };
        }
    }
}
