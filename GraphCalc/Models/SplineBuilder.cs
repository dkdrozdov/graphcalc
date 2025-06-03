using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Controls.Platform;
using MathNet.Numerics.LinearAlgebra;

namespace GraphCalc.Models;

public static class SplineBuilder
{
    private static Spline BuildQuadraticSplineGlobal(List<Vector2> points, bool natural = true)
    {
        Console.Write($"Building spline from {points.Count} points.\n");

        if (points.Count < 2) return new Spline([], []);
        if (points.Count == 2) return BuildLinearSpline(points);

        int segmentsCount = points.Count - 1 - 1; // one segment less because we're treating first two segments as one
        int vars = segmentsCount * 3;

        double[,] A = new double[vars, vars];
        double[] B = new double[vars];

        // parabola for the first fragment

        A[0, 0] = points[0].X * points[0].X;
        A[0, 1] = points[0].X;
        A[0, 2] = 1;

        A[1, 0] = points[1].X * points[1].X;
        A[1, 1] = points[1].X;
        A[1, 2] = 1;

        A[2, 0] = points[2].X * points[2].X;
        A[2, 1] = points[2].X;
        A[2, 2] = 1;

        B[0] = points[0].Y;
        B[1] = points[1].Y;
        B[2] = points[2].Y;

        for (int i = 1; i < segmentsCount; i++)
        {
            // points are offset because we're treating first two segments as one
            var start = points[i + 1];
            var end = points[i + 2];

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

        List<SplineSegment> splineSegments = [];


        // split first parabolic segment into two segments with the same coefficients
        splineSegments.Add(new(points[0], points[1], [x[2], x[1], x[0]]));
        splineSegments.Add(new(points[1], points[2], [x[2], x[1], x[0]]));

        for (int i = 1; i < segmentsCount; i++)
        {
            double[] coefs = [x[i * 3], x[i * 3 + 1], x[i * 3 + 2]];
            // points are offset because we're treating first two segments as one
            splineSegments.Add(new(points[i + 1], points[i + 2], [.. coefs.Reverse()]));
        }

        return new Spline(points, splineSegments);
    }

    public static Spline BuildQuadraticSpline(List<Vector2> points, bool natural = true)
    {
        return BuildQuadraticSplineGlobal(points, natural);

        // List<SplineSegment> splineSegments = [];
        // if (points.Count < 2) return new Spline([], []);
        // if (points.Count == 2) return BuildLinearSpline(points, natural);



        // return new Spline(points, splineSegments);
    }

    private static Spline BuildLinearSpline(List<Vector2> points)
    {
        List<SplineSegment> splineSegments = [];

        if (points.Count < 2) return new Spline([], []);

        for (int i = 1; i < points.Count; i++)
        {
            var segment = BuildLinearSegment(points[i - 1], points[i]);
            if (segment != null) splineSegments.Add(segment);
        }

        return new Spline(points, splineSegments);
    }

    private static SplineSegment? BuildLinearSegment(Vector2 p1, Vector2 p2)
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

        return new SplineSegment(p1, p2, coefficients);
    }
}