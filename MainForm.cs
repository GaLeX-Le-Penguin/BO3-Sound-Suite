using System.Reflection;
using System.Runtime.InteropServices;
using BO3SoundSuite.Controls;
using BO3SoundSuite.Models;
using BO3SoundSuite.Modules;
using BO3SoundSuite.Services;

namespace BO3SoundSuite;

public sealed class MainForm : Form
{
    private readonly Panel _content = new();
    private readonly RoundedButton _audioButton = new();
    private readonly RoundedButton _aliasButton = new();
    private readonly RoundedButton _szcButton = new();
    private readonly ModernComboBox _language = new();
    private readonly Label _appName = new();
    private readonly Label _appSub = new();
    private readonly Label _langLabel = new();
    private readonly Label _footer = new();
    private readonly PictureBox _glxLogo = new();
    private readonly AliasCsvControl _alias = new();
    private readonly AudioConverterControl _audio = new();
    private readonly SzcEditorControl _szc = new();
    private UserControl? _current;

    public MainForm()
    {
        Text = "BO3 Sound Suite";
        var appIcon = LoadEmbeddedAppIcon();
        if (appIcon != null) Icon = appIcon;
        ShowIcon = true;
        BackColor = Theme.AppBack;
        ForeColor = Theme.Text;
        MinimumSize = new Size(1220, 760);
        Size = new Size(1500, 900);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f);
        DoubleBuffered = true;
        BuildUi();
        DetectLanguage();
        ShowModule(_audio, _audioButton);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        TryApplyModernWindowChrome();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Theme.AppBack,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 282));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Sidebar,
            Padding = new Padding(18, 18, 18, 16),
            Margin = Padding.Empty
        };
        root.Controls.Add(sidebar, 0, 0);

        var sideLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = Theme.Sidebar,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        sideLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
        sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        sideLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 205));
        sidebar.Controls.Add(sideLayout);

        var brand = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Radius = 16,
            BackColor = Theme.Card,
            BorderColor = Theme.BorderSoft,
            BorderThickness = 1,
            Margin = Padding.Empty,
            Padding = new Padding(18, 12, 18, 10)
        };
        var brandLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Theme.Card,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        brandLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
        brandLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
        _appName.Dock = DockStyle.Fill;
        _appName.ForeColor = Theme.Text;
        _appName.Font = new Font("Segoe UI Semibold", 15f, FontStyle.Bold);
        _appName.TextAlign = ContentAlignment.BottomLeft;
        _appName.AutoEllipsis = false;
        _appSub.Dock = DockStyle.Fill;
        _appSub.ForeColor = Theme.Muted;
        _appSub.Font = new Font("Segoe UI", 8.6f);
        _appSub.TextAlign = ContentAlignment.TopLeft;
        brandLayout.Controls.Add(_appName, 0, 0);
        brandLayout.Controls.Add(_appSub, 0, 1);
        brand.Controls.Add(brandLayout);
        sideLayout.Controls.Add(brand, 0, 0);

        var nav = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Theme.Sidebar,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        nav.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        nav.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        nav.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        ConfigureNav(_audioButton, () => ShowModule(_audio, _audioButton));
        ConfigureNav(_aliasButton, () => ShowModule(_alias, _aliasButton));
        ConfigureNav(_szcButton, () => ShowModule(_szc, _szcButton));
        nav.Controls.Add(_audioButton, 0, 0);
        nav.Controls.Add(_aliasButton, 0, 1);
        nav.Controls.Add(_szcButton, 0, 2);
        sideLayout.Controls.Add(nav, 0, 2);

        var bottom = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = Theme.Sidebar,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _langLabel.Dock = DockStyle.Fill;
        _langLabel.ForeColor = Theme.Muted;
        _langLabel.Font = new Font("Segoe UI Semibold", 8.6f, FontStyle.Bold);
        _langLabel.TextAlign = ContentAlignment.MiddleLeft;
        _langLabel.Margin = new Padding(3, 0, 0, 0);
        bottom.Controls.Add(_langLabel, 0, 0);

        _language.Dock = DockStyle.Fill;
        _language.Margin = Padding.Empty;
        _language.Radius = 10;
        _language.Items.AddRange(Enum.GetNames<AppLanguage>());
        _language.SelectedIndexChanged += (_, _) =>
        {
            if (_language.SelectedItem is string s && Enum.TryParse<AppLanguage>(s, out var lang))
            {
                Localization.Set(lang);
                ApplyLanguage();
            }
        };
        bottom.Controls.Add(_language, 0, 1);

        _footer.Dock = DockStyle.Fill;
        _footer.TextAlign = ContentAlignment.MiddleCenter;
        _footer.ForeColor = Theme.Muted;
        _footer.Font = new Font("Segoe UI Semibold", 8.1f, FontStyle.Bold);
        _footer.Margin = Padding.Empty;
        bottom.Controls.Add(_footer, 0, 3);

        _glxLogo.Dock = DockStyle.Fill;
        _glxLogo.Margin = new Padding(20, 2, 20, 0);
        _glxLogo.SizeMode = PictureBoxSizeMode.Zoom;
        _glxLogo.BackColor = Theme.Sidebar;
        _glxLogo.Image = LoadEmbeddedLogo();
        bottom.Controls.Add(_glxLogo, 0, 4);
        sideLayout.Controls.Add(bottom, 0, 4);

        _content.Dock = DockStyle.Fill;
        _content.BackColor = Theme.AppBack;
        _content.Padding = Padding.Empty;
        _content.Margin = Padding.Empty;
        root.Controls.Add(_content, 1, 0);
    }

    private void ConfigureNav(RoundedButton button, Action click)
    {
        button.Dock = DockStyle.Fill;
        button.Height = 42;
        button.Radius = 12;
        button.Margin = new Padding(0, 0, 0, 8);
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.Padding = new Padding(20, 0, 16, 0);
        button.Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold);
        button.IconText = string.Empty;
        button.ForeColor = Theme.Text;
        button.BackColor = Color.FromArgb(30, 34, 41);
        button.HoverBackColor = Theme.CardHover;
        button.PressedBackColor = Color.FromArgb(42, 47, 57);
        button.Click += (_, _) => click();
    }

    private void DetectLanguage()
    {
        Localization.Set(AppLanguage.Auto);
        _language.SelectedIndex = 0;
        ApplyLanguage();
    }

    private void ShowModule(UserControl module, RoundedButton button)
    {
        if (_current == module) return;
        _content.SuspendLayout();
        _content.Controls.Clear();
        module.Dock = DockStyle.Fill;
        _content.Controls.Add(module);
        _content.ResumeLayout();
        _current = module;

        foreach (var nav in new[] { _audioButton, _aliasButton, _szcButton })
        {
            nav.BackColor = Color.FromArgb(30, 34, 41);
            nav.AccentBar = false;
        }
        button.BackColor = Color.FromArgb(47, 40, 78);
        button.AccentBar = true;
        button.Invalidate();
    }

    private void ApplyLanguage()
    {
        _appName.Text = "BO3 Sound Suite";
        _appSub.Text = Localization.T("Audio Mod Tools", "Audio Mod Tools");
        _audioButton.Text = Localization.T("Audio", "Audio");
        _aliasButton.Text = "CSV";
        _szcButton.Text = "SZC";
        _langLabel.Text = Localization.T("LANGUE", "LANGUAGE");
        _footer.Text = "BLACK OPS III MOD TOOLS\r\nSound workflow";
        _alias.ApplyLanguage();
        _audio.ApplyLanguage();
        _szc.ApplyLanguage();
    }

    private static Icon? LoadEmbeddedAppIcon()
    {
        // Prefer the icon compiled into the executable. This is the most reliable
        // source for the title bar and taskbar, including Visual Studio runs.
        try
        {
            var executablePath = Environment.ProcessPath;
            if (!string.IsNullOrWhiteSpace(executablePath) && File.Exists(executablePath))
            {
                using var associated = Icon.ExtractAssociatedIcon(executablePath);
                if (associated != null)
                    return (Icon)associated.Clone();
            }
        }
        catch { }

        // Embedded-resource fallback.
        try
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("BO3SoundSuite.Assets.bo3_sound_suite.ico");
            if (stream == null) return null;
            using var source = new Icon(stream);
            return (Icon)source.Clone();
        }
        catch { return null; }
    }

    private static Image? LoadEmbeddedLogo()
    {
        try
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BO3SoundSuite.Assets.glx_logo.png");
            if (stream == null) return null;
            using var image = Image.FromStream(stream);
            return new Bitmap(image);
        }
        catch { return null; }
    }

    private void TryApplyModernWindowChrome()
    {
        if (!OperatingSystem.IsWindows()) return;
        try
        {
            var dark = 1;
            DwmSetWindowAttribute(Handle, 20, ref dark, sizeof(int));
            var rounded = 2;
            DwmSetWindowAttribute(Handle, 33, ref rounded, sizeof(int));
        }
        catch { }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
