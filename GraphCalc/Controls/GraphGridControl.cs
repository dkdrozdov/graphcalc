using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using GraphCalc.Models;
using GraphCalc.ViewModels;
using System.Linq;
using System.Globalization;
using Avalonia.Input;
using System.Numerics;
using Avalonia.Controls.Shapes;

namespace GraphCalc.Controls;

public class GraphGridControl : UserControl
{
    public static readonly DirectProperty<GraphGridControl, PreferencesViewModel?> PreferencesProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, PreferencesViewModel?>(
            nameof(Preferences),
            o => o.Preferences,
            (o, v) => o.Preferences = v);

    private PreferencesViewModel? _preferences;

    public PreferencesViewModel? Preferences
    {
        get => _preferences;
        set => SetAndRaise(PreferencesProperty, ref _preferences, value);
    }


    public static readonly DirectProperty<GraphGridControl, DrawableGraphsViewModel?> GraphsProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, DrawableGraphsViewModel?>(
            nameof(Graphs),
            o => o.Graphs,
            (o, v) => o.Graphs = v);

    private DrawableGraphsViewModel? _graphs;

    public DrawableGraphsViewModel? Graphs
    {
        get => _graphs;
        set => SetAndRaise(GraphsProperty, ref _graphs, value);
    }

    public static readonly DirectProperty<GraphGridControl, string> LogProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, string>(
            nameof(Log),
            o => o.Log,
            (o, v) => o.Log = v);

    private string _log = "";

    public string Log
    {
        get => _log;
        set => SetAndRaise(LogProperty, ref _log, value);
    }

    public static readonly DirectProperty<GraphGridControl, double> OffsetXProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, double>(
            nameof(OffsetX),
            o => o.OffsetX,
            (o, v) => o.OffsetX = v);

    private double _offsetX = 0;

    public double OffsetX
    {
        get => _offsetX;
        set => SetAndRaise(OffsetXProperty, ref _offsetX, value);
    }

    public static readonly DirectProperty<GraphGridControl, double> OffsetYProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, double>(
            nameof(OffsetY),
            o => o.OffsetY,
            (o, v) => o.OffsetY = v);

    private double _offsetY = 0;

    public double OffsetY
    {
        get => _offsetY;
        set => SetAndRaise(OffsetYProperty, ref _offsetY, value);
    }

    public static readonly DirectProperty<GraphGridControl, double> ZoomXProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, double>(
            nameof(ZoomX),
            o => o.ZoomX,
            (o, v) => o.ZoomX = v);

    private double _zoomX = 0;

    public double ZoomX
    {
        get => _zoomX;
        set => SetAndRaise(ZoomXProperty, ref _zoomX, value);
    }

    public static readonly DirectProperty<GraphGridControl, double> ZoomYProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, double>(
            nameof(ZoomY),
            o => o.ZoomY,
            (o, v) => o.ZoomY = v);

    private double _zoomY = 0;
    private bool _captured;
    private Point pressedPosition;

    public double ZoomY
    {
        get => _zoomY;
        set => SetAndRaise(ZoomYProperty, ref _zoomY, value);
    }


    public GraphGridControl()
    {
        // Make sure Render is updated whenever one of these properties changes
        AffectsRender<GraphGridControl>(OffsetXProperty, OffsetYProperty, ZoomXProperty, ZoomYProperty);
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == OffsetXProperty || e.Property == OffsetYProperty || e.Property == ZoomXProperty || e.Property == ZoomYProperty)
        {
            // UpdateTransform();
            UpdateLog();
        }
    }

    // private void UpdateTransform(){

    // }

    const double baseLineDistance = 100.0;
    const double coordinateUnit = baseLineDistance;
    private int GridLevel => (int)Math.Floor(Math.Log2(ZoomX));
    private double GridSpacing => baseLineDistance * Math.Pow(2, -GridLevel);

    private void UpdateLog()
    {
        var bl = Bounds.Left;
        var br = Bounds.Right;
        var bt = Bounds.Top;
        var bb = Bounds.Bottom;
        var zs = ZoomX * baseLineDistance;
        var ls = Bounds.Width / (baseLineDistance * ZoomX);
        var gl = GridLevel;
        var gs = GridSpacing;

        Log = $"OffsetX: {OffsetX:F2} OffsetY: {OffsetY:F2} ZoomX: {ZoomX:F1} ZoomY: {ZoomY:F1}\n"
            + $"Left: {bl:F2} Right: {br:F2} Top: {bt:F2} Bottom: {bb:F2}\n"
            + $"Space between lines: {zs:F2} Lines: {ls:F2}\n"
            + $"LeftCoords: {GetLeftBorder():F1} RightCoords: {GetRightBorder():F1} TopCoords: {GetTopBorder():F1} BottomCoords: {GetBottomBorder():F1}\n"
            + $"GridLevel: {gl} GridSpacing: {gs}";
    }

    private double GetLeftBorder() => -(Bounds.Center.X + OffsetX) / (ZoomX * coordinateUnit);
    private double GetRightBorder() => -(-Bounds.Center.X + OffsetX) / (ZoomX * coordinateUnit);
    private double GetTopBorder() => (Bounds.Center.Y + OffsetY) / (ZoomY * coordinateUnit);
    private double GetBottomBorder() => (-Bounds.Center.Y + OffsetY) / (ZoomY * coordinateUnit);

    private void DrawGrid(DrawingContext context, double gridLinesDistance, double thickness, double opacity, Color color)
    {

        var viewCenter = Bounds.Center;
        var canvasOffset = new Point(OffsetX, OffsetY) + viewCenter;

        var gridLinesTransformedDistanceX = gridLinesDistance * ZoomX;
        var verticalGridlinesOnViewX = Bounds.Width / gridLinesTransformedDistanceX;
        var canvasLeft = -canvasOffset.X;
        var canvasRight = canvasLeft + Bounds.Width;

        var leftIndex = (int)(canvasLeft / gridLinesTransformedDistanceX) + ((canvasLeft < 0) ? 0 : 1);

        for (int i = leftIndex; i < leftIndex + verticalGridlinesOnViewX; i++)
        {
            var x = canvasOffset.X + i * gridLinesTransformedDistanceX;
            context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness), new Point(x, Bounds.Bottom), new Point(x, Bounds.Top));
        }

        var gridLinesTransformedDistanceY = gridLinesDistance * ZoomY;
        var verticalGridlinesOnViewY = Bounds.Height / gridLinesTransformedDistanceY;
        var canvasTop = -canvasOffset.Y;
        var canvasBottom = canvasTop + Bounds.Height;

        var topIndex = (int)(canvasTop / gridLinesTransformedDistanceY) + ((canvasTop < 0) ? 0 : 1);

        for (int i = topIndex; i < topIndex + verticalGridlinesOnViewY; i++)
        {
            var y = canvasOffset.Y + i * gridLinesTransformedDistanceY;
            context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness), new Point(Bounds.Left, y), new Point(Bounds.Right, y));
        }
    }


    private void DrawAxes(DrawingContext context, double thickness, double opacity, Color color)
    {
        var viewCenter = Bounds.Center;
        var canvasOffset = new Point(OffsetX, OffsetY) + viewCenter;

        if (Preferences!.ShowYAxis)
            if (Bounds.Left <= canvasOffset.X && canvasOffset.X <= Bounds.Right)
                context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness),
                                    new Point(canvasOffset.X, Bounds.Bottom),
                                    new Point(canvasOffset.X, Bounds.Top)
                                );

        if (Preferences!.ShowXAxis)
            if (Bounds.Top <= canvasOffset.Y && canvasOffset.Y <= Bounds.Bottom)
                context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness),
                                    new Point(Bounds.Left, canvasOffset.Y),
                                    new Point(Bounds.Right, canvasOffset.Y)
                                );

    }


    public static void DrawPoint(DrawingContext context, Point p, IBrush brush, double size)
    {
        context.DrawEllipse(brush, null, p, size, size);
    }

    public static void DrawPath(DrawingContext context, double thickness, List<Point> points, IBrush brush)
    {
        if (points.Count < 2) return;

        IEnumerable<IEnumerable<Point>> segments = [];

        int start = 0;
        int finish = 0;
        foreach (var p in points)
        {
            if (double.IsInfinity(p.Y) || double.IsNaN(p.Y))
            {
                segments = segments.Append(points[start..finish]);
                start = finish + 1;
            }
            finish++;
        }
        segments = segments.Append(points[start..finish]);

        foreach (var s in segments)
            context.DrawGeometry(brush, new Pen(brush, thickness), new PolylineGeometry(s, false));
        // var prev = points.First();
        // foreach (var point in points.Skip(1))
        // {
        //     context.DrawLine(new Pen(brush, thickness), prev, point);
        //     prev = point;
        // }
    }

    public Point CanvasToLocal(double x, double y)
    {
        var viewCenter = Bounds.Center;
        var canvasOffset = new Point(OffsetX, OffsetY) + viewCenter;

        Point point = new(canvasOffset.X + x * ZoomX * coordinateUnit,
                         canvasOffset.Y - y * ZoomY * coordinateUnit);

        // if (point.Y > Bounds.Bottom || point.Y < Bounds.Top) return point.WithY(double.NaN);
        return point;
    }

    public override void Render(DrawingContext context)
    {
        var prefs = Preferences!;
        if (prefs.ShowGridlines)
        {
            if (prefs.ShowMinorGridlines) DrawGrid(context, GridSpacing / 5, 1.0, 1.0, Colors.WhiteSmoke);
            DrawGrid(context, GridSpacing, 1.0, 1.0, Colors.Silver);
        }
        DrawAxes(context, 1.1, 1.0, Colors.Black);
        if (_captured && _isValueAtPointButtonPressed) DrawVertical(context, pressedPosition.X, Colors.Silver, 1.0, 1.0);
        DrawGraphs(context, Graphs);
        if (prefs.ShowAxisNumbers) DrawTicks(context, GridSpacing, 14, 1.0, Colors.Black);
        DrawLabels(context, Graphs);

        base.Render(context);

        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
    }

    private void DrawVertical(DrawingContext context, double x, Color color, double opacity, double thickness)
    {
        context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness),
                                    new Point(CanvasToLocal(x, 0).X, Bounds.Bottom),
                                    new Point(CanvasToLocal(x, 0).X, Bounds.Top)
                                );
    }

    double GraphResolution { get => _preferences?.RenderStep ?? 0.025; }

    private void DrawGraphs(DrawingContext context, DrawableGraphsViewModel? graphs)
    {
        if (graphs == null) return;

        foreach (var graphViewModel in graphs?.Graphs.Where(x => !x.IsHidden) ?? [])
        {
            var graph = graphViewModel.Graph;
            var lineWidth = graphViewModel.LineWidth;
            var lineOpacity = graphViewModel.LineOpacity;
            var brush = graphViewModel.Brush;
            List<Point> points = [.. graph.PointsInBox(GetLeftBorder(),
                                      GetRightBorder(),
                                      GetBottomBorder(),
                                      GetTopBorder(),
                                      GraphResolution / ZoomX )
                        .Select(p => CanvasToLocal(p.X, p.Y))
                ];
            DrawPath(context, lineWidth, points, brush);

            List<Vector2> specialPoints = [.. graph.SpecialPoints()];
            if (_captured && _isValueAtPointButtonPressed)
            {
                Vector2? p = graphViewModel.Graph.PointAt(pressedPosition.X);
                if (p != null)
                {
                    specialPoints = [.. specialPoints, (Vector2)p];
                }
            }

            foreach (var point in (specialPoints ?? []).Where(p => !double.IsNaN(CanvasToLocal(p.X, p.Y).Y))) DrawPoint(context, CanvasToLocal(point.X, point.Y), brush, 2.2 * Math.Sqrt(lineWidth));
        }

    }

    private void DrawLabels(DrawingContext context, DrawableGraphsViewModel? graphs)
    {
        if (_captured && _isValueAtPointButtonPressed)
        {
            foreach (var graphViewModel in graphs?.Graphs.Where(x => !x.IsHidden) ?? [])
            {
                var graph = graphViewModel.Graph;
                var lineWidth = graphViewModel.LineWidth;
                var lineOpacity = graphViewModel.LineOpacity;
                var brush = graphViewModel.Brush;

                Vector2? p = graphViewModel.Graph.PointAt(pressedPosition.X);
                if (p != null && !double.IsNaN(CanvasToLocal(p.Value.X, p.Value.Y).Y))
                {
                    var formattedText = new FormattedText(
                            $"({pressedPosition.X:G2}, {((Vector2)p).Y:G2})",
                            CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            new Typeface(Typeface.Default.FontFamily, FontStyle.Normal, FontWeight.DemiBold, FontStretch.Normal),
                            14.0,
                            brush);

                    var textSize = new Size(formattedText.Width, formattedText.Height);
                    context.DrawRectangle(
                        new SolidColorBrush(Colors.White, 0.8),
                        new Pen(brush, 0.8),
                        new Rect(new Point(CanvasToLocal(pressedPosition.X, 0).X,
                                            CanvasToLocal(0, ((Vector2)p).Y).Y), textSize),
                        3,
                        3);

                    context.DrawText(
                        formattedText,
                        new Point(CanvasToLocal(pressedPosition.X, 0).X /*+ textSize.Width*/, CanvasToLocal(0, ((Vector2)p).Y).Y));
                }
            }
        }
    }

    private void DrawTicks(DrawingContext context, double gridSpacing, double size, double opacity, Color color)
    {
        var viewCenter = Bounds.Center;
        var canvasOffset = new Point(OffsetX, OffsetY) + viewCenter;

        // 0 tick
        if (Preferences!.ShowXAxis || Preferences!.ShowYAxis)
        {
            var formattedText = new FormattedText(
                    "0",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(Typeface.Default.FontFamily, FontStyle.Normal, FontWeight.DemiBold, FontStretch.Normal),
                    size,
                    new SolidColorBrush(Colors.Black, opacity));

            var textSize = new Size(formattedText.Width, formattedText.Height);
            context.DrawText(
                formattedText,
                new Point(Math.Clamp(canvasOffset.X - textSize.Width - (Preferences!.ShowYAxis ? 5.0 : 0.0), Bounds.Left, Bounds.Right - textSize.Width),
                    Math.Clamp(canvasOffset.Y - ((!Preferences!.ShowXAxis) ? textSize.Height / 2.0 : 0.0), Bounds.Top, Bounds.Bottom - textSize.Height)));
        }

        // horizontal ticks
        if (Preferences!.ShowXAxis)
        {
            var gridLinesTransformedDistanceX = gridSpacing * ZoomX;
            var verticalGridlinesOnViewX = Bounds.Width / gridLinesTransformedDistanceX;
            var canvasLeft = -canvasOffset.X;

            var leftIndex = (int)(canvasLeft / gridLinesTransformedDistanceX) + ((canvasLeft < 0) ? 0 : 1);

            for (int i = leftIndex; i < leftIndex + verticalGridlinesOnViewX; i++)
            {
                var x = canvasOffset.X + i * gridLinesTransformedDistanceX;
                var formattedText = new FormattedText(
                $"{i * gridSpacing / coordinateUnit:G}",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(Typeface.Default.FontFamily, FontStyle.Normal, FontWeight.DemiBold, FontStretch.Normal),
                size,
                new SolidColorBrush(Colors.Black, opacity));

                var textSize = new Size(formattedText.Width, formattedText.Height);
                if (i != 0) context.DrawText(formattedText, new Point(x - textSize.Width / 2.0,
                    Math.Clamp(canvasOffset.Y, Bounds.Top, Bounds.Bottom - textSize.Height)));
            }
        }

        // vertical ticks
        if (Preferences!.ShowYAxis)
        {
            var gridLinesTransformedDistanceY = gridSpacing * ZoomY;
            var verticalGridlinesOnViewY = Bounds.Height / gridLinesTransformedDistanceY;
            var canvasTop = -canvasOffset.Y;
            var topIndex = (int)(canvasTop / gridLinesTransformedDistanceY) + ((canvasTop < 0) ? 0 : 1);

            for (int i = topIndex; i < topIndex + verticalGridlinesOnViewY; i++)
            {
                var y = canvasOffset.Y + i * gridLinesTransformedDistanceY;

                var formattedText = new FormattedText(
                $"{-i * gridSpacing / coordinateUnit:G}",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(Typeface.Default.FontFamily, FontStyle.Normal, FontWeight.DemiBold, FontStretch.Normal),
                size,
                new SolidColorBrush(Colors.Black, opacity));

                var textSize = new Size(formattedText.Width, formattedText.Height);
                if (i != 0) context.DrawText(formattedText,
                    new Point(Math.Clamp(canvasOffset.X - textSize.Width - 5.0, Bounds.Left, Bounds.Right - textSize.Width),
                                y - textSize.Height / 2.0));
            }
        }
    }

    private bool _isDragging;
    private bool _isValueAtPointButtonPressed;

    public void Pressed(PointerPressedEventArgs e)
    {
        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        if (!(properties.IsRightButtonPressed || properties.IsLeftButtonPressed))
            return;

        if (properties.IsRightButtonPressed) _isValueAtPointButtonPressed = true;

        _captured = true;
        _isDragging = false;
        _deltaDragged = 0;

        return;
    }

    public void Released(PointerReleasedEventArgs e)
    {
        _captured = false;
        _isValueAtPointButtonPressed = false;

        if (!_isDragging) ClickedLocal(e.GetPosition(this));

        return;
    }

    private void ClickedLocal(Point point)
    {
        var xCanvas = (point.X - Bounds.Center.X - OffsetX) / (ZoomX * coordinateUnit);
        var yCanvas = -(point.Y - Bounds.Center.Y - OffsetY) / (ZoomY * coordinateUnit);

        ClickedCanvas(new Vector2((float)xCanvas, (float)yCanvas));
    }

    private void ClickedCanvas(Vector2 point)
    {
        foreach (var g in Graphs?.Graphs.OfType<DrawableSplineViewModel>() ?? [])
            if (g.IsEditSelected) g.AddPoint(point.X, point.Y);
    }

    private double _deltaDragged;
    private Point _previousPoint;

    public void Moved(PointerEventArgs e)
    {
        Point position = e.GetPosition(this);

        var xCanvas = (position.X - Bounds.Center.X - OffsetX) / (ZoomX * coordinateUnit);
        var yCanvas = -(position.Y - Bounds.Center.Y - OffsetY) / (ZoomY * coordinateUnit);

        pressedPosition = new Point(xCanvas, yCanvas);

        if (_captured)
        {
            _deltaDragged += Math.Sqrt(Math.Pow(_previousPoint.X - position.X, 2) + Math.Pow(_previousPoint.Y - position.Y, 2));
            if (_deltaDragged > 0.1)
            {
                _isDragging = true;
            }

        }
        _previousPoint = position;

        return;
    }
}