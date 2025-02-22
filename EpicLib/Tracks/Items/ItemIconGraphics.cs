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

using EpicLib.Compression;
using EpicLib.Utility;

namespace EpicLib.Tracks.Items;

/// <summary>
///     Item icon graphics manager.
/// </summary>
public sealed class ItemIconGraphics : IDisposable
{
    private readonly Tile[][] _tiles;
    private readonly Tile2bpp _topBorder;

    public ItemIconGraphics(byte[] romBuffer, Offsets offsets)
    {
        var itemGfx = Codec.Decompress(romBuffer, offsets[Offset.ItemIconGraphics]);
        var itemCount = Enum.GetValues(typeof(ItemType)).Length;
        var startOffset = offsets[Offset.ItemIconTileLayout];
        _tiles = new Tile2bpp[itemCount][];

        for (var i = 0; i < itemCount; i++)
        {
            var offset = startOffset + i * 2;
            _tiles[i] = GetTiles(romBuffer, offset, itemGfx);
        }

        var topBorderOffset = offsets[Offset.TopBorderTileLayout];
        var tileIndex = (byte)(romBuffer[topBorderOffset] & 0x7F);
        var properties = romBuffer[topBorderOffset + 1];
        _topBorder = GetTile(tileIndex, properties, itemGfx);
    }

    public void Dispose()
    {
        foreach (var tiles in _tiles)
        foreach (var tile in tiles)
            tile.Dispose();

        GC.SuppressFinalize(this);
    }

    private static Tile[] GetTiles(byte[] romBuffer, int offset, byte[] itemGfx)
    {
        var tileIndex = (byte)(romBuffer[offset] & 0x7F);
        var properties = romBuffer[offset + 1];

        Tile[] tiles = new Tile2bpp[4];

        for (var i = 0; i < tiles.Length; i++) tiles[i] = GetTile((byte)(tileIndex + i), properties, itemGfx);

        return tiles;
    }

    private static Tile2bpp GetTile(byte tileIndex, byte properties, byte[] itemGfx)
    {
        const int bytesPerTile = 16;
        var gfx = Utilities.ReadBlock(itemGfx, tileIndex * bytesPerTile, bytesPerTile);
        return new Tile2bpp(gfx, properties);
    }

    private Tile[] GetTiles(ItemType type)
    {
        return _tiles[(int)type];
    }

    public Tile GetTile(ItemType type, int x, int y)
    {
        var tiles = _tiles[(int)type];
        var subIndex = y * 2 + x;
        return tiles[subIndex];
    }
}