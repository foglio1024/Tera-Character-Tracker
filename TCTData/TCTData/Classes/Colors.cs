using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;

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

        public static Color DarkTheme_Background = Color.FromArgb(0xff, 0x1e, 0x21, 0x24);
        public static Color DarkTheme_Card = Color.FromArgb(0xff, 0x36, 0x39, 0x3e);
        public static Color DarkTheme_Bar = Color.FromArgb(0xff, 0x2e, 0x31, 0x36);
        public static Color DarkTheme_Foreground1 = Color.FromArgb(255,255,255,255);
        public static Color DarkTheme_Foreground2 = Color.FromArgb(178,255,255,255);
        public static Color DarkTheme_Foreground3 = Color.FromArgb(127,255,255,255);
        public static Color DarkTheme_Dividers = Color.FromArgb(30, 255, 255, 255);

        public static Color LightTheme_Background = Color.FromArgb(0xff, 0xf5, 0xf5, 0xf5);
        public static Color LightTheme_Card = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        public static Color LightTheme_Bar = Color.FromArgb(0xff, 0xf0, 0xf0, 0xf0);
        public static Color LightTheme_Foreground1 = Color.FromArgb(221,0,0,0);
        public static Color LightTheme_Foreground2 = Color.FromArgb(137,0,0,0);
        public static Color LightTheme_Foreground3 = Color.FromArgb(96,0,0,0);
        public static Color LightTheme_Dividers = Color.FromArgb(30, 0, 0, 0);



    }

    public static class Shadows
    {
        public static DropShadowEffect LightThemeShadow = new DropShadowEffect { ShadowDepth = .5, Opacity = .2, BlurRadius = 4 };
        public static DropShadowEffect DarkThemeShadow = new DropShadowEffect { ShadowDepth = 1, Opacity = .5, BlurRadius = 8 };
    }
}
