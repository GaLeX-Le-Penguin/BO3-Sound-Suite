using System.Collections;
using System.Drawing.Drawing2D;
using BO3SoundSuite.Services;

namespace BO3SoundSuite.Controls;

public sealed class ModernComboBox : Control
{
    public sealed class ModernItemCollection : IEnumerable
    {
        private readonly List<object> _values = new();
        public int Count => _values.Count;
        public object this[int index] => _values[index];
        public void Clear() => _values.Clear();
        public void Add(object item) => _values.Add(item);
        public void AddRange(IEnumerable items)
        {
            foreach (var item in items) if (item != null) _values.Add(item);
        }
        public IEnumerator GetEnumerator() => _values.GetEnumerator();
    }

    private readonly ModernItemCollection _items = new();
    private int _selectedIndex = -1;
    private bool _hovered;
    public int Radius { get; set; } = 10;
    public ModernItemCollection Items => _items;
    public event EventHandler? SelectedIndexChanged;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            var next = value;
            if (next < -1) next = -1;
            if (next >= _items.Count) next = _items.Count - 1;
            if (_selectedIndex == next) return;
            _selectedIndex = next;
            Invalidate();
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public object? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    public ModernComboBox()
    {
        Height = 36;
        BackColor = Theme.Input;
        ForeColor = Theme.Text;
        Font = new Font("Segoe UI", 9.5f);
        Cursor = Cursors.Hand;
        TabStop = true;
        DoubleBuffered = true;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor ?? Theme.AppBack);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
        var rect = new RectangleF(1, 1, Math.Max(1, Width - 2), Math.Max(1, Height - 2));
        using var path = RoundedPanel.RoundedRect(rect, Radius);
        using var fill = new SolidBrush(_hovered && Enabled ? Theme.CardHover : BackColor);
        e.Graphics.FillPath(fill, path);
        using var pen = new Pen(Focused ? Theme.AccentSoft : Theme.Border, Focused ? 1.4f : 1f) { Alignment = PenAlignment.Inset };
        e.Graphics.DrawPath(pen, path);

        var text = SelectedItem?.ToString() ?? string.Empty;
        var textRect = new Rectangle(12, 0, Math.Max(0, Width - 46), Height);
        TextRenderer.DrawText(e.Graphics, text, Font, textRect, Enabled ? ForeColor : Theme.Muted,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

        var cx = Width - 20;
        var cy = Height / 2 + 1;
        using var arrow = new Pen(Enabled ? Theme.Muted : Theme.Border, 1.8f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
        e.Graphics.DrawLines(arrow, new[] { new Point(cx - 4, cy - 2), new Point(cx, cy + 2), new Point(cx + 4, cy - 2) });
    }

    protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left && Enabled) ShowDropDown();
    }
    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!Enabled || _items.Count == 0) return;
        if (e.KeyCode == Keys.Down)
        {
            SelectedIndex = Math.Min(_items.Count - 1, Math.Max(0, SelectedIndex + 1));
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Up)
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
            e.Handled = true;
        }
        else if (e.KeyCode is Keys.Enter or Keys.Space)
        {
            ShowDropDown();
            e.Handled = true;
        }
    }

    private void ShowDropDown()
    {
        if (_items.Count == 0) return;
        Focus();

        var drop = new ToolStripDropDown
        {
            AutoSize = false,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            BackColor = Theme.Card,
            DropShadowEnabled = false
        };
        var list = new ListBox
        {
            BorderStyle = BorderStyle.None,
            DrawMode = DrawMode.OwnerDrawFixed,
            ItemHeight = 30,
            BackColor = Theme.Card,
            ForeColor = Theme.Text,
            IntegralHeight = false,
            Font = Font
        };
        foreach (var item in _items) list.Items.Add(item);
        if (SelectedIndex >= 0 && SelectedIndex < list.Items.Count) list.SelectedIndex = SelectedIndex;
        list.DrawItem += (_, e) =>
        {
            if (e.Index < 0) return;
            var selected = (e.State & DrawItemState.Selected) != 0;
            using var b = new SolidBrush(selected ? Color.FromArgb(55, 48, 88) : Theme.Card);
            e.Graphics.FillRectangle(b, e.Bounds);
            var tr = new Rectangle(e.Bounds.Left + 11, e.Bounds.Top, e.Bounds.Width - 22, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, list.Items[e.Index]?.ToString() ?? string.Empty, Font, tr, Theme.Text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        };

        var visible = Math.Min(_items.Count, 10);
        var height = visible * list.ItemHeight + 2;
        list.Size = new Size(Math.Max(Width, 140), height);
        var host = new ToolStripControlHost(list)
        {
            AutoSize = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Size = list.Size
        };
        drop.Items.Add(host);
        drop.Size = list.Size;
        list.MouseClick += (_, _) =>
        {
            if (list.SelectedIndex >= 0) SelectedIndex = list.SelectedIndex;
            drop.Close();
        };
        list.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter && list.SelectedIndex >= 0)
            {
                SelectedIndex = list.SelectedIndex;
                drop.Close();
            }
            else if (e.KeyCode == Keys.Escape) drop.Close();
        };
        drop.Opened += (_, _) =>
        {
            try
            {
                using var shape = RoundedPanel.RoundedRect(new Rectangle(0, 0, drop.Width, drop.Height), 10);
                drop.Region?.Dispose();
                drop.Region = new Region(shape);
            }
            catch { }
            list.Focus();
        };
        drop.Show(this, new Point(0, Height + 4));
    }
}

public sealed class ModernNumericBox : Control
{
    private readonly TextBox _editor = new();
    private decimal _minimum;
    private decimal _maximum = 100;
    private decimal _value;
    private bool _internal;
    public int Radius { get; set; } = 10;
    public event EventHandler? ValueChanged;

    public decimal Minimum
    {
        get => _minimum;
        set { _minimum = value; if (_maximum < value) _maximum = value; Value = _value; }
    }
    public decimal Maximum
    {
        get => _maximum;
        set { _maximum = value; if (_minimum > value) _minimum = value; Value = _value; }
    }
    public decimal Value
    {
        get => _value;
        set
        {
            var next = Math.Max(_minimum, Math.Min(_maximum, value));
            if (next == _value && _editor.Text == next.ToString("0")) return;
            _value = next;
            _internal = true;
            _editor.Text = next.ToString("0");
            _internal = false;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    public ModernNumericBox()
    {
        Height = 36;
        BackColor = Theme.Input;
        ForeColor = Theme.Text;
        Font = new Font("Segoe UI", 9.5f);
        Cursor = Cursors.IBeam;
        DoubleBuffered = true;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        _editor.BorderStyle = BorderStyle.None;
        _editor.BackColor = Theme.Input;
        _editor.ForeColor = Theme.Text;
        _editor.Font = Font;
        _editor.TextAlign = HorizontalAlignment.Center;
        _editor.Text = "0";
        _editor.Leave += (_, _) => Commit();
        _editor.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { Commit(); e.SuppressKeyPress = true; }
            else if (e.KeyCode == Keys.Up) { Value++; e.SuppressKeyPress = true; }
            else if (e.KeyCode == Keys.Down) { Value--; e.SuppressKeyPress = true; }
        };
        _editor.Enter += (_, _) => Invalidate();
        _editor.Leave += (_, _) => Invalidate();
        Controls.Add(_editor);
        UpdateEditorBounds();
    }

    protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(Parent?.BackColor ?? Theme.AppBack);
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new RectangleF(1, 1, Math.Max(1, Width - 2), Math.Max(1, Height - 2));
        using var path = RoundedPanel.RoundedRect(rect, Radius);
        using var fill = new SolidBrush(Enabled ? Theme.Input : Theme.Card);
        e.Graphics.FillPath(fill, path);
        using var border = new Pen(_editor.Focused ? Theme.AccentSoft : Theme.Border, _editor.Focused ? 1.4f : 1f) { Alignment = PenAlignment.Inset };
        e.Graphics.DrawPath(border, path);
        var x = Width - 15;
        var top = Height / 2 - 6;
        var bottom = Height / 2 + 6;
        using var p = new Pen(Enabled ? Theme.Muted : Theme.Border, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        e.Graphics.DrawLines(p, new[] { new Point(x - 3, top + 2), new Point(x, top - 1), new Point(x + 3, top + 2) });
        e.Graphics.DrawLines(p, new[] { new Point(x - 3, bottom - 2), new Point(x, bottom + 1), new Point(x + 3, bottom - 2) });
    }
    protected override void OnResize(EventArgs e) { base.OnResize(e); UpdateEditorBounds(); Invalidate(); }
    protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); _editor.Enabled = Enabled; Invalidate(); }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (!Enabled) return;
        if (e.X >= Width - 28) Value += e.Y < Height / 2 ? 1 : -1;
        else _editor.Focus();
    }
    protected override void OnMouseWheel(MouseEventArgs e) { Value += e.Delta > 0 ? 1 : -1; base.OnMouseWheel(e); }

    private void Commit()
    {
        if (_internal) return;
        if (decimal.TryParse(_editor.Text, out var parsed)) Value = parsed;
        else _editor.Text = Value.ToString("0");
    }
    private void UpdateEditorBounds()
    {
        var h = _editor.PreferredHeight;
        var y = Math.Max(3, (Height - h) / 2);
        _editor.SetBounds(8, y, Math.Max(1, Width - 36), h);
    }
}
