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

public class Spline(List<Vector2> points, List<SplineSegment> splineSegments)
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
}