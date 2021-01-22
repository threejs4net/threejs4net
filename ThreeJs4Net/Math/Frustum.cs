using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThreeJs4Net.Properties;

namespace ThreeJs4Net.Math
{
    public class Frustum : INotifyPropertyChanged
    {
        public Plane[] Planes = new Plane[6];

        /// <summary>
        /// 
        /// </summary>
        public Frustum() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        public Frustum(Plane p0)
        {
            this.Initialize(p0, null, null, null, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public Frustum(Plane p0, Plane p1)
        {
            this.Initialize(p0, p1, null, null, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Frustum(Plane p0, Plane p1, Plane p2)
        {
            this.Initialize(p0, p1, p2, null, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public Frustum(Plane p0, Plane p1, Plane p2, Plane p3)
        {
            this.Initialize(p0, p1, p2, p3, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        public Frustum(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4)
        {
            this.Initialize(p0, p1, p2, p3, p4, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        public Frustum(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
        {
            this.Initialize(p0, p1, p2, p3, p4, p5);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        public void Initialize(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
        {
            if (p0 != null) this.Planes[0].Copy(p0);
            if (p1 != null) this.Planes[1].Copy(p1);
            if (p2 != null) this.Planes[2].Copy(p2);
            if (p3 != null) this.Planes[3].Copy(p3);
            if (p4 != null) this.Planes[4].Copy(p4);
            if (p5 != null) this.Planes[5].Copy(p5);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Frustum SetFromMatrix (Matrix4 m )
        {
		    var planes = this.Planes;

		    var me = m.elements;

		    var me0 = me[0]; var me1 = me[1]; var me2 = me[2]; var me3 = me[3];
		    var me4 = me[4]; var me5 = me[5]; var me6 = me[6]; var me7 = me[7];
		    var me8 = me[8]; var me9 = me[9]; var me10 = me[10]; var me11 = me[11];
		    var me12 = me[12]; var me13 = me[13]; var me14 = me[14]; var me15 = me[15];

            planes[0].SetComponents(me3 - me0, me7 - me4, me11 - me8, me15 - me12).Normalize();
            planes[1].SetComponents(me3 + me0, me7 + me4, me11 + me8, me15 + me12).Normalize();
            planes[2].SetComponents(me3 + me1, me7 + me5, me11 + me9, me15 + me13).Normalize();
            planes[3].SetComponents(me3 - me1, me7 - me5, me11 - me9, me15 - me13).Normalize();
            planes[4].SetComponents(me3 - me2, me7 - me6, me11 - me10, me15 - me14).Normalize();
            planes[5].SetComponents(me3 + me2, me7 + me6, me11 + me10, me15 + me14).Normalize();

		    return this;
	    }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
