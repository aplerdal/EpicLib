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

using System.ComponentModel;

namespace EpicLib;

/// <summary>
///     An 8x8 graphic tile.
/// </summary>
public abstract class Tile : IDisposable, INotifyPropertyChanged
{
    public const int Size = 8;

    private byte[] _graphics;

    private Palette _palette;

    public virtual Palette Palette
    {
        get => _palette;
        set
        {
            if (_palette == value) return;

            _palette = value;

            OnPropertyChanged(PropertyNames.Tile.Palette);
        }
    }

    public byte[] Graphics
    {
        get => _graphics;
        set
        {
            _graphics = value;
            OnPropertyChanged(PropertyNames.Tile.Graphics);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public abstract int GetColorIndexAt(int x, int y);

    public virtual bool Contains(int colorIndex)
    {
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
            if (GetColorIndexAt(x, y) == colorIndex)
                return true;

        return false;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}