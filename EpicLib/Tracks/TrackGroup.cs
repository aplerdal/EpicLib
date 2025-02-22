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

using System.Collections;
using System.ComponentModel;
using EpicLib.Settings;

namespace EpicLib.Tracks;

/// <summary>
///     A group of tracks.
/// </summary>
public class TrackGroup : IEnumerable<Track>, INotifyPropertyChanged
{
    private readonly Track[] _tracks;

    public TrackGroup(SuffixedTextItem nameItem, Track[] tracks)
    {
        SuffixedNameItem = nameItem;
        SuffixedNameItem.PropertyChanged += SuffixedNameItem_PropertyChanged;
        _tracks = tracks;

        foreach (var track in tracks) track.PropertyChanged += OnPropertyChanged;
    }

    public SuffixedTextItem SuffixedNameItem { get; }

    public string Name => SuffixedNameItem.Value;

    public bool Modified
    {
        get
        {
            foreach (var track in _tracks)
                if (track.Modified)
                    return true;

            return false;
        }
    }

    public int Count => _tracks.Length;

    public Track this[int index]
    {
        get => _tracks[index];
        set => _tracks[index] = value;
    }

    public IEnumerator<Track> GetEnumerator()
    {
        foreach (var track in _tracks) yield return track;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tracks.GetEnumerator();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SuffixedNameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(PropertyNames.Track.SuffixedNameItem);
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(sender, e);
    }

    public void ResetModifiedState()
    {
        foreach (var track in _tracks) track.ResetModifiedState();
    }
}