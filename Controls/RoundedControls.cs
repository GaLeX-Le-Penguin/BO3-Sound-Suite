using System.Drawing.Drawing2D;

namespace BO3SoundSuite.Controls;

public class RoundedPanel : Panel
{
    private int _radius = 14;
    public int Radius { get => _radius; set { _radius = Math.Max(2, value); UpdateClipRegion(); Invalidate(); } }
    public Color BorderColor { get; set; } = Color.Transparent;
    public float BorderThickness { get; set; }
    private bool _clipChildrenToRadius;
    public bool ClipChildrenToRadius
    {
        get => _clipChildrenToRadius;
        set { _clipChildrenToRadius = value; UpdateClipRegion(); Invalidate(); }
    }

    public RoundedPanel()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
    }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        UpdateClipRegion();
        Invalidate();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateClipRegion();
    }

    private void UpdateClipRegion()
    {
        if (!_clipChildrenToRadius || Width <= 2 || Height <= 2)
        {
            if (!_clipChildrenToRadius && Region != null)
            {
                var old = Region;
                Region = null;
                old.Dispose();
            }
            return;
        }
        using var path = RoundedRect(new RectangleF(0.5f, 0.5f, Math.Max(1f, Width - 1f), Math.Max(1f, Height - 1f)), Radius);
        var replacement = new Region(path);
        var previous = Region;
        Region = replacement;
        previous?.Dispose();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
        e.Graphics.Clear(Parent?.BackColor ?? BackColor);

        var outer = new RectangleF(1f, 1f, Math.Max(1f, Width - 2f), Math.Max(1f, Height - 2f));
        using var outerPath = RoundedRect(outer, Math.Max(2f, Radius - 1f));

        if (BorderThickness > 0f && BorderColor != Color.Transparent)
        {
            using var border = new SolidBrush(BorderColor);
            e.Graphics.FillPath(border, outerPath);

            var inset = Math.Max(1f, BorderThickness) + 1f;
            var inner = new RectangleF(inset, inset, Math.Max(1f, Width - inset * 2f), Math.Max(1f, Height - inset * 2f));
            using var innerPath = RoundedRect(inner, Math.Max(2f, Radius - inset));
            using var fill = new SolidBrush(BackColor);
            e.Graphics.FillPath(fill, innerPath);
        }
        else
        {
            using var fill = new SolidBrush(BackColor);
            e.Graphics.FillPath(fill, outerPath);
        }
    }

    internal static GraphicsPath RoundedRect(Rectangle bounds, int radius) =>
        RoundedRect(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height), radius);

    internal static GraphicsPath RoundedRect(RectangleF bounds, float radius)
    {
        var safeRadius = Math.Max(1f, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2f));
        var diameter = Math.Max(2f, safeRadius * 2f);
        var path = new GraphicsPath();
        path.StartFigure();
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}

public class RoundedButton : Button
{
    public int Radius { get; set; } = 11;
    public Color HoverBackColor { get; set; } = Color.FromArgb(55, 61, 71);
    public Color PressedBackColor { get; set; } = Color.FromArgb(63, 69, 80);
    public Color BorderColor { get; set; } = Color.Transparent;
    public string IconText { get; set; } = string.Empty;
    public Font? IconFont { get; set; }
    public bool AccentBar { get; set; }
    public Color AccentBarColor { get; set; } = Color.FromArgb(139, 112, 255);
    private bool _hover;
    private bool _pressed;

    public RoundedButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Height = 40;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs mevent) { if (mevent.Button == MouseButtons.Left) _pressed = true; Invalidate(); base.OnMouseDown(mevent); }
    protected override void OnMouseUp(MouseEventArgs mevent) { _pressed = false; Invalidate(); base.OnMouseUp(mevent); }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? BackColor);
        var rect = new RectangleF(1, 1, Math.Max(1, Width - 2), Math.Max(1, Height - 2));
        using var path = RoundedPanel.RoundedRect(rect, Radius);
        var fill = !Enabled ? Color.FromArgb(35, 39, 46) : _pressed ? PressedBackColor : _hover ? HoverBackColor : BackColor;
        using var brush = new SolidBrush(fill);
        e.Graphics.FillPath(brush, path);

        if (BorderColor != Color.Transparent)
        {
            using var pen = new Pen(BorderColor, 1f) { Alignment = PenAlignment.Inset };
            e.Graphics.DrawPath(pen, path);
        }

        if (AccentBar)
        {
            using var accent = new SolidBrush(AccentBarColor);
            e.Graphics.FillRoundedRectangle(accent, new RectangleF(4, Height * 0.27f, 3.5f, Height * 0.46f), 2f);
        }

        var content = Rectangle.FromLTRB(Padding.Left, Padding.Top, Width - Padding.Right, Height - Padding.Bottom);
        var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
        flags |= TextAlign switch
        {
            ContentAlignment.MiddleLeft or ContentAlignment.TopLeft or ContentAlignment.BottomLeft => TextFormatFlags.Left,
            ContentAlignment.MiddleRight or ContentAlignment.TopRight or ContentAlignment.BottomRight => TextFormatFlags.Right,
            _ => TextFormatFlags.HorizontalCenter
        };

        if (!string.IsNullOrWhiteSpace(IconText))
        {
            var iconFont = IconFont ?? new Font("Segoe UI Symbol", Math.Max(10f, Font.Size + 1.5f));
            var iconWidth = TextRenderer.MeasureText(IconText, iconFont, new Size(int.MaxValue, Height), TextFormatFlags.NoPadding).Width;
            var iconRect = new Rectangle(content.Left, content.Top, iconWidth + 5, content.Height);
            TextRenderer.DrawText(e.Graphics, IconText, iconFont, iconRect, Enabled ? ForeColor : Color.Gray,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            content = Rectangle.FromLTRB(iconRect.Right + 7, content.Top, content.Right, content.Bottom);
            flags &= ~TextFormatFlags.HorizontalCenter;
            flags |= TextFormatFlags.Left;
            if (IconFont == null) iconFont.Dispose();
        }

        TextRenderer.DrawText(e.Graphics, Text, Font, content, Enabled ? ForeColor : Color.Gray, flags);
    }
}

internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF bounds, float radius)
    {
        using var path = RoundedPanel.RoundedRect(bounds, radius);
        graphics.FillPath(brush, path);
    }
}
