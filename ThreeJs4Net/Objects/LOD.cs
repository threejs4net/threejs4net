using System.Collections.Generic;
using ThreeJs4Net.Core;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Objects
{
    public class LOD : Object3D
    {
        class Object3DEx
        {
            public float distance;
            public Object3D object3D;
        }

        private readonly List<Object3DEx> objects = new List<Object3DEx>();

        /// <summary>
        /// Constructor
        /// </summary>
        public LOD()
        {
            this.type = "LOD";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="distance"></param>
        public void AddLevel(Object3D object3D, float distance = 0)
        {
            distance = System.Math.Abs(distance);

            var l = 0;
            for (l = 0; l < this.objects.Count; l ++ ) {
                if ( distance < this.objects[ l ].distance ) {
                    break;
                }
            }

            this.objects.Insert(l, new Object3DEx() { distance = distance, object3D = object3D});

            this.Add(object3D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Object3D GetObjectForDistance (float distance )
        {
            var i = 0;
            for (i = 1; i < this.objects.Count; i++ ) 
            {
                if (distance < this.objects[ i ].distance ) {
                    break;
                }
            }

            return this.objects[i - 1].object3D;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Raycast(Raycaster raycaster, ref List<Intersect> intersects)
        {
            var matrixPosition = new Vector3();

            matrixPosition.SetFromMatrixPosition(this.MatrixWorld);
            var distance = raycaster.Ray.Origin.DistanceTo(matrixPosition);
            this.GetObjectForDistance(distance).Raycast(raycaster, ref intersects);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public void Update(Object3D camera)
        {
            var v1 = new Vector3();
            var v2 = new Vector3();

            if (this.objects.Count > 1)
            {
                v1.SetFromMatrixPosition(camera.MatrixWorld);
                v2.SetFromMatrixPosition(this.MatrixWorld);

                var distance = v1.DistanceTo(v2);

                this.objects[0].object3D.Visible = true;

                var i = 0;
                for (i = 1; i < this.objects.Count; i++)
                {
                    if (distance >= this.objects[i].distance)
                    {
                        this.objects[i - 1].object3D.Visible = false;
                        this.objects[i].object3D.Visible = true;
                    }
                    else
                    {
                        break;
                    }
                }

                for (; i < this.objects.Count; i++)
                {
                    this.objects[i].object3D.Visible = false;
                }
            }

        }


    }
}
