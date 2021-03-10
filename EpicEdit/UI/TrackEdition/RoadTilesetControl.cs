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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a list of <see cref="Theme"/> objects, and the tileset of the selected theme.
    /// </summary>
    internal partial class RoadTilesetControl : UserControl
    {
        #region Events
        /// <summary>
        /// Raised when the selected theme has been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SelectedThemeChanged;

        /// <summary>
        /// Raised when a new tile has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SelectedTileChanged;

        /// <summary>
        /// Raised when the track map has been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> TrackMapChanged;

        /// <summary>
        /// Raised when tile graphics have been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<byte>> TileChanged;

        /// <summary>
        /// Raised when the theme tileset has been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> TilesetChanged;

        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add => this.tilesetPanel.ColorSelected += value;
            remove => this.tilesetPanel.ColorSelected -= value;
        }
        #endregion Events

        /// <summary>
        /// Used to draw the tileset.
        /// </summary>
        private RoadTilesetDrawer drawer;

        private Track track;

        /// <summary>
        /// Flag to differentiate user actions and automatic actions.
        /// </summary>
        private bool fireEvents;

        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track
        {
            get => this.track;
            set
            {
                if (this.track == value)
                {
                    return;
                }

                if (this.track != null)
                {
                    this.track.ColorGraphicsChanged -= this.track_ColorsGraphicsChanged;
                    this.track.ColorsGraphicsChanged -= this.track_ColorsGraphicsChanged;
                    this.track.PropertyChanged -= this.track_PropertyChanged;
                }

                this.track = value;

                this.track.ColorGraphicsChanged += this.track_ColorsGraphicsChanged;
                this.track.ColorsGraphicsChanged += this.track_ColorsGraphicsChanged;
                this.track.PropertyChanged += this.track_PropertyChanged;

                this.SelectTrackTheme();
            }
        }

        private byte selectedTile;

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte SelectedTile
        {
            get => this.selectedTile;
            set
            {
                if (this.selectedTile == value)
                {
                    return;
                }

                this.fireEvents = false;
                this.selectedTile = value;
                this.SetCurrentTile();
                this.fireEvents = true;
            }
        }

        private RoadTile SelectedRoadTile => this.track.RoadTileset[this.selectedTile];

        public RoadTilesetControl()
        {
            this.InitializeComponent();

            this.tilesetPanel.Zoom = RoadTilesetDrawer.Zoom;
        }

        private void track_ColorsGraphicsChanged(object sender, EventArgs e)
        {
            if ((sender as Palette).Index < Palettes.SpritePaletteStart)
            {
                this.UpdateTileset();
            }
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.Track.Theme)
            {
                this.SelectTrackTheme();
            }
        }

        public void InitOnFirstRomLoad()
        {
            this.drawer = new RoadTilesetDrawer(this.tilesetPanel.Size);

            // The following event handler is added here rather than in the Designer.cs
            // to save us a null check on this.drawer in each of the corresponding functions,
            // because the drawer doesn't exist yet before a ROM is loaded.
            this.tilesetPanel.Paint += this.TilesetPanelPaint;

            // The following event handler is added here rather than in the Designer.cs
            // to avoid an extra repaint triggered by
            // selecting the current theme in the theme ComboBox.
            this.themeComboBox.SelectedIndexChanged += this.ThemeComboBoxSelectedIndexChanged;

            this.InitTileGenreComboBox();

            this.InitOnRomLoad();
        }

        public void InitOnRomLoad()
        {
            this.themeComboBox.Init();
        }

        private void InitTileGenreComboBox()
        {
            this.tileGenreComboBox.DataSource = Enum.GetValues(typeof(RoadTileGenre));
            this.tileGenreComboBox.SelectedIndexChanged += this.TileGenreComboBoxSelectedIndexChanged;
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.track.Theme = this.themeComboBox.SelectedTheme;

            this.tilesetPanel.Tileset = this.track.RoadTileset;
            this.ResetTileset();
            this.SetCurrentTile();

            this.SelectedThemeChanged(this, EventArgs.Empty);
        }

        private void SetCurrentTile()
        {
            this.SelectTileGenre();
            this.SelectTilePalette();
            this.tilesetPanel.Invalidate();
        }

        private void TileGenreComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.SelectedRoadTile.Genre = (RoadTileGenre)this.tileGenreComboBox.SelectedItem;
        }

        private void TilePaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            int palIndex = (int)this.tilePaletteNumericUpDown.Value;
            this.SelectedRoadTile.Palette = this.track.Theme.Palettes[palIndex];

            // Could be optimized by not updating the whole cache,
            // and not repainting the whole panel (but it's already fast enough)
            this.UpdateTileset();

            this.TileChanged(this, new EventArgs<byte>(this.selectedTile));
        }

        public void UpdateTileset()
        {
            this.ResetTileset();
            this.tilesetPanel.Refresh();
        }

        public void SetTheme(int number)
        {
            Base1NumericUpDown ud = this.tilePaletteNumericUpDown;
            if (number >= ud.Minimum && number <= ud.Maximum)
            {
                ud.Value = number;
            }
        }

        public void SelectPenTool()
        {
            this.pencilButton.PerformClick();
        }

        public void SelectPaintBucketTool()
        {
            this.bucketButton.PerformClick();
        }

        private void ResetTileset()
        {
            this.drawer.Tileset = this.track.RoadTileset;
        }

        private void SelectTrackTheme()
        {
            this.fireEvents = false;
            this.themeComboBox.SelectedItem = this.track.Theme;
            this.fireEvents = true;
        }

        private void SelectTileGenre()
        {
            this.tileGenreComboBox.SelectedItem = this.SelectedRoadTile.Genre;
        }

        private void SelectTilePalette()
        {
            this.tilePaletteNumericUpDown.Value = this.SelectedRoadTile.Palette.Index;
        }

        private void TilesetPanelPaint(object sender, PaintEventArgs e)
        {
            this.drawer.DrawTileset(e.Graphics, this.selectedTile);
        }

        private void TilesetPanelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            const int Zoom = RoadTilesetDrawer.Zoom;
            int rowTileCount = this.tilesetPanel.Width / (Tile.Size * Zoom);
            byte newSelectedTile = (byte)((e.X / (Tile.Size * Zoom)) + (e.Y / (Tile.Size * Zoom)) * rowTileCount);

            if (this.selectedTile != newSelectedTile)
            {
                this.SelectedTile = newSelectedTile;

                this.SelectedTileChanged(this, EventArgs.Empty);
            }
        }

        private void TileGenreComboBoxFormat(object sender, ListControlConvertEventArgs e)
        {
            object val = e.Value;
            e.Value = Utilities.ByteToHexString((byte)val) + "- " + UITools.GetDescription(val);
        }

        private void ImportExportRoadTilesetButtonClick(object sender, EventArgs e)
        {
            using (RoadTilesetImportExportForm form = new RoadTilesetImportExportForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.Action == RoadTilesetImportExportAction.Import)
                    {
                        switch (form.Type)
                        {
                            case RoadTilesetImportExportType.Graphics:
                                this.ShowImportTilesetGraphicsDialog();
                                break;

                            case RoadTilesetImportExportType.Genres:
                                this.ShowImportTilesetGenresDialog();
                                break;

                            case RoadTilesetImportExportType.Palettes:
                                this.ShowImportTilesetPalettesDialog();
                                break;
                        }
                    }
                    else
                    {
                        switch (form.Type)
                        {
                            case RoadTilesetImportExportType.Graphics:
                                this.ShowExportTilesetGraphicsDialog();
                                break;

                            case RoadTilesetImportExportType.Genres:
                                this.ShowExportTilesetGenresDialog();
                                break;

                            case RoadTilesetImportExportType.Palettes:
                                this.ShowExportTilesetPalettesDialog();
                                break;
                        }
                    }
                }
            }
        }

        private void ShowImportTilesetGraphicsDialog()
        {
            if (UITools.ShowImportTilesetGraphicsDialog(this.track.RoadTileset.GetTiles()))
            {
                this.UpdateTileset();
                this.TilesetChanged(this, EventArgs.Empty);
            }
        }

        private void ShowImportTilesetGenresDialog()
        {
            if (UITools.ShowImportBinaryDataDialog(this.track.RoadTileset.SetTileGenreBytes))
            {
                this.SelectTileGenre();
            }
        }

        private void ShowImportTilesetPalettesDialog()
        {
            if (UITools.ShowImportBinaryDataDialog(this.track.RoadTileset.SetTilePaletteBytes))
            {
                this.UpdateTileset();
                this.SelectTilePalette();
                this.TilesetChanged(this, EventArgs.Empty);
            }
        }

        private void ShowExportTilesetGraphicsDialog()
        {
            UITools.ShowExportTilesetGraphicsDialog(this.drawer.Image, this.track.Theme.RoadTileset.GetTiles(), this.track.Theme.Name + "road gfx");
        }

        private void ShowExportTilesetGenresDialog()
        {
            UITools.ShowExportBinaryDataDialog(this.track.RoadTileset.GetTileGenreBytes, this.track.Theme.Name + "road types");
        }

        private void ShowExportTilesetPalettesDialog()
        {
            UITools.ShowExportBinaryDataDialog(this.track.RoadTileset.GetTilePaletteBytes, this.track.Theme.Name + "road pals");
        }

        private void ResetMapButtonClick(object sender, EventArgs e)
        {
            DialogResult result = UITools.ShowWarning("Do you really want to reset the map?");

            if (result == DialogResult.Yes)
            {
                this.track.Map.Clear(this.selectedTile);
                this.TrackMapChanged(this, EventArgs.Empty);
            }
        }

        public bool BucketMode => this.bucketButton.Checked;

        private sealed class TilesetPanel : TilePanel
        {
            [Browsable(false), DefaultValue(typeof(RoadTileset), "")]
            public RoadTileset Tileset { get; set; }

            private int TilesPerRow => (int)(this.Width / (Tile.Size * this.Zoom));

            protected override Tile GetTileAt(int x, int y)
            {
                // Convert from pixel precision to tile precision
                x /= Tile.Size;
                y /= Tile.Size;

                int index = y * this.TilesPerRow + x;
                return this.Tileset[index];
            }
        }
    }
}
