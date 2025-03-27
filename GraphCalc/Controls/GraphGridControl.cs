using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;
using System.ComponentModel;
using Avalonia.Interactivity;
using System;
using Avalonia.Metadata;

namespace GraphCalc.Controls;

public class GraphGridControl : UserControl
{
    private TranslateTransform _transform = null!;
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

        Log = $"Left: {bl:F2}, Right: {br:F2}, Top: {bt:F2}, Bottom: {bb:F2}, Space between lines: {zs:F2}, Lines: {ls:F2}";
    }

    private void DrawGrid(DrawingContext context, double gridLinesDistance, double thickness, double opacity)
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
            var normalizedX = x < 0 ? x : x;
            context.DrawLine(new Pen(new SolidColorBrush(Colors.Red, opacity), thickness), new Point(normalizedX, Bounds.Bottom), new Point(normalizedX, Bounds.Top));
        }

        // var gridLinesTransformedDistanceY = gridLinesDistance * ZoomY;
        // var verticalGridlinesOnViewY = Bounds.Height / gridLinesTransformedDistanceY;
        // var canvasTop = -canvasOffset.Y;
        // var canvasBottom = canvasTop + Bounds.Height;

        // var topIndex = (int)(canvasTop / gridLinesTransformedDistanceY) + ((canvasTop < 0) ? 0 : 1);

        // for (int i = topIndex; i < topIndex + verticalGridlinesOnViewY; i++)
        // {
        //     var y = canvasOffset.Y + i * gridLinesTransformedDistanceY;
        //     var normalizedY = y < 0 ? y : y;
        //     context.DrawLine(new Pen(new SolidColorBrush(Colors.Red, opacity), thickness), new Point(Bounds.Left, normalizedY), new Point(Bounds.Right, normalizedY));
        // }
    }

    public override void Render(DrawingContext context)
    {
        var viewCenter = Bounds.Center;
        var canvasOffset = new Point(OffsetX, OffsetY) + viewCenter;

        // context.DrawLine(new Pen(new SolidColorBrush(Colors.Red, 1), 1), new Point(0, 0), new Point(100, 100));
        // context.DrawLine(new Pen(new SolidColorBrush(Colors.Red, 1), 1), new Point(Bounds.Left - OffsetX, Bounds.Top), new Point(Bounds.Right, Bounds.Top));
        // context.DrawLine(new Pen(new SolidColorBrush(Colors.Red, 1), 1), new Point(0, 0), new Point(OffsetX, OffsetY));

        DrawGrid(context, baseLineDistance, 2.4, 0.8);
        // DrawGrid(context, 10.0, 2.0, 0.6);

        // context.DrawLine(new Pen(new SolidColorBrush(Colors.Red, 1), 1.5), new Point(canvasCenter.X, Bounds.Bottom), new Point(canvasCenter.X, Bounds.Top));

        // Cross
        if (Bounds.Left <= canvasOffset.X && canvasOffset.X <= Bounds.Right)
            context.DrawLine(new Pen(new SolidColorBrush(Colors.Gray, 0.5), 3.5), new Point(canvasOffset.X, Bounds.Bottom), new Point(canvasOffset.X, Bounds.Top));

        if (Bounds.Top <= canvasOffset.Y && canvasOffset.Y <= Bounds.Bottom)
            context.DrawLine(new Pen(new SolidColorBrush(Colors.Gray, 0.5), 3.5), new Point(Bounds.Left, canvasOffset.Y), new Point(Bounds.Right, canvasOffset.Y));


        // context.DrawRectangle(
        //     new SolidColorBrush(GetCellAtOrDefault(i, j)?.State?.Color ?? Colors.Black),
        //     null,
        //     new Rect(
        //         new Point(
        //             Bounds.Width * i / columns,
        //             Bounds.Height * j / rows),
        //         new Point(
        //             Bounds.Width * (i + 1) / columns,
        //             Bounds.Height * (j + 1) / rows)));



        // context.DrawEllipse(
        //     Brushes.White,
        //     null,
        //     new Point(0.5 * Bounds.Width + Bounds.Left,
        // 0.5 * Bounds.Height + Bounds.Top),
        //     10,
        //     10);
        // // }

        base.Render(context);

        // Request next frame as soon as possible, if the game is running. Remember to reset the stopwatch.
        // if (IsRunning)
        // {
        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        // _stopwatch.Restart();
        // }
    }


}