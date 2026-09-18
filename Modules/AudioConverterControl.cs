using BO3SoundSuite.Controls;
using BO3SoundSuite.Services;

namespace BO3SoundSuite.Modules;

public sealed class AudioConverterControl : UserControl
{
    private enum LogLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    private readonly DataGridView _grid = new();
    private readonly RoundedButton _add = new();
    private readonly RoundedButton _remove = new();
    private readonly RoundedButton _browseOutput = new();
    private readonly RoundedButton _convert = new();
    private readonly RoundedButton _clear = new();
    private readonly ModernCheckBox _backup = new();
    private readonly Label _title = new();
    private readonly Label _subtitle = new();
    private readonly Label _drop = new();
    private readonly Label _logLabel = new();
    private readonly CodeEditorBox _log = new();
    private readonly ModernToolTip _tip = new();
    private ModernGridHost? _gridHost;
    private bool _busy;

    public AudioConverterControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.AppBack;
        AllowDrop = true;
        DragEnter += OnDragEnter;
        DragDrop += OnDragDrop;
        BuildUi();
        ApplyLanguage();
    }

    private void BuildUi()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(30, 24, 30, 24),
            BackColor = Theme.AppBack
        };
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 178));
        Controls.Add(outer);

        var header = new Panel { Dock = DockStyle.Fill, BackColor = Theme.AppBack };
        _title.AutoSize = true;
        _title.Font = new Font("Segoe UI Semibold", 21, FontStyle.Bold);
        _title.ForeColor = Theme.Text;
        _title.Location = new Point(0, 0);
        _subtitle.AutoSize = true;
        _subtitle.Font = new Font("Segoe UI", 9.5f);
        _subtitle.ForeColor = Theme.Muted;
        _subtitle.Location = new Point(2, 40);
        header.Controls.AddRange(new Control[] { _title, _subtitle });
        outer.Controls.Add(header, 0, 0);

        var bar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 1,
            Padding = Padding.Empty,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty
        };
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14f));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12f));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18f));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17f));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 29f));
        ConfigureButton(_add, (_, _) => AddDialog(), false);
        ConfigureButton(_remove, (_, _) => RemoveSelected(), false);
        ConfigureButton(_browseOutput, (_, _) => BrowseOutputForSelection(), false);
        ConfigureButton(_clear, (_, _) => ClearAll(), false);
        ConfigureButton(_convert, async (_, _) => await ConvertAllAsync(), true);
        bar.Controls.Add(_add, 0, 0);
        bar.Controls.Add(_remove, 1, 0);
        bar.Controls.Add(_browseOutput, 2, 0);
        bar.Controls.Add(_clear, 3, 0);
        bar.Controls.Add(_convert, 4, 0);
        _backup.AutoSizeToText = false;
        _backup.Dock = DockStyle.Fill;
        _backup.ForeColor = Theme.Text;
        _backup.Font = new Font("Segoe UI", 9.2f);
        _backup.Margin = new Padding(12, 6, 0, 6);
        _backup.Checked = false;
        bar.Controls.Add(_backup, 5, 0);
        outer.Controls.Add(bar, 0, 1);

        _grid.Dock = DockStyle.Fill;
        Theme.StyleGrid(_grid);
        _grid.AllowUserToAddRows = false;
        _grid.MultiSelect = true;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add(new ModernToggleColumn
        {
            Name = "Do",
            HeaderText = "",
            Width = 58,
            MinimumWidth = 58,
            Resizable = DataGridViewTriState.False,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            ReadOnly = true
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Input", HeaderText = "Input", ReadOnly = true, Width = 350 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Output", HeaderText = "Output", Width = 350 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Format", HeaderText = "Format", ReadOnly = true, Width = 190 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", ReadOnly = true, Width = 175 });
        _grid.CellMouseClick += GridCellMouseClick;
        _grid.KeyDown += GridKeyDown;
        _gridHost = new ModernGridHost(_grid)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 7),
            Radius = 14
        };
        outer.Controls.Add(_gridHost, 0, 2);

        var dropCard = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Radius = 12,
            BackColor = Theme.Card,
            BorderColor = Theme.BorderSoft,
            BorderThickness = 1f,
            Margin = new Padding(0, 3, 0, 3),
            Padding = new Padding(15, 0, 15, 0)
        };
        _drop.Dock = DockStyle.Fill;
        _drop.TextAlign = ContentAlignment.MiddleLeft;
        _drop.ForeColor = Theme.Muted;
        _drop.Font = new Font("Segoe UI", 9.3f);
        dropCard.Controls.Add(_drop);
        outer.Controls.Add(dropCard, 0, 3);

        var logWrap = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        logWrap.RowStyles.Add(new RowStyle(SizeType.Absolute, 31));
        logWrap.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _logLabel.Dock = DockStyle.Fill;
        _logLabel.TextAlign = ContentAlignment.MiddleLeft;
        _logLabel.ForeColor = Theme.Text;
        _logLabel.Font = new Font("Segoe UI Semibold", 9.7f, FontStyle.Bold);
        _log.ReadOnly = true;
        _log.WordWrap = true;
        _log.EditorFont = CreateCodeFont(9.2f);
        _log.Dock = DockStyle.Fill;
        _log.Margin = Padding.Empty;
        _log.Radius = 12;
        _log.BorderColor = Theme.BorderSoft;
        _log.BorderThickness = 1f;
        logWrap.Controls.Add(_logLabel, 0, 0);
        logWrap.Controls.Add(_log, 0, 1);
        outer.Controls.Add(logWrap, 0, 4);
    }

    private static Font CreateCodeFont(float size)
    {
        try { return new Font("Cascadia Mono", size); }
        catch { return new Font("Consolas", size); }
    }

    private static void ConfigureButton(RoundedButton b, EventHandler handler, bool accent)
    {
        b.Dock = DockStyle.Fill;
        b.Height = 42;
        b.Radius = 11;
        b.Margin = new Padding(0, 6, 8, 6);
        Theme.StyleButton(b, accent);
        b.Click += handler;
        b.BackColor = accent ? Theme.Accent : Color.FromArgb(37, 42, 50);
        b.HoverBackColor = accent ? Theme.AccentHover : Theme.CardHover;
        b.PressedBackColor = accent ? Theme.AccentPressed : Color.FromArgb(48, 54, 64);
    }

    public void ApplyLanguage()
    {
        _title.Text = Localization.T("Convertisseur audio", "Audio converter");
        _subtitle.Text = Localization.T("Conversion directe vers WAV BO3 48 kHz / PCM 16 bits, sans préfixe de fichier.", "Direct conversion to BO3 WAV 48 kHz / 16-bit PCM, without filename prefixes.");
        _add.Text = Localization.T("Ajouter des sons", "Add audio");
        _remove.Text = Localization.T("Supprimer", "Remove");
        _browseOutput.Text = Localization.T("Dossier de sortie", "Output folder");
        _clear.Text = Localization.T("Vider", "Clear");
        _convert.Text = Localization.T("Convertir tout", "Convert all");
        _backup.Text = Localization.T("Créer un backup si la sortie existe", "Create backup when output exists");
        _drop.Text = Localization.T("Glissez WAV / MP3 / AIFF / WMA ici. Les sons convertis sont enregistrés dans le dossier du fichier d'origine.", "Drop WAV / MP3 / AIFF / WMA here. Converted sounds are saved in the original file's folder.");
        _logLabel.Text = Localization.T("Console log", "Conversion log");
        _grid.Columns["Input"].HeaderText = Localization.T("Entrée", "Input");
        _grid.Columns["Output"].HeaderText = Localization.T("Sortie", "Output");
        _grid.Columns["Status"].HeaderText = Localization.T("État", "Status");
        _tip.SetToolTip(_backup, Localization.T("Avant l'écrasement d'un WAV existant, crée une copie horodatée dans le même dossier.", "Before overwriting an existing WAV, creates a timestamped copy in the same folder."));
        _tip.SetToolTip(_browseOutput, Localization.T("Affecte le dossier choisi uniquement aux lignes sélectionnées. Les autres sorties restent inchangées.", "Applies the selected folder only to selected rows. Other output paths stay unchanged."));
        _tip.SetToolTip(_add, Localization.T("S'il y a déjà un son dans la liste, le sélecteur s'ouvre dans son dossier. Sinon il s'ouvre dans Téléchargements.", "If audio is already listed, the picker opens in its folder. Otherwise it opens in Downloads."));
    }

    private void GridCellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (_busy || e.RowIndex < 0 || e.ColumnIndex < 0 || e.Button != MouseButtons.Left) return;
        if (!string.Equals(_grid.Columns[e.ColumnIndex].Name, "Do", StringComparison.Ordinal)) return;
        ToggleRowEnabled(e.RowIndex);
    }

    private void GridKeyDown(object? sender, KeyEventArgs e)
    {
        if (_busy || e.KeyCode != Keys.Space || _grid.CurrentCell == null) return;
        if (!string.Equals(_grid.Columns[_grid.CurrentCell.ColumnIndex].Name, "Do", StringComparison.Ordinal)) return;
        ToggleRowEnabled(_grid.CurrentCell.RowIndex);
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void ToggleRowEnabled(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _grid.Rows.Count) return;
        var cell = _grid.Rows[rowIndex].Cells["Do"];
        var current = Convert.ToBoolean(cell.Value ?? false);
        cell.Value = !current;
        _grid.InvalidateCell(cell);
    }

    private void AddDialog()
    {
        using var ofd = new OpenFileDialog
        {
            Title = Localization.T("Ajouter des sons", "Add audio"),
            Filter = "Audio (*.wav;*.mp3;*.aiff;*.aif;*.wma)|*.wav;*.mp3;*.aiff;*.aif;*.wma|All files (*.*)|*.*",
            Multiselect = true,
            CheckFileExists = true,
            InitialDirectory = GetAudioPickerInitialDirectory()
        };
        if (ofd.ShowDialog() == DialogResult.OK) AddFiles(ofd.FileNames);
    }

    private string GetAudioPickerInitialDirectory()
    {
        IEnumerable<DataGridViewRow> PreferredRows()
        {
            foreach (DataGridViewRow row in _grid.SelectedRows) yield return row;
            if (_grid.CurrentRow != null) yield return _grid.CurrentRow;
            foreach (DataGridViewRow row in _grid.Rows) yield return row;
        }

        var seen = new HashSet<int>();
        foreach (var row in PreferredRows())
        {
            if (!seen.Add(row.Index)) continue;
            var input = Convert.ToString(row.Cells["Input"].Value);
            if (string.IsNullOrWhiteSpace(input)) continue;
            try
            {
                var directory = Path.GetDirectoryName(input);
                if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory)) return directory;
            }
            catch { }
        }

        return Bo3PathService.GetDownloadsDirectory();
    }

    private void AddFiles(IEnumerable<string> files)
    {
        var added = 0;
        foreach (var input in files.Where(File.Exists).Where(IsSupported))
        {
            var fullInput = Path.GetFullPath(input);
            var output = GetDefaultOutputPath(fullInput);
            var idx = _grid.Rows.Add(true, fullInput, output, AudioConversionService.Describe(fullInput), Localization.T("Prêt", "Ready"));
            _grid.Rows[idx].Cells["Status"].Style.ForeColor = Theme.Muted;
            AppendLog(Localization.T($"Ajouté : {Path.GetFileName(fullInput)} -> {output}", $"Added: {Path.GetFileName(fullInput)} -> {output}"), LogLevel.Info);
            added++;
        }
        if (added == 0)
            AppendLog(Localization.T("Aucun fichier audio compatible ajouté.", "No compatible audio file was added."), LogLevel.Warning);
        _gridHost?.RefreshScrollBars();
    }

    private static bool IsSupported(string f) => new[] { ".wav", ".mp3", ".aiff", ".aif", ".wma" }.Contains(Path.GetExtension(f).ToLowerInvariant());

    private static string GetDefaultOutputPath(string input)
    {
        return Path.ChangeExtension(input, ".wav");
    }

    private void RemoveSelected()
    {
        var rows = _grid.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(r => r.Index).ToList();
        foreach (var row in rows)
        {
            var input = Convert.ToString(row.Cells["Input"].Value) ?? string.Empty;
            _grid.Rows.Remove(row);
            AppendLog(Localization.T($"Supprimé : {Path.GetFileName(input)}", $"Removed: {Path.GetFileName(input)}"), LogLevel.Warning);
        }
        _gridHost?.RefreshScrollBars();
    }

    private void ClearAll()
    {
        _grid.Rows.Clear();
        _gridHost?.RefreshScrollBars();
        _log.Clear();
    }

    private void BrowseOutputForSelection()
    {
        var selected = _grid.SelectedRows.Cast<DataGridViewRow>().ToList();
        if (selected.Count == 0 && _grid.CurrentRow != null) selected.Add(_grid.CurrentRow);
        if (selected.Count == 0) return;

        using var fbd = new FolderBrowserDialog
        {
            Description = Localization.T("Choisir le dossier de sortie des lignes sélectionnées", "Choose output folder for selected rows"),
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };
        var firstOutput = Convert.ToString(selected[0].Cells["Output"].Value);
        var initial = !string.IsNullOrWhiteSpace(firstOutput) ? Path.GetDirectoryName(firstOutput) : null;
        fbd.SelectedPath = Bo3PathService.GetExistingOrNearestParent(initial, Bo3PathService.GetDownloadsDirectory());
        if (fbd.ShowDialog() != DialogResult.OK) return;
        foreach (var row in selected)
        {
            var input = Convert.ToString(row.Cells["Input"].Value) ?? string.Empty;
            var output = Path.Combine(fbd.SelectedPath, Path.GetFileNameWithoutExtension(input) + ".wav");
            row.Cells["Output"].Value = output;
            AppendLog(Localization.T($"Sortie définie : {Path.GetFileName(input)} -> {output}", $"Output set: {Path.GetFileName(input)} -> {output}"), LogLevel.Info);
        }
    }

    private async Task ConvertAllAsync()
    {
        if (_busy) return;
        _grid.EndEdit();
        _log.Clear();
        var jobs = _grid.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Do"].Value ?? false)).ToList();
        if (jobs.Count == 0)
        {
            AppendLog(Localization.T("Aucune ligne activée pour la conversion.", "No enabled row to convert."), LogLevel.Warning);
            return;
        }

        var conflictedRows = FindOutputConflicts(jobs);
        foreach (var row in conflictedRows)
        {
            var output = Convert.ToString(row.Cells["Output"].Value) ?? string.Empty;
            SetStatus(row, Localization.T("Conflit de sortie", "Output conflict"), true);
            AppendLog(Localization.T($"ÉCHEC : plusieurs lignes utilisent la même sortie : {output}", $"FAILED: multiple rows use the same output: {output}"), LogLevel.Error);
        }
        jobs = jobs.Where(r => !conflictedRows.Contains(r)).ToList();
        if (jobs.Count == 0)
        {
            AppendLog(Localization.T("Conversion annulée : corrigez les conflits de sortie.", "Conversion cancelled: fix the output conflicts."), LogLevel.Warning);
            return;
        }

        var createBackup = _backup.Checked;
        _busy = true;
        SetBusyState(true);
        var ok = 0;
        var failed = conflictedRows.Count;
        AppendLog(Localization.T($"Début de conversion : {jobs.Count} fichier(s).", $"Conversion started: {jobs.Count} file(s)."), LogLevel.Info);

        try
        {
            foreach (var row in jobs)
            {
                var input = Convert.ToString(row.Cells["Input"].Value) ?? string.Empty;
                var output = Convert.ToString(row.Cells["Output"].Value) ?? string.Empty;
                if (!File.Exists(input))
                {
                    SetStatus(row, Localization.T("Entrée introuvable", "Input missing"), true);
                    AppendLog(Localization.T($"ÉCHEC : fichier introuvable : {input}", $"FAILED: input file not found: {input}"), LogLevel.Error);
                    failed++;
                    continue;
                }
                if (string.IsNullOrWhiteSpace(output))
                {
                    SetStatus(row, Localization.T("Sortie vide", "No output"), true);
                    AppendLog(Localization.T($"ÉCHEC : aucune sortie pour {Path.GetFileName(input)}", $"FAILED: no output for {Path.GetFileName(input)}"), LogLevel.Error);
                    failed++;
                    continue;
                }

                SetStatus(row, Localization.T("Conversion...", "Converting..."), false);
                AppendLog(Localization.T($"Conversion : {Path.GetFileName(input)}", $"Converting: {Path.GetFileName(input)}"), LogLevel.Info);
                try
                {
                    await Task.Run(() => AudioConversionService.ConvertToBo3Wav(input, output, createBackup));
                    SetStatus(row, Localization.T("Terminé", "Done"), false);
                    AppendLog(Localization.T($"OK : {output}", $"OK: {output}"), LogLevel.Success);
                    ok++;
                }
                catch (Exception ex)
                {
                    SetStatus(row, Localization.T("Échec", "Failed") + ": " + ex.Message, true);
                    AppendLog(Localization.T($"ÉCHEC : {Path.GetFileName(input)} - {ex.Message}", $"FAILED: {Path.GetFileName(input)} - {ex.Message}"), LogLevel.Error);
                    failed++;
                }
            }
        }
        finally
        {
            _busy = false;
            SetBusyState(false);
            AppendLog(Localization.T($"Terminé : {ok} réussi(s), {failed} échec(s).", $"Finished: {ok} succeeded, {failed} failed."), failed == 0 ? LogLevel.Success : ok == 0 ? LogLevel.Error : LogLevel.Warning);
        }
    }

    private static HashSet<DataGridViewRow> FindOutputConflicts(IEnumerable<DataGridViewRow> rows)
    {
        var conflicts = new HashSet<DataGridViewRow>();
        var groups = rows
            .Select(row => new { Row = row, Key = NormalizeOutputKey(Convert.ToString(row.Cells["Output"].Value)) })
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);

        foreach (var group in groups)
            foreach (var item in group)
                conflicts.Add(item.Row);
        return conflicts;
    }

    private static string NormalizeOutputKey(string? output)
    {
        if (string.IsNullOrWhiteSpace(output)) return string.Empty;
        try { return Path.GetFullPath(output.Trim()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
        catch { return output.Trim(); }
    }

    private void SetBusyState(bool busy)
    {
        var enabled = !busy;
        _convert.Enabled = enabled;
        _add.Enabled = enabled;
        _remove.Enabled = enabled;
        _browseOutput.Enabled = enabled;
        _clear.Enabled = enabled;
        _backup.Enabled = enabled;
        _grid.Enabled = enabled;
    }

    private void SetStatus(DataGridViewRow row, string text, bool error)
    {
        row.Cells["Status"].Value = text;
        row.Cells["Status"].Style.ForeColor = error ? Color.FromArgb(235, 115, 125) : Theme.Text;
    }

    private void AppendLog(string text, LogLevel level = LogLevel.Info)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var color = level switch
        {
            LogLevel.Success => Theme.LogSuccess,
            LogLevel.Warning => Theme.LogWarning,
            LogLevel.Error => Theme.LogError,
            _ => Theme.LogInfo
        };

        _log.AppendColoredText($"[{DateTime.Now:HH:mm:ss}] ", Theme.Muted);
        _log.AppendColoredText(text + Environment.NewLine, color);

        // Trim in-place so RichTextBox formatting/colors are preserved.
        if (_log.Text.Length > 80000)
            _log.RemoveFirstCharacters(_log.Text.Length - 60000);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files)
        {
            e.Effect = DragDropEffects.None;
            return;
        }
        e.Effect = files.Any(f => File.Exists(f) && IsSupported(f)) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files) AddFiles(files);
    }
}
