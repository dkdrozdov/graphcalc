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

        return points;
    }

    public IEnumerable<Vector2> SpecialPoints() => points;
}