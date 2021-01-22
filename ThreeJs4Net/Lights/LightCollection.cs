
using System.Collections.Generic;

namespace ThreeJs4Net.Lights
{
    public class LightCollection
    {
        public class Ambient 
        {
            public List<float> colors = new List<float>();
            //public float length; always 1!
            // No position for ambient light
        }

        public class Directional
        {
            public float length;
            public List<float> colors = new List<float>();
            public List<float> positions = new List<float>();
        }

        public class Point
        {
            public float length;
            public List<float> colors = new List<float>();
            public List<float> positions = new List<float>();
            public List<float> distances = new List<float>();
        }

        public class Spot
        {
            public float length;
            public List<float> colors = new List<float>();
            public List<float> positions = new List<float>();
            public List<float> distances = new List<float>();
            public List<float> directions = new List<float>();
            public List<float> anglesCos = new List<float>();
            public List<float> exponents = new List<float>();
        }

        public class Hemi
        {
            public float length;
            public List<float> skyColors = new List<float>();
            public List<float> groundColors = new List<float>();
            public List<float> positions = new List<float>();
        }


        public Ambient ambient = new Ambient();
        public Directional directional = new Directional();
        public Point point = new Point();
        public Spot spot = new Spot();
        public Hemi hemi = new Hemi();
    }
}
