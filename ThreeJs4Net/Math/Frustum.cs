using ThreeJs4Net.Core;
using ThreeJs4Net.Extras.Objects;

namespace ThreeJs4Net.Math
{
    public class Frustum
    {
        public Plane[] Planes = new Plane[6];

        public Frustum()
        {
            this.Initialize(null, null, null, null, null, null);
        }

        public Frustum(Plane p0, Plane p1 = null, Plane p2 = null, Plane p3 = null, Plane p4 = null, Plane p5 = null)
        {
            this.Initialize(p0, p1, p2, p3, p4, p5);
        }

        public void Initialize(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
        {
            this.Planes[0] = p0 == null ? new Plane() : p0.Clone();
            this.Planes[1] = p1 == null ? new Plane() : p1.Clone();
            this.Planes[2] = p2 == null ? new Plane() : p2.Clone();
            this.Planes[3] = p3 == null ? new Plane() : p3.Clone();
            this.Planes[4] = p4 == null ? new Plane() : p4.Clone();
            this.Planes[5] = p5 == null ? new Plane() : p5.Clone();
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        #region --- Already in R116 ---

        public Frustum Set(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
        {
            var planes = this.Planes;

            planes[0].Copy(p0);
            planes[1].Copy(p1);
            planes[2].Copy(p2);
            planes[3].Copy(p3);
            planes[4].Copy(p4);
            planes[5].Copy(p5);

            return this;
        }

        public Frustum Clone()
        {
            return new Frustum().Copy(this);
        }

        public Frustum Copy(Frustum frustum)
        {
            var planes = this.Planes;

            for (var i = 0; i < 6; i++)
            {
                planes[i].Copy(frustum.Planes[i]);
            }

            return this;
        }

        public Frustum SetFromProjectionMatrix(Matrix4 m)
        {
            var planes = this.Planes;
            var me = m.Elements;
            float me0 = me[0], me1 = me[1], me2 = me[2], me3 = me[3];
            float me4 = me[4], me5 = me[5], me6 = me[6], me7 = me[7];
            float me8 = me[8], me9 = me[9], me10 = me[10], me11 = me[11];
            float me12 = me[12], me13 = me[13], me14 = me[14], me15 = me[15];

            planes[0].SetComponents(me3 - me0, me7 - me4, me11 - me8, me15 - me12).Normalize();
            planes[1].SetComponents(me3 + me0, me7 + me4, me11 + me8, me15 + me12).Normalize();
            planes[2].SetComponents(me3 + me1, me7 + me5, me11 + me9, me15 + me13).Normalize();
            planes[3].SetComponents(me3 - me1, me7 - me5, me11 - me9, me15 - me13).Normalize();
            planes[4].SetComponents(me3 - me2, me7 - me6, me11 - me10, me15 - me14).Normalize();
            planes[5].SetComponents(me3 + me2, me7 + me6, me11 + me10, me15 + me14).Normalize();

            return this;
        }

        public bool IntersectsObject(Object3D object3d)
        {
            var geometry = object3d.Geometry;

            if (geometry.BoundingSphere == null)
            {
                geometry.ComputeBoundingSphere();
            }

            var sphere = new Sphere().Copy(geometry.BoundingSphere).ApplyMatrix4(object3d.MatrixWorld);
            return this.IntersectsSphere(sphere);
        }

        public bool IntersectsSprite(Sprite sprite)
        {
            var sphere = new Sphere();
            sphere.Center.Set(0, 0, 0);
            sphere.Radius = (float)0.7071067811865476;
            sphere.ApplyMatrix4(sprite.MatrixWorld);

            return this.IntersectsSphere(sphere);
        }

        public bool IntersectsSphere(Sphere sphere)
        {
            var planes = this.Planes;
            var center = sphere.Center;
            var negRadius = -sphere.Radius;

            for (var i = 0; i < 6; i++)
            {
                var distance = planes[i].DistanceToPoint(center);
                if (distance < negRadius)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IntersectsBox(Box3 box)
        {
            var planes = this.Planes;
            var vector = new Vector3();

            for (var i = 0; i < 6; i++)
            {
                var plane = planes[i];

                // corner at max distance
                vector.X = plane.Normal.X > 0 ? box.Max.X : box.Min.X;
                vector.Y = plane.Normal.Y > 0 ? box.Max.Y : box.Min.Y;
                vector.Z = plane.Normal.Z > 0 ? box.Max.Z : box.Min.Z;

                if (plane.DistanceToPoint(vector) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsPoint(Vector3 point)
        {
            var planes = this.Planes;

            for (var i = 0; i < 6; i++)
            {
                if (planes[i].DistanceToPoint(point) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
