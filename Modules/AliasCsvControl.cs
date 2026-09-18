using System.Text;
using BO3SoundSuite.Controls;
using BO3SoundSuite.Models;
using BO3SoundSuite.Services;

namespace BO3SoundSuite.Modules;

public sealed class AliasCsvControl : UserControl
{
    private readonly List<SoundEntry> _entries = new();
    private int _currentIndex = -1;
    private string _importedCsvFileName = string.Empty;
    private string _importedCsvPath = string.Empty;
    private bool _loading;
    private bool _updatingSounds;
    private bool _updatingValues;

    private readonly Label _title = new();
    private readonly Label _subtitle = new();
    private readonly Label _soundsLabel = new();
    private readonly Label _audioLabel = new();
    private readonly Label _waveLabel = new();
    private readonly Label _pathLabel = new();
    private readonly Label _aliasLabel = new();
    private readonly Label _presetLabel = new();
    private readonly Label _volumeLabel = new();
    private readonly Label _loopingLabel = new();
    private readonly Label _detectedLabel = new();
    private readonly Label _editorLabel = new();
    private readonly Label _status = new();

    private readonly RoundedButton _new = new();
    private readonly RoundedButton _import = new();
    private readonly RoundedButton _addWav = new();
    private readonly RoundedButton _duplicate = new();
    private readonly RoundedButton _export = new();
    private readonly RoundedButton _addRow = new();
    private readonly RoundedButton _deleteRow = new();
    private readonly RoundedButton _browseAudio = new();
    private readonly RoundedButton _tabCsv = new();
    private readonly RoundedButton _tabValues = new();

    private readonly DataGridView _sounds = new();
    private readonly DataGridView _values = new();
    private readonly TextBox _audio = new();
    private readonly TextBox _bo3Path = new();
    private readonly TextBox _alias = new();
    private readonly ModernComboBox _preset = new();
    private readonly ModernComboBox _volume = new();
    private readonly ModernNumericBox _customVolume = new();
    private readonly ModernCheckBox _looping = new();
    private readonly CodeEditorBox _preview = new();
    private readonly ModernToolTip _tip = new();
    private readonly System.Windows.Forms.Timer _parameterHoverTimer = new() { Interval = 900 };
    private string _hoveredParameter = string.Empty;
    private ModernGridHost? _soundsHost;
    private ModernGridHost? _valuesHost;
    private Panel? _editorPages;

    public AliasCsvControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.AppBack;
        AllowDrop = true;
        DragEnter += OnDragEnter;
        DragDrop += OnDragDrop;
        BuildUi();
        WireEvents();
        _entries.Add(new SoundEntry());
        RefreshSoundsGrid(0);
        SelectEntry(0);
        ApplyLanguage();
    }

    private SoundEntry? CurrentEntry => _currentIndex >= 0 && _currentIndex < _entries.Count ? _entries[_currentIndex] : null;

    private void BuildUi()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(30, 24, 30, 24),
            Margin = Padding.Empty,
            BackColor = Theme.AppBack
        };
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 356));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(outer);

        var header = new Panel { Dock = DockStyle.Fill, BackColor = Theme.AppBack, Margin = Padding.Empty };
        _title.AutoSize = true;
        _title.Font = new Font("Segoe UI Semibold", 21f, FontStyle.Bold);
        _title.ForeColor = Theme.Text;
        _title.Location = new Point(0, 0);
        _subtitle.AutoSize = true;
        _subtitle.Font = new Font("Segoe UI", 9.5f);
        _subtitle.ForeColor = Theme.Muted;
        _subtitle.Location = new Point(2, 40);
        header.Controls.AddRange(new Control[] { _title, _subtitle });
        outer.Controls.Add(header, 0, 0);

        var toolbar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26));
        ConfigureButton(_new, (_, _) => NewDocument(), false);
        ConfigureButton(_import, (_, _) => ImportCsv(), false);
        ConfigureButton(_addWav, (_, _) => BrowseAndAssignAudio(), true);
        ConfigureButton(_duplicate, (_, _) => DuplicateCurrent(), false);
        ConfigureButton(_export, (_, _) => ExportCsv(), true);
        toolbar.Controls.Add(_new, 0, 0);
        toolbar.Controls.Add(_import, 1, 0);
        toolbar.Controls.Add(_addWav, 2, 0);
        toolbar.Controls.Add(_duplicate, 3, 0);
        toolbar.Controls.Add(_export, 4, 0);
        outer.Controls.Add(toolbar, 0, 1);

        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 29));
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71));
        main.Controls.Add(BuildSoundsCard(), 0, 0);
        main.Controls.Add(BuildSettingsCard(), 1, 0);
        outer.Controls.Add(main, 0, 2);

        outer.Controls.Add(BuildEditorCard(), 0, 3);
    }

    private Control BuildSoundsCard()
    {
        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Radius = 18,
            BackColor = Theme.Card,
            BorderColor = Theme.Border,
            BorderThickness = 1.15f,
            ClipChildrenToRadius = true,
            Margin = new Padding(0, 4, 8, 8),
            Padding = new Padding(14, 10, 14, 12)
        };
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Theme.Card,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        _soundsLabel.Dock = DockStyle.Fill;
        _soundsLabel.TextAlign = ContentAlignment.MiddleLeft;
        _soundsLabel.ForeColor = Theme.Text;
        _soundsLabel.Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold);
        layout.Controls.Add(_soundsLabel, 0, 0);

        Theme.StyleGrid(_sounds);
        _sounds.AllowUserToAddRows = false;
        _sounds.AllowUserToDeleteRows = false;
        _sounds.ReadOnly = true;
        _sounds.MultiSelect = false;
        _sounds.RowHeadersVisible = false;
        _sounds.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _sounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "Number", HeaderText = "#", Width = 42, SortMode = DataGridViewColumnSortMode.NotSortable });
        _sounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "Alias", HeaderText = "Alias", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, MinimumWidth = 90, SortMode = DataGridViewColumnSortMode.NotSortable });
        _sounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "Audio", HeaderText = "WAV", Width = 105, SortMode = DataGridViewColumnSortMode.NotSortable });
        _soundsHost = new ModernGridHost(_sounds)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 2, 0, 3),
            Radius = 13,
            BorderColor = Theme.Border,
            BorderThickness = 1f,
            ClipChildrenToRadius = true
        };
        layout.Controls.Add(_soundsHost, 0, 1);

        var rowButtons = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Theme.Card,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0)
        };
        rowButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        rowButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        ConfigureButton(_addRow, (_, _) => AddRow(), false);
        ConfigureButton(_deleteRow, (_, _) => DeleteCurrent(), false);
        _addRow.Margin = new Padding(0, 0, 5, 0);
        _deleteRow.Margin = new Padding(5, 0, 0, 0);
        rowButtons.Controls.Add(_addRow, 0, 0);
        rowButtons.Controls.Add(_deleteRow, 1, 0);
        layout.Controls.Add(rowButtons, 0, 2);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildSettingsCard()
    {
        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Radius = 15,
            BackColor = Theme.Card,
            BorderColor = Theme.BorderSoft,
            BorderThickness = 1f,
            ClipChildrenToRadius = true,
            Margin = new Padding(8, 4, 0, 8),
            Padding = new Padding(16, 11, 16, 12)
        };
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 8,
            BackColor = Theme.Card,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));

        ConfigureFieldLabel(_audioLabel);
        grid.Controls.Add(_audioLabel, 0, 0);
        grid.SetColumnSpan(_audioLabel, 6);
        Theme.StyleTextBox(_audio);
        _audio.ReadOnly = true;
        _audio.AutoSize = false;
        var audioHost = new RoundedInputHost(_audio, 11, 7) { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0) };
        grid.Controls.Add(audioHost, 0, 1);
        grid.SetColumnSpan(audioHost, 5);
        ConfigureButton(_browseAudio, (_, _) => BrowseAndAssignAudio(), false);
        _browseAudio.Margin = Padding.Empty;
        grid.Controls.Add(_browseAudio, 5, 1);

        _waveLabel.Dock = DockStyle.Fill;
        _waveLabel.ForeColor = Theme.Muted;
        _waveLabel.Font = new Font("Segoe UI", 8.8f);
        _waveLabel.TextAlign = ContentAlignment.MiddleLeft;
        grid.Controls.Add(_waveLabel, 0, 2);
        grid.SetColumnSpan(_waveLabel, 6);

        ConfigureFieldLabel(_pathLabel);
        grid.Controls.Add(_pathLabel, 0, 3);
        grid.SetColumnSpan(_pathLabel, 6);
        Theme.StyleTextBox(_bo3Path);
        _bo3Path.AutoSize = false;
        var pathHost = new RoundedInputHost(_bo3Path, 11, 7) { Dock = DockStyle.Fill, Margin = Padding.Empty };
        grid.Controls.Add(pathHost, 0, 4);
        grid.SetColumnSpan(pathHost, 6);

        ConfigureFieldLabel(_aliasLabel);
        ConfigureFieldLabel(_presetLabel);
        ConfigureFieldLabel(_loopingLabel);
        grid.Controls.Add(_aliasLabel, 0, 5);
        grid.SetColumnSpan(_aliasLabel, 2);
        grid.Controls.Add(_presetLabel, 2, 5);
        grid.SetColumnSpan(_presetLabel, 2);
        grid.Controls.Add(_loopingLabel, 4, 5);
        grid.SetColumnSpan(_loopingLabel, 2);

        Theme.StyleTextBox(_alias);
        _alias.AutoSize = false;
        var aliasHost = new RoundedInputHost(_alias, 11, 7) { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0) };
        grid.Controls.Add(aliasHost, 0, 6);
        grid.SetColumnSpan(aliasHost, 2);
        _preset.Dock = DockStyle.Fill;
        _preset.Margin = new Padding(0, 0, 10, 0);
        grid.Controls.Add(_preset, 2, 6);
        grid.SetColumnSpan(_preset, 2);
        _looping.Dock = DockStyle.Fill;
        _looping.AutoSizeToText = false;
        _looping.Margin = Padding.Empty;
        _looping.ForeColor = Theme.Text;
        _looping.Font = new Font("Segoe UI", 9.2f);
        grid.Controls.Add(_looping, 4, 6);
        grid.SetColumnSpan(_looping, 2);

        var bottom = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 2,
            BackColor = Theme.Card,
            Margin = Padding.Empty,
            Padding = new Padding(0, 6, 0, 0)
        };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 23));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        ConfigureFieldLabel(_volumeLabel);
        bottom.Controls.Add(_volumeLabel, 0, 0);
        bottom.SetColumnSpan(_volumeLabel, 2);
        _volume.Dock = DockStyle.Fill;
        _volume.Margin = new Padding(0, 1, 10, 1);
        bottom.Controls.Add(_volume, 0, 1);
        bottom.SetColumnSpan(_volume, 2);
        _customVolume.Dock = DockStyle.Fill;
        _customVolume.Margin = new Padding(0, 1, 10, 1);
        _customVolume.Minimum = 0;
        _customVolume.Maximum = 100;
        _customVolume.Value = 75;
        bottom.Controls.Add(_customVolume, 2, 1);
        _detectedLabel.Dock = DockStyle.Fill;
        _detectedLabel.ForeColor = Theme.AccentHover;
        _detectedLabel.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
        _detectedLabel.TextAlign = ContentAlignment.MiddleLeft;
        bottom.Controls.Add(_detectedLabel, 3, 0);
        bottom.SetColumnSpan(_detectedLabel, 2);
        bottom.SetRowSpan(_detectedLabel, 2);
        grid.Controls.Add(bottom, 0, 7);
        grid.SetColumnSpan(bottom, 6);

        card.Controls.Add(grid);
        return card;
    }

    private Control BuildEditorCard()
    {
        var wrap = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        wrap.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        wrap.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var tabRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        tabRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        tabRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));
        tabRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330));
        ConfigureButton(_tabCsv, (_, _) => ShowEditorPage(false), false);
        ConfigureButton(_tabValues, (_, _) => ShowEditorPage(true), false);
        _tabCsv.Margin = new Padding(0, 2, 6, 2);
        _tabValues.Margin = new Padding(0, 2, 6, 2);
        tabRow.Controls.Add(_tabCsv, 0, 0);
        tabRow.Controls.Add(_tabValues, 1, 0);
        _editorLabel.Dock = DockStyle.Fill;
        _editorLabel.TextAlign = ContentAlignment.MiddleLeft;
        _editorLabel.ForeColor = Theme.Muted;
        _editorLabel.Font = new Font("Segoe UI", 8.8f);
        tabRow.Controls.Add(_editorLabel, 2, 0);
        _status.Dock = DockStyle.Fill;
        _status.TextAlign = ContentAlignment.MiddleRight;
        _status.ForeColor = Theme.Muted;
        _status.Font = new Font("Segoe UI", 8.6f);
        tabRow.Controls.Add(_status, 3, 0);
        wrap.Controls.Add(tabRow, 0, 0);

        _editorPages = new Panel { Dock = DockStyle.Fill, BackColor = Theme.AppBack, Margin = Padding.Empty, Padding = Padding.Empty };
        _preview.Dock = DockStyle.Fill;
        _preview.Margin = Padding.Empty;
        _preview.ReadOnly = true;
        _preview.WordWrap = false;
        _preview.EditorFont = CreateCodeFont(9.2f);
        _editorPages.Controls.Add(_preview);

        Theme.StyleGrid(_values);
        _values.Dock = DockStyle.Fill;
        _values.AllowUserToAddRows = false;
        _values.AllowUserToDeleteRows = false;
        _values.RowHeadersVisible = false;
        _values.MultiSelect = false;
        _values.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _values.Columns.Add(new DataGridViewTextBoxColumn { Name = "Field", HeaderText = "Paramètre", ReadOnly = true, Width = 240, SortMode = DataGridViewColumnSortMode.NotSortable });
        _values.Columns.Add(new DataGridViewTextBoxColumn { Name = "Value", HeaderText = "Valeur", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable });
        _valuesHost = new ModernGridHost(_values) { Dock = DockStyle.Fill, Margin = Padding.Empty, Visible = false };
        _editorPages.Controls.Add(_valuesHost);
        wrap.Controls.Add(_editorPages, 0, 1);
        ShowEditorPage(false);
        return wrap;
    }

    private static void ConfigureFieldLabel(Label label)
    {
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.ForeColor = Theme.Muted;
        label.Font = new Font("Segoe UI Semibold", 8.9f, FontStyle.Bold);
    }

    private static void ConfigureButton(RoundedButton button, EventHandler handler, bool accent)
    {
        button.Dock = DockStyle.Fill;
        button.Height = 40;
        button.Radius = 11;
        button.Margin = new Padding(0, 6, 8, 6);
        Theme.StyleButton(button, accent);
        button.BackColor = accent ? Theme.Accent : Color.FromArgb(37, 42, 50);
        button.HoverBackColor = accent ? Theme.AccentHover : Theme.CardHover;
        button.PressedBackColor = accent ? Theme.AccentPressed : Color.FromArgb(48, 54, 64);
        button.Click += handler;
    }

    private static Font CreateCodeFont(float size)
    {
        try { return new Font("Cascadia Mono", size); }
        catch { return new Font("Consolas", size); }
    }

    private void WireEvents()
    {
        _sounds.SelectionChanged += (_, _) =>
        {
            if (_updatingSounds || _sounds.SelectedRows.Count == 0) return;
            SelectEntry(_sounds.SelectedRows[0].Index);
        };
        _alias.TextChanged += (_, _) =>
        {
            if (_loading) return;
            SaveControlsToCurrentEntry();
            var entry = CurrentEntry;
            if (entry != null && (entry.ImportedFromCsv || entry.ManualOverrides.ContainsKey("Name"))) entry.ManualOverrides["Name"] = _alias.Text;
            UpdatePreview();
        };
        _bo3Path.TextChanged += (_, _) =>
        {
            if (_loading) return;
            SaveControlsToCurrentEntry();
            var entry = CurrentEntry;
            if (entry != null && (entry.ImportedFromCsv || entry.ManualOverrides.ContainsKey("FileSpec"))) entry.ManualOverrides["FileSpec"] = ResolveFileSpec(entry);
            UpdatePreview();
        };
        _preset.SelectedIndexChanged += (_, _) =>
        {
            if (_loading) return;
            var entry = CurrentEntry;
            if (entry != null) ClearProfileOverrides(entry);
            if (_preset.SelectedIndex == (int)AliasPreset.Ambience3D) _looping.Checked = true;
            SaveControlsToCurrentEntry();
            UpdatePreview();
        };
        _volume.SelectedIndexChanged += (_, _) =>
        {
            UpdateVolumeControls();
            if (_loading) return;
            var entry = CurrentEntry;
            if (entry != null) { entry.ManualOverrides.Remove("VolMin"); entry.ManualOverrides.Remove("VolMax"); }
            SaveControlsToCurrentEntry();
            UpdatePreview();
        };
        _customVolume.ValueChanged += (_, _) => { if (!_loading) { SaveControlsToCurrentEntry(); UpdatePreview(); } };
        _looping.CheckedChanged += (_, _) =>
        {
            if (_loading) return;
            SaveControlsToCurrentEntry();
            var entry = CurrentEntry;
            if (entry != null && (entry.ImportedFromCsv || entry.ManualOverrides.ContainsKey("Looping"))) entry.ManualOverrides["Looping"] = _looping.Checked ? "LOOPING" : "NONLOOPING";
            UpdatePreview();
        };
        _values.CellValueChanged += ValuesCellValueChanged;
        _values.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (_values.IsCurrentCellDirty) _values.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        _values.MouseMove += ValuesMouseMove;
        _values.MouseLeave += (_, _) => CancelParameterHelp(true);
        _values.Scroll += (_, _) => CancelParameterHelp(true);
        _values.CellBeginEdit += (_, _) => CancelParameterHelp(true);
        _parameterHoverTimer.Tick += ParameterHoverTimerTick;
    }

    public void ApplyLanguage()
    {
        _title.Text = "Alias CSV";
        _subtitle.Text = Localization.T("Configuration complète des sons, import, édition des paramètres et export BO3.", "Full sound configuration, import, parameter editing and BO3 export.");
        _new.Text = Localization.T("Nouveau", "New");
        _import.Text = Localization.T("Importer CSV", "Import CSV");
        _addWav.Text = Localization.T("Ajouter WAV", "Add WAV");
        _duplicate.Text = Localization.T("Dupliquer", "Duplicate");
        _export.Text = Localization.T("Enregistrer CSV", "Save CSV");
        _soundsLabel.Text = Localization.T("Sons du CSV", "CSV sounds");
        _addRow.Text = Localization.T("Ajouter une ligne", "Add row");
        _deleteRow.Text = Localization.T("Supprimer", "Delete");
        _audioLabel.Text = Localization.T("Fichier audio", "Audio file");
        _browseAudio.Text = Localization.T("Parcourir", "Browse");
        _pathLabel.Text = Localization.T("Chemin BO3 relatif à sound_assets", "BO3 path relative to sound_assets");
        _aliasLabel.Text = Localization.T("Alias", "Alias");
        _presetLabel.Text = Localization.T("Configuration / preset", "Configuration / preset");
        _loopingLabel.Text = Localization.T("Lecture", "Playback");
        _looping.Text = Localization.T("Boucle", "Looping");
        _volumeLabel.Text = Localization.T("Volume", "Volume");
        _tabCsv.Text = "CSV";
        _tabValues.Text = Localization.T("Paramètres", "Parameters");
        _editorLabel.Text = Localization.T(
            "Valeurs bleues = modification manuelle. Survolez un paramètre environ 1 seconde pour afficher sa description.",
            "Blue values = manual override. Hover a parameter for about 1 second to show its description.");
        _values.Columns["Field"].HeaderText = Localization.T("Paramètre", "Parameter");
        _values.Columns["Value"].HeaderText = Localization.T("Valeur", "Value");
        _sounds.Columns["Audio"].HeaderText = "WAV";

        var presetIndex = Math.Max(0, _preset.SelectedIndex);
        _preset.Items.Clear();
        _preset.Items.AddRange(new object[]
        {
            "Auto",
            Localization.T("Effet 3D", "3D effect"),
            Localization.T("Effet 2D", "2D effect"),
            Localization.T("Voix 3D", "3D voice"),
            Localization.T("Musique 2D", "2D music"),
            Localization.T("Ambiance 3D", "3D ambience"),
            Localization.T("Interface / UI 2D", "Interface / UI 2D")
        });
        _preset.SelectedIndex = Math.Min(presetIndex, _preset.Items.Count - 1);
        _preset.Invalidate();

        var volumeIndex = Math.Max(0, _volume.SelectedIndex);
        _volume.Items.Clear();
        _volume.Items.AddRange(new object[]
        {
            "Auto", "25", "50", "75", "90", "100", Localization.T("Personnalisé", "Custom")
        });
        _volume.SelectedIndex = Math.Min(volumeIndex, _volume.Items.Count - 1);
        _volume.Invalidate();

        _tip.SetToolTip(_addWav, Localization.T("Ajoute plusieurs WAV. Chaque WAV devient un son configurable séparément.", "Adds multiple WAV files. Each WAV becomes a separately configurable sound."));
        _tip.SetToolTip(_preset, Localization.T("Applique un profil BO3 puis laisse chaque colonne modifiable dans l'onglet Paramètres.", "Applies a BO3 profile while keeping every column editable in the Parameters tab."));
        _tip.SetToolTip(_bo3Path, Localization.T("Chemin relatif à sound_assets. Si le WAV est déjà dans sound_assets, ce chemin est détecté automatiquement.", "Path relative to sound_assets. If the WAV is already in sound_assets, this path is detected automatically."));
        _tip.SetToolTip(_tabValues, Localization.T(
            "Ouvre tous les paramètres de la ligne sélectionnée. Survolez un nom de paramètre ou sa valeur environ 1 seconde pour obtenir une description issue de la documentation BO3/communauté.",
            "Opens every parameter for the selected row. Hover a parameter name or value for about 1 second to get a description based on BO3/community documentation."));
        CancelParameterHelp(true);
        LoadCurrentEntryIntoControls();
        UpdatePreview();
    }

    private void ShowEditorPage(bool values)
    {
        if (_valuesHost == null || _editorPages == null) return;
        if (!values) CancelParameterHelp(true);
        _valuesHost.Visible = values;
        _preview.Visible = !values;
        if (values) _valuesHost.BringToFront(); else _preview.BringToFront();
        _tabValues.BackColor = values ? Color.FromArgb(47, 40, 78) : Color.FromArgb(37, 42, 50);
        _tabCsv.BackColor = values ? Color.FromArgb(37, 42, 50) : Color.FromArgb(47, 40, 78);
    }

    private void NewDocument()
    {
        _entries.Clear();
        _entries.Add(new SoundEntry());
        _importedCsvFileName = string.Empty;
        _importedCsvPath = string.Empty;
        _currentIndex = -1;
        RefreshSoundsGrid(0);
        SelectEntry(0);
        _status.Text = string.Empty;
    }

    private void AddRow()
    {
        SaveControlsToCurrentEntry();
        var index = _currentIndex >= 0 ? _currentIndex + 1 : _entries.Count;
        index = Math.Clamp(index, 0, _entries.Count);
        _entries.Insert(index, new SoundEntry());
        RefreshSoundsGrid(index);
        SelectEntry(index);
    }

    private void DeleteCurrent()
    {
        if (_currentIndex < 0 || _currentIndex >= _entries.Count) return;
        _entries.RemoveAt(_currentIndex);
        if (_entries.Count == 0) _entries.Add(new SoundEntry());
        var next = Math.Min(_currentIndex, _entries.Count - 1);
        _currentIndex = -1;
        RefreshSoundsGrid(next);
        SelectEntry(next);
    }

    private void DuplicateCurrent()
    {
        SaveControlsToCurrentEntry();
        var source = CurrentEntry;
        if (source == null) return;
        var clone = new SoundEntry
        {
            AudioPath = source.AudioPath,
            Bo3Subfolder = source.Bo3Subfolder,
            Alias = string.IsNullOrWhiteSpace(source.Alias) ? string.Empty : source.Alias + "_copy",
            PresetIndex = source.PresetIndex,
            VolumeIndex = source.VolumeIndex,
            CustomVolume = source.CustomVolume,
            Looping = source.Looping,
            AudioInfo = source.AudioInfo,
            ImportedFileSpec = source.ImportedFileSpec,
            ImportedFromCsv = source.ImportedFromCsv
        };
        foreach (var pair in source.ManualOverrides) clone.ManualOverrides[pair.Key] = pair.Value;
        if (clone.ManualOverrides.ContainsKey("Name")) clone.ManualOverrides["Name"] = clone.Alias;
        var index = _currentIndex + 1;
        _entries.Insert(index, clone);
        RefreshSoundsGrid(index);
        SelectEntry(index);
    }

    private void BrowseAndAssignAudio()
    {
        var initialDirectory = Bo3PathService.GetSoundAssetsDirectory();
        using var dialog = new OpenFileDialog
        {
            Title = Localization.T("Choisir un ou plusieurs WAV", "Choose one or more WAV files"),
            Filter = "WAV (*.wav)|*.wav|All files (*.*)|*.*",
            Multiselect = true,
            CheckFileExists = true
        };
        if (!string.IsNullOrWhiteSpace(initialDirectory)) dialog.InitialDirectory = initialDirectory;
        if (dialog.ShowDialog() == DialogResult.OK) AddOrAssignAudioFiles(dialog.FileNames);
    }

    private void AddOrAssignAudioFiles(IEnumerable<string> files)
    {
        var wavs = files.Where(f => File.Exists(f) && Path.GetExtension(f).Equals(".wav", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (wavs.Length == 0) return;
        SaveControlsToCurrentEntry();
        var target = _currentIndex;
        if (target < 0 || target >= _entries.Count)
        {
            _entries.Add(new SoundEntry());
            target = _entries.Count - 1;
        }
        for (var i = 0; i < wavs.Length; i++)
        {
            var index = target + i;
            if (i > 0) _entries.Insert(Math.Min(index, _entries.Count), new SoundEntry());
            AssignAudio(_entries[Math.Min(index, _entries.Count - 1)], wavs[i]);
        }
        RefreshSoundsGrid(target);
        SelectEntry(target);
    }

    private void AssignAudio(SoundEntry entry, string file)
    {
        var preserveImported = entry.ImportedFromCsv;
        entry.AudioPath = file;
        entry.ImportedFileSpec = string.Empty;
        if (!preserveImported)
        {
            entry.Alias = SoundAliasCsvBuilder.SanitizeAlias(Path.GetFileNameWithoutExtension(file));
            entry.ManualOverrides.Clear();
        }
        entry.ManualOverrides.Remove("FileSpec");
        var relative = SoundAliasCsvBuilder.TryGetFileSpecFromSoundAssets(file);
        if (!string.IsNullOrWhiteSpace(relative))
        {
            var slash = relative.LastIndexOf('\\');
            entry.Bo3Subfolder = slash >= 0 ? relative[..slash] : string.Empty;
        }
        entry.AudioInfo = AliasWavInspector.Read(file);
        var resolved = ResolveFileSpec(entry);
        if (entry.PresetIndex == 0 && SoundAliasCsvBuilder.ShouldAutoLoop(resolved, entry.Alias)) entry.Looping = true;
    }

    private void ImportCsv()
    {
        SaveControlsToCurrentEntry();
        var initialDirectory = Bo3PathService.GetAliasesDirectory();
        using var dialog = new OpenFileDialog
        {
            Filter = "BO3 sound alias CSV (*.csv)|*.csv|All files (*.*)|*.*",
            Multiselect = false,
            CheckFileExists = true
        };
        if (!string.IsNullOrWhiteSpace(initialDirectory)) dialog.InitialDirectory = initialDirectory;
        if (dialog.ShowDialog() != DialogResult.OK) return;
        try
        {
            var text = File.ReadAllText(dialog.FileName, Encoding.UTF8);
            var rows = SoundAliasCsvBuilder.ParseCsv(text, out var unknownHeaders);
            if (rows.Count == 0) throw new InvalidDataException(Localization.T("Le CSV ne contient aucune ligne.", "The CSV contains no rows."));
            _entries.Clear();
            foreach (var row in rows) _entries.Add(CreateImportedEntry(row));
            _importedCsvFileName = Path.GetFileName(dialog.FileName);
            _importedCsvPath = dialog.FileName;
            _currentIndex = -1;
            RefreshSoundsGrid(0);
            SelectEntry(0);
            _status.Text = Localization.T($"{rows.Count} son(s) importé(s)", $"{rows.Count} sound(s) imported");
            _status.ForeColor = Theme.AccentHover;
            if (unknownHeaders > 0)
                MessageBox.Show(Localization.T($"{unknownHeaders} colonne(s) inconnue(s) ont été ignorées.", $"{unknownHeaders} unknown column(s) were ignored."), "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private SoundEntry CreateImportedEntry(Dictionary<string, string> row)
    {
        var entry = new SoundEntry
        {
            ImportedFromCsv = true,
            Alias = row.TryGetValue("Name", out var name) ? name : string.Empty,
            ImportedFileSpec = row.TryGetValue("FileSpec", out var fileSpec) ? (fileSpec ?? string.Empty).Replace('/', '\\') : string.Empty,
            Looping = row.TryGetValue("Looping", out var looping) && looping.Equals("LOOPING", StringComparison.OrdinalIgnoreCase),
            PresetIndex = 0
        };
        if (!string.IsNullOrWhiteSpace(entry.ImportedFileSpec))
        {
            var slash = entry.ImportedFileSpec.LastIndexOf('\\');
            entry.Bo3Subfolder = slash >= 0 ? entry.ImportedFileSpec[..slash] : string.Empty;
        }
        if (row.TryGetValue("VolMin", out var volMin) && row.TryGetValue("VolMax", out var volMax) &&
            volMin == volMax && int.TryParse(volMin, out var volume) && volume is >= 0 and <= 100)
        {
            entry.VolumeIndex = 6;
            entry.CustomVolume = volume;
        }
        var generated = BuildGeneratedRow(entry);
        foreach (var field in SoundAliasCsvBuilder.Headers)
        {
            var imported = row.TryGetValue(field, out var importedValue) ? importedValue ?? string.Empty : string.Empty;
            var automatic = generated.TryGetValue(field, out var automaticValue) ? automaticValue ?? string.Empty : string.Empty;
            if (!string.Equals(imported, automatic, StringComparison.Ordinal)) entry.ManualOverrides[field] = imported;
        }
        if (entry.VolumeIndex == 6) { entry.ManualOverrides.Remove("VolMin"); entry.ManualOverrides.Remove("VolMax"); }
        return entry;
    }

    private void SelectEntry(int index)
    {
        if (index < 0 || index >= _entries.Count) return;
        if (_currentIndex != index) SaveControlsToCurrentEntry();
        _currentIndex = index;
        LoadCurrentEntryIntoControls();
        RefreshSoundsGrid(index);
        UpdatePreview();
    }

    private void LoadCurrentEntryIntoControls()
    {
        _loading = true;
        try
        {
            var entry = CurrentEntry;
            if (entry == null)
            {
                _audio.Text = string.Empty;
                _bo3Path.Text = string.Empty;
                _alias.Text = string.Empty;
                _preset.SelectedIndex = 0;
                _volume.SelectedIndex = 0;
                _customVolume.Value = 75;
                _looping.Checked = false;
                _waveLabel.Text = Localization.T("Aucun WAV lié.", "No WAV linked.");
                return;
            }
            _audio.Text = entry.AudioPath;
            _bo3Path.Text = entry.Bo3Subfolder;
            _alias.Text = entry.Alias;
            _preset.SelectedIndex = Math.Clamp(entry.PresetIndex, 0, Math.Max(0, _preset.Items.Count - 1));
            _volume.SelectedIndex = Math.Clamp(entry.VolumeIndex, 0, Math.Max(0, _volume.Items.Count - 1));
            _customVolume.Value = Math.Clamp(entry.CustomVolume, _customVolume.Minimum, _customVolume.Maximum);
            _looping.Checked = entry.Looping;
            UpdateVolumeControls();
            DisplayWaveInfo(entry);
        }
        finally { _loading = false; }
    }

    private void SaveControlsToCurrentEntry()
    {
        if (_loading) return;
        var entry = CurrentEntry;
        if (entry == null) return;
        entry.AudioPath = _audio.Text;
        entry.Bo3Subfolder = _bo3Path.Text;
        entry.Alias = _alias.Text;
        entry.PresetIndex = Math.Max(0, _preset.SelectedIndex);
        entry.VolumeIndex = Math.Max(0, _volume.SelectedIndex);
        entry.CustomVolume = _customVolume.Value;
        entry.Looping = _looping.Checked;
    }

    private void DisplayWaveInfo(SoundEntry entry)
    {
        if (entry.ImportedFromCsv && !entry.HasAudio)
        {
            _waveLabel.Text = Localization.T("Ligne importée : WAV non lié. Les valeurs CSV sont conservées.", "Imported row: WAV not linked. CSV values are preserved.");
            _waveLabel.ForeColor = Theme.Muted;
            return;
        }
        var info = entry.AudioInfo;
        if (info == null)
        {
            _waveLabel.Text = Localization.T("Aucun WAV lié.", "No WAV linked.");
            _waveLabel.ForeColor = Theme.Muted;
            return;
        }
        if (!string.IsNullOrWhiteSpace(info.Error))
        {
            _waveLabel.Text = Localization.T("WAV invalide : ", "Invalid WAV: ") + info.Error;
            _waveLabel.ForeColor = Theme.Danger;
            return;
        }
        _waveLabel.Text = $"{info.SampleRate} Hz | {info.BitsPerSample} bit | {info.Channels} ch | {(info.IsPcm ? "PCM" : $"format {info.AudioFormat}")}";
        _waveLabel.ForeColor = info.IsBo3Compatible ? Color.FromArgb(116, 201, 146) : Color.FromArgb(223, 171, 93);
    }

    private AliasPreset EffectivePreset(SoundEntry entry)
    {
        var selected = entry.PresetIndex >= 0 && entry.PresetIndex <= (int)AliasPreset.Ui2D ? (AliasPreset)entry.PresetIndex : AliasPreset.Auto;
        return selected == AliasPreset.Auto ? SoundAliasCsvBuilder.DetectPreset(ResolveFileSpec(entry), entry.Alias) : selected;
    }

    private string ResolveFileSpec(SoundEntry entry)
    {
        if (entry.HasAudio) return SoundAliasCsvBuilder.ResolveFileSpec(entry.Bo3Subfolder, entry.AudioPath);
        if (!string.IsNullOrWhiteSpace(entry.ImportedFileSpec)) return SoundAliasCsvBuilder.ResolveFileSpec(entry.Bo3Subfolder, Path.GetFileName(entry.ImportedFileSpec));
        return SoundAliasCsvBuilder.ResolveFileSpec(entry.Bo3Subfolder, entry.AudioPath);
    }

    private int? VolumeOverride(SoundEntry entry) => entry.VolumeIndex switch
    {
        1 => 25,
        2 => 50,
        3 => 75,
        4 => 90,
        5 => 100,
        6 => decimal.ToInt32(entry.CustomVolume),
        _ => null
    };

    private Dictionary<string, string> BuildGeneratedRow(SoundEntry entry) =>
        SoundAliasCsvBuilder.BuildRow(entry.Alias, ResolveFileSpec(entry), EffectivePreset(entry), entry.Looping, VolumeOverride(entry));

    private Dictionary<string, string> BuildCurrentRow(SoundEntry entry)
    {
        var row = BuildGeneratedRow(entry);
        foreach (var pair in entry.ManualOverrides) if (row.ContainsKey(pair.Key)) row[pair.Key] = pair.Value ?? string.Empty;
        return row;
    }

    private List<Dictionary<string, string>> BuildAllConfiguredRows() =>
        _entries.Where(e => e.CanExport).Select(BuildCurrentRow).ToList();

    private void UpdateVolumeControls()
    {
        var custom = _volume.SelectedIndex == 6;
        _customVolume.Enabled = custom;
        _customVolume.Visible = custom;
    }

    private void UpdatePreview()
    {
        if (!_loading) SaveControlsToCurrentEntry();
        var entry = CurrentEntry;
        if (entry != null)
        {
            var detected = SoundAliasCsvBuilder.DetectPreset(ResolveFileSpec(entry), entry.Alias);
            _detectedLabel.Text = Localization.T("Détecté : ", "Detected: ") + PresetName(detected);
            RefreshValuesGrid(BuildCurrentRow(entry), entry);
        }
        else
        {
            _detectedLabel.Text = string.Empty;
            _values.Rows.Clear();
        }
        _preview.Text = SoundAliasCsvBuilder.BuildCsv(BuildAllConfiguredRows()).TrimEnd('\r', '\n');
        RefreshSoundsGrid(_currentIndex);
    }

    private string PresetName(AliasPreset preset) => preset switch
    {
        AliasPreset.Effect3D => Localization.T("Effet 3D", "3D effect"),
        AliasPreset.Effect2D => Localization.T("Effet 2D", "2D effect"),
        AliasPreset.Voice3D => Localization.T("Voix 3D", "3D voice"),
        AliasPreset.Music2D => Localization.T("Musique 2D", "2D music"),
        AliasPreset.Ambience3D => Localization.T("Ambiance 3D", "3D ambience"),
        AliasPreset.Ui2D => Localization.T("Interface 2D", "2D interface"),
        _ => "Auto"
    };

    private void RefreshSoundsGrid(int preserveIndex)
    {
        if (_updatingSounds) return;
        _updatingSounds = true;
        try
        {
            _sounds.Rows.Clear();
            for (var i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                var alias = entry.ManualOverrides.TryGetValue("Name", out var manualName) ? manualName : entry.Alias;
                var wav = entry.HasAudio ? Path.GetFileName(entry.AudioPath) : !string.IsNullOrWhiteSpace(entry.ImportedFileSpec) ? Path.GetFileName(entry.ImportedFileSpec) : Localization.T("Vide", "Empty");
                var row = _sounds.Rows.Add((i + 1).ToString(), alias, wav);
                if (!entry.CanExport) _sounds.Rows[row].DefaultCellStyle.ForeColor = Theme.Muted;
            }
            if (preserveIndex >= 0 && preserveIndex < _sounds.Rows.Count)
            {
                _sounds.ClearSelection();
                _sounds.Rows[preserveIndex].Selected = true;
                _sounds.CurrentCell = _sounds.Rows[preserveIndex].Cells["Alias"];
            }
            _soundsHost?.RefreshScrollBars();
        }
        finally { _updatingSounds = false; }
    }

    private void RefreshValuesGrid(Dictionary<string, string> row, SoundEntry entry)
    {
        if (_updatingValues) return;
        _updatingValues = true;
        try
        {
            var first = _values.FirstDisplayedScrollingRowIndex;
            var selected = _values.CurrentCell?.RowIndex ?? -1;
            _values.Rows.Clear();
            foreach (var field in SoundAliasCsvBuilder.Headers)
            {
                var index = _values.Rows.Add(field, row.TryGetValue(field, out var value) ? value : string.Empty);
                if (entry.ManualOverrides.ContainsKey(field)) _values.Rows[index].Cells[1].Style.ForeColor = Color.FromArgb(150, 185, 255);
            }
            if (selected >= 0 && selected < _values.Rows.Count) _values.CurrentCell = _values.Rows[selected].Cells[1];
            if (first >= 0 && first < _values.Rows.Count) _values.FirstDisplayedScrollingRowIndex = first;
            _valuesHost?.RefreshScrollBars();
        }
        finally { _updatingValues = false; }
    }

    private void ValuesMouseMove(object? sender, MouseEventArgs e)
    {
        var hit = _values.HitTest(e.X, e.Y);
        if (hit.RowIndex < 0 || hit.RowIndex >= _values.Rows.Count)
        {
            CancelParameterHelp(true);
            return;
        }

        var field = Convert.ToString(_values.Rows[hit.RowIndex].Cells[0].Value)?.Trim() ?? string.Empty;
        if (field.Length == 0)
        {
            CancelParameterHelp(true);
            return;
        }

        if (string.Equals(field, _hoveredParameter, StringComparison.OrdinalIgnoreCase)) return;

        _tip.Hide(_values);
        _tip.SetToolTip(_values, string.Empty);
        _parameterHoverTimer.Stop();
        _hoveredParameter = field;
        _parameterHoverTimer.Start();
    }

    private void ParameterHoverTimerTick(object? sender, EventArgs e)
    {
        _parameterHoverTimer.Stop();
        if (string.IsNullOrWhiteSpace(_hoveredParameter) || !_values.Visible) return;

        var client = _values.PointToClient(Cursor.Position);
        if (!_values.ClientRectangle.Contains(client))
        {
            CancelParameterHelp(true);
            return;
        }

        var hit = _values.HitTest(client.X, client.Y);
        if (hit.RowIndex < 0 || hit.RowIndex >= _values.Rows.Count)
        {
            CancelParameterHelp(true);
            return;
        }

        var fieldUnderCursor = Convert.ToString(_values.Rows[hit.RowIndex].Cells[0].Value)?.Trim() ?? string.Empty;
        if (!string.Equals(fieldUnderCursor, _hoveredParameter, StringComparison.OrdinalIgnoreCase))
        {
            _hoveredParameter = fieldUnderCursor;
            if (_hoveredParameter.Length > 0) _parameterHoverTimer.Start();
            return;
        }

        var text = AliasFieldDocumentation.Get(_hoveredParameter);
        if (string.IsNullOrWhiteSpace(text)) return;

        _tip.SetToolTip(_values, text);
        var location = new Point(Math.Min(client.X + 18, Math.Max(0, _values.ClientSize.Width - 24)),
                                 Math.Min(client.Y + 20, Math.Max(0, _values.ClientSize.Height - 24)));
        _tip.Show(text, _values, location, 10000);
    }

    private void CancelParameterHelp(bool clearField)
    {
        _parameterHoverTimer.Stop();
        _tip.Hide(_values);
        _tip.SetToolTip(_values, string.Empty);
        if (clearField) _hoveredParameter = string.Empty;
    }

    private void ValuesCellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_updatingValues || _loading || e.RowIndex < 0 || e.ColumnIndex != 1) return;
        var entry = CurrentEntry;
        if (entry == null) return;
        var field = Convert.ToString(_values.Rows[e.RowIndex].Cells[0].Value) ?? string.Empty;
        var value = Convert.ToString(_values.Rows[e.RowIndex].Cells[1].Value) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(field)) return;
        var generated = BuildGeneratedRow(entry);
        var automatic = generated.TryGetValue(field, out var generatedValue) ? generatedValue : string.Empty;
        if (string.Equals(value, automatic, StringComparison.Ordinal)) entry.ManualOverrides.Remove(field);
        else entry.ManualOverrides[field] = value;
        _values.Rows[e.RowIndex].Cells[1].Style.ForeColor = entry.ManualOverrides.ContainsKey(field) ? Color.FromArgb(150, 185, 255) : _values.DefaultCellStyle.ForeColor;
        _preview.Text = SoundAliasCsvBuilder.BuildCsv(BuildAllConfiguredRows()).TrimEnd('\r', '\n');
        RefreshSoundsGrid(_currentIndex);
    }

    private static void ClearProfileOverrides(SoundEntry entry)
    {
        string[] fields =
        {
            "Storage", "Bus", "VolumeGroup", "DuckGroup", "ReverbSend", "VolMin", "VolMax",
            "DistMin", "DistMaxDry", "DistMaxWet", "LimitCount", "LimitType", "PanType", "Pan", "IsMusic", "DistanceLpf"
        };
        foreach (var field in fields) entry.ManualOverrides.Remove(field);
    }

    private void ExportCsv()
    {
        SaveControlsToCurrentEntry();
        var configured = _entries.Where(e => e.CanExport).ToList();
        if (configured.Count == 0)
        {
            MessageBox.Show(Localization.T("Aucun son configuré.", "No configured sounds."), "CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        foreach (var entry in configured)
        {
            if (entry.HasAudio && !File.Exists(entry.AudioPath))
            {
                MessageBox.Show(Localization.T("Fichier audio introuvable : ", "Audio file not found: ") + entry.AudioPath, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        var rows = BuildAllConfiguredRows();
        if (!ValidatePriorityRanges(rows, out var validationMessage))
        {
            MessageBox.Show(validationMessage, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ShowEditorPage(true);
            return;
        }
        var incompatible = configured.Count(e => e.HasAudio && (e.AudioInfo == null || !e.AudioInfo.IsBo3Compatible));
        if (incompatible > 0)
        {
            var answer = MessageBox.Show(
                Localization.T($"{incompatible} WAV ne sont pas en PCM 16 bits / 48 kHz. Continuer ?", $"{incompatible} WAV files are not 16-bit PCM / 48 kHz. Continue?"),
                "CSV", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;
        }
        using var dialog = new SaveFileDialog
        {
            Filter = "BO3 sound alias CSV (*.csv)|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = !string.IsNullOrWhiteSpace(_importedCsvFileName) ? _importedCsvFileName : configured.Count == 1 ? SoundAliasCsvBuilder.SanitizeAlias(configured[0].Alias) + "_aliases.csv" : "user_aliases.csv"
        };
        var importedDirectory = string.IsNullOrWhiteSpace(_importedCsvPath) ? string.Empty : Path.GetDirectoryName(_importedCsvPath) ?? string.Empty;
        var aliasesDirectory = Bo3PathService.GetAliasesDirectory();
        var initialDirectory = Directory.Exists(importedDirectory) ? importedDirectory : aliasesDirectory;
        if (!string.IsNullOrWhiteSpace(initialDirectory)) dialog.InitialDirectory = initialDirectory;
        if (dialog.ShowDialog() != DialogResult.OK) return;
        try
        {
            File.WriteAllText(dialog.FileName, SoundAliasCsvBuilder.BuildCsv(rows), new UTF8Encoding(false));
            _importedCsvPath = dialog.FileName;
            _importedCsvFileName = Path.GetFileName(dialog.FileName);
            _status.Text = Localization.T($"{configured.Count} son(s) enregistré(s)", $"{configured.Count} sound(s) saved");
            _status.ForeColor = Color.FromArgb(116, 201, 146);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "CSV", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private static bool ValidatePriorityRanges(IEnumerable<Dictionary<string, string>> rows, out string message)
    {
        var index = 0;
        foreach (var row in rows)
        {
            index++;
            var priorityMode = (row.TryGetValue("LimitType", out var limit) && limit.Equals("PRIORITY", StringComparison.OrdinalIgnoreCase)) ||
                               (row.TryGetValue("EntityLimitType", out var entityLimit) && entityLimit.Equals("PRIORITY", StringComparison.OrdinalIgnoreCase));
            if (!priorityMode) continue;
            var min = row.TryGetValue("PriorityMin", out var minValue) ? minValue?.Trim() ?? string.Empty : string.Empty;
            var max = row.TryGetValue("PriorityMax", out var maxValue) ? maxValue?.Trim() ?? string.Empty : string.Empty;
            if (min.Length > 0 && min.Equals(max, StringComparison.OrdinalIgnoreCase))
            {
                var name = row.TryGetValue("Name", out var n) ? n : $"#{index}";
                message = Localization.T(
                    $"Alias '{name}' : PriorityMin et PriorityMax ne peuvent pas être identiques quand LimitType/EntityLimitType vaut PRIORITY.",
                    $"Alias '{name}': PriorityMin and PriorityMax cannot be identical when LimitType/EntityLimitType is PRIORITY.");
                return false;
            }
        }
        message = string.Empty;
        return true;
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Any(f => Path.GetExtension(f).Equals(".wav", StringComparison.OrdinalIgnoreCase)))
            e.Effect = DragDropEffects.Copy;
    }

    private void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files) AddOrAssignAudioFiles(files);
    }
}
