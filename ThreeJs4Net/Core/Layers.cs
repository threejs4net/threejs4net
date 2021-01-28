using System;
using System.Collections.Generic;
using ThreeJs4Net.Cameras;
using ThreeJs4Net.Lights;
using ThreeJs4Net.Math;
using ThreeJs4Net.Renderers.Renderables;
using ThreeJs4Net.Scenes;

namespace ThreeJs4Net.Core
{
    public class Layers
    {
        #region --- Already in R116 ---
        public long Mask { get; set; }

        public void Set(int channel)
        {
            this.Mask = 1 << channel | 0;
        }

        public void Enable(int channel ) {

            this.Mask |= 1 << channel | 0;
        }

        public void EnableAll()
        {

            this.Mask = 0xffffffff | 0;
        }

        public void Toggle(int channel ) {
            this.Mask ^= 1 << channel | 0;
        }

        public void Disable(int channel ) {
            this.Mask &= ~ ( 1 << channel | 0 );
        }
        public void DisableAll() {
            this.Mask = 0;
        }

        public bool Test(Layers layers ) {
            return ( this.Mask & layers.Mask ) != 0;
        }
        #endregion
    }
}