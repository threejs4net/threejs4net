using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras.Core
{
    public class Shape : Path
    {
        public Guid uuid;
        public List<Path> holes;

        public Shape() : base()
        {
            this.uuid = Guid.NewGuid();
            this.holes = new List<Path>();
        }

        public Shape(IEnumerable<Vector2> points = null) : base(points)
        {
            this.uuid = Guid.NewGuid();
            this.holes = new List<Path>();
        }

        public List<Vector2[]> GetPointsHoles(int divisions)
        {
            List<Vector2[]> holesPts = new List<Vector2[]>();
            for (var i = 0; i < this.holes.Count; i++)
            {
                holesPts.Add(this.holes[i].GetPoints(divisions).ToArray());
            }

            return holesPts;
        }

        // get points of shape and holes (keypoints based on segments parameter)
        public ShapePoints ExtractPoints(int divisions)
        {
            return new ShapePoints
            {
                shape = this.GetPoints(divisions),
                holes = this.GetPointsHoles(divisions)
            };
        }

        //public Shape Copy(Shape source)
        //{
        //    base.Copy(source);

        //    this.holes = new List<Path>();

        //    for (var i = 0; i < source.holes.Count; i++)
        //    {
        //        var hole = source.holes[i];
        //        this.holes.Add(hole.Clone());
        //    }
        //    return this;
        //}
    }

    public class ShapePoints
    {
        public List<Vector2> shape;
        public List<Vector2[]> holes;
    }
}
