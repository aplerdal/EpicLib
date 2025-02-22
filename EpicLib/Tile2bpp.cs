#region GPL statement

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

namespace EpicLib;

/// <summary>
///     A 2-bit per pixel tile.
/// </summary>
public class Tile2bpp : Tile
{
    private Palettes _palettes;

    private Tile2bppProperties _properties;

    public Tile2bpp(byte[] gfx, Palettes palettes) : this(gfx, palettes, 0)
    {
    }

    public Tile2bpp(byte[] gfx, byte properties) : this(gfx, null, properties)
    {
    }

    public Tile2bpp(byte[] gfx, Palettes palettes, byte properties)
    {
        Graphics = gfx;
        Properties = new Tile2bppProperties(properties);
        Palettes = palettes;
    }

    public override Palette Palette
    {
        get => _palettes?[Properties.PaletteIndex];
        set
        {
            if (value == null) return;

            Properties = new Tile2bppProperties
            {
                PaletteIndex = value.Index,
                SubPaletteIndex = Properties.SubPaletteIndex,
                Flip = Properties.Flip
            };

            base.Palette = Palette;
        }
    }

    public Palettes Palettes
    {
        get => _palettes;
        set
        {
            if (_palettes == value) return;

            _palettes = value;
            SetPalette();
        }
    }

    public virtual Tile2bppProperties Properties
    {
        get => _properties;
        set
        {
            if (_properties == value) return;

            _properties = value;
            SetPalette();
        }
    }

    protected void SetPalette()
    {
        if (base.Palette != Palette)
            // Setting the base Palette lets us listen to the palette color change events,
            // and will update the Bitmap if the Palette has changed.
            base.Palette = Palette;
    }

    private RomColor[] GetSubPalette()
    {
        var palette = Palette;
        var subPalIndex = Properties.SubPaletteIndex;

        return new[]
        {
            Palettes.BackColor,
            palette[subPalIndex + 1],
            palette[subPalIndex + 2],
            palette[subPalIndex + 3]
        };
    }


    public override int GetColorIndexAt(int x, int y)
    {
        if ((Properties.Flip & TileFlip.X) == 0) x = Size - 1 - x;

        if ((Properties.Flip & TileFlip.Y) != 0) y = Size - 1 - y;

        var val1 = Graphics[y * 2];
        var val2 = Graphics[y * 2 + 1];
        var mask = 1 << x;
        var colorIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
        return Properties.SubPaletteIndex + colorIndex;
    }

    public override bool Contains(int colorIndex)
    {
        // Tile2bpp instances have transparent pixels where the color 0 is,
        // so consider they don't contain it. This lets us avoid unnecessarily recreating
        // the tile image when the color 0 is changed.
        return colorIndex != 0 && base.Contains(colorIndex);
    }
}