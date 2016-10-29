using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera
{
    public static class UI
    {
        static public TeraMainWindow MainWin;
        static public void UpdateLog(string data)
        {
            MainWin.UpdateLog(data);
        }

        public static class Colors
        {
            public static Color SolidBaseColor = Color.FromArgb(255, 0x60, 0x7B, 0x8D);
            public static Color SolidAccentColor = Color.FromArgb(255, 255, 120, 42);
            public static Color SolidYellow = Color.FromArgb(255, 255, 166, 77);
            public static Color SolidGreen = Color.FromArgb(255, 88, 180, 91);
            public static Color SolidRed = Color.FromArgb(255, 255, 80, 80);
            public static Color SolidGray = Color.FromArgb(255, 200, 200, 200);

            public static Color FadedBaseColor = Color.FromArgb(150, 0x60, 0x7B, 0x8D);
            public static Color FadedAccentColor = Color.FromArgb(150, 255, 120, 42);
            public static Color FadedYellow = Color.FromArgb(150, 255, 166, 77);
            public static Color FadedGreen = Color.FromArgb(150, 88, 180, 91);
            public static Color FadedRed = Color.FromArgb(150, 255, 80, 80);
            public static Color FadedGray = Color.FromArgb(150, 200, 200, 200);

        }


    }
}
