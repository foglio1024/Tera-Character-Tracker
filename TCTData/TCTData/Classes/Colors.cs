using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TCTData
{
    public static class Colors
    {
        public static Color SolidBaseColor = Color.FromArgb(255, 0, 123, 206);
        public static Color SolidAccentColor = Color.FromArgb(255, 255, 120, 42);
        public static Color SolidYellow = Color.FromArgb(255, 255, 166, 77);
        public static Color SolidGreen = Color.FromArgb(255, 90, 180, 110);
        public static Color SolidRed = Color.FromArgb(255, 255, 80, 80);
        public static Color SolidGray = Color.FromArgb(255, 200, 200, 200);

        static byte alpha = 150;
        public static Color FadedBaseColor = Color.FromArgb(alpha, SolidBaseColor.R, SolidBaseColor.G, SolidBaseColor.B);
        public static Color FadedAccentColor = Color.FromArgb(alpha, SolidAccentColor.R, SolidAccentColor.G, SolidAccentColor.B);
        public static Color FadedYellow = Color.FromArgb(alpha, SolidYellow.R, SolidYellow.G, SolidYellow.B);
        public static Color FadedGreen = Color.FromArgb(alpha, SolidGreen.R, SolidGreen.G, SolidGreen.B);
        public static Color FadedRed = Color.FromArgb(alpha, SolidRed.R, SolidRed.G, SolidRed.B);
        public static Color FadedGray = Color.FromArgb(alpha, SolidGray.R, SolidGray.G, SolidGray.B);

        public static Color BrightGreen = Color.FromArgb(255, 0, 255, 100);
        public static Color BrightRed = System.Windows.Media.Colors.Red;
    }

}
