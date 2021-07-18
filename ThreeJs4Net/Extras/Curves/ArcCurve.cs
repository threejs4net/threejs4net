/*------
 * R116
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeJs4Net.Extras.Curves
{
    public class ArcCurve : EllipseCurve
    {
        public ArcCurve(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise ): 
            base(aX, aY, aRadius, aRadius, aStartAngle, aEndAngle, aClockwise)
        {
        }
    }
}
