using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;
using System.ComponentModel;
using Avalonia.Interactivity;
using System;
using Avalonia.Metadata;
using System.Collections.Generic;
using GraphCalc.Models;
using GraphCalc.ViewModels;
using System.Linq;

namespace GraphCalc.Controls;

public class GraphGridControl : UserControl
{

    public static readonly DirectProperty<GraphGridControl, SplinesViewModel?> SplinesProperty =
        AvaloniaProperty.RegisterDirect<GraphGridControl, SplinesViewModel?>(
            nameof(Splines),
            o => o.Splines,
            (o, v) => o.Splines = v);

    private SplinesViewModel? _splines;

    public SplinesViewModel? Splines
    {
        get => _splines;
        set => SetAndRaise(SplinesProperty, ref _splines, value);
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

    private void UpdateLog()
    {
        var bl = Bounds.Left;
        var br = Bounds.Right;
        var bt = Bounds.Top;
        var bb = Bounds.Bottom;
        var zs = ZoomX * baseLineDistance;
        var ls = Bounds.Width / (baseLineDistance * ZoomX);

        Log = $"Left: {bl:F2}, Right: {br:F2}, Top: {bt:F2}, Bottom: {bb:F2}, Space between lines: {zs:F2}, Lines: {ls:F2}\n LeftCoords: {GetLeftBorder():F1} RightCoords: {GetRightBorder():F1}";
    }

    private double GetLeftBorder() => -(Bounds.Center.X + OffsetX) / ZoomX;
    private double GetRightBorder() => -(-Bounds.Center.X + OffsetX) / ZoomX;

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

        if (Bounds.Left <= canvasOffset.X && canvasOffset.X <= Bounds.Right)
            context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness),
                                new Point(canvasOffset.X, Bounds.Bottom),
                                new Point(canvasOffset.X, Bounds.Top)
                            );

        if (Bounds.Top <= canvasOffset.Y && canvasOffset.Y <= Bounds.Bottom)
            context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness),
                                new Point(Bounds.Left, canvasOffset.Y),
                                new Point(Bounds.Right, canvasOffset.Y)
                            );

    }


    public static void DrawPoint(DrawingContext context, Point p)
    {
        context.DrawEllipse(new SolidColorBrush(Colors.Black, 1.0), null, p, 3, 3);
    }

    public static void DrawPath(DrawingContext context, double thickness, double opacity, Color color, List<Point> points)
    {
        if (points.Count < 2) return;

        var prev = points.First();
        foreach (var point in points.Skip(1))
        {
            context.DrawLine(new Pen(new SolidColorBrush(color, opacity), thickness), prev, point);
            prev = point;
        }
    }

    public Point CanvasToLocal(double x, double y)
    {
        var viewCenter = Bounds.Center;
        var canvasOffset = new Point(OffsetX, OffsetY) + viewCenter;

        return new Point(canvasOffset.X + x * ZoomX, canvasOffset.Y - y * ZoomY);
    }

    public override void Render(DrawingContext context)
    {
        DrawAxes(context, 3.5, 0.5, Colors.Gray);
        DrawGrid(context, baseLineDistance, 2.4, 0.8, Colors.WhiteSmoke);

        foreach (var spline in Splines?.Splines ?? [])
        {
            DrawPath(context, 1.0, 1.0, Colors.Black,
                [.. CalculateSpline(GetLeftBorder(), GetRightBorder(), 5.0 / ZoomX,
                [.. spline.Points.Select(p=>new Point(p.X,p.Y))], spline.Calculate)
                    .Select(p => CanvasToLocal(p.X, p.Y))]);
            foreach (var point in spline?.Points ?? []) DrawPoint(context, CanvasToLocal(point.X, point.Y));
        }

        base.Render(context);

        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
    }

    private static List<Point> Calculate(double left, double right, double step, Func<double, double> func)
    {
        List<double> x = [];
        List<Point> points = [];

        for (int i = 0; left + i * step < right; i++) x.Add(left + i * step);

        x.ForEach(x => points.Add(new Point(x, func(x))));
        return points;
    }

    private static List<Point> CalculateSpline(double left, double right, double step, List<Point> fixedPoints, Func<double, SplineCalculationResult> func)
    {
        List<double> x = [];
        List<Point> points = [];

        for (int i = 0; left + i * step < right; i++) x.Add(left + i * step);

        x.ForEach(x =>
        {
            var r = func(x);
            if (r.Exists) points.Add(new Point(x, r.Value));
        });

        points = [.. points.Concat(fixedPoints).OrderBy(x => x.X)];

        return points;
    }
}