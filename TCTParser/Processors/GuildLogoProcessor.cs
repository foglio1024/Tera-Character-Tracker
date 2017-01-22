using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tera;

namespace TCTParser.Processors
{
    class GuildLogoProcessor
    {

        const int OFFSET_OFFSET = 4 * 2;
        const int SIZE_OFFSET = 6 * 2;
        const int CHAR_ID_OFFSET = 8 * 2;
        const int GUILD_ID_OFFSET = 12 * 2;

        internal Bitmap GetLogo(string p)
        {
            var size = GetSize(p);
            var offset = GetOffset(p);

            var logoData = StringUtils.StringToByteArray(p.Substring(offset * 2, size * 2));

            var GuildLogo = new Bitmap(64, 64, PixelFormat.Format8bppIndexed);
            var paletteSize = (size - 0x1018) / 3;

            if (paletteSize > 0x100 || paletteSize < 1)
            {
                Console.WriteLine("Missed guild logo format");
                return null;
            }
            var palette = GuildLogo.Palette;
            for (var i = 0; i < paletteSize; i++)
            {
                palette.Entries[i] = Color.FromArgb(logoData[0x14 + i * 3], logoData[0x15 + i * 3], logoData[0x16 + i * 3]);
            }

            var pixels = GuildLogo.LockBits(new Rectangle(0, 0, 64, 64), ImageLockMode.WriteOnly, GuildLogo.PixelFormat);
            Marshal.Copy(logoData, size - 0x1000, pixels.Scan0, 0x1000);
            GuildLogo.UnlockBits(pixels);
            GuildLogo.Palette = palette;

            return GuildLogo;

        }

        int GetOffset(string p)
        {
            return StringUtils.Hex2BStringToInt(p.Substring(OFFSET_OFFSET));
        }

        int GetSize(string p)
        {
            return StringUtils.Hex2BStringToInt(p.Substring(SIZE_OFFSET));
        }

        internal uint GetGuildID(string p)
        {
            return (uint)StringUtils.Hex4BStringToInt(p.Substring(GUILD_ID_OFFSET));
        }

        uint GetCharID(string p)
        {
            return (uint)StringUtils.Hex4BStringToInt(p.Substring(CHAR_ID_OFFSET));
        }

    }
}
