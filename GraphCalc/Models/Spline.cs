using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GraphCalc.Models;

public class SplineCalculationResult(bool exists, double value)
{
    public bool Exists { get; } = exists; public double Value { get; } = value;
}

public class Spline(IEnumerable<Vector2> points, IEnumerable<ISplineSegment> splineSegments) : IDrawableGraph
{
    public List<Vector2> Points { get; } = [.. points.OrderBy(x => x.X)];
    public IEnumerable<ISplineSegment> SplineSegments { get; } = splineSegments;

    public bool IsParametric { get; set; } = false;

    public SplineCalculationResult Calculate(double x)
    {
        if (Points.Count < 2
                || x < Points.First().X
                || x > Points.Last().X
            )
            return new SplineCalculationResult(false, 0);

        var result = SplineSegments
            .Select(segment => segment
                                .Calculate(x, Points.First() == segment.Start, true))
            .Where(x => x != null)
            .Sum();

        return new SplineCalculationResult(result != null, result ?? 0);
    }

    public Vector2? PointAt(double x)
    {
        var r = Calculate(x);
        return r.Exists ? new Vector2((float)x, (float)r.Value) : null;
    }

    public IEnumerable<Vector2> PointsInBox(double x1, double x2, double y1, double y2, double step)
    {
        List<Vector2> points = [];
        if (Points.Count == 0) return points;

        List<double> grid = [];
        for (int i = 0; x1 + i * step < x2; i++) grid.Add(x1 + i * step);
        grid.Add(x2);
        grid = [.. grid.Concat(Points.Select(p => (double)p.X)).OrderBy(x => x)];
        grid = [.. grid.Where(x => x >= Points.First().X && x <= Points.Last().X)];

        grid.ForEach(tick =>
        {
            try
            {
                var result = Calculate(tick);
                if (result.Exists)
                    points.Add(new Vector2((float)tick, (float)result.Value));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        });

        // points = [.. points.Where(p => p.Y >= y1 && p.Y <= y2)];

        return points;
    }

    public IEnumerable<Vector2> SpecialPoints() => points;
}


public class ParametricSpline(IEnumerable<Vector2> points, Spline xSpline, Spline ySpline, IEnumerable<KeyValuePair<double, Vector2>> parametrizedPoints) : IDrawableGraph
{
    public List<Vector2> Points { get; } = [.. points.OrderBy(x => x.X)];
    Spline XSpline = xSpline;
    Spline YSpline = ySpline;
    IEnumerable<KeyValuePair<double, Vector2>> _parametrizedPoints = parametrizedPoints;

    public bool IsParametric { get; set; } = false;

    // public SplineCalculationResult Calculate(double t)
    // {
    //     // var xResult = XSpline.Calculate(t);
    //     var yResult = YSpline.Calculate(t);

    //     return yResult;
    // }

    public Vector2? PointAt(double x)
    {
        return YSpline.PointAt(x);
    }

    public IEnumerable<Vector2> PointsInBox(double x1, double x2, double y1, double y2, double step)
    {
        if (Points.Count == 0) return [];

        var length = _parametrizedPoints.Last().Key;

        var xPoints = XSpline.PointsInBox(0, length, x1, x2, step);
        var yPoints = YSpline.PointsInBox(0, length, y1, y2, step);

        return xPoints.Zip(yPoints)
                .Select(p => new Vector2(p.First.Y, p.Second.Y));
    }

    public IEnumerable<Vector2> SpecialPoints() => points;
}