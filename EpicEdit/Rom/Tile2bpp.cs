﻿#region GPL statement
/*Epic Edit is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

using System;
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom
{
    internal struct Tile2bppProperties
    {
        private int paletteIndex;
        public int PaletteIndex
        {
            get { return this.paletteIndex; }
            set
            {
                if (value < 0 || value > 15)
                {
                    throw new ArgumentOutOfRangeException("value", "The palette index value should be between 0 and 15.");
                }

                this.paletteIndex = value;
            }
        }

        private int subPaletteIndex;
        public int SubPaletteIndex
        {
            get { return this.subPaletteIndex; }
            set
            {
                if (value != 0 && value != 4 && value != 8 && value != 12)
                {
                    throw new ArgumentOutOfRangeException("value", "The sub palette index value should be positive, a multiple of 4, and no higher than 12.");
                }

                this.subPaletteIndex = value;
            }
        }

        private Flip flip;
        public Flip Flip
        {
            get { return this.flip; }
            set { this.flip = value; }
        }

        public Tile2bppProperties(byte data)
        {
            byte flipMask = (byte)(Flip.X | Flip.Y);
            byte paletteData = (byte)(data &~ flipMask);

            if ((paletteData & 0x03) != 0)
            {
                throw new ArgumentOutOfRangeException("data", "Invalid 2bpp tile property data. The 2 lower bits should be 0.");
            }

            this.paletteIndex = (paletteData & 0x30) >> 4;
            this.subPaletteIndex = (paletteData & 0xC);
            this.flip = (Flip)(data & flipMask);
        }

        public void FlipX()
        {
            this.FlipXY(Flip.X);
        }

        public void FlipY()
        {
            this.FlipXY(Flip.Y);
        }

        private void FlipXY(Flip value)
        {
            if ((this.flip & value) != 0)
            {
                this.flip ^= value;
            }
            else
            {
                this.flip |= value;
            }
        }

        public byte GetByte()
        {
            return (byte)((byte)this.flip | (this.paletteIndex << 4) | this.subPaletteIndex);
        }
    }

    /// <summary>
    /// A 2-bit per pixel tile.
    /// </summary>
    internal class Tile2bpp : Tile
    {
        public override Palette Palette
        {
            get { return this.palettes[this.properties.PaletteIndex]; }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.properties.PaletteIndex = value.Index;
            }
        }

        private Palettes palettes;
        public Palettes Palettes
        {
            get { return this.palettes; }
            set
            {
                if (this.palettes == value)
                {
                    return;
                }

                this.palettes = value;

                if (value != null)
                {
                    this.UpdateBitmap();
                }
            }
        }

        protected Tile2bppProperties properties;

        private RomColor[] GetSubPalette()
        {
            int subPalIndex = this.properties.SubPaletteIndex;

            return new RomColor[]
            {
                this.Palette[subPalIndex],
                this.Palette[subPalIndex + 1],
                this.Palette[subPalIndex + 2],
                this.Palette[subPalIndex + 3]
            };
        }

        public Tile2bpp(byte[] gfx, Palettes palettes)
        {
            this.Init(gfx, palettes, 0);
        }

        public Tile2bpp(byte[] gfx, byte properties)
        {
            this.Init(gfx, null, properties);
        }

        public Tile2bpp(byte[] gfx, Palettes palettes, byte properties)
        {
            this.Init(gfx, palettes, properties);
        }

        private void Init(byte[] gfx, Palettes palettes, byte properties)
        {
            this.Graphics = gfx;
            this.properties = new Tile2bppProperties(properties);
            this.Palettes = palettes;
        }

        protected override void GenerateBitmap()
        {
            if (this.Palettes == null)
            {
                throw new InvalidOperationException("Cannot generate Bitmap as the Palettes have not been set.");
            }
            this.bitmap = GraphicsConverter.GetBitmapFrom2bppPlanar(this.Graphics, this.Palettes, this.properties);
        }

        protected override void GenerateGraphics()
        {
            FastBitmap fBitmap = new FastBitmap(this.bitmap);
            RomColor[] palette = this.GetSubPalette();

            for (int y = 0; y < Tile.Size; y++)
            {
                byte val1 = 0;
                byte val2 = 0;
                for (int x = 0; x < Tile.Size; x++)
                {
                    int xPos = (Tile.Size - 1) - x;
                    RomColor color = (RomColor)fBitmap.GetPixel(xPos, y);
                    int colorIndex = Utilities.GetColorIndex(color, palette);
                    val1 |= (byte)((colorIndex & 0x01) << x);
                    val2 |= (byte)(((colorIndex & 0x02) << x) >> 1);
                }

                this.Graphics[y * 2] = val1;
                this.Graphics[y * 2 + 1] = val2;
            }

            fBitmap.Release();

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            this.GenerateBitmap();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            if ((this.properties.Flip & Flip.X) == 0)
            {
                x = (Tile.Size - 1) - x;
            }

            if ((this.properties.Flip & Flip.Y) != 0)
            {
                y = (Tile.Size - 1) - y;
            }

            byte val1 = this.Graphics[(y * 2)];
            byte val2 = this.Graphics[(y * 2) + 1];
            int mask = 1 << x;
            int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
            return this.properties.SubPaletteIndex + colIndex;
        }
    }
}
