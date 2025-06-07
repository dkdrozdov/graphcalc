using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Platform.Storage;

namespace GraphCalc.Models
{
    public interface ISplineSegment
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }
        public double? Calculate(double x, bool includeLeft, bool includeRight);
    }

    public class PolynomialSplineSegment(Vector2 start, Vector2 end, List<double> coefficients) : ISplineSegment
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

    public class LagrangeSplineSegment(Vector2 start, Vector2 end, IEnumerable<Vector2> points) : ISplineSegment
    {
        public Vector2 Start { get; } = start;
        public Vector2 End { get; } = end;
        public double? Calculate(Vector2 left, double x)
        {
            var xi = left.X;
            double product = 1;

            foreach (var p in points)
            {
                var xj = p.X;
                if (p != left) product *= (x - xj) / (xi - xj);
            }

            return left.Y * product;
        }

        public double? Calculate(double x, bool includeLeft, bool includeRight)
        {
            return Calculate(Start, x) + (points.Last() == End ? Calculate(End, x) : 0);
        }
    }

    public class HermiteSplineSegment(Vector2 start, Vector2 end, double startTangent, double endTangent) : ISplineSegment
    {
        public Vector2 Start { get; } = start;
        public Vector2 End { get; } = end;

        private readonly double _startTangent = startTangent;
        private readonly double _endTangent = endTangent;

        static double BasisStart(double localX)
        {
            return 1
                    - 3 * localX * localX
                    + 2 * localX * localX * localX;
        }
        double BasisStartTangent(double localX)
        {
            var h = End.X - Start.X;
            return h * (localX
                        - 2 * localX * localX
                        + localX * localX * localX);
        }
        static double BasisEnd(double localX)
        {
            return 3 * localX * localX
                    - 2 * localX * localX * localX;
        }
        double BasisEndTangent(double localX)
        {
            var h = End.X - Start.X;
            return h * (-localX * localX
                        + localX * localX * localX);
        }
        double GetLocalX(double x)
        {
            var h = End.X - Start.X;

            return (x - Start.X) / h;
        }

        public double? Calculate(double x, bool includeLeft, bool includeRight)
        {
            Func<double, bool> checkLeft = includeLeft ? ((double x) => Start.X <= x) : ((double x) => Start.X < x);
            Func<double, bool> checkRight = includeRight ? ((double x) => x <= End.X) : ((double x) => x < End.X);
            if (!(checkLeft(x) && checkRight(x))) return null;

            var localX = GetLocalX(x);

            return Start.Y * BasisStart(localX)
                    + _startTangent * BasisStartTangent(localX)
                    + End.Y * BasisEnd(localX)
                    + _endTangent * BasisEndTangent(localX);
        }
    }
}