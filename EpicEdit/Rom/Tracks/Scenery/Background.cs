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
using System.Drawing;
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks.Scenery
{
    /// <summary>
    /// Represents the background of a track.
    /// </summary>
    internal class Background : IDisposable
    {
        private const int FrontPaletteStart = 4;
        private const int BackPaletteStart = 6;

        private BackgroundTileset tileset;
        private BackgroundLayout layout;

        public Background(BackgroundTileset tileset, BackgroundLayout layout)
        {
            this.tileset = tileset;
            this.layout = layout;
        }

        public Bitmap GetFrontTileBitmap(int x, int y)
        {
            return this.GetTileBitmap(true, x, y);
        }

        public Bitmap GetBackTileBitmap(int x, int y)
        {
            return this.GetTileBitmap(false, x, y);
        }

        private Bitmap GetTileBitmap(bool front, int x, int y)
        {
            byte tileId;
            byte properties;
            this.layout.GetTileData(front, x, y, out tileId, out properties);
            Tile2bpp tile = this.tileset[tileId];
            return Background.GetTileBitmap(tile, properties, front);
        }

        private static Bitmap GetTileBitmap(Tile2bpp tile, byte properties, bool front)
        {
            Tile2bppProperties props = new Tile2bppProperties(properties);
            int start = front ? FrontPaletteStart : BackPaletteStart;
            props.PaletteIndex += front ? FrontPaletteStart : BackPaletteStart;

            return GraphicsConverter.GetBitmapFrom2bppPlanar(tile.Graphics, tile.Palettes, props);
        }

        public void Dispose()
        {
            this.tileset.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
