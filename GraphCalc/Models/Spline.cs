using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GraphCalc.Models;

public class SplineCalculationResult(bool exists, double value)
{
    public bool Exists { get; } = exists; public double Value { get; } = value;
}

public class SplineSegment(Vector2 start, Vector2 end, List<double> coefficients)
{
    public Vector2 Start { get; } = start;
    public Vector2 End { get; } = end;

    // a0*x^0 + a1*x^1 + a2*x^2 ...
    List<double> Coefficients { get; } = coefficients;

    public double? Calculate(double x, bool includeLeft, bool includeRight)
    {
        Func<double, bool> checkLeft = includeLeft ? ((double x) => Start.X <= x) : ((double x) => Start.X < x);
        Func<double, bool> checkRight = includeRight ? ((double x) => x <= End.X) : ((double x) => x < End.X);
        if (!(checkLeft(x) && checkRight(x))) return null;

        int power = 0;
        double sum = 0;

        foreach (var coef in Coefficients)
        {
            sum += coef * Math.Pow(x, power);
            power++;
        }

        return sum;
    }
}

public class Spline(List<Vector2> points, List<SplineSegment> splineSegments) : IDrawableGraph
{
    public List<Vector2> Points { get; } = [.. points.OrderBy(x => x.X)];
    public List<SplineSegment> SplineSegments { get; } = splineSegments;

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

    public List<Vector2> PointsInBox(double x1, double x2, double y1, double y2, double step)
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

        // points = [.. points.Concat(Points).OrderBy(x => x.X)];

        return points;
    }

    public List<Vector2> SpecialPoints() => points;
}