using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leap_C
{
    public static class CoordinateConverter
    {
        public static int TranslateToLeft(decimal x, int width)
        {
            const decimal minX = -150;
            const decimal maxX = 150;
            decimal deviceWidth = Math.Abs(minX) + Math.Abs(maxX);
            decimal multiplier = width / deviceWidth;
            decimal normalizedLeft = x + Math.Abs(minX);
            return Convert.ToInt32(normalizedLeft * multiplier);
        }
    }
}
