using System;
using System.Drawing;

namespace Izume.Mobile.Odeo.Common
{
    /// <summary>
    /// Summary description for Constants.
    /// </summary>
    public class Constants
    {
        public class Colors
        {
            public static Color LightBlue = Color.FromArgb(231, 242, 246);
            public static Color MediumBlue = Color.FromArgb(208, 230, 236);
            public static Color DarkBlue = Color.FromArgb(74, 133, 182);
            public static Color HotPink = Color.FromArgb(254, 64, 159);            
        }

        public class Fonts
        {
            public static Font Small = new Font("Nina", 8.0F, FontStyle.Regular);
            public static Font SmallEmphasis = new Font("Nina", 8.0F, FontStyle.Bold);
            public static Font Normal = new Font("Nina", 9.0F, FontStyle.Regular);
            public static Font Emphasis = new Font("Nina", 9.0F, FontStyle.Bold);
        }
    }
}
