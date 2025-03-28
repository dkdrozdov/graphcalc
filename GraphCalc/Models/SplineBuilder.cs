using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace GraphCalc.Models;

public static class SplineBuilder
{
    public static Spline? BuildQuadraticSpline(List<Vector2> points, bool natural = true)
    {
        if (points.Count < 2) return null;

        int segmentsCount = points.Count - 1;
        int vars = segmentsCount * 3;

        double[,] A = new double[vars, vars];
        double[] B = new double[vars];

        for (int i = 0; i < segmentsCount; i++)
        {
            var start = points[i];
            var end = points[i + 1];

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
            if (i == 0) A[i * 3 + 2, i * 3] = 1;
            else
            {
                A[i * 3 + 2, (i - 1) * 3] = 2 * start.X;
                A[i * 3 + 2, (i - 1) * 3 + 1] = 1;
                A[i * 3 + 2, (i - 1) * 3 + 2] = 0;


                A[i * 3 + 2, i * 3] = -2 * start.X;
                A[i * 3 + 2, i * 3 + 1] = -1;
                A[i * 3 + 2, i * 3 + 2] = 0;
            }
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

        var matA = Matrix<double>.Build.DenseOfArray(A);
        var matB = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(B);
        var x = matA.Solve(matB);

        List<SplineSegment> splineSegments = [];

        for (int i = 0; i < segmentsCount; i++)
        {
            double[] coefs = [x[i * 3], x[i * 3 + 1], x[i * 3 + 2]];
            splineSegments.Add(new(points[i], points[i + 1], [.. coefs.Reverse()]));
        }

        return new Spline(points, splineSegments);
    }
}