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

namespace EpicLib.Tracks.Objects;

/// <summary>
///     Track object graphics manager.
/// </summary>
public sealed class TrackObjectGraphics : IDisposable
{
    private readonly Tile[][] _tiles;

    public TrackObjectGraphics(byte[] romBuffer, Offsets offsets)
    {
        var typeCount = Enum.GetValues(typeof(TrackObjectType)).Length;
        var count = typeCount + 2; // + 2 to account for moving Match Race object and items
        _tiles = new TrackObjectTile[count][];
        var offsetLocation = offsets[Offset.TrackObjectGraphics];
        byte[] tilesetGfx;
        int[] tileIndexes;

        for (var i = 0; i < typeCount; i++)
        {
            var type = (TrackObjectType)i;
            var offset = GetGraphicsOffset(type, romBuffer, offsetLocation);
            tilesetGfx = Codec.Decompress(romBuffer, offset);
            tileIndexes = GetTileIndexes(type);
            _tiles[i] = GetTiles(tilesetGfx, tileIndexes);
        }

        tileIndexes = GetMatchRaceTileIndexes();

        tilesetGfx = Codec.Decompress(romBuffer, offsets[Offset.MatchRaceObjectGraphics]);
        _tiles[_tiles.Length - 2] = GetTiles(tilesetGfx, tileIndexes);

        tilesetGfx = Codec.Decompress(romBuffer, offsets[Offset.ItemGraphics]);
        _tiles[_tiles.Length - 1] = GetTiles(tilesetGfx, tileIndexes);
    }

    public void Dispose()
    {
        foreach (var tiles in _tiles)
        foreach (var tile in tiles)
            tile.Dispose();

        GC.SuppressFinalize(this);
    }

    private static Tile[] GetTiles(byte[] tilesetGfx, int[] tileIndexes)
    {
        Tile[] tiles = new TrackObjectTile[tileIndexes.Length];

        for (var i = 0; i < tileIndexes.Length; i++)
        {
            var gfx = Utilities.ReadBlock(tilesetGfx, tileIndexes[i], 32);
            tiles[i] = new TrackObjectTile(gfx);
        }

        return tiles;
    }

    private Tile[] GetTiles(TrackObjectType tileset)
    {
        return _tiles[(int)tileset];
    }

    private int GetMatchRaceTileIndex(bool moving)
    {
        return moving ? _tiles.Length - 2 : _tiles.Length - 1;
    }

    private static int[] GetTileIndexes(TrackObjectType type)
    {
        if (type == TrackObjectType.Plant || type == TrackObjectType.Fish)
            return new[] { 4 * 32, 5 * 32, 20 * 32, 21 * 32 };

        return new[] { 32 * 32, 33 * 32, 48 * 32, 49 * 32 };
    }

    private static int[] GetMatchRaceTileIndexes()
    {
        return new[] { 0, 32, 64, 96 };
    }

    public Tile GetTile(GPTrack track, TrackObject trackObject, int x, int y)
    {
        int index;

        if (!(trackObject is TrackObjectMatchRace matchRaceObject))
        {
            index = (int)track.Objects.Tileset;
        }
        else
        {
            var moving = matchRaceObject.Direction != TrackObjectDirection.None;
            index = GetMatchRaceTileIndex(moving);
        }

        var tiles = _tiles[index];
        var subIndex = y * 2 + x;
        return tiles[subIndex];
    }

    private static int GetGraphicsOffset(TrackObjectType tileset, byte[] romBuffer, int offsetLocation)
    {
        switch (tileset)
        {
            case TrackObjectType.Pipe:
                offsetLocation += 3;
                break;

            case TrackObjectType.Thwomp:
                offsetLocation += 18;
                break;

            case TrackObjectType.Mole:
                offsetLocation += 6;
                break;

            case TrackObjectType.Plant:
                offsetLocation += 9;
                break;

            case TrackObjectType.Fish:
                offsetLocation += 15;
                break;

            case TrackObjectType.RThwomp:
                offsetLocation += 21;
                break;
        }

        return Utilities.BytesToOffset(romBuffer, offsetLocation);
    }
}