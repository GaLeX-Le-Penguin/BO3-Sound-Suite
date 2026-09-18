using BO3SoundSuite.Controls;
using BO3SoundSuite.Services;

namespace BO3SoundSuite.Modules;

public sealed class SzcEditorControl : UserControl
{
    private readonly RoundedButton _open = new();
    private readonly RoundedButton _save = new();
    private readonly RoundedButton _apply = new();
    private readonly RoundedButton _add = new();
    private readonly TextBox _name = new();
    private readonly TextBox _filename = new();
    private readonly CodeEditorBox _raw = new();
    private readonly CodeEditorBox _preview = new();
    private readonly ModernCheckBox _backup = new();
    private readonly Label _title = new();
    private readonly Label _subtitle = new();
    private readonly Label _nameLabel = new();
    private readonly Label _filenameLabel = new();
    private readonly Label _previewLabel = new();
    private readonly Label _rawLabel = new();
    private readonly ModernToolTip _tip = new();
    private string? _path;
    private string _previousName = string.Empty;
    private bool _internalChange;

    public SzcEditorControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.AppBack;
        AllowDrop = true;
        DragEnter += OnDragEnter;
        DragDrop += OnDragDrop;
        BuildUi();
        ApplyLanguage();
        _name.TextChanged += NameChanged;
        _filename.TextChanged += (_, _) => UpdatePreview();
        UpdatePreview();
    }

    private void BuildUi()
    {
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(30, 24, 30, 24),
            BackColor = Theme.AppBack,
            Margin = Padding.Empty
        };
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(outer);

        var header = new Panel { Dock = DockStyle.Fill, BackColor = Theme.AppBack, Margin = Padding.Empty };
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

        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31));
        ConfigureButton(_open, (_, _) => OpenDialog(), false);
        ConfigureButton(_save, (_, _) => Save(), true);
        ConfigureButton(_apply, (_, _) => ApplyAlias(), false);
        ConfigureButton(_add, (_, _) => AddAlias(), false);
        actions.Controls.Add(_open, 0, 0);
        actions.Controls.Add(_save, 1, 0);
        actions.Controls.Add(_apply, 2, 0);
        actions.Controls.Add(_add, 3, 0);
        _backup.AutoSizeToText = false;
        _backup.Dock = DockStyle.Fill;
        _backup.ForeColor = Theme.Text;
        _backup.Font = new Font("Segoe UI", 9.2f);
        _backup.Margin = new Padding(14, 6, 0, 6);
        _backup.Checked = false;
        actions.Controls.Add(_backup, 4, 0);
        outer.Controls.Add(actions, 0, 1);

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        fields.Controls.Add(CreateCleanField(_nameLabel, _name, new Padding(0, 3, 8, 7)), 0, 0);
        fields.Controls.Add(CreateCleanField(_filenameLabel, _filename, new Padding(8, 3, 0, 7)), 1, 0);
        outer.Controls.Add(fields, 0, 2);

        var editors = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        editors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        editors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));

        _preview.ReadOnly = true;
        _preview.WordWrap = true;
        _preview.EditorFont = CreateCodeFont(10f);
        _raw.ReadOnly = false;
        _raw.AcceptsTab = true;
        _raw.WordWrap = false;
        _raw.EditorFont = CreateCodeFont(10f);

        editors.Controls.Add(CreateEditorSection(_previewLabel, _preview, new Padding(0, 3, 8, 0)), 0, 0);
        editors.Controls.Add(CreateEditorSection(_rawLabel, _raw, new Padding(8, 3, 0, 0)), 1, 0);
        outer.Controls.Add(editors, 0, 3);
    }

    private static Control CreateCleanField(Label label, TextBox input, Padding margin)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Theme.AppBack,
            Margin = margin,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        label.ForeColor = Theme.Muted;
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Font = new Font("Segoe UI Semibold", 9.1f, FontStyle.Bold);
        Theme.StyleTextBox(input);
        input.AutoSize = false;
        var host = new RoundedInputHost(input, 12, 8)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Radius = 11,
            BackColor = Theme.Input,
            BorderColor = Theme.Border,
            BorderThickness = 1f
        };
        layout.Controls.Add(label, 0, 0);
        layout.Controls.Add(host, 0, 1);
        return layout;
    }

    private static Control CreateEditorSection(Label label, CodeEditorBox editor, Padding margin)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Theme.AppBack,
            Margin = margin,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        label.ForeColor = Theme.Muted;
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Font = new Font("Segoe UI Semibold", 9.3f, FontStyle.Bold);
        editor.Dock = DockStyle.Fill;
        editor.Margin = Padding.Empty;
        editor.Radius = 13;
        editor.BorderColor = Theme.Border;
        editor.BorderThickness = 1f;
        layout.Controls.Add(label, 0, 0);
        layout.Controls.Add(editor, 0, 1);
        return layout;
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
        _title.Text = Localization.T("Éditeur SZC", "SZC editor");
        _subtitle.Text = Localization.T("Chargez le zoneconfig, modifiez la source ALIAS et enregistrez sans retoucher le bloc à la main.", "Load a zoneconfig, edit the ALIAS source and save without hand-editing the block.");
        _open.Text = Localization.T("Ouvrir SZC", "Open SZC");
        _save.Text = Localization.T("Enregistrer", "Save");
        _apply.Text = Localization.T("Remplacer l'ALIAS", "Replace ALIAS");
        _add.Text = Localization.T("Ajouter un ALIAS", "Add ALIAS");
        _backup.Text = Localization.T("Backup avant sauvegarde", "Backup before save");
        _nameLabel.Text = Localization.T("Nom de la source", "Source name");
        _filenameLabel.Text = Localization.T("Fichier CSV", "CSV filename");
        _previewLabel.Text = Localization.T("Bloc généré en temps réel", "Live generated block");
        _rawLabel.Text = Localization.T("Contenu SZC", "SZC content");
        _tip.SetToolTip(_name, Localization.T("Le bloc généré change immédiatement. Si le nom du fichier suit encore le nom de la source, il est renommé automatiquement en .csv.", "The generated block changes immediately. If the filename still follows the source name, it is automatically renamed to .csv."));
        _tip.SetToolTip(_apply, Localization.T("Remplace le premier bloc de Type ALIAS trouvé. S'il n'existe pas, il est ajouté dans Sources.", "Replaces the first Type ALIAS block found. If none exists, it is inserted into Sources."));
    }

    private void NameChanged(object? sender, EventArgs e)
    {
        if (_internalChange) return;
        var oldAuto = string.IsNullOrWhiteSpace(_filename.Text) || _filename.Text.Equals(_previousName + ".csv", StringComparison.OrdinalIgnoreCase);
        var current = _name.Text.Trim();
        if (oldAuto)
        {
            _internalChange = true;
            _filename.Text = string.IsNullOrWhiteSpace(current) ? string.Empty : current + ".csv";
            _internalChange = false;
        }
        _previousName = current;
        UpdatePreview();
    }

    private void UpdatePreview() => _preview.Text = SzcService.BuildAliasBlock(_name.Text.Trim(), _filename.Text.Trim());

    private void OpenDialog()
    {
        var usermaps = Bo3PathService.GetUsermapsDirectory();
        using var ofd = new OpenFileDialog
        {
            Filter = "BO3 zoneconfig (*.szc)|*.szc|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (!string.IsNullOrWhiteSpace(usermaps)) ofd.InitialDirectory = usermaps;
        if (ofd.ShowDialog() == DialogResult.OK) LoadFile(ofd.FileName);
    }

    private void LoadFile(string path)
    {
        try
        {
            _path = path;
            _raw.Text = File.ReadAllText(path);
            var alias = SzcService.GetFirstAlias(_raw.Text);
            _internalChange = true;
            if (alias.HasValue)
            {
                _name.Text = alias.Value.Name;
                _filename.Text = alias.Value.Filename;
                _previousName = alias.Value.Name;
            }
            else
            {
                _name.Text = string.Empty;
                _filename.Text = string.Empty;
                _previousName = string.Empty;
            }
            _internalChange = false;
            UpdatePreview();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "SZC", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void ApplyAlias()
    {
        _raw.Text = SzcService.ReplaceFirstAlias(_raw.Text, _name.Text.Trim(), _filename.Text.Trim());
    }

    private void AddAlias()
    {
        _raw.Text = SzcService.InsertAlias(_raw.Text, _name.Text.Trim(), _filename.Text.Trim());
    }

    private void Save()
    {
        var importedDirectory = !string.IsNullOrWhiteSpace(_path) ? Path.GetDirectoryName(_path) ?? string.Empty : string.Empty;
        var usermaps = Bo3PathService.GetUsermapsDirectory();
        var initialDirectory = Directory.Exists(importedDirectory) ? importedDirectory : usermaps;
        var defaultFileName = !string.IsNullOrWhiteSpace(_path)
            ? Path.GetFileName(_path)
            : string.IsNullOrWhiteSpace(_name.Text) ? "zm_mod.szc" : _name.Text.Trim() + ".szc";

        using var sfd = new SaveFileDialog
        {
            Filter = "BO3 zoneconfig (*.szc)|*.szc",
            DefaultExt = "szc",
            AddExtension = true,
            OverwritePrompt = true,
            FileName = defaultFileName
        };
        if (!string.IsNullOrWhiteSpace(initialDirectory)) sfd.InitialDirectory = initialDirectory;
        if (sfd.ShowDialog() != DialogResult.OK) return;

        var path = sfd.FileName;
        try
        {
            if (_backup.Checked && File.Exists(path))
                FileSafetyService.CreateTimestampedBackup(path);
            File.WriteAllText(path, _raw.Text, new System.Text.UTF8Encoding(false));
            _path = path;
            MessageBox.Show(Localization.T("SZC enregistré.", "SZC saved."), "SZC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "SZC", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true) e.Effect = DragDropEffects.Copy;
    }

    private void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files) return;
        var file = files.FirstOrDefault(f => Path.GetExtension(f).Equals(".szc", StringComparison.OrdinalIgnoreCase));
        if (file != null) LoadFile(file);
    }
}
