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

using EpicEdit.Rom;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Tools;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace EpicEdit.UI
{
    /// <summary>
    /// The main window of the program.
    /// </summary>
    internal sealed partial class MainForm : Form
    {
        /// <summary>
        /// The state of the window before going full screen, to restore it later.
        /// </summary>
        private FormWindowState _previousWindowState;

        [STAThread]
        public static void Main(string[] args)
        {
            if (Platform.IsMono)
            {
                // HACK: Swallow UI thread exceptions. This is needed to work around Mono bugs,
                // most notably the fact disabled controls still raise events on Mac (Carbon driver).
                // E.g: hovering the track map panel before loading a ROM makes the application crash,
                // because Mono raises events that shouldn't be raised at this point.
                // We can safely ignore such exceptions.
                Application.ThreadException += delegate { };
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Application.Run(new MainForm(args));
        }

        public MainForm(string[] args)
        {
            Text = Application.ProductName;
            InitializeComponent();

            if (args.Length > 0 && File.Exists(args[0]))
            {
                UITools.ImportData(OpenRom, args[0]);
            }
        }

        private void UpdateApplicationTitle()
        {
            Text = Context.Game.FileName + (!Context.Game.Modified ? null : "*") + " - " + Application.ProductName;
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (HasPendingChanges())
            {
                bool cancelExit = PromptToSaveRom();
                e.Cancel = cancelExit;
            }
        }

        /// <summary>
        /// Prompts user to save the ROM (called when there are pending changes).
        /// </summary>
        /// <returns>Whether the action (ie: closing the application, or opening another ROM) must be cancelled.</returns>
        private bool PromptToSaveRom()
        {
            bool cancelPreviousAction = false;

            DialogResult dialogResult = UITools.ShowWarning(
                "Do you want to save the changes to \"" + Context.Game.FileName + "\"?",
                MessageBoxButtons.YesNoCancel);

            switch (dialogResult)
            {
                case DialogResult.Yes:
                    SaveRom(Context.Game.FilePath);
                    break;

                case DialogResult.No:
                    break;

                case DialogResult.Cancel:
                    cancelPreviousAction = true;
                    break;
            }

            return cancelPreviousAction;
        }

        private static bool HasPendingChanges()
        {
            return Context.Game != null && Context.Game.Modified;
        }

        private void TrackEditorOpenRomDialogRequested(object sender, EventArgs e)
        {
            ShowOpenRomDialog();
        }

        private void ShowOpenRomDialog()
        {
            if (HasPendingChanges())
            {
                bool cancelOpen = PromptToSaveRom();
                if (cancelOpen)
                {
                    return;
                }
            }

            UITools.ShowImportDataDialog(OpenRom, FileDialogFilters.RomOrZippedRom);
        }

        private void OpenRom(string filePath)
        {
            // Do not directly set the Context.Game property,
            // in case the ROM is invalid (ie: Exception thrown in the Game constructor).
            Game game = new Game(filePath);

            if (Context.Game == null) // First ROM loading
            {
                Context.Game = game;
                trackEditor.InitOnFirstRomLoad();
            }
            else
            {
                Context.Game.PropertyChanged -= Context_Game_PropertyChanged;
                Context.Game.Dispose();
                Context.Game = null;
                Context.Game = game;
                trackEditor.InitOnRomLoad();
            }

            Context.Game.PropertyChanged += Context_Game_PropertyChanged;
            UpdateApplicationTitle();
        }

        private void Context_Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateApplicationTitle();
        }

        private void TrackEditorFileDragged(object sender, EventArgs<string> e)
        {
            string filePath = e.Value;

            if (filePath.EndsWith(".smkc", StringComparison.OrdinalIgnoreCase) ||
                filePath.EndsWith(".mkt", StringComparison.OrdinalIgnoreCase))
            {
                if (Context.Game == null)
                {
                    UITools.ShowError("You cannot load a track before having loaded a ROM.");
                }
                else
                {
                    trackEditor.ImportTrack(filePath);
                }
            }
            else
            {
                if (HasPendingChanges())
                {
                    bool cancelOpen = PromptToSaveRom();
                    if (cancelOpen)
                    {
                        return;
                    }
                }

                UITools.ImportData(OpenRom, filePath);
            }
        }

        private void TrackEditorSaveRomDialogRequested(object sender, EventArgs e)
        {
            ShowSaveRomDialog();
        }

        /// <summary>
        /// Calls the dialog to save the ROM.
        /// </summary>
        private void ShowSaveRomDialog()
        {
            string fileName = Context.Game.FileName;
            string ext = Path.GetExtension(fileName);

            // Make it so the loaded file extension is the default choice when resaving
            string filter = string.Format(FileDialogFilters.Rom, ext);

            fileName = Path.GetFileNameWithoutExtension(fileName);

            UITools.ShowExportDataDialog(SaveRom, fileName, filter);
        }

        private void SaveRom(string filePath)
        {
            Context.Game.SaveRom(filePath);
        }

        private void TrackEditorToggleScreenModeRequested(object sender, EventArgs e)
        {
            ToggleScreenMode();
        }

        private void ToggleScreenMode()
        {
            if (FormBorderStyle != FormBorderStyle.None)
            {
                // Go full screen
                FormBorderStyle = FormBorderStyle.None;
                _previousWindowState = WindowState;
                WindowState = FormWindowState.Maximized;

                // HACK: Toggle form visibility to make it cover the task bar.
                // On Windows XP: if the form was already maximized, the task bar wouldn't be covered.
                // If the form wasn't maximized, it would be covered but not repainted right away.
                Visible = false;
                Visible = true;
            }
            else
            {
                // Go back to windowed mode
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = _previousWindowState;
            }
        }
    }
}
