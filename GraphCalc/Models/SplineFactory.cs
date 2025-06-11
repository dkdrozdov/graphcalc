using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Controls.Platform;
using DialogHostAvalonia.Utilities;
using MathNet.Numerics.LinearAlgebra;

namespace GraphCalc.Models;

public static class SplineFactory
{
    public static ParametricSpline ParametricSpline(IEnumerable<Vector2> points, Func<IEnumerable<Vector2>, Spline> factoryMethod)
    {
        if (points.Count() < 2) return new ParametricSpline([], new Spline([], []), new Spline([], []), []);

        IEnumerable<KeyValuePair<double, Vector2>> parametrizedPoints = [];

        double tprev = 0;
        Vector2 prevPoint = points.First();

        parametrizedPoints = parametrizedPoints.Append(new(0, points.First()));

        foreach (var point in points.Skip(1))
        {
            var dx = prevPoint.X - point.X;
            var dy = prevPoint.Y - point.Y;

            double length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            double t = tprev + length;

            parametrizedPoints = parametrizedPoints.Append(new(t, point));

            tprev = t;
            prevPoint = point;
        }

        return new ParametricSpline(points, factoryMethod(parametrizedPoints.Select(p => new Vector2((float)p.Key, p.Value.X))),
            factoryMethod(parametrizedPoints.Select(p => new Vector2((float)p.Key, p.Value.Y))), parametrizedPoints);
    }


    public static Spline QuadraticInterpolationSpline(IEnumerable<Vector2> points)
    {
        Console.Write($"Building spline from {points.Count()} points.\n");

        if (points.Count() < 2) return new Spline([], []);
        if (points.Count() == 2) return LagrangeInterpolationSpline(points);

        int segmentsCount = points.Count() - 1 - 1; // one segment less because we're treating first two segments as one
        int vars = segmentsCount * 3;

        double[,] A = new double[vars, vars];
        double[] B = new double[vars];

        // parabola for the first fragment

        A[0, 0] = points.ElementAt(0).X * points.ElementAt(0).X;
        A[0, 1] = points.ElementAt(0).X;
        A[0, 2] = 1;

        A[1, 0] = points.ElementAt(1).X * points.ElementAt(1).X;
        A[1, 1] = points.ElementAt(1).X;
        A[1, 2] = 1;

        A[2, 0] = points.ElementAt(2).X * points.ElementAt(2).X;
        A[2, 1] = points.ElementAt(2).X;
        A[2, 2] = 1;

        B[0] = points.ElementAt(0).Y;
        B[1] = points.ElementAt(1).Y;
        B[2] = points.ElementAt(2).Y;

        for (int i = 1; i < segmentsCount; i++)
        {
            // points are offset because we're treating first two segments as one
            var start = points.ElementAt(i + 1);
            var end = points.ElementAt(i + 2);

            // first point
            A[i * 3, i * 3] = start.X * start.X;
            A[i * 3, i * 3 + 1] = start.X;
            A[i * 3, i * 3 + 2] = 1;

            B[i * 3] = start.Y;

            // second point
            A[i * 3 + 1, i * 3] = end.X * end.X;
            A[i * 3 + 1, i * 3 + 1] = end.X;
            A[i * 3 + 1, i * 3 + 2] = 1;

            B[i * 3 + 1] = end.Y;

            // left edge condition
            A[i * 3 + 2, (i - 1) * 3] = 2 * start.X;
            A[i * 3 + 2, (i - 1) * 3 + 1] = 1;
            A[i * 3 + 2, (i - 1) * 3 + 2] = 0;

            // third point
            A[i * 3 + 2, i * 3] = -2 * start.X;
            A[i * 3 + 2, i * 3 + 1] = -1;
            A[i * 3 + 2, i * 3 + 2] = 0;
        }

        for (int i = 0; i < vars; i++)
        {
            for (int j = 0; j < vars; j++)
            {
                Console.Write($"{A[i, j]}\t");
            }
            Console.Write("\n");
        }

        Console.Write("\n");
        for (int i = 0; i < B.Count(); i++)
        {
            Console.Write($"{B[i]}\t");
        }
        Console.Write("\n");

        var matA = Matrix<double>.Build.DenseOfArray(A);
        var matB = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(B);
        var x = matA.Solve(matB);

        List<PolynomialSplineSegment> splineSegments = [];


        // split first parabolic segment into two segments with the same coefficients
        splineSegments.Add(new(points.ElementAt(0), points.ElementAt(1), [x[2], x[1], x[0]]));
        splineSegments.Add(new(points.ElementAt(1), points.ElementAt(2), [x[2], x[1], x[0]]));

        for (int i = 1; i < segmentsCount; i++)
        {
            double[] coefs = [x[i * 3], x[i * 3 + 1], x[i * 3 + 2]];
            // points are offset because we're treating first two segments as one
            splineSegments.Add(new(points.ElementAt(i + 1), points.ElementAt(i + 2), [.. coefs.Reverse()]));
        }

        return new Spline(points, splineSegments);
    }

    public static Spline LagrangeInterpolationSpline(IEnumerable<Vector2> points)
    {
        Console.Write($"Building spline from {points.Count()} points.\n");

        if (points.Count() < 2) return new Spline([], []);
        if (points.Count() == 2) return LinearInterpolationSpline(points);

        int segmentsCount = points.Count() - 1;

        List<LagrangeSplineSegment> splineSegments = [];

        for (int i = 0; i < segmentsCount; i++)
            splineSegments.Add(new(points.ElementAt(i), points.ElementAt(i + 1), points));

        return new Spline(points, splineSegments);
    }

    public static Spline LinearInterpolationSpline(IEnumerable<Vector2> points)
    {
        List<PolynomialSplineSegment> splineSegments = [];

        if (points.Count() < 2) return new Spline([], []);

        for (int i = 1; i < points.Count(); i++)
        {
            var segment = LinearSegment(points.ElementAt(i - 1), points.ElementAt(i));
            if (segment != null) splineSegments.Add(segment);
        }

        return new Spline(points, splineSegments);
    }

    private static PolynomialSplineSegment? LinearSegment(Vector2 p1, Vector2 p2)
    {
        var x0 = p1.X;
        var x1 = p2.X;
        var y0 = p1.Y;
        var y1 = p2.Y;

        if (x0 == x1) return null;

        List<double> coefficients = [];

        var b = (y1 - y0) / (x1 - x0);
        var c = y0 - b * x0;

        coefficients.Add(c);
        coefficients.Add(b);

        return new PolynomialSplineSegment(p1, p2, coefficients);
    }

    private static double CalculateTangentLagrangeMiddle(Vector2 prev, Vector2 current, Vector2 next)
    {
        var hprev = current.X - prev.X;
        var h = next.X - current.X;

        return -h / (hprev * (hprev + h)) * prev.Y
                - (hprev - h) / (hprev * h) * current.Y
                + hprev / (h * (hprev + h)) * next.Y;
    }

    private static double CalculateTangentLagrangeLeft(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var h1 = p2.X - p1.X;
        var h2 = p3.X - p2.X;

        return -(2 * h1 + h2) / (h1 * (h1 + h2)) * p1.Y
                + (h1 + h2) / (h1 * h2) * p2.Y
                - h1 / ((h1 + h2) * h2) * p3.Y;
    }

    private static double CalculateTangentLagrangeRight(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var h1 = p2.X - p1.X;
        var h2 = p3.X - p2.X;

        return h2 / (h1 * (h1 + h2)) * p1.Y
                - (h1 + h2) / (h1 * h2) * p2.Y
                + (2 * h2 + h1) / (h2 * (h1 + h2)) * p3.Y;
    }


    private static double CalculateTangentLagrange(Vector2? pm2, Vector2? pm1, Vector2 p1, Vector2? p2, Vector2? p3)
    {
        {
            if (pm1 is Vector2 prev && p2 is Vector2 next) return CalculateTangentLagrangeMiddle(prev, p1, next);
        }

        {
            if (pm1 == null && p2 is Vector2 pp2 && p3 is Vector2 pp3) return CalculateTangentLagrangeLeft(p1, pp2, pp3);
        }

        {
            if (p2 == null && pm1 is Vector2 prev && pm2 is Vector2 prevprev) return CalculateTangentLagrangeRight(prevprev, prev, p1);
        }

        return 0;
    }

    private static Vector2? ElementAtOrNull(IEnumerable<Vector2> points, int index)
    {
        var count = points.Count();

        if (index < 0 || index >= count) return null;
        return points.ElementAt(index);
    }

    public static Spline HermitCubicInterpolationSpline(IEnumerable<Vector2> points)
    {
        Console.Write($"Building spline from {points.Count()} points.\n");

        if (points.Count() < 2) return new Spline([], []);
        if (points.Count() == 2) return LinearInterpolationSpline(points);

        int segmentsCount = points.Count() - 1;
        List<HermiteSplineSegment> segments = [];

        for (int i = 0; i < segmentsCount; i++)
        {
            Vector2 p = points.ElementAt(i);
            Vector2 p1 = points.ElementAt(i + 1);
            Vector2? pm1 = ElementAtOrNull(points, i - 1);
            Vector2? pm2 = ElementAtOrNull(points, i - 2);
            Vector2? p2 = ElementAtOrNull(points, i + 2);
            Vector2? p3 = ElementAtOrNull(points, i + 3);

            var startTangent = CalculateTangentLagrange(pm2, pm1, p, p1, p2);
            var endTangent = CalculateTangentLagrange(pm1, p, p1, p2, p3);

            segments.Add(new(p, p1, startTangent, endTangent));
        }


        return new Spline(points, segments);
    }

}