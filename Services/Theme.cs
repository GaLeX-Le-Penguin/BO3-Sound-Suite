namespace BO3SoundSuite.Services;

public static class Theme
{
    public static readonly Color AppBack = Color.FromArgb(15, 17, 21);
    public static readonly Color Sidebar = Color.FromArgb(20, 23, 28);
    public static readonly Color Card = Color.FromArgb(25, 29, 35);
    public static readonly Color CardHover = Color.FromArgb(31, 36, 44);
    public static readonly Color Input = Color.FromArgb(20, 24, 29);
    public static readonly Color Border = Color.FromArgb(50, 57, 67);
    public static readonly Color BorderSoft = Color.FromArgb(39, 45, 53);
    public static readonly Color Text = Color.FromArgb(240, 243, 248);
    public static readonly Color Muted = Color.FromArgb(157, 167, 181);
    public static readonly Color Accent = Color.FromArgb(124, 92, 255);
    public static readonly Color AccentHover = Color.FromArgb(142, 116, 255);
    public static readonly Color AccentPressed = Color.FromArgb(105, 74, 231);
    public static readonly Color AccentSoft = Color.FromArgb(111, 91, 195);
    public static readonly Color Danger = Color.FromArgb(201, 78, 91);

    // Console log colors. Kept bright enough to remain readable on the dark UI.
    public static readonly Color LogInfo = Color.FromArgb(116, 174, 255);
    public static readonly Color LogSuccess = Color.FromArgb(99, 209, 151);
    public static readonly Color LogWarning = Color.FromArgb(242, 190, 84);
    public static readonly Color LogError = Color.FromArgb(239, 105, 119);
    public static readonly Color ScrollTrack = Color.FromArgb(32, 37, 44);
    public static readonly Color ScrollThumb = Color.FromArgb(82, 90, 103);

    public static void StyleButton(Button b, bool accent = false)
    {
        b.BackColor = accent ? Accent : Color.FromArgb(37, 42, 50);
        b.ForeColor = Text;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
        b.Height = 40;
    }

    public static void StyleTextBox(TextBox t)
    {
        t.BackColor = Input;
        t.ForeColor = Text;
        t.BorderStyle = BorderStyle.None;
        t.Font = new Font("Segoe UI", 9.5f);
    }

    public static void StyleCombo(ComboBox c)
    {
        c.BackColor = Input;
        c.ForeColor = Text;
        c.FlatStyle = FlatStyle.Flat;
        c.Font = new Font("Segoe UI", 9.5f);
    }

    public static void StyleGrid(DataGridView g)
    {
        g.BackgroundColor = Input;
        g.BorderStyle = BorderStyle.None;
        g.GridColor = BorderSoft;
        g.EnableHeadersVisualStyles = false;
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        g.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 35, 42);
        g.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 35, 42);
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
        g.DefaultCellStyle.BackColor = Input;
        g.DefaultCellStyle.ForeColor = Text;
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(67, 55, 123);
        g.DefaultCellStyle.SelectionForeColor = Text;
        g.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
        g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 26, 31);
        g.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 35, 42);
        g.RowHeadersDefaultCellStyle.ForeColor = Muted;
        g.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 51, 60);
        g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        g.RowTemplate.Height = 30;
        g.ColumnHeadersHeight = 38;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.RowHeadersWidth = 40;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }
}
