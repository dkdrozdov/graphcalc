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
    Vector2 Start { get; } = start;
    Vector2 End { get; } = end;

    // a0*x^0 + a1*x^1 + a2*x^2 ...
    List<double> Coefficients { get; } = coefficients;

    public double Calculate(double x)
    {
        if (!(Start.X < x && x < End.X)) return 0;

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

        return new SplineCalculationResult(true, SplineSegments.Select(segment => segment.Calculate(x)).Sum());
    }

    public List<Vector2> PointsInBox(double x1, double x2, double y1, double y2, double step)
    {
        List<Vector2> points = [];
        if (Points.Count == 0) return points;

        List<double> grid = [];
        for (int i = 0; x1 + i * step < x2; i++) grid.Add(x1 + i * step);
        grid.Add(x2);
        grid = [.. grid.Concat(points.Select(p => (double)p.X)).OrderBy(x => x)];

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

        points = [.. points.Concat(points).OrderBy(x => x.X)];

        return points;
    }

    public List<Vector2> SpecialPoints() => points;
}