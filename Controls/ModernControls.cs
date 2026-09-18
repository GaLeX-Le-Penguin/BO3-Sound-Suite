using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using BO3SoundSuite.Services;

namespace BO3SoundSuite.Controls;

public sealed class RoundedInputHost : RoundedPanel
{
    public Control InnerControl { get; }

    public RoundedInputHost(Control inner, int horizontalPadding = 12, int verticalPadding = 7)
    {
        InnerControl = inner;
        Radius = 10;
        BackColor = Theme.Input;
        BorderColor = Theme.Border;
        BorderThickness = 1f;
        Padding = new Padding(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);
        inner.Dock = DockStyle.Fill;
        inner.Margin = Padding.Empty;
        inner.BackColor = Theme.Input;
        inner.ForeColor = Theme.Text;
        Controls.Add(inner);
        inner.Enter += (_, _) => { BorderColor = Theme.AccentSoft; Invalidate(); };
        inner.Leave += (_, _) => { BorderColor = Theme.Border; Invalidate(); };
    }
}

public sealed class ModernCheckBox : CheckBox
{
    public Color AccentColor { get; set; } = Theme.Accent;
    public bool AutoSizeToText { get; set; } = true;

    public ModernCheckBox()
    {
        AutoSize = false;
        Height = 30;
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var text = TextRenderer.MeasureText(Text, Font, proposedSize, TextFormatFlags.NoPadding);
        return new Size(text.Width + 34, Math.Max(28, text.Height + 8));
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        if (AutoSizeToText)
        {
            var pref = GetPreferredSize(Size.Empty);
            Width = pref.Width;
            Height = pref.Height;
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor ?? BackColor);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var box = new Rectangle(1, (Height - 18) / 2, 18, 18);
        using var path = RoundedPanel.RoundedRect(box, 5);
        using var fill = new SolidBrush(Checked ? AccentColor : Theme.Input);
        e.Graphics.FillPath(fill, path);
        using var border = new Pen(Checked ? AccentColor : Theme.Border, 1f);
        e.Graphics.DrawPath(border, path);
        if (Checked)
        {
            using var pen = new Pen(Color.White, 2f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
            e.Graphics.DrawLines(pen, new[] { new Point(5, box.Top + 9), new Point(8, box.Top + 12), new Point(14, box.Top + 6) });
        }
        var textRect = new Rectangle(29, 0, Math.Max(0, Width - 29), Height);
        TextRenderer.DrawText(e.Graphics, Text, Font, textRect, Enabled ? ForeColor : Theme.Muted,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
    }
}


public sealed class ModernToggleColumn : DataGridViewCheckBoxColumn
{
    public ModernToggleColumn()
    {
        CellTemplate = new ModernToggleCell();
        TrueValue = true;
        FalseValue = false;
        ThreeState = false;
    }
}

public sealed class ModernToggleCell : DataGridViewCheckBoxCell
{
    public ModernToggleCell() : base(false)
    {
        TrueValue = true;
        FalseValue = false;
        FlatStyle = FlatStyle.Flat;
    }

    public override object Clone()
    {
        return (ModernToggleCell)base.Clone();
    }

    protected override void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates elementState,
        object? value,
        object? formattedValue,
        string? errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
        var withoutGlyph = paintParts & ~DataGridViewPaintParts.ContentForeground;
        base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue,
            errorText, cellStyle, advancedBorderStyle, withoutGlyph);

        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var isChecked = value is bool b && b;
        var trackWidth = 34f;
        var trackHeight = 18f;
        var x = cellBounds.Left + (cellBounds.Width - trackWidth) / 2f;
        var y = cellBounds.Top + (cellBounds.Height - trackHeight) / 2f;
        var track = new RectangleF(x, y, trackWidth, trackHeight);

        using var trackPath = RoundedPanel.RoundedRect(track, trackHeight / 2f);
        using var trackBrush = new SolidBrush(isChecked ? Theme.Accent : Color.FromArgb(55, 61, 70));
        graphics.FillPath(trackBrush, trackPath);

        using var border = new Pen(isChecked ? Theme.AccentHover : Theme.Border, 1f);
        graphics.DrawPath(border, trackPath);

        const float thumbSize = 12f;
        var thumbX = isChecked ? track.Right - thumbSize - 3f : track.Left + 3f;
        var thumbY = track.Top + (track.Height - thumbSize) / 2f;
        using var thumbBrush = new SolidBrush(isChecked ? Color.White : Theme.Muted);
        graphics.FillEllipse(thumbBrush, thumbX, thumbY, thumbSize, thumbSize);
    }
}

public sealed class WaveformLogo : Control
{
    public WaveformLogo()
    {
        DoubleBuffered = true;
        Size = new Size(46, 46);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = RoundedPanel.RoundedRect(rect, 13);
        using var fill = new SolidBrush(Theme.Accent);
        e.Graphics.FillPath(fill, path);

        var heights = new[] { 10, 19, 27, 17, 23, 12 };
        var x = rect.Left + 9;
        using var wave = new SolidBrush(Color.White);
        foreach (var h in heights)
        {
            var y = rect.Top + (rect.Height - h) / 2f;
            e.Graphics.FillRoundedRectangle(wave, new RectangleF(x, y, 3.2f, h), 1.6f);
            x += 5;
        }
    }
}

public sealed class ModernScrollBar : Control
{
    private int _value;
    private int _maximum;
    private int _largeChange = 1;
    private bool _dragging;
    private int _dragOffset;

    public Orientation Orientation { get; set; } = Orientation.Vertical;
    public int Maximum { get => _maximum; set { _maximum = Math.Max(0, value); Value = Math.Min(Value, _maximum); Invalidate(); } }
    public int LargeChange { get => _largeChange; set { _largeChange = Math.Max(1, value); Invalidate(); } }
    public int Value
    {
        get => _value;
        set
        {
            var next = Math.Clamp(value, 0, Maximum);
            if (_value == next) return;
            _value = next;
            Invalidate();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler? ValueChanged;

    public ModernScrollBar()
    {
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
        BackColor = Theme.Card;
        Size = new Size(13, 100);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? Theme.Card);
        var track = GetTrackRectangle();
        using var trackBrush = new SolidBrush(Theme.ScrollTrack);
        e.Graphics.FillRoundedRectangle(trackBrush, track, Math.Min(track.Width, track.Height) / 2f);
        var thumb = GetThumbRectangle();
        using var thumbBrush = new SolidBrush(Enabled && Maximum > 0 ? Theme.ScrollThumb : Theme.ScrollTrack);
        e.Graphics.FillRoundedRectangle(thumbBrush, thumb, Math.Min(thumb.Width, thumb.Height) / 2f);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (Maximum <= 0) return;
        var thumb = Rectangle.Round(GetThumbRectangle());
        var p = Orientation == Orientation.Vertical ? e.Y : e.X;
        var start = Orientation == Orientation.Vertical ? thumb.Top : thumb.Left;
        var end = Orientation == Orientation.Vertical ? thumb.Bottom : thumb.Right;
        if (p >= start && p <= end)
        {
            _dragging = true;
            _dragOffset = p - start;
            Capture = true;
        }
        else
        {
            SetValueFromPointer(p, (Orientation == Orientation.Vertical ? thumb.Height : thumb.Width) / 2);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_dragging) return;
        var p = Orientation == Orientation.Vertical ? e.Y : e.X;
        SetValueFromPointer(p, _dragOffset);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragging = false;
        Capture = false;
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        Value += e.Delta > 0 ? -Math.Max(1, LargeChange / 3) : Math.Max(1, LargeChange / 3);
        base.OnMouseWheel(e);
    }

    private RectangleF GetTrackRectangle()
    {
        if (Orientation == Orientation.Vertical)
        {
            var w = 5f;
            return new RectangleF((Width - w) / 2f, 3, w, Math.Max(1, Height - 6));
        }
        else
        {
            var h = 5f;
            return new RectangleF(3, (Height - h) / 2f, Math.Max(1, Width - 6), h);
        }
    }

    private RectangleF GetThumbRectangle()
    {
        var track = GetTrackRectangle();
        var trackLength = Orientation == Orientation.Vertical ? track.Height : track.Width;
        var ratio = Maximum <= 0 ? 1f : Math.Clamp(LargeChange / (float)(Maximum + LargeChange), 0.08f, 1f);
        var thumbLength = Math.Max(26f, trackLength * ratio);
        thumbLength = Math.Min(trackLength, thumbLength);
        var travel = Math.Max(0f, trackLength - thumbLength);
        var pos = Maximum <= 0 ? 0f : travel * Value / Maximum;
        return Orientation == Orientation.Vertical
            ? new RectangleF(track.X - 1f, track.Y + pos, track.Width + 2f, thumbLength)
            : new RectangleF(track.X + pos, track.Y - 1f, thumbLength, track.Height + 2f);
    }

    private void SetValueFromPointer(int pointer, int offset)
    {
        var track = GetTrackRectangle();
        var thumb = GetThumbRectangle();
        var trackStart = Orientation == Orientation.Vertical ? track.Top : track.Left;
        var trackLength = Orientation == Orientation.Vertical ? track.Height : track.Width;
        var thumbLength = Orientation == Orientation.Vertical ? thumb.Height : thumb.Width;
        var travel = Math.Max(1f, trackLength - thumbLength);
        var local = pointer - offset - trackStart;
        Value = (int)Math.Round(Math.Clamp(local / travel, 0f, 1f) * Maximum);
    }
}

public sealed class ModernGridHost : RoundedPanel
{
    public DataGridView Grid { get; }
    private readonly ModernScrollBar _vertical = new() { Orientation = Orientation.Vertical, Dock = DockStyle.Fill };
    private readonly ModernScrollBar _horizontal = new() { Orientation = Orientation.Horizontal, Dock = DockStyle.Fill };
    private readonly TableLayoutPanel _layout;
    private bool _syncing;

    public ModernGridHost(DataGridView grid)
    {
        Grid = grid;
        Radius = 14;
        BackColor = Theme.Input;
        BorderColor = Theme.Border;
        BorderThickness = 1f;
        ClipChildrenToRadius = true;
        Padding = new Padding(1);

        _layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Theme.Input, Margin = Padding.Empty, Padding = Padding.Empty };
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
        Controls.Add(_layout);

        grid.Dock = DockStyle.Fill;
        grid.ScrollBars = ScrollBars.None;
        grid.Margin = Padding.Empty;
        _layout.Controls.Add(grid, 0, 0);
        _layout.Controls.Add(_vertical, 1, 0);
        _layout.Controls.Add(_horizontal, 0, 1);
        var corner = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Input, Margin = Padding.Empty };
        _layout.Controls.Add(corner, 1, 1);

        _vertical.ValueChanged += (_, _) => ScrollGridVertically();
        _horizontal.ValueChanged += (_, _) => ScrollGridHorizontally();
        grid.Scroll += (_, _) => SyncFromGrid();
        grid.MouseWheel += (_, _) => QueueSync();
        grid.Resize += (_, _) => QueueUpdate();
        grid.RowsAdded += (_, _) => QueueUpdate();
        grid.RowsRemoved += (_, _) => QueueUpdate();
        grid.ColumnAdded += (_, _) => QueueUpdate();
        grid.ColumnRemoved += (_, _) => QueueUpdate();
        grid.ColumnWidthChanged += (_, _) => QueueUpdate();
        HandleCreated += (_, _) => UpdateRanges();
    }

    public void RefreshScrollBars() => UpdateRanges();

    private void QueueUpdate()
    {
        if (!IsHandleCreated || IsDisposed) return;
        BeginInvoke(new Action(UpdateRanges));
    }

    private void QueueSync()
    {
        if (!IsHandleCreated || IsDisposed) return;
        BeginInvoke(new Action(SyncFromGrid));
    }

    private void UpdateRanges()
    {
        if (!IsHandleCreated || Grid.IsDisposed) return;
        try
        {
            var validRows = Grid.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) + (Grid.AllowUserToAddRows ? 1 : 0);
            var visibleRows = Math.Max(1, Grid.DisplayedRowCount(false));
            _vertical.LargeChange = visibleRows;
            _vertical.Maximum = Math.Max(0, validRows - visibleRows);
            _vertical.Visible = _vertical.Maximum > 0;

            var visibleCols = Math.Max(1, Grid.DisplayedColumnCount(false));
            var scrollableCols = Grid.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible && !c.Frozen);
            _horizontal.LargeChange = visibleCols;
            _horizontal.Maximum = Math.Max(0, scrollableCols - visibleCols);
            _horizontal.Visible = _horizontal.Maximum > 0;

            _layout.ColumnStyles[1].Width = _vertical.Visible ? 14f : 0f;
            _layout.RowStyles[1].Height = _horizontal.Visible ? 14f : 0f;
            _layout.PerformLayout();
            SyncFromGrid();
        }
        catch { }
    }

    private void ScrollGridVertically()
    {
        if (_syncing || Grid.RowCount == 0) return;
        try
        {
            _syncing = true;
            var index = Math.Min(_vertical.Value, Math.Max(0, Grid.RowCount - 1));
            while (index < Grid.RowCount && !Grid.Rows[index].Visible) index++;
            if (index < Grid.RowCount) Grid.FirstDisplayedScrollingRowIndex = index;
        }
        catch { }
        finally { _syncing = false; }
    }

    private void ScrollGridHorizontally()
    {
        if (_syncing || Grid.ColumnCount == 0) return;
        try
        {
            _syncing = true;
            var cols = Grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible && !c.Frozen).OrderBy(c => c.DisplayIndex).ToList();
            if (cols.Count == 0) return;
            var col = cols[Math.Min(_horizontal.Value, cols.Count - 1)];
            Grid.FirstDisplayedScrollingColumnIndex = col.Index;
        }
        catch { }
        finally { _syncing = false; }
    }

    private void SyncFromGrid()
    {
        if (_syncing) return;
        try
        {
            _syncing = true;
            if (Grid.FirstDisplayedScrollingRowIndex >= 0)
                _vertical.Value = Math.Min(_vertical.Maximum, Grid.FirstDisplayedScrollingRowIndex);
            if (Grid.FirstDisplayedScrollingColumnIndex >= 0)
            {
                var cols = Grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible && !c.Frozen).OrderBy(c => c.DisplayIndex).ToList();
                var idx = cols.FindIndex(c => c.Index == Grid.FirstDisplayedScrollingColumnIndex);
                if (idx >= 0) _horizontal.Value = Math.Min(_horizontal.Maximum, idx);
            }
        }
        catch { }
        finally { _syncing = false; }
    }
}

public sealed class CodeEditorBox : RoundedPanel
{
    private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
    private const int EM_LINESCROLL = 0x00B6;
    private readonly RichTextBox _editor = new();
    private readonly ModernScrollBar _vertical = new() { Orientation = Orientation.Vertical, Dock = DockStyle.Fill };
    private readonly ModernScrollBar _horizontal = new() { Orientation = Orientation.Horizontal, Dock = DockStyle.Fill };
    private bool _syncing;
    private bool _wordWrap;

    public bool ReadOnly { get => _editor.ReadOnly; set => _editor.ReadOnly = value; }
    public bool WordWrap
    {
        get => _wordWrap;
        set
        {
            _wordWrap = value;
            _editor.WordWrap = value;
            _horizontal.Visible = !value;
            UpdateRanges();
        }
    }
    [AllowNull]
    public override string Text { get => _editor.Text; set { _editor.Text = value ?? string.Empty; UpdateRanges(); } }
    public Font EditorFont { get => _editor.Font; set { _editor.Font = value; UpdateRanges(); } }
    public bool AcceptsTab { get => _editor.AcceptsTab; set => _editor.AcceptsTab = value; }
    public event EventHandler? EditorTextChanged;

    public CodeEditorBox()
    {
        Radius = 12;
        BackColor = Theme.Input;
        BorderColor = Theme.Border;
        BorderThickness = 1f;
        ClipChildrenToRadius = true;
        Padding = new Padding(1);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Theme.Input, Margin = Padding.Empty, Padding = Padding.Empty };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 14));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));
        Controls.Add(layout);

        _editor.Dock = DockStyle.Fill;
        _editor.BorderStyle = BorderStyle.None;
        _editor.BackColor = Theme.Input;
        _editor.ForeColor = Theme.Text;
        _editor.Font = new Font("Cascadia Mono", 10f);
        _editor.ScrollBars = RichTextBoxScrollBars.None;
        _editor.DetectUrls = false;
        _editor.Margin = new Padding(10, 9, 4, 4);
        layout.Controls.Add(_editor, 0, 0);
        layout.Controls.Add(_vertical, 1, 0);
        layout.Controls.Add(_horizontal, 0, 1);
        layout.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Theme.Input, Margin = Padding.Empty }, 1, 1);

        _vertical.ValueChanged += (_, _) => ApplyVertical();
        _horizontal.ValueChanged += (_, _) => ApplyHorizontal();
        _editor.TextChanged += (_, e) => { UpdateRanges(); EditorTextChanged?.Invoke(this, e); };
        _editor.VScroll += (_, _) => SyncVertical();
        _editor.HScroll += (_, _) => SyncHorizontal();
        _editor.MouseWheel += (_, _) => QueueEditorSync();
        _editor.KeyUp += (_, _) => QueueEditorSync();
        Resize += (_, _) => QueueEditorUpdate();
        _editor.Enter += (_, _) => { BorderColor = Theme.AccentSoft; Invalidate(); };
        _editor.Leave += (_, _) => { BorderColor = Theme.Border; Invalidate(); };
        WordWrap = false;
    }

    public void SelectAll() => _editor.SelectAll();
    public void FocusEditor() => _editor.Focus();
    public void Clear()
    {
        _editor.Clear();
        UpdateRanges();
    }
    public void AppendText(string value)
    {
        AppendColoredText(value, Theme.Text);
    }

    public void AppendColoredText(string value, Color color)
    {
        if (string.IsNullOrEmpty(value)) return;
        _editor.SelectionStart = _editor.TextLength;
        _editor.SelectionLength = 0;
        _editor.SelectionColor = color;
        _editor.AppendText(value);
        _editor.SelectionColor = Theme.Text;
        _editor.SelectionStart = _editor.TextLength;
        _editor.SelectionLength = 0;
        _editor.ScrollToCaret();
        UpdateRanges();
    }

    public void RemoveFirstCharacters(int count)
    {
        if (count <= 0 || _editor.TextLength == 0) return;
        var remove = Math.Min(count, _editor.TextLength);
        _editor.Select(0, remove);
        _editor.SelectedText = string.Empty;
        _editor.SelectionStart = _editor.TextLength;
        _editor.SelectionLength = 0;
        _editor.ScrollToCaret();
        UpdateRanges();
    }
    public void ScrollToEnd()
    {
        _editor.SelectionStart = _editor.TextLength;
        _editor.SelectionLength = 0;
        _editor.ScrollToCaret();
        SyncVertical();
    }

    private void QueueEditorUpdate()
    {
        if (!IsHandleCreated || IsDisposed) return;
        BeginInvoke(new Action(UpdateRanges));
    }

    private void QueueEditorSync()
    {
        if (!IsHandleCreated || IsDisposed) return;
        BeginInvoke(new Action(() => { SyncVertical(); SyncHorizontal(); }));
    }

    private void UpdateRanges()
    {
        if (!IsHandleCreated || _editor.IsDisposed) return;
        try
        {
            var visibleLines = Math.Max(1, _editor.ClientSize.Height / Math.Max(1, TextRenderer.MeasureText("Ag", _editor.Font).Height));
            _vertical.LargeChange = visibleLines;
            _vertical.Maximum = Math.Max(0, _editor.Lines.Length - visibleLines);
            _vertical.Visible = _vertical.Maximum > 0;

            if (!WordWrap)
            {
                var charWidth = Math.Max(1, TextRenderer.MeasureText("M", _editor.Font, Size.Empty, TextFormatFlags.NoPadding).Width);
                var visibleChars = Math.Max(1, _editor.ClientSize.Width / charWidth);
                var longest = _editor.Lines.Length == 0 ? 0 : _editor.Lines.Max(l => l.Length);
                _horizontal.LargeChange = visibleChars;
                _horizontal.Maximum = Math.Max(0, longest - visibleChars);
                _horizontal.Visible = _horizontal.Maximum > 0;
            }
            else
            {
                _horizontal.Maximum = 0;
                _horizontal.Visible = false;
            }
            SyncVertical();
            SyncHorizontal();
        }
        catch { }
    }

    private void ApplyVertical()
    {
        if (_syncing || !_editor.IsHandleCreated) return;
        try
        {
            _syncing = true;
            var current = (int)SendMessage(_editor.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero);
            var delta = _vertical.Value - current;
            if (delta != 0) SendMessage(_editor.Handle, EM_LINESCROLL, IntPtr.Zero, new IntPtr(delta));
        }
        finally { _syncing = false; }
    }

    private void ApplyHorizontal()
    {
        if (_syncing || WordWrap || !_editor.IsHandleCreated) return;
        try
        {
            _syncing = true;
            var current = GetHorizontalOffsetEstimate();
            var delta = _horizontal.Value - current;
            if (delta != 0) SendMessage(_editor.Handle, EM_LINESCROLL, new IntPtr(delta), IntPtr.Zero);
        }
        finally { _syncing = false; }
    }

    private void SyncVertical()
    {
        if (_syncing || !_editor.IsHandleCreated) return;
        try
        {
            _syncing = true;
            var first = (int)SendMessage(_editor.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero);
            _vertical.Value = Math.Min(_vertical.Maximum, Math.Max(0, first));
        }
        finally { _syncing = false; }
    }

    private void SyncHorizontal()
    {
        if (_syncing || WordWrap) return;
        // RichTextBox does not expose the horizontal character offset directly.
        // Keep the custom bar authoritative when it is dragged; wheel/keyboard horizontal scrolling is uncommon here.
    }

    private int GetHorizontalOffsetEstimate() => _horizontal.Value;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}

public class ModernToolTip : ToolTip
{
    public ModernToolTip()
    {
        InitialDelay = 700;
        ReshowDelay = 150;
        AutoPopDelay = 9000;
        ShowAlways = true;
        OwnerDraw = true;
        Popup += (_, e) =>
        {
            var text = GetToolTip(e.AssociatedControl);
            var proposed = new Size(370, 0);
            var measured = TextRenderer.MeasureText(text, new Font("Segoe UI", 9f), proposed, TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
            e.ToolTipSize = new Size(Math.Min(390, measured.Width + 24), measured.Height + 18);
        };
        Draw += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Transparent);
            using var path = RoundedPanel.RoundedRect(new Rectangle(0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1), 9);
            using var fill = new SolidBrush(Color.FromArgb(43, 48, 56));
            using var border = new Pen(Theme.Border);
            e.Graphics.FillPath(fill, path);
            e.Graphics.DrawPath(border, path);
            var rect = Rectangle.Inflate(e.Bounds, -11, -8);
            TextRenderer.DrawText(e.Graphics, e.ToolTipText, new Font("Segoe UI", 9f), rect, Theme.Text,
                TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPrefix);
        };
    }
}
