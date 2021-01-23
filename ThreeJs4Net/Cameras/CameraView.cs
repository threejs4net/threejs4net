using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeJs4Net.Cameras
{
    public class CameraView
    {
        public bool Enabled { get; set; }
        public float FullWidth { get; set; }
        public float FullHeight { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public CameraView Copy(CameraView source)
        {
            if (source == null)
            {
                return null;
            }

            this.Enabled = source.Enabled;
            this.FullWidth = source.FullWidth;
            this.FullHeight = source.FullHeight;
            this.OffsetX = source.OffsetX;
            this.OffsetY = source.OffsetY;
            this.Width = source.Width;
            this.Height = source.Height;

            return this;
        }
    }
}
